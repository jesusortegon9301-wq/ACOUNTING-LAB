
using System;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP08Composite;

public sealed record AccountHierarchy(
    string Code,
    bool AllowsPosting,
    decimal OwnBalance,
    IReadOnlyList<AccountHierarchy> Children);

public interface IAccountTreeChallenge
{
    decimal Total(AccountHierarchy account);
}

public sealed class AccountTreeChallenge : IAccountTreeChallenge
{
    public decimal Total(AccountHierarchy account)
    {
        ArgumentNullException.ThrowIfNull(account);

        // Una cuenta que permite contabilizar
        // debe ser una cuenta individual, sin hijos.
        if (account.AllowsPosting)
        {
            if (account.Children.Count > 0)
            {
                throw new ArgumentException(
                    "Una cuenta que permite contabilizar no puede contener cuentas hijas.",
                    nameof(account));
            }

            return account.OwnBalance;
        }

        // Una cuenta agrupadora suma recursivamente
        // los saldos de todas sus cuentas hijas.
        decimal total = 0m;

        foreach (var child in account.Children)
        {
            total += Total(child);
        }

        return total;
    }
}
