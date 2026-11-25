
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP11Flyweight;

public sealed record MovementClassificationInput(
    Guid Id,
    decimal Amount,
    string Code,
    string Label,
    string FictionalTaxScope);

public sealed record ClassificationMetadata(
    string Code,
    string Label,
    string FictionalTaxScope);

public sealed record ClassifiedMovement(
    Guid Id,
    decimal Amount,
    ClassificationMetadata Metadata);

public interface ISharedClassificationChallenge
{
    IReadOnlyList<ClassifiedMovement> Prepare(
        IReadOnlyList<MovementClassificationInput> movements);
}

public sealed class SharedClassificationChallenge : ISharedClassificationChallenge
{
    public IReadOnlyList<ClassifiedMovement> Prepare(
        IReadOnlyList<MovementClassificationInput> movements)
    {
        ArgumentNullException.ThrowIfNull(movements);

        // Flyweight: almacena y reutiliza los metadatos compartidos.
        var sharedMetadata = new Dictionary<
            (string Code, string Label, string FictionalTaxScope),
            ClassificationMetadata>();

        var result = new List<ClassifiedMovement>(movements.Count);

        foreach (var movement in movements)
        {
            ArgumentNullException.ThrowIfNull(movement);

            var key = (
                movement.Code,
                movement.Label,
                movement.FictionalTaxScope);

            // Si la clasificación ya existe, reutiliza el mismo objeto.
            if (!sharedMetadata.TryGetValue(key, out var metadata))
            {
                metadata = new ClassificationMetadata(
                    movement.Code,
                    movement.Label,
                    movement.FictionalTaxScope);

                sharedMetadata.Add(key, metadata);
            }

            // Cada movimiento conserva su propio Id y Amount.
            result.Add(new ClassifiedMovement(
                movement.Id,
                movement.Amount,
                metadata));
        }

        return result;
    }
}
