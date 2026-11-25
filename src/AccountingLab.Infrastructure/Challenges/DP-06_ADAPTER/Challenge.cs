
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using AccountingLab.Domain.Education;

namespace AccountingLab.Infrastructure.Challenges.DP06Adapter;

public sealed record BankImportRequest(string Provider, string Payload);

public sealed record ImportedBankMovement(
    string ExternalId,
    DateOnly Date,
    string Description,
    decimal Debit,
    decimal Credit,
    string Currency);

public interface IBankImportChallenge
{
    IReadOnlyList<ImportedBankMovement> Import(BankImportRequest request);
}

public sealed class BankImportChallenge : IBankImportChallenge
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IReadOnlyList<ImportedBankMovement> Import(BankImportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Payload))
        {
            throw new FormatException("El contenido bancario está vacío.");
        }

        return request.Provider?.Trim() switch
        {
            "BancoA" => ImportBancoA(request.Payload),
            "BancoB" => ImportBancoB(request.Payload),
            _ => throw new ArgumentException(
                "El proveedor bancario no es compatible.",
                nameof(request))
        };
    }

    // ADAPTER: convierte el formato de BancoA
    // al modelo común ImportedBankMovement.
    private static IReadOnlyList<ImportedBankMovement> ImportBancoA(
        string payload)
    {
        var records = Deserialize<BancoARecord>(payload);

        var movements = new List<ImportedBankMovement>();

        foreach (var record in records)
        {
            if (record is null)
            {
                throw new FormatException("Movimiento bancario inválido.");
            }

            movements.Add(new ImportedBankMovement(
                RequireValue(record.Id, "id"),
                ParseDate(record.Date, "date"),
                RequireValue(record.Description, "description"),
                ParseAmount(record.Debit, "debit"),
                ParseAmount(record.Credit, "credit"),
                RequireValue(record.Currency, "currency")
            ));
        }

        return movements;
    }

    // ADAPTER: convierte el formato de BancoB
    // al mismo modelo común.
    private static IReadOnlyList<ImportedBankMovement> ImportBancoB(
        string payload)
    {
        var records = Deserialize<BancoBRecord>(payload);

        var movements = new List<ImportedBankMovement>();

        foreach (var record in records)
        {
            if (record is null)
            {
                throw new FormatException("Movimiento bancario inválido.");
            }

            var signedAmount = ParseAmount(
                record.SignedAmount,
                "signedAmount");

            var debit = signedAmount < 0
                ? Math.Abs(signedAmount)
                : 0m;

            var credit = signedAmount > 0
                ? signedAmount
                : 0m;

            movements.Add(new ImportedBankMovement(
                RequireValue(record.Reference, "reference"),
                ParseDate(record.BookedOn, "bookedOn"),
                RequireValue(record.Concept, "concept"),
                debit,
                credit,
                RequireValue(record.IsoCurrency, "isoCurrency")
            ));
        }

        return movements;
    }

    // Deserializa el JSON y devuelve una lista de registros.
    private static List<T> Deserialize<T>(string payload)
    {
        try
        {
            return JsonSerializer.Deserialize<List<T>>(
                payload,
                JsonOptions)
                ?? throw new FormatException(
                    "El contenido JSON no contiene movimientos.");
        }
        catch (JsonException exception)
        {
            throw new FormatException(
                "El contenido bancario tiene un formato JSON inválido.",
                exception);
        }
    }

    private static string RequireValue(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FormatException(
                $"El campo {field} es obligatorio.");
        }

        return value;
    }

    private static DateOnly ParseDate(string? value, string field)
    {
        if (!DateOnly.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            throw new FormatException(
                $"La fecha del campo {field} no es válida.");
        }

        return date;
    }

    private static decimal ParseAmount(string? value, string field)
    {
        if (!decimal.TryParse(
                value,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var amount))
        {
            throw new FormatException(
                $"El importe del campo {field} no es válido.");
        }

        return amount;
    }

    // Estructura original de BancoA.
    private sealed class BancoARecord
    {
        public string? Id { get; set; }
        public string? Date { get; set; }
        public string? Description { get; set; }
        public string? Debit { get; set; }
        public string? Credit { get; set; }
        public string? Currency { get; set; }
    }

    // Estructura original de BancoB.
    private sealed class BancoBRecord
    {
        public string? Reference { get; set; }
        public string? BookedOn { get; set; }
        public string? Concept { get; set; }
        public string? SignedAmount { get; set; }
        public string? IsoCurrency { get; set; }
    }
}
