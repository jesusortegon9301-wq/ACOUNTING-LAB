
using System;
using System.Collections.Generic;
using System.Linq;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP17Memento;

public sealed class BudgetDraftLine
{
    public string AccountCode { get; set; } = "";
    public decimal Amount { get; set; }
}

public sealed class BudgetDraft
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public bool Approved { get; set; }
    public List<BudgetDraftLine> Lines { get; set; } = new();
}

public interface IBudgetSnapshot
{
}

public interface IBudgetHistoryChallenge
{
    IBudgetSnapshot Save(BudgetDraft draft);
    BudgetDraft Restore(BudgetDraft current, IBudgetSnapshot snapshot);
}

internal sealed class BudgetSnapshot : IBudgetSnapshot
{
    public Guid Id { get; }
    public string Name { get; }
    public bool Approved { get; }
    public IReadOnlyList<BudgetDraftLine> Lines { get; }

    public BudgetSnapshot(BudgetDraft draft)
    {
        Id = draft.Id;
        Name = draft.Name;
        Approved = draft.Approved;

        Lines = draft.Lines
            .Select(line => new BudgetDraftLine
            {
                AccountCode = line.AccountCode,
                Amount = line.Amount
            })
            .ToList();
    }
}

public sealed class BudgetHistoryChallenge : IBudgetHistoryChallenge
{
    public IBudgetSnapshot Save(BudgetDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (draft.Approved)
        {
            throw new InvalidOperationException(
                "No se puede crear una instantánea de un presupuesto aprobado.");
        }

        if (draft.Lines is null)
        {
            throw new ArgumentException(
                "Las líneas del presupuesto son obligatorias.",
                nameof(draft));
        }

        return new BudgetSnapshot(draft);
    }

    public BudgetDraft Restore(BudgetDraft current, IBudgetSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(snapshot);

        if (snapshot is not BudgetSnapshot savedSnapshot)
        {
            throw new ArgumentException(
                "La instantánea no pertenece a este mecanismo de historial.",
                nameof(snapshot));
        }

        return new BudgetDraft
        {
            Id = savedSnapshot.Id,
            Name = savedSnapshot.Name,
            Approved = savedSnapshot.Approved,
            Lines = savedSnapshot.Lines
                .Select(line => new BudgetDraftLine
                {
                    AccountCode = line.AccountCode,
                    Amount = line.Amount
                })
                .ToList()
        };
    }
}
