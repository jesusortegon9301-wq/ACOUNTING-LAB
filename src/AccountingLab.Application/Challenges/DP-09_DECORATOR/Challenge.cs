
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP09Decorator;

public sealed record DocumentExportRequest(
    string Body,
    IReadOnlyList<string> Features,
    IReadOnlyDictionary<string, string> Metadata);

public sealed record ExportedAccountingDocument(
    string Content,
    IReadOnlyList<string> AppliedFeatures,
    IReadOnlyDictionary<string, string> Metadata);

public interface IDocumentEnrichmentChallenge
{
    ExportedAccountingDocument Export(DocumentExportRequest request);
}

// Contrato común de los componentes del Decorator.
internal interface IDocumentContent
{
    string GetContent();
}

// Componente concreto: documento original.
internal sealed class BaseDocument : IDocumentContent
{
    private readonly string _body;

    public BaseDocument(string body)
    {
        _body = body;
    }

    public string GetContent()
    {
        return _body;
    }
}

// Decorator base.
internal abstract class DocumentDecorator : IDocumentContent
{
    protected readonly IDocumentContent Document;

    protected DocumentDecorator(IDocumentContent document)
    {
        Document = document;
    }

    public abstract string GetContent();
}

// Decorator de auditoría.
internal sealed class AuditDecorator : DocumentDecorator
{
    public AuditDecorator(IDocumentContent document)
        : base(document)
    {
    }

    public override string GetContent()
    {
        return Document.GetContent() + Environment.NewLine + "[AUDIT]";
    }
}

// Decorator de metadatos.
internal sealed class MetadataDecorator : DocumentDecorator
{
    private readonly IReadOnlyDictionary<string, string> _metadata;

    public MetadataDecorator(
        IDocumentContent document,
        IReadOnlyDictionary<string, string> metadata)
        : base(document)
    {
        _metadata = metadata;
    }

    public override string GetContent()
    {
        var content = new StringBuilder(Document.GetContent());

        content.AppendLine();
        content.AppendLine("[METADATA]");

        foreach (var item in _metadata)
        {
            content.AppendLine($"{item.Key}={item.Value}");
        }

        return content.ToString().TrimEnd();
    }
}

// Implementación principal del desafío.
public sealed class DocumentEnrichmentChallenge
    : IDocumentEnrichmentChallenge
{
    public ExportedAccountingDocument Export(
        DocumentExportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Features is null)
            throw new ArgumentException(
                "La lista de funcionalidades es obligatoria.",
                nameof(request));

        if (request.Metadata is null)
            throw new ArgumentException(
                "Los metadatos son obligatorios.",
                nameof(request));

        if (request.Body is null)
            throw new ArgumentException(
                "El contenido del documento es obligatorio.",
                nameof(request));

        // Crear el documento original.
        IDocumentContent document = new BaseDocument(request.Body);

        // Aplicar los Decorators solicitados.
        foreach (var feature in request.Features)
        {
            document = feature switch
            {
                "auditoria" => new AuditDecorator(document),

                "metadatos" => new MetadataDecorator(
                    document,
                    request.Metadata),

                _ => throw new ArgumentException(
                    $"La funcionalidad '{feature}' no está disponible.",
                    nameof(request))
            };
        }

        // Copiar los metadatos para no modificar el original.
        var metadataCopy = new Dictionary<string, string>(
            request.Metadata);

        // Devolver el documento enriquecido.
        return new ExportedAccountingDocument(
            document.GetContent(),
            request.Features.ToArray(),
            metadataCopy);
    }
}
