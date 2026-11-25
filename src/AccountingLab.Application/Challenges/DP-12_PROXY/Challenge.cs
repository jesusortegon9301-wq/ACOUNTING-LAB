
using System;
using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP12Proxy;

public sealed record SensitiveReportRequest(
    Guid CompanyId,
    bool MayRead,
    string ReportKey);

public sealed record SensitiveFinancialReport(
    Guid CompanyId,
    string ReportKey,
    decimal ConfidentialAmount);

public interface ISensitiveReportChallenge
{
    SensitiveFinancialReport Get(
        SensitiveReportRequest request,
        Func<SensitiveFinancialReport> loadReport);
}

public sealed class SensitiveReportChallenge : ISensitiveReportChallenge
{
    public SensitiveFinancialReport Get(
        SensitiveReportRequest request,
        Func<SensitiveFinancialReport> loadReport)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(loadReport);

        // 1. Validar autorización antes de acceder al recurso.
        if (!request.MayRead)
        {
            throw new UnauthorizedAccessException(
                "El usuario no tiene permiso para consultar este reporte.");
        }

        // 2. Validar los datos de la solicitud.
        if (request.CompanyId == Guid.Empty)
        {
            throw new ArgumentException(
                "La identificación de la empresa es obligatoria.",
                nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.ReportKey))
        {
            throw new ArgumentException(
                "La identificación del reporte es obligatoria.",
                nameof(request));
        }

        // 3. El Proxy permite el acceso a la fuente autorizada.
        var report = loadReport();

        if (report is null)
        {
            throw new InvalidOperationException(
                "No se pudo obtener el reporte solicitado.");
        }

        // 4. Verificar que el reporte pertenezca a la empresa solicitante.
        if (report.CompanyId != request.CompanyId)
        {
            throw new InvalidOperationException(
                "El reporte no pertenece a la empresa solicitada.");
        }

        // 5. Verificar que corresponda al reporte solicitado.
        if (!string.Equals(
            report.ReportKey,
            request.ReportKey,
            StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "El reporte obtenido no corresponde al solicitado.");
        }

        return report;
    }
}
