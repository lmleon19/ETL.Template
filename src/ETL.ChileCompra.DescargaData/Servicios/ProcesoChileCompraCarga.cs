using ETL.ChileCompra.DescargaData.Model;
using ETL.Common.Resultados;
using ETL.Common.Servicios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.DescargaData.Servicios;

public sealed class ProcesoChileCompraCarga
{
    private readonly OpcionesChileCompra opciones;
    private readonly IConfiguration configuration;
    private readonly ILogger<ProcesoChileCompraCarga> logger;
    private readonly CalculadorPeriodosProceso calculadorPeriodos;
    private readonly PreparadorCarpetasTrabajo preparadorCarpetas;
    private readonly LimpiadorTablasStage limpiadorTablasStage;
    private readonly GeneradorUrlsChileCompra generadorUrls;
    private readonly DescargaHttp descargaHttp;
    private readonly Zip zip;
    private readonly ValidadorArchivosStage validadorArchivosStage;
    private readonly LectorArchivosStage lectorArchivosStage;
    private readonly MapeadorCsvStage mapeadorCsvStage;
    private readonly CargadorStageChileCompra cargadorStage;
    private readonly RepositorioParidadMoneda repositorioParidadMoneda;
    private readonly ProveedorIdOrPortalInstitucion proveedorIdOrPortal;
    private readonly ProcedimientoAlmacenado procedimientoAlmacenado;
    private IReadOnlyList<PeriodoProceso> periodos = [];

    public ProcesoChileCompraCarga(
        IOptions<OpcionesChileCompra> opciones,
        IConfiguration configuration,
        ILogger<ProcesoChileCompraCarga> logger,
        CalculadorPeriodosProceso calculadorPeriodos,
        PreparadorCarpetasTrabajo preparadorCarpetas,
        LimpiadorTablasStage limpiadorTablasStage,
        GeneradorUrlsChileCompra generadorUrls,
        DescargaHttp descargaHttp,
        Zip zip,
        ValidadorArchivosStage validadorArchivosStage,
        LectorArchivosStage lectorArchivosStage,
        MapeadorCsvStage mapeadorCsvStage,
        CargadorStageChileCompra cargadorStage,
        RepositorioParidadMoneda repositorioParidadMoneda,
        ProveedorIdOrPortalInstitucion proveedorIdOrPortal,
        ProcedimientoAlmacenado procedimientoAlmacenado)
    {
        this.opciones = opciones.Value;
        this.configuration = configuration;
        this.logger = logger;
        this.calculadorPeriodos = calculadorPeriodos;
        this.preparadorCarpetas = preparadorCarpetas;
        this.limpiadorTablasStage = limpiadorTablasStage;
        this.generadorUrls = generadorUrls;
        this.descargaHttp = descargaHttp;
        this.zip = zip;
        this.validadorArchivosStage = validadorArchivosStage;
        this.lectorArchivosStage = lectorArchivosStage;
        this.mapeadorCsvStage = mapeadorCsvStage;
        this.cargadorStage = cargadorStage;
        this.repositorioParidadMoneda = repositorioParidadMoneda;
        this.proveedorIdOrPortal = proveedorIdOrPortal;
        this.procedimientoAlmacenado = procedimientoAlmacenado;
    }

    public void RegistrarInicioProceso() => logger.LogInformation("Inicio ETL ChileCompra DescargaData.");

    public void RegistrarFinProceso() => logger.LogInformation("Fin ETL ChileCompra DescargaData.");

    public async Task<ResultadoOperacion> PrepararEjecucionAsync(CancellationToken cancellationToken = default)
    {
        preparadorCarpetas.Preparar();
        return await limpiadorTablasStage.LimpiarAsync(cancellationToken);
    }

    public ResultadoOperacion CalcularPeriodosProceso()
    {
        periodos = calculadorPeriodos.Calcular(DateOnly.FromDateTime(DateTime.Today));
        return ResultadoOperacion.Correcto("Periodos calculados correctamente.");
    }

