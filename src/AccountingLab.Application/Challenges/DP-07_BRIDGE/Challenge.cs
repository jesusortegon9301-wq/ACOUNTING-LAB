
using System;
using System.Globalization;
using System.Text.Json;
using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP07Bridge;

public sealed record ReportPresentationRequest(
    string ReportType,
    string Format,
    decimal Amount);

public sealed record RenderedFinancialReport(
    string ReportType,
    string Format,
    string ContentType,
    string Content);

public interface IFinancialRepresentationChallenge
{
    RenderedFinancialReport Render(ReportPresentationRequest request);
}

// Implementador del patrón Bridge.
internal interface IReportRenderer
{
    string Format { get; }
    string ContentType { get; }

    string Render(string reportType, decimal amount);
}

// Implementación concreta para JSON.
internal sealed class JsonReportRenderer : IReportRenderer
{
    public string Format => "json";

    public string ContentType => "application/json";

    public string Render(string reportType, decimal amount)
    {
        var content = new
        {
            reportType,
            amount = amount.ToString(
                "0.0000",
                CultureInfo.InvariantCulture)
        };

        return JsonSerializer.Serialize(content);
    }
}

// Implementación concreta para CSV.
internal sealed class CsvReportRenderer : IReportRenderer
{
    public string Format => "csv";

    public string ContentType => "text/csv";

    public string Render(string reportType, decimal amount)
    {
        var formattedAmount = amount.ToString(
            "0.0000",
            CultureInfo.InvariantCulture);

        return $"reportType,amount{Environment.NewLine}" +
               $"{reportType},{formattedAmount}";
    }
}

// Abstracción del informe.
internal abstract class FinancialReport
{
    protected readonly IReportRenderer Renderer;

    protected FinancialReport(IReportRenderer renderer)
    {
        Renderer = renderer;
    }

    public abstract string ReportType { get; }

    public RenderedFinancialReport Render(decimal amount)
    {
        return new RenderedFinancialReport(
            ReportType,
            Renderer.Format,
            Renderer.ContentType,
            Renderer.Render(ReportType, amount));
    }
}

// Informe de balance.
internal sealed class BalanceReport : FinancialReport
{
    public BalanceReport(IReportRenderer renderer)
        : base(renderer)
    {
    }

    public override string ReportType => "balance";
}

// Informe de resultados.
internal sealed class ResultsReport : FinancialReport
{
    public ResultsReport(IReportRenderer renderer)
        : base(renderer)
    {
    }

    public override string ReportType => "resultados";
}

// Implementación principal del desafío Bridge.
public sealed class FinancialRepresentationChallenge
    : IFinancialRepresentationChallenge
{
    public RenderedFinancialReport Render(
        ReportPresentationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Seleccionar la representación.
        // Se declara explícitamente IReportRenderer
        // para evitar el error de inferencia del switch.
        IReportRenderer renderer = request.Format?.Trim() switch
        {
            "json" => new JsonReportRenderer(),
            "csv" => new CsvReportRenderer(),

            _ => throw new ArgumentException(
                "El formato de representación no está disponible.",
                nameof(request))
        };

        // Seleccionar el tipo de informe.
        FinancialReport report = request.ReportType?.Trim() switch
        {
            "balance" => new BalanceReport(renderer),
            "resultados" => new ResultsReport(renderer),

            _ => throw new ArgumentException(
                "El tipo de informe no está disponible.",
                nameof(request))
        };

        return report.Render(request.Amount);
    }
}
