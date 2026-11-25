
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP14Command;

public sealed record AccountingOperationRequest(
    Guid CompanyId,
    Guid OperationId,
    string Kind,
    Guid TargetId,
    decimal Amount);

public sealed record AccountingOperationReceipt(
    Guid OperationId,
    Guid TargetId,
    string Kind,
    decimal AppliedAmount,
    decimal RemainingAmount,
    Guid LinkedJournalEntryId,
    bool Replayed);

public sealed record AccountingOperationContext(
    decimal OutstandingAmount,
    bool TargetIsPublished,
    IReadOnlyList<AccountingOperationReceipt> CompletedOperations);

public interface IIdentifiedOperationChallenge
{
    AccountingOperationReceipt Execute(
        AccountingOperationRequest request,
        AccountingOperationContext context);
}

internal interface IAccountingCommand
{
    AccountingOperationReceipt Execute(
        AccountingOperationRequest request,
        AccountingOperationContext context);
}

internal sealed class ApplyPaymentCommand : IAccountingCommand
{
    public AccountingOperationReceipt Execute(
        AccountingOperationRequest request,
        AccountingOperationContext context)
    {
        if (request.Amount > context.OutstandingAmount)
        {
            throw new InvalidOperationException(
                "El pago no puede superar el saldo pendiente.");
        }

        var remainingAmount = context.OutstandingAmount - request.Amount;

        return new AccountingOperationReceipt(
            request.OperationId,
            request.TargetId,
            request.Kind,
            request.Amount,
            remainingAmount,
            Guid.NewGuid(),
            false);
    }
}

internal sealed class ReverseVoucherCommand : IAccountingCommand
{
    public AccountingOperationReceipt Execute(
        AccountingOperationRequest request,
        AccountingOperationContext context)
    {
        if (!context.TargetIsPublished)
        {
            throw new InvalidOperationException(
                "No se puede reversar un comprobante que no está publicado.");
        }

        return new AccountingOperationReceipt(
            request.OperationId,
            request.TargetId,
            request.Kind,
            request.Amount,
            context.OutstandingAmount,
            Guid.NewGuid(),
            false);
    }
}

public sealed class IdentifiedOperationChallenge : IIdentifiedOperationChallenge
{
    private readonly IReadOnlyDictionary<string, IAccountingCommand> _commands =
        new Dictionary<string, IAccountingCommand>(StringComparer.Ordinal)
        {
            ["apply-payment"] = new ApplyPaymentCommand(),
            ["reverse-voucher"] = new ReverseVoucherCommand()
        };

    public AccountingOperationReceipt Execute(
        AccountingOperationRequest request,
        AccountingOperationContext context)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);

        if (request.CompanyId == Guid.Empty)
            throw new ArgumentException("La empresa es obligatoria.", nameof(request));

        if (request.OperationId == Guid.Empty)
            throw new ArgumentException("El identificador de operación es obligatorio.", nameof(request));

        if (request.TargetId == Guid.Empty)
            throw new ArgumentException("El identificador del objetivo es obligatorio.", nameof(request));

        if (request.Amount <= 0m)
            throw new ArgumentException("El importe debe ser mayor que cero.", nameof(request));

        if (context.OutstandingAmount < 0m)
            throw new ArgumentException("El saldo pendiente no puede ser negativo.", nameof(context));

        if (context.CompletedOperations is null)
            throw new ArgumentException("El historial de operaciones es obligatorio.", nameof(context));

        // Comprobar si la operación ya fue ejecutada.
        foreach (var completed in context.CompletedOperations)
        {
            if (completed.OperationId != request.OperationId)
                continue;

            // Evitar reutilizar un identificador para otra operación.
            if (completed.TargetId != request.TargetId ||
                !string.Equals(completed.Kind, request.Kind, StringComparison.Ordinal) ||
                completed.AppliedAmount != request.Amount)
            {
                throw new InvalidOperationException(
                    "El identificador de operación ya fue utilizado con otros datos.");
            }

            // Devolver el resultado anterior sin generar otro asiento.
            return completed with { Replayed = true };
        }

        // Seleccionar el comando correspondiente.
        if (!_commands.TryGetValue(request.Kind, out var command))
        {
            throw new ArgumentException(
                $"El tipo de operación '{request.Kind}' no está soportado.",
                nameof(request));
        }

        // Ejecutar el comando seleccionado.
        return command.Execute(request, context);
    }
}
