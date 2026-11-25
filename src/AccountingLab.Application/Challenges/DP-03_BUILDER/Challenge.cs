
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP03Builder;

// ======================================================
// 1. CONTRATOS ORIGINALES
// ======================================================

public sealed record FinancialReportRequest(
    string Title,
    DateOnly From,
    DateOnly To,
    IReadOnlyList<string> Sections,
    IReadOnlyList<string> Annexes);

public sealed record FinancialReport(
    string Title,
    DateOnly From,
    DateOnly To,
    IReadOnlyList<string> Sections,
    IReadOnlyList<string> Annexes);

public interface IReportAssemblyChallenge
{
    FinancialReport Build(FinancialReportRequest request);
}

// ======================================================
// 2. BUILDER
// ======================================================

internal sealed class FinancialReportBuilder
{
    private string _title = string.Empty;
    private DateOnly _from;
    private DateOnly _to;
    private IReadOnlyList<string> _sections = Array.Empty<string>();
    private IReadOnlyList<string> _annexes = Array.Empty<string>();

    public FinancialReportBuilder SetTitle(string title)
    {
        _title = title;
        return this;
    }

    public FinancialReportBuilder SetPeriod(DateOnly from, DateOnly to)
    {
        _from = from;
        _to = to;
        return this;
    }

    public FinancialReportBuilder SetSections(
        IReadOnlyList<string> sections)
    {
        _sections = new List<string>(sections);
        return this;
    }

    public FinancialReportBuilder SetAnnexes(
        IReadOnlyList<string> annexes)
    {
        _annexes = new List<string>(annexes);
        return this;
    }

    public FinancialReport Build()
    {
        return new FinancialReport(
            _title,
            _from,
            _to,
            new List<string>(_sections),
            new List<string>(_annexes));
    }
}

// ======================================================
// 3. IMPLEMENTACIÓN DEL DESAFÍO
// ======================================================

public sealed class ReportAssemblyChallenge : IReportAssemblyChallenge
{
    public FinancialReport Build(FinancialReportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.From > request.To)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final.",
                nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "El título del informe es obligatorio.",
                nameof(request));
        }

        if (request.Sections is null || request.Sections.Count == 0)
        {
            throw new ArgumentException(
                "El informe debe contener al menos una sección.",
                nameof(request));
        }

        var annexes = request.Annexes ?? Array.Empty<string>();

        var builder = new FinancialReportBuilder();

        return builder
            .SetTitle(request.Title)
            .SetPeriod(request.From, request.To)
            .SetSections(request.Sections)
            .SetAnnexes(annexes)
            .Build();
    }
}
