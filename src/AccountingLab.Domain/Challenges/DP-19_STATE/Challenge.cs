
using System;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP19State;

public sealed record InvoiceLifecycleRequest(
    string CurrentState,
    string Action,
    decimal OutstandingAmount,
    DateOnly DueDate,
    DateOnly BusinessDate);

public sealed record InvoiceTransitionResult(
    string State,
    bool PaymentAllowed);

public interface IInvoiceLifecycleChallenge
{
    InvoiceTransitionResult Transition(InvoiceLifecycleRequest request);
}

public sealed class InvoiceLifecycleChallenge : IInvoiceLifecycleChallenge
{
    public InvoiceTransitionResult Transition(InvoiceLifecycleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.CurrentState))
            throw new ArgumentException("El estado actual es obligatorio.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.Action))
            throw new ArgumentException("La acción es obligatoria.", nameof(request));

        if (request.OutstandingAmount < 0m)
            throw new ArgumentException("El saldo pendiente no puede ser negativo.", nameof(request));

        var currentState = request.CurrentState.Trim().ToLowerInvariant();
        var action = request.Action.Trim().ToLowerInvariant();

        return currentState switch
        {
            "draft" => HandleDraft(action, request),

            "issued" => HandleIssued(action, request),

            "paid" => HandlePaid(action),

            "overdue" => HandleOverdue(action, request),

            _ => throw new InvalidOperationException(
                $"El estado '{request.CurrentState}' no está soportado.")
        };
    }

    private static InvoiceTransitionResult HandleDraft(
        string action,
        InvoiceLifecycleRequest request)
    {
        if (action == "emit")
        {
            return new InvoiceTransitionResult(
                "issued",
                request.OutstandingAmount > 0m);
        }

        throw new InvalidOperationException(
            $"La acción '{action}' no está permitida para una factura en borrador.");
    }

    private static InvoiceTransitionResult HandleIssued(
        string action,
        InvoiceLifecycleRequest request)
    {
        if (action == "pay")
        {
            return new InvoiceTransitionResult(
                "paid",
                false);
        }

        if (action == "mark-overdue")
        {
            if (request.BusinessDate > request.DueDate &&
                request.OutstandingAmount > 0m)
            {
                return new InvoiceTransitionResult(
                    "overdue",
                    true);
            }

            return new InvoiceTransitionResult(
                "issued",
                request.OutstandingAmount > 0m);
        }

        throw new InvalidOperationException(
            $"La acción '{action}' no está permitida para una factura emitida.");
    }

    private static InvoiceTransitionResult HandlePaid(string action)
    {
        throw new InvalidOperationException(
            $"La acción '{action}' no está permitida para una factura pagada.");
    }

    private static InvoiceTransitionResult HandleOverdue(
        string action,
        InvoiceLifecycleRequest request)
    {
        if (action == "pay")
        {
            return new InvoiceTransitionResult(
                "paid",
                false);
        }

        if (action == "mark-overdue")
        {
            return new InvoiceTransitionResult(
                "overdue",
                request.OutstandingAmount > 0m);
        }

        throw new InvalidOperationException(
            $"La acción '{action}' no está permitida para una factura vencida.");
    }
}
