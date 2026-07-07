namespace ETL.Common.Servicios;

public sealed record ValorRut(
    int Numero,
    string? DigitoVerificador,
    string? TextoOriginal,
    bool EstructuraValida,
    bool EsValido);