    public async Task<ResultadoOperacion> PrepararConversionMonedaAsync(CancellationToken cancellationToken = default)
    {
        string rutaDestino = Path.Combine(opciones.Carpetas.BaseTrabajo, "ParidadMoneda.csv");
        ResultadoOperacion<string> resultado = await descargaHttp.DescargarArchivoAsync(generadorUrls.ObtenerUrlParidadMoneda(), rutaDestino, cancellationToken);

        if (!resultado.Exitoso)
        {
            return ResultadoOperacion.Error(resultado.Mensaje, resultado.Excepcion);
        }

        try
        {
            await repositorioParidadMoneda.CargarAsync(rutaDestino, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ResultadoOperacion.Error("No fue posible cargar el archivo de paridad de moneda.", ex);
        }

        return ResultadoOperacion.Correcto("Archivo de paridad de moneda descargado y cargado correctamente.");
    }

    public async Task<ResultadoOperacion> DescargarLicitacionesAsync(CancellationToken cancellationToken = default)
    {
        if (!ExistePeriodoCalculado(out ResultadoOperacion? error))
        {
            return error!;
        }

        foreach (PeriodoProceso periodo in periodos)
        {
            RegistrarInicioPeriodo("descarga de licitaciones", periodo);

            string url = generadorUrls.ObtenerUrlLicitaciones(periodo);
            string rutaDestino = preparadorCarpetas.ObtenerRutaZipLicitaciones(periodo);
            ResultadoOperacion<string> resultado = await descargaHttp.DescargarArchivoAsync(url, rutaDestino, cancellationToken);

            if (!resultado.Exitoso)
            {
                return ResultadoOperacion.Error(resultado.Mensaje, resultado.Excepcion);
            }

            RegistrarFinPeriodo("descarga de licitaciones", periodo);
        }

        return ResultadoOperacion.Correcto("Archivos ZIP de licitaciones descargados correctamente.");
    }

    public async Task<ResultadoOperacion> DescargarOCAsync(CancellationToken cancellationToken = default)
    {
        if (!ExistePeriodoCalculado(out ResultadoOperacion? error))
        {
            return error!;
        }

        foreach (PeriodoProceso periodo in periodos)
        {
            RegistrarInicioPeriodo("descarga de OC", periodo);

            string url = generadorUrls.ObtenerUrlOC(periodo);
            string rutaDestino = preparadorCarpetas.ObtenerRutaZipOC(periodo);
            ResultadoOperacion<string> resultado = await descargaHttp.DescargarArchivoAsync(url, rutaDestino, cancellationToken);

            if (!resultado.Exitoso)
            {
                return ResultadoOperacion.Error(resultado.Mensaje, resultado.Excepcion);
            }

            RegistrarFinPeriodo("descarga de OC", periodo);
        }

        return ResultadoOperacion.Correcto("Archivos ZIP de OC descargados correctamente.");
    }

    public Task<ResultadoOperacion> DescomprimirArchivosAsync()
    {
        ResultadoOperacion resultadoLicitaciones = DescomprimirCarpeta(
            preparadorCarpetas.ObtenerCarpetaZipLicitaciones(),
            preparadorCarpetas.ObtenerCarpetaExtraidosLicitaciones());

        if (!resultadoLicitaciones.Exitoso)
        {
            return Task.FromResult(resultadoLicitaciones);
        }

        ResultadoOperacion resultadoOC = DescomprimirCarpeta(
            preparadorCarpetas.ObtenerCarpetaZipOC(),
            preparadorCarpetas.ObtenerCarpetaExtraidosOC());

        return Task.FromResult(resultadoOC);
    }

    public async Task<ResultadoOperacion> ValidarArchivosCsvAsync(CancellationToken cancellationToken = default)
    {
        string[] archivosLicitaciones = ObtenerCsvLicitaciones().ToArray();
        string[] archivosOC = ObtenerCsvOC().ToArray();

        if (archivosLicitaciones.Length == 0)
        {
            return ResultadoOperacion.Error("No se encontraron CSV de licitaciones para validar.");
        }

        if (archivosOC.Length == 0)
        {
            return ResultadoOperacion.Error("No se encontraron CSV de OC para validar.");
        }

        foreach (string rutaArchivo in archivosLicitaciones)
        {
            ResultadoOperacion resultado = await validadorArchivosStage.ValidarLicitacionesAsync(rutaArchivo, cancellationToken);

            if (!resultado.Exitoso)
            {
                return resultado;
            }
        }

        foreach (string rutaArchivo in archivosOC)
        {
            ResultadoOperacion resultado = await validadorArchivosStage.ValidarOCAsync(rutaArchivo, cancellationToken);

            if (!resultado.Exitoso)
            {
                return resultado;
            }
        }

        return ResultadoOperacion.Correcto("Archivos CSV validados correctamente.");
    }

    public async Task<ResultadoOperacion> CargarTablasIntermediasAsync(CancellationToken cancellationToken = default)
    {
        ResultadoOperacion cargaLicitaciones = await CargarStageLicitacionesAsync(cancellationToken);

        if (!cargaLicitaciones.Exitoso)
        {
            return cargaLicitaciones;
        }

        return await CargarStageOCAsync(cancellationToken);
    }

    public async Task<ResultadoOperacion> CargarStageLicitacionesAsync(CancellationToken cancellationToken = default)
    {
        ResultadoOperacion resultadoLookup = await proveedorIdOrPortal.CargarAsync(cancellationToken);

        if (!resultadoLookup.Exitoso)
        {
            return resultadoLookup;
        }

        try
        {
            List<RegistroLicitacionStage> licitaciones = [];

            foreach (string rutaArchivo in ObtenerCsvLicitaciones())
            {
                ResultadoOperacion<IReadOnlyList<Dictionary<string, string>>> resultadoLectura = await lectorArchivosStage.LeerCsvAsync(rutaArchivo, cancellationToken);

                if (!resultadoLectura.Exitoso)
                {
                    return ResultadoOperacion.Error(resultadoLectura.Mensaje, resultadoLectura.Excepcion);
                }

                licitaciones.AddRange(mapeadorCsvStage.MapearLicitaciones(resultadoLectura.Valor!, Path.GetFileName(rutaArchivo)));
            }

            return await cargadorStage.CargarAsync(
                opciones.Tablas.StageLicitaciones,
                licitaciones,
                cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error al leer, mapear o cargar Stage de licitaciones ChileCompra.");
            return ResultadoOperacion.Error("No fue posible cargar el Stage de licitaciones.", ex);
        }
    }

    public async Task<ResultadoOperacion> CargarStageOCAsync(CancellationToken cancellationToken = default)
    {
        ResultadoOperacion resultadoLookup = await proveedorIdOrPortal.CargarAsync(cancellationToken);

        if (!resultadoLookup.Exitoso)
        {
            return resultadoLookup;
        }

        try
        {
            List<RegistroOrdenCompraStage> oc = [];

            foreach (string rutaArchivo in ObtenerCsvOC())
            {
                ResultadoOperacion<IReadOnlyList<Dictionary<string, string>>> resultadoLectura = await lectorArchivosStage.LeerCsvAsync(rutaArchivo, cancellationToken);

                if (!resultadoLectura.Exitoso)
                {
                    return ResultadoOperacion.Error(resultadoLectura.Mensaje, resultadoLectura.Excepcion);
                }

                oc.AddRange(mapeadorCsvStage.MapearOC(resultadoLectura.Valor!, Path.GetFileName(rutaArchivo)));
            }

            return await cargadorStage.CargarAsync(
                opciones.Tablas.StageOC,
                oc,
                cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error al leer, mapear o cargar Stage de OC ChileCompra.");
            return ResultadoOperacion.Error("No fue posible cargar el Stage de OC.", ex);
        }
    }

    public async Task<ResultadoOperacion> EjecutarTraspasoFinalAsync(CancellationToken cancellationToken = default)
    {
        ResultadoOperacion creacionTablas = await EjecutarCreacionTablasFinalesAnualesAsync(cancellationToken);

        if (!creacionTablas.Exitoso)
        {
            return creacionTablas;
        }

        ResultadoOperacion traspasoLicitaciones = await EjecutarTraspasoFinalLicitacionesAsync(cancellationToken);

        if (!traspasoLicitaciones.Exitoso)
        {
            return traspasoLicitaciones;
        }

        return await EjecutarTraspasoFinalOCAsync(cancellationToken);
    }

    public async Task<ResultadoOperacion> EjecutarCreacionTablasFinalesAnualesAsync(CancellationToken cancellationToken = default) =>
        await EjecutarProcedimientoAsync(
            opciones.Procedimientos.CrearTablasFinalesAnuales,
            "creacion de tablas finales anuales",
            cancellationToken);

    public async Task<ResultadoOperacion> EjecutarTraspasoFinalLicitacionesAsync(CancellationToken cancellationToken = default) =>
        await EjecutarProcedimientoAsync(
            opciones.Procedimientos.TraspasoFinalLicitaciones,
            "licitaciones",
            cancellationToken);

    public async Task<ResultadoOperacion> EjecutarTraspasoFinalOCAsync(CancellationToken cancellationToken = default) =>
        await EjecutarProcedimientoAsync(
            opciones.Procedimientos.TraspasoFinalOC,
            "OC",
            cancellationToken);

    private async Task<ResultadoOperacion> EjecutarProcedimientoAsync(
        string nombreProcedimiento,
        string nombreProceso,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(nombreProcedimiento))
        {
            logger.LogWarning("Procedimiento de {Proceso} no configurado.", nombreProceso);
            return ResultadoOperacion.Correcto($"Procedimiento de {nombreProceso} omitido por configuracion.");
        }

        string cadenaConexion = configuration.GetConnectionString("ChileCompra") ?? string.Empty;
        return await procedimientoAlmacenado.EjecutarAsync(cadenaConexion, nombreProcedimiento, cancellationToken);
    }

    public ResultadoOperacion ArchivarZipFinales()
    {
        MoverZipFinales(preparadorCarpetas.ObtenerCarpetaZipLicitaciones(), preparadorCarpetas.ObtenerCarpetaFinalLicitaciones());
        MoverZipFinales(preparadorCarpetas.ObtenerCarpetaZipOC(), preparadorCarpetas.ObtenerCarpetaFinalOC());

        return ResultadoOperacion.Correcto("Archivos ZIP movidos a carpeta final correctamente.");
    }

    private ResultadoOperacion DescomprimirCarpeta(string carpetaZip, string carpetaDestino)
    {
        foreach (string rutaZip in Directory.EnumerateFiles(carpetaZip, "*.zip", SearchOption.TopDirectoryOnly))
        {
            ResultadoOperacion<IReadOnlyList<string>> resultado = zip.DescomprimirArchivo(rutaZip, carpetaDestino);

            if (!resultado.Exitoso)
            {
                return ResultadoOperacion.Error(resultado.Mensaje, resultado.Excepcion);
            }
        }

        return ResultadoOperacion.Correcto("Archivos ZIP descomprimidos correctamente.");
    }

    private IEnumerable<string> ObtenerCsvLicitaciones() =>
        ObtenerCsv(preparadorCarpetas.ObtenerCarpetaExtraidosLicitaciones());

    private IEnumerable<string> ObtenerCsvOC() =>
        ObtenerCsv(preparadorCarpetas.ObtenerCarpetaExtraidosOC());

    private static IEnumerable<string> ObtenerCsv(string carpeta) =>
        Directory.Exists(carpeta)
            ? Directory.EnumerateFiles(carpeta, "*.csv", SearchOption.AllDirectories).OrderBy(r => r, StringComparer.OrdinalIgnoreCase)
            : [];

    private static void MoverZipFinales(string carpetaOrigen, string carpetaDestino)
    {
        Directory.CreateDirectory(carpetaDestino);

        foreach (string rutaZip in Directory.EnumerateFiles(carpetaOrigen, "*.zip", SearchOption.TopDirectoryOnly))
        {
            string rutaDestino = Path.Combine(carpetaDestino, Path.GetFileName(rutaZip));
            File.Move(rutaZip, rutaDestino, overwrite: true);
        }
    }

    private bool ExistePeriodoCalculado(out ResultadoOperacion? error)
    {
        if (periodos.Count > 0)
        {
            error = null;
            return true;
        }

        error = ResultadoOperacion.Error("Los periodos del proceso no han sido calculados.");
        return false;
    }

    private void RegistrarInicioPeriodo(string nombrePaso, PeriodoProceso periodo)
    {
        logger.LogInformation(
            "Inicio periodo {Anio}-{Mes:00} para {Paso}.",
            periodo.Anio,
            periodo.Mes,
            nombrePaso);
    }

    private void RegistrarFinPeriodo(string nombrePaso, PeriodoProceso periodo)
    {
        logger.LogInformation(
            "Fin periodo {Anio}-{Mes:00} para {Paso}.",
            periodo.Anio,
            periodo.Mes,
            nombrePaso);
    }
}

