
using System;
using System.Collections.Generic;

namespace AccountingLab.Domain.Challenges.DP02AbstractFactory;

// 1. Contratos públicos

public sealed record AccountingProfileRequest(
    string Profile,
    string FictionalRegime);

public sealed record AccountingFamilyMember(
    string Role,
    string CompatibilityKey);

public sealed record AccountingServiceFamily(
    string Profile,
    string FictionalRegime,
    IReadOnlyList<AccountingFamilyMember> Members);

public interface IAccountingProfileChallenge
{
    AccountingServiceFamily CreateFamily(
        AccountingProfileRequest request);
}

// 2. Productos abstractos

internal interface ICalculationService
{
    string CompatibilityKey { get; }
}

internal interface IValidationService
{
    string CompatibilityKey { get; }
}

internal interface IReportService
{
    string CompatibilityKey { get; }
}

// 3. Fábrica abstracta

internal interface IAccountingServiceFactory
{
    ICalculationService CreateCalculation();
    IValidationService CreateValidation();
    IReportService CreateReport();
}

// 4. Productos concretos comerciales

internal sealed class CommercialCalculation : ICalculationService
{
    public string CompatibilityKey { get; }

    public CommercialCalculation(string key)
    {
        CompatibilityKey = key;
    }
}

internal sealed class CommercialValidation : IValidationService
{
    public string CompatibilityKey { get; }

    public CommercialValidation(string key)
    {
        CompatibilityKey = key;
    }
}

internal sealed class CommercialReport : IReportService
{
    public string CompatibilityKey { get; }

    public CommercialReport(string key)
    {
        CompatibilityKey = key;
    }
}

// 5. Productos concretos de servicios

internal sealed class ServicesCalculation : ICalculationService
{
    public string CompatibilityKey { get; }

    public ServicesCalculation(string key)
    {
        CompatibilityKey = key;
    }
}

internal sealed class ServicesValidation : IValidationService
{
    public string CompatibilityKey { get; }

    public ServicesValidation(string key)
    {
        CompatibilityKey = key;
    }
}

internal sealed class ServicesReport : IReportService
{
    public string CompatibilityKey { get; }

    public ServicesReport(string key)
    {
        CompatibilityKey = key;
    }
}

// 6. Fábrica concreta comercial

internal sealed class CommercialFactory : IAccountingServiceFactory
{
    private readonly string _key;

    public CommercialFactory(string key)
    {
        _key = key;
    }

    public ICalculationService CreateCalculation()
        => new CommercialCalculation(_key);

    public IValidationService CreateValidation()
        => new CommercialValidation(_key);

    public IReportService CreateReport()
        => new CommercialReport(_key);
}

// 7. Fábrica concreta servicios

internal sealed class ServicesFactory : IAccountingServiceFactory
{
    private readonly string _key;

    public ServicesFactory(string key)
    {
        _key = key;
    }

    public ICalculationService CreateCalculation()
        => new ServicesCalculation(_key);

    public IValidationService CreateValidation()
        => new ServicesValidation(_key);

    public IReportService CreateReport()
        => new ServicesReport(_key);
}

// 8. Implementación del desafío

public sealed class AccountingProfileChallenge
    : IAccountingProfileChallenge
{
    public AccountingServiceFamily CreateFamily(
        AccountingProfileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var profile = request.Profile?.Trim().ToLowerInvariant();

        var regime = request.FictionalRegime?.Trim();

        if (string.IsNullOrWhiteSpace(profile))
        {
            throw new ArgumentException(
                "El perfil es obligatorio.",
                nameof(request));
        }

        if (string.IsNullOrWhiteSpace(regime))
        {
            throw new ArgumentException(
                "El régimen es obligatorio.",
                nameof(request));
        }

        var compatibilityKey =
            $"{profile.ToUpperInvariant()}:{regime.ToUpperInvariant()}";

        IAccountingServiceFactory factory = profile switch
        {
            "comercial" => new CommercialFactory(compatibilityKey),
            "servicios" => new ServicesFactory(compatibilityKey),

            _ => throw new ArgumentException(
                $"Perfil contable no soportado: {request.Profile}",
                nameof(request))
        };

        var calculation = factory.CreateCalculation();
        var validation = factory.CreateValidation();
        var report = factory.CreateReport();

        var members = new List<AccountingFamilyMember>
        {
            new("calculo", calculation.CompatibilityKey),
            new("validacion", validation.CompatibilityKey),
            new("reporte", report.CompatibilityKey)
        };

        return new AccountingServiceFamily(
            profile,
            regime,
            members);
    }
}
