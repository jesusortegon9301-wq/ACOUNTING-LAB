
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP10Facade;

public sealed record ClosingAccountBalance(
    string AccountCode,
    decimal Debit,
    decimal Credit);

public sealed record ClosePeriodRequest(
    Guid CompanyId,
    DateOnly From,
    DateOnly To,
    IReadOnlyList<ClosingAccountBalance> Balances,
    int UnpostedDocuments);

public sealed record PeriodClosingSummary(
    Guid CompanyId,
    DateOnly From,
    DateOnly To,
    bool Closed,
    decimal TotalDebit,
    decimal TotalCredit);

public interface IPeriodClosingChallenge
{
    PeriodClosingSummary Close(ClosePeriodRequest request);
}

// Subsistema encargado de validar el cierre contable.
internal sealed class PeriodClosingValidator
{
    public void Validate(ClosePeriodRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CompanyId == Guid.Empty)
        {
            throw new ArgumentException(
                "La identificación de la empresa es obligatoria.",
                nameof(request));
        }

        if (request.From > request.To)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final.",
                nameof(request));
        }

        if (request.UnpostedDocuments < 0)
        {
            throw new ArgumentException(
                "La cantidad de documentos pendientes no puede ser negativa.",
                nameof(request));
        }

        if (request.UnpostedDocuments > 0)
        {
            throw new InvalidOperationException(
                "No se puede cerrar el período porque existen documentos pendientes de contabilizar.");
        }

        if (request.Balances is null)
        {
            throw new ArgumentException(
                "Los saldos contables son obligatorios.",
                nameof(request));
        }

        foreach (var balance in request.Balances)
        {
            if (balance is null)
            {
                throw new ArgumentException(
                    "La lista contiene un saldo contable inválido.",
                    nameof(request));
            }

            if (string.IsNullOrWhiteSpace(balance.AccountCode))
            {
                throw new ArgumentException(
                    "El código de cuenta es obligatorio.",
                    nameof(request));
            }

            if (balance.Debit < 0 || balance.Credit < 0)
            {
                throw new ArgumentException(
                    "Los valores de débito y crédito no pueden ser negativos.",
                    nameof(request));
            }
        }
    }
}

// Subsistema encargado de consolidar los saldos.
internal sealed class ClosingBalanceConsolidator
{
    public (decimal TotalDebit, decimal TotalCredit) Consolidate(
        IReadOnlyList<ClosingAccountBalance> balances)
    {
        decimal totalDebit = 0m;
        decimal totalCredit = 0m;

        foreach (var balance in balances)
        {
            totalDebit += balance.Debit;
            totalCredit += balance.Credit;
        }

        return (totalDebit, totalCredit);
    }
}

// Fachada: coordina los subsistemas y ejecuta el cierre.
public sealed class PeriodClosingChallenge : IPeriodClosingChallenge
{
    private readonly PeriodClosingValidator _validator;
    private readonly ClosingBalanceConsolidator _consolidator;

    public PeriodClosingChallenge()
    {
        _validator = new PeriodClosingValidator();
        _consolidator = new ClosingBalanceConsolidator();
    }

    public PeriodClosingSummary Close(ClosePeriodRequest request)
    {
        // Paso 1: validar la información del cierre.
        _validator.Validate(request);

        // Paso 2: consolidar los débitos y créditos.
        var totals = _consolidator.Consolidate(request.Balances);

        // Paso 3: comprobar que el cierre esté equilibrado.
        if (totals.TotalDebit != totals.TotalCredit)
        {
            throw new InvalidOperationException(
                "No se puede cerrar el período porque los débitos y créditos no están equilibrados.");
        }

        // Paso 4: confirmar el cierre contable.
        return new PeriodClosingSummary(
            request.CompanyId,
            request.From,
            request.To,
            true,
            totals.TotalDebit,
            totals.TotalCredit);
    }
}
