
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP21TemplateMethod;

public sealed record BankStatementItem(
    string Id,
    string Reference,
    DateOnly Date,
    decimal Amount);

public sealed record ReconciliationLedgerItem(
    string Id,
    string Reference,
    DateOnly Date,
    decimal Amount);

public sealed record BankReconciliationRequest(
    string Mode,
    IReadOnlyList<BankStatementItem> BankItems,
    IReadOnlyList<ReconciliationLedgerItem> LedgerItems,
    decimal Tolerance);

public sealed record ReconciliationMatch(
    string BankItemId,
    string LedgerItemId,
    decimal Difference);

public sealed record BankReconciliationResult(
    IReadOnlyList<ReconciliationMatch> Matches,
    IReadOnlyList<string> UnmatchedBankItems,
    IReadOnlyList<string> UnmatchedLedgerItems);

public interface IBankReconciliationChallenge
{
    BankReconciliationResult Reconcile(BankReconciliationRequest request);
}

// Template Method: define el algoritmo general de conciliación.
internal abstract class ReconciliationTemplate
{
    public BankReconciliationResult Reconcile(BankReconciliationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Tolerance < 0m)
            throw new ArgumentOutOfRangeException(nameof(request.Tolerance));

        if (request.BankItems is null)
            throw new ArgumentException("Los movimientos bancarios son obligatorios.");

        if (request.LedgerItems is null)
            throw new ArgumentException("Los movimientos contables son obligatorios.");

        var matches = new List<ReconciliationMatch>();
        var unmatchedBank = new List<string>();
        var unmatchedLedger = new List<string>();

        var ledgerUsed = new bool[request.LedgerItems.Count];

        // Paso 1: recorrer los movimientos bancarios.
        foreach (var bankItem in request.BankItems)
        {
            if (bankItem is null)
                throw new ArgumentException("Existe un movimiento bancario inválido.");

            var foundMatch = false;

            // Paso 2: buscar un movimiento contable compatible.
            for (var i = 0; i < request.LedgerItems.Count; i++)
            {
                if (ledgerUsed[i])
                    continue;

                var ledgerItem = request.LedgerItems[i];

                if (ledgerItem is null)
                    throw new ArgumentException("Existe un movimiento contable inválido.");

                // Paso 3: comparar referencia y fecha.
                if (!string.Equals(
                        bankItem.Reference,
                        ledgerItem.Reference,
                        StringComparison.OrdinalIgnoreCase) ||
                    bankItem.Date != ledgerItem.Date)
                {
                    continue;
                }

                var difference = Math.Abs(
                    bankItem.Amount - ledgerItem.Amount);

                // Paso 4: aplicar el criterio definido por la subclase.
                if (!IsAmountMatch(difference, request.Tolerance))
                    continue;

                // Paso 5: registrar la coincidencia.
                matches.Add(new ReconciliationMatch(
                    bankItem.Id,
                    ledgerItem.Id,
                    difference));

                // Evitar reutilizar el movimiento contable.
                ledgerUsed[i] = true;
                foundMatch = true;
                break;
            }

            if (!foundMatch)
                unmatchedBank.Add(bankItem.Id);
        }

        // Paso 6: identificar movimientos contables sin coincidencia.
        for (var i = 0; i < request.LedgerItems.Count; i++)
        {
            if (!ledgerUsed[i])
                unmatchedLedger.Add(request.LedgerItems[i].Id);
        }

        return new BankReconciliationResult(
            matches,
            unmatchedBank,
            unmatchedLedger);
    }

    // Operación que cada implementación especializada define.
    protected abstract bool IsAmountMatch(
        decimal difference,
        decimal tolerance);
}

// Implementación para coincidencias exactas.
internal sealed class ExactReconciliation : ReconciliationTemplate
{
    protected override bool IsAmountMatch(
        decimal difference,
        decimal tolerance)
    {
        return difference == 0m;
    }
}

// Implementación para coincidencias con tolerancia.
internal sealed class ToleranceReconciliation : ReconciliationTemplate
{
    protected override bool IsAmountMatch(
        decimal difference,
        decimal tolerance)
    {
        return difference <= tolerance;
    }
}

public sealed class BankReconciliationChallenge : IBankReconciliationChallenge
{
    public BankReconciliationResult Reconcile(BankReconciliationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Mode))
            throw new ArgumentException("El modo de conciliación es obligatorio.");

        ReconciliationTemplate template = request.Mode.ToLowerInvariant() switch
        {
            "exact" => new ExactReconciliation(),
            "tolerance" => new ToleranceReconciliation(),
            _ => throw new ArgumentException(
                $"El modo de conciliación '{request.Mode}' no está soportado.")
        };

        return template.Reconcile(request);
    }
}
