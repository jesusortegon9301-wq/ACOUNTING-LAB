
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP13ChainOfResponsibility;

public sealed record PaymentApprovalRequest(
    decimal Amount,
    decimal AvailableFunds,
    bool PolicyAllows,
    decimal AuthorizationLimit,
    IReadOnlyList<string> Checks);

public sealed record PaymentApprovalDecision(
    bool Approved,
    string? RejectedBy,
    IReadOnlyList<string> VisitedChecks);

public interface IPaymentApprovalChallenge
{
    PaymentApprovalDecision Evaluate(PaymentApprovalRequest request);
}

internal interface IPaymentCheck
{
    string Name { get; }

    bool IsApproved(PaymentApprovalRequest request);
}

internal sealed class PolicyCheck : IPaymentCheck
{
    public string Name => "politica";

    public bool IsApproved(PaymentApprovalRequest request)
    {
        return request.PolicyAllows;
    }
}

internal sealed class FundsCheck : IPaymentCheck
{
    public string Name => "fondos";

    public bool IsApproved(PaymentApprovalRequest request)
    {
        return request.Amount <= request.AvailableFunds;
    }
}

internal sealed class AuthorizationLimitCheck : IPaymentCheck
{
    public string Name => "limite";

    public bool IsApproved(PaymentApprovalRequest request)
    {
        return request.Amount <= request.AuthorizationLimit;
    }
}

public sealed class PaymentApprovalChallenge : IPaymentApprovalChallenge
{
    private readonly IReadOnlyDictionary<string, IPaymentCheck> _checks =
        new Dictionary<string, IPaymentCheck>(StringComparer.Ordinal)
        {
            ["politica"] = new PolicyCheck(),
            ["fondos"] = new FundsCheck(),
            ["limite"] = new AuthorizationLimitCheck()
        };

    public PaymentApprovalDecision Evaluate(PaymentApprovalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Checks is null)
            throw new ArgumentException("La cadena de validaciones es obligatoria.", nameof(request));

        if (request.Amount < 0m ||
            request.AvailableFunds < 0m ||
            request.AuthorizationLimit < 0m)
        {
            throw new ArgumentException(
                "Los valores monetarios no pueden ser negativos.",
                nameof(request));
        }

        var visitedChecks = new List<string>();

        foreach (var checkName in request.Checks)
        {
            if (string.IsNullOrWhiteSpace(checkName) ||
                !_checks.TryGetValue(checkName, out var check))
            {
                throw new ArgumentException(
                    $"La validación '{checkName}' no está soportada.",
                    nameof(request));
            }

            visitedChecks.Add(check.Name);

            if (!check.IsApproved(request))
            {
                return new PaymentApprovalDecision(
                    false,
                    check.Name,
                    visitedChecks.ToArray());
            }
        }

        return new PaymentApprovalDecision(
            true,
            null,
            visitedChecks.ToArray());
    }
}
