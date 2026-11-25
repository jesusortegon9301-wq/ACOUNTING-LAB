
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP04Prototype;

// ======================================================
// 1. MODELO DE LÍNEA CONTABLE
// ======================================================

public sealed class TemplateLine
{
    public string AccountCode { get; set; } = "";
    public decimal Amount { get; set; }
    public List<string> Tags { get; set; } = new();
}

// ======================================================
// 2. MODELO DE PLANTILLA RECURRENTE
// ======================================================

public sealed class RecurringTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public List<TemplateLine> Lines { get; set; } = new();
}

// ======================================================
// 3. CONTRATO DEL DESAFÍO
// ======================================================

public interface ITemplateCopyChallenge
{
    RecurringTemplate Copy(RecurringTemplate source);
}

// ======================================================
// 4. IMPLEMENTACIÓN PROTOTYPE
// ======================================================

public sealed class TemplateCopyChallenge : ITemplateCopyChallenge
{
    public RecurringTemplate Copy(RecurringTemplate source)
    {
        ArgumentNullException.ThrowIfNull(source);

        // Crear una nueva plantilla independiente
        var copy = new RecurringTemplate
        {
            Id = Guid.NewGuid(),
            Name = source.Name,
            Lines = new List<TemplateLine>()
        };

        // Copiar cada línea y sus etiquetas
        foreach (var line in source.Lines)
        {
            if (line is null)
            {
                throw new ArgumentException(
                    "La plantilla contiene una línea nula.",
                    nameof(source));
            }

            var copiedLine = new TemplateLine
            {
                AccountCode = line.AccountCode,
                Amount = line.Amount,
                Tags = line.Tags is null
                    ? new List<string>()
                    : new List<string>(line.Tags)
            };

            copy.Lines.Add(copiedLine);
        }

        return copy;
    }
}
