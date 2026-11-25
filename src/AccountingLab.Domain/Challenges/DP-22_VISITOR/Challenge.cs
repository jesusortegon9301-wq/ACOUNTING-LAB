
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP22Visitor;

public interface IAccountingAnalysisDocument
{
    Guid Id { get; }

    void Accept(IAccountingAnalysisVisitor visitor);
}

public sealed record InvoiceAnalysisDocument(
    Guid Id,
    decimal NetAmount,
    decimal TaxAmount) : IAccountingAnalysisDocument
{
    public void Accept(IAccountingAnalysisVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public sealed record PaymentAnalysisDocument(
    Guid Id,
    decimal Amount) : IAccountingAnalysisDocument
{
    public void Accept(IAccountingAnalysisVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public sealed record AnalysisPostingLine(
    decimal Debit,
    decimal Credit);

public sealed record JournalAnalysisDocument(
    Guid Id,
    IReadOnlyList<AnalysisPostingLine> Lines) : IAccountingAnalysisDocument
{
    public void Accept(IAccountingAnalysisVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public sealed record DocumentAnalysisRequest(
    string View,
    IReadOnlyList<IAccountingAnalysisDocument> Documents);

public sealed record DocumentAnalysisResult(
    decimal TotalAmount,
    int DocumentCount,
    IReadOnlyDictionary<string, decimal> ByKind);

public interface IDocumentAnalysisChallenge
{
    DocumentAnalysisResult Analyze(DocumentAnalysisRequest request);
}

public interface IAccountingAnalysisVisitor
{
    void Visit(InvoiceAnalysisDocument document);
    void Visit(PaymentAnalysisDocument document);
    void Visit(JournalAnalysisDocument document);
}

internal sealed class GrossAnalysisVisitor : IAccountingAnalysisVisitor
{
    private readonly Dictionary<string, decimal> _amounts =
        new(StringComparer.OrdinalIgnoreCase);

    public void Visit(InvoiceAnalysisDocument document)
    {
        _amounts["invoice"] = document.NetAmount + document.TaxAmount;
    }

    public void Visit(PaymentAnalysisDocument document)
    {
        _amounts["payment"] = document.Amount;
    }

    public void Visit(JournalAnalysisDocument document)
    {
        decimal total = 0m;

        foreach (var line in document.Lines)
        {
            total += line.Debit;
        }

        _amounts["journal"] = total;
    }

    public IReadOnlyDictionary<string, decimal> Amounts => _amounts;
}

internal sealed class CountAnalysisVisitor : IAccountingAnalysisVisitor
{
    private readonly Dictionary<string, decimal> _counts =
        new(StringComparer.OrdinalIgnoreCase);

    public void Visit(InvoiceAnalysisDocument document)
    {
        _counts["invoice"] = 1m;
    }

    public void Visit(PaymentAnalysisDocument document)
    {
        _counts["payment"] = 1m;
    }

    public void Visit(JournalAnalysisDocument document)
    {
        _counts["journal"] = 1m;
    }

    public IReadOnlyDictionary<string, decimal> Counts => _counts;
}

public sealed class DocumentAnalysisChallenge : IDocumentAnalysisChallenge
{
    public DocumentAnalysisResult Analyze(DocumentAnalysisRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.View))
        {
            throw new ArgumentException(
                "La vista de análisis es obligatoria.",
                nameof(request));
        }

        if (request.Documents is null)
        {
            throw new ArgumentException(
                "La colección de documentos es obligatoria.",
                nameof(request));
        }

        if (string.Equals(
                request.View,
                "gross",
                StringComparison.OrdinalIgnoreCase))
        {
            var visitor = new GrossAnalysisVisitor();

            foreach (var document in request.Documents)
            {
                if (document is null)
                {
                    throw new ArgumentException(
                        "La colección contiene un documento inválido.",
                        nameof(request));
                }

                document.Accept(visitor);
            }

            var amounts = visitor.Amounts;

            decimal total = 0m;

            foreach (var amount in amounts.Values)
            {
                total += amount;
            }

            return new DocumentAnalysisResult(
                total,
                request.Documents.Count,
                amounts);
        }

        if (string.Equals(
                request.View,
                "count",
                StringComparison.OrdinalIgnoreCase))
        {
            var visitor = new CountAnalysisVisitor();

            foreach (var document in request.Documents)
            {
                if (document is null)
                {
                    throw new ArgumentException(
                        "La colección contiene un documento inválido.",
                        nameof(request));
                }

                document.Accept(visitor);
            }

            var counts = visitor.Counts;

            return new DocumentAnalysisResult(
                request.Documents.Count,
                request.Documents.Count,
                counts);
        }

        throw new ArgumentException(
            $"La vista de análisis '{request.View}' no está soportada.",
            nameof(request));
    }
}
