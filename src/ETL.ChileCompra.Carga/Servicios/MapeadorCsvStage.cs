using ETL.ChileCompra.Carga.Model;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class MapeadorCsvStage
{
    private readonly MetadataStage metadataStage;
    private readonly ConvertidorValoresChileCompra convertidorValores;
    private readonly NormalizadorRutChile normalizadorRut;
    private readonly RepositorioParidadMoneda repositorioParidadMoneda;
    private readonly ProveedorIdOrPortalInstitucion proveedorIdOrPortal;

    public MapeadorCsvStage(
        MetadataStage metadataStage,
        ConvertidorValoresChileCompra convertidorValores,
        NormalizadorRutChile normalizadorRut,
        RepositorioParidadMoneda repositorioParidadMoneda,
        ProveedorIdOrPortalInstitucion proveedorIdOrPortal)
    {
        this.metadataStage = metadataStage;
        this.convertidorValores = convertidorValores;
        this.normalizadorRut = normalizadorRut;
        this.repositorioParidadMoneda = repositorioParidadMoneda;
        this.proveedorIdOrPortal = proveedorIdOrPortal;
    }

    public IReadOnlyList<RegistroLicitacionStage> MapearLicitaciones(IEnumerable<Dictionary<string, string>> registrosCsv, string archivo)
    {
        List<RegistroLicitacionStage> registros = [];

        foreach (Dictionary<string, string> registroCsv in registrosCsv)
        {
            RegistroLicitacionStage registro = MapearRegistro<RegistroLicitacionStage>(registroCsv);
            ValorRut rutProveedor = normalizadorRut.Limpiar(registro.RutProveedor);
            ValorRut rutInstitucion = normalizadorRut.Limpiar(registro.RutUnidad);

            registro.Archivo = archivo;
            registro.InstitucionIdOrPortal = proveedorIdOrPortal.Obtener(rutInstitucion.Numero);
            registro.MonedaOrigenTotalAdjudicado = registro.MontoLineaAdjudica;
            registro.ProveedorRutNumero = rutProveedor.Numero;
            registro.ClpTotalAdjudicado = repositorioParidadMoneda.ConvertirAClp(
                ConvertirDecimal(registro.MontoLineaAdjudica),
                registro.MonedaOferta ?? registro.MonedaAdquisicion ?? registro.CodigoMoneda,
                registro.FechaPublicacion ?? registro.FechaEnvioOferta);
            registro.RutProveedor2 = registro.RutProveedor;
            registro.ValorTiempoRenovacion = null;

            registros.Add(registro);
        }

        return registros;
    }

    public IReadOnlyList<RegistroOrdenCompraStage> MapearOrdenesCompra(IEnumerable<Dictionary<string, string>> registrosCsv, string archivo)
    {
        List<RegistroOrdenCompraStage> registros = [];

        foreach (Dictionary<string, string> registroCsv in registrosCsv)
        {
            RegistroOrdenCompraStage registro = MapearRegistro<RegistroOrdenCompraStage>(registroCsv);
            ValorRut rutProveedor = normalizadorRut.Limpiar(registro.RutSucursal);
            ValorRut rutInstitucion = normalizadorRut.Limpiar(registro.RutUnidadCompra);

            registro.Archivo = archivo;
            registro.ProveedorRutNumero = rutProveedor.Numero;
            registro.ProveedorRutDv = rutProveedor.DigitoVerificador;
            registro.InstitucionRutNumero = rutInstitucion.Numero;
            registro.InstitucionRutDv = rutInstitucion.DigitoVerificador;
            registro.IdOrPortal = proveedorIdOrPortal.Obtener(rutInstitucion.Numero);
            registro.TotalClp = repositorioParidadMoneda.ConvertirAClp(
                ConvertirDecimal(registro.TotalLineaNeto),
                registro.MonedaItem ?? registro.TipoMonedaOC,
                registro.FechaEnvio);
            registro.RutSucursal2 = registro.RutSucursal;
            registro.IdPlanDeCompra = null;

            registros.Add(registro);
        }

        return registros;
    }

    private TRegistroStage MapearRegistro<TRegistroStage>(Dictionary<string, string> registroCsv)
        where TRegistroStage : new()
    {
        TRegistroStage registro = new();

        foreach ((var propiedad, ColumnaStageAttribute columna) in metadataStage.ObtenerColumnas<TRegistroStage>())
        {
            if (columna.Origen != OrigenColumnaStage.Csv)
            {
                continue;
            }

            registroCsv.TryGetValue(columna.NombreCsv, out string? valorCsv);
            object? valor = ConvertirValor(valorCsv, propiedad.PropertyType);
            propiedad.SetValue(registro, valor);
        }

        return registro;
    }

    private object? ConvertirValor(string? valor, Type tipoPropiedad)
    {
        Type tipo = Nullable.GetUnderlyingType(tipoPropiedad) ?? tipoPropiedad;

        if (tipo == typeof(string))
        {
            return convertidorValores.ConvertirTexto(valor);
        }

        if (tipo == typeof(int))
        {
            return convertidorValores.ConvertirInt(valor);
        }

        if (tipo == typeof(long))
        {
            return convertidorValores.ConvertirLong(valor);
        }

        if (tipo == typeof(double))
        {
            return convertidorValores.ConvertirDouble(valor);
        }

        if (tipo == typeof(decimal))
        {
            return convertidorValores.ConvertirDecimal(valor);
        }

        if (tipo == typeof(DateTime))
        {
            return convertidorValores.ConvertirFecha(valor);
        }

        return null;
    }

    private static decimal? ConvertirDecimal(double? valor) =>
        valor.HasValue ? Convert.ToDecimal(valor.Value) : null;
}

