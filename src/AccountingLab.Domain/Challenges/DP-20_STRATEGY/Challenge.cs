
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP20Strategy;

public sealed record FictionalTaxRule(
    string Name,
    string OperationKind,
    DateOnly ValidFrom,
    DateOnly ValidTo,
    string CalculationKind,
    decimal Rate,
    decimal FixedAmount);

public sealed record TaxCalculationRequest(
    decimal BaseAmount,
    string OperationKind,
    DateOnly AccountingDate,
    IReadOnlyList<FictionalTaxRule> Rules);

public sealed record TaxCalculationResult(
    decimal BaseAmount,
    decimal AppliedRate,
    decimal TaxAmount,
    string RuleName);

public interface IConfigurableTaxChallenge
{
    TaxCalculationResult Calculate(TaxCalculationRequest request);
}

internal interface ITaxCalculationStrategy
{
    decimal Calculate(decimal baseAmount, FictionalTaxRule rule);
}

internal sealed class PercentageTaxStrategy : ITaxCalculationStrategy
{
    public decimal Calculate(decimal baseAmount, FictionalTaxRule rule)
    {
        return baseAmount * rule.Rate;
    }
}

internal sealed class FixedTaxStrategy : ITaxCalculationStrategy
{
    public decimal Calculate(decimal baseAmount, FictionalTaxRule rule)
    {
        return rule.FixedAmount;
    }
}

public sealed class ConfigurableTaxChallenge : IConfigurableTaxChallenge
{
    private readonly IReadOnlyDictionary<string, ITaxCalculationStrategy> _strategies =
        new Dictionary<string, ITaxCalculationStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            ["percentage"] = new PercentageTaxStrategy(),
            ["fixed"] = new FixedTaxStrategy()
        };

    public TaxCalculationResult Calculate(TaxCalculationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.BaseAmount < 0m)
            throw new ArgumentException("La base de cálculo no puede ser negativa.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.OperationKind))
            throw new ArgumentException("El tipo de operación es obligatorio.", nameof(request));

        if (request.Rules is null)
            throw new ArgumentException("Las reglas de cálculo son obligatorias.", nameof(request));

        // Buscar una regla compatible y vigente.
        var matchingRules = new List<FictionalTaxRule>();

        foreach (var rule in request.Rules)
        {
            if (rule is null)
                throw new ArgumentException("La lista contiene una regla inválida.", nameof(request));

            if (string.Equals(
                    rule.OperationKind,
                    request.OperationKind,
                    StringComparison.OrdinalIgnoreCase) &&
                request.AccountingDate >= rule.ValidFrom &&
                request.AccountingDate <= rule.ValidTo)
            {
                matchingRules.Add(rule);
            }
        }

        if (matchingRules.Count == 0)
        {
            throw new InvalidOperationException(
                "No existe una regla de cálculo vigente para esta operación y fecha.");
        }

        if (matchingRules.Count > 1)
        {
            throw new InvalidOperationException(
                "Existe más de una regla vigente para la operación solicitada.");
        }

        var selectedRule = matchingRules[0];

        if (string.IsNullOrWhiteSpace(selectedRule.Name))
            throw new ArgumentException("El nombre de la regla es obligatorio.", nameof(request));

        if (selectedRule.Rate < 0m || selectedRule.FixedAmount < 0m)
            throw new ArgumentException("Los valores de la regla no pueden ser negativos.", nameof(request));

        // Seleccionar la estrategia correspondiente.
        if (!_strategies.TryGetValue(selectedRule.CalculationKind, out var strategy))
        {
            throw new ArgumentException(
                $"El algoritmo '{selectedRule.CalculationKind}' no está soportado.",
                nameof(request));
        }

        if (string.Equals(
                selectedRule.CalculationKind,
                "percentage",
                StringComparison.OrdinalIgnoreCase) &&
            selectedRule.Rate > 1m)
        {
            throw new ArgumentException(
                "La tasa porcentual debe estar expresada entre 0 y 1.",
                nameof(request));
        }

        // Ejecutar el algoritmo seleccionado.
        var calculatedAmount = strategy.Calculate(request.BaseAmount, selectedRule);

        // Redondear el resultado a cuatro decimales.
        var taxAmount = decimal.Round(
            calculatedAmount,
            4,
            MidpointRounding.AwayFromZero);

        var appliedRate = string.Equals(
            selectedRule.CalculationKind,
            "percentage",
            StringComparison.OrdinalIgnoreCase)
                ? selectedRule.Rate
                : 0m;

        return new TaxCalculationResult(
            request.BaseAmount,
            appliedRate,
            taxAmount,
            selectedRule.Name);
    }
}
