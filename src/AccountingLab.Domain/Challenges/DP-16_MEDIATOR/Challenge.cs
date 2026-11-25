
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP16Mediator;

public sealed record InvoiceProcessingRequest(
    Guid InvoiceId,
    decimal Amount,
    bool IsValid,
    IReadOnlyList<string> Participants);

public sealed record InvoiceProcessingResult(
    Guid InvoiceId,
    bool Published,
    IReadOnlyList<string> ParticipantsInvoked);

public interface IInvoiceCoordinationChallenge
{
    InvoiceProcessingResult Process(InvoiceProcessingRequest request);
}

internal sealed class InvoiceMediator
{
    private static readonly string[] RequiredParticipants =
    {
        "validacion",
        "contabilizacion",
        "publicacion"
    };

    public InvoiceProcessingResult Process(InvoiceProcessingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.InvoiceId == Guid.Empty)
            throw new ArgumentException("El identificador de factura es obligatorio.", nameof(request));

        if (request.Participants is null)
            throw new ArgumentException("Los participantes son obligatorios.", nameof(request));

        var registered = new HashSet<string>(StringComparer.Ordinal);

        foreach (var participant in request.Participants)
        {
            if (string.IsNullOrWhiteSpace(participant))
                throw new ArgumentException("Existe un participante inválido.", nameof(request));

            if (!registered.Add(participant))
            {
                throw new ArgumentException(
                    $"El participante '{participant}' está registrado más de una vez.",
                    nameof(request));
            }

            if (Array.IndexOf(RequiredParticipants, participant) < 0)
            {
                throw new ArgumentException(
                    $"El participante '{participant}' no está soportado.",
                    nameof(request));
            }
        }

        var invoked = new List<string>();

        // La validación es el primer paso obligatorio.
        if (!registered.Contains("validacion"))
            throw new ArgumentException("Falta el participante de validación.", nameof(request));

        invoked.Add("validacion");

        if (!request.IsValid)
        {
            return new InvoiceProcessingResult(
                request.InvoiceId,
                false,
                invoked.ToArray());
        }

        // La contabilización requiere una validación exitosa.
        if (!registered.Contains("contabilizacion"))
            throw new ArgumentException("Falta el participante de contabilización.", nameof(request));

        invoked.Add("contabilizacion");

        // La publicación requiere que la factura haya sido contabilizada.
        if (!registered.Contains("publicacion"))
            throw new ArgumentException("Falta el participante de publicación.", nameof(request));

        invoked.Add("publicacion");

        return new InvoiceProcessingResult(
            request.InvoiceId,
            true,
            invoked.ToArray());
    }
}

public sealed class InvoiceCoordinationChallenge : IInvoiceCoordinationChallenge
{
    private readonly InvoiceMediator _mediator = new();

    public InvoiceProcessingResult Process(InvoiceProcessingRequest request)
    {
        return _mediator.Process(request);
    }
}
