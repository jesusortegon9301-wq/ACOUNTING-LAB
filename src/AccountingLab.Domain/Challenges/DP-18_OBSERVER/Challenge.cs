
using System;
using System.Collections.Generic;
using System.Linq;
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP18Observer;

public sealed record FinancialEvent(
    Guid EventId,
    string Kind,
    decimal Amount);

public interface IFinancialNotificationsChallenge
{
    void Subscribe(string subscriberId, Action<FinancialEvent> receive);
    void Unsubscribe(string subscriberId);
    IReadOnlyList<string> Publish(FinancialEvent financialEvent);
}

public sealed class FinancialNotificationsChallenge : IFinancialNotificationsChallenge
{
    private readonly Dictionary<string, Action<FinancialEvent>> _subscribers =
        new(StringComparer.Ordinal);

    public void Subscribe(string subscriberId, Action<FinancialEvent> receive)
    {
        if (string.IsNullOrWhiteSpace(subscriberId))
        {
            throw new ArgumentException(
                "El identificador del suscriptor es obligatorio.",
                nameof(subscriberId));
        }

        ArgumentNullException.ThrowIfNull(receive);

        if (_subscribers.ContainsKey(subscriberId))
        {
            throw new ArgumentException(
                $"El suscriptor '{subscriberId}' ya está registrado.",
                nameof(subscriberId));
        }

        _subscribers.Add(subscriberId, receive);
    }

    public void Unsubscribe(string subscriberId)
    {
        if (string.IsNullOrWhiteSpace(subscriberId))
        {
            throw new ArgumentException(
                "El identificador del suscriptor es obligatorio.",
                nameof(subscriberId));
        }

        _subscribers.Remove(subscriberId);
    }

    public IReadOnlyList<string> Publish(FinancialEvent financialEvent)
    {
        ArgumentNullException.ThrowIfNull(financialEvent);

        var activeSubscribers = _subscribers
            .OrderBy(subscriber => subscriber.Key, StringComparer.Ordinal)
            .ToList();

        var recipients = new List<string>();

        foreach (var subscriber in activeSubscribers)
        {
            subscriber.Value(financialEvent);
            recipients.Add(subscriber.Key);
        }

        return recipients.AsReadOnly();
    }
}
