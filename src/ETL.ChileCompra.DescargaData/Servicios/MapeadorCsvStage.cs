using ETL.ChileCompra.DescargaData.Model;
using ETL.Common.Servicios;

namespace ETL.ChileCompra.DescargaData.Servicios;

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
            registro.InstitucionIdOrPortal = ObtenerIdOrPortal(rutInstitucion);
            registro.MonedaOrigenTotalAdjudicado = registro.MontoLineaAdjudica;
            registro.ProveedorRutNumero = ObtenerNumeroRutValido(rutProveedor);
            registro.ClpTotalAdjudicado = repositorioParidadMoneda.ConvertirAClp(
                ConvertirDecimal(registro.MontoLineaAdjudica),
                registro.CodigoMoneda,
                registro.FechaPublicacion ?? registro.FechaEnvioOferta);
            registro.RutProveedor2 = registro.RutProveedor;
            registro.ValorTiempoRenovacion = null;

            registros.Add(registro);
        }

        return registros;
    }

    public IReadOnlyList<RegistroOrdenCompraStage> MapearOC(IEnumerable<Dictionary<string, string>> registrosCsv, string archivo)
    {
        List<RegistroOrdenCompraStage> registros = [];

        foreach (Dictionary<string, string> registroCsv in registrosCsv)
        {
            RegistroOrdenCompraStage registro = MapearRegistro<RegistroOrdenCompraStage>(registroCsv);
            ValorRut rutProveedor = normalizadorRut.Limpiar(registro.RutSucursal);
            ValorRut rutInstitucionEstructura = normalizadorRut.ValidarEstructura(registro.RutUnidadCompra);

            registro.Archivo = archivo;
            registro.ProveedorRutNumero = ObtenerNumeroRutValido(rutProveedor);
            registro.ProveedorRutDv = ObtenerDigitoVerificadorRutValido(rutProveedor);
            registro.InstitucionRutNumero = ObtenerNumeroRutConEstructuraValida(rutInstitucionEstructura);
            registro.InstitucionRutDv = ObtenerDigitoVerificadorRutConEstructuraValida(rutInstitucionEstructura);
            registro.IdOrPortal = ObtenerIdOrPortal(rutInstitucionEstructura);
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

    private string? ObtenerIdOrPortal(ValorRut rut) =>
        rut.EstructuraValida ? proveedorIdOrPortal.Obtener(rut) : null;

    private static int? ObtenerNumeroRutValido(ValorRut rut) =>
        rut.EsValido ? rut.Numero : null;

    private static string? ObtenerDigitoVerificadorRutValido(ValorRut rut) =>
        rut.EsValido ? rut.DigitoVerificador : null;

    private static int? ObtenerNumeroRutConEstructuraValida(ValorRut rut) =>
        rut.EstructuraValida ? rut.Numero : null;

    private static string? ObtenerDigitoVerificadorRutConEstructuraValida(ValorRut rut) =>
        rut.EstructuraValida ? rut.DigitoVerificador : null;
}


