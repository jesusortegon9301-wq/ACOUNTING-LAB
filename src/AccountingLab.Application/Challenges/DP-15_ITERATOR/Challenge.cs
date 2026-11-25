
using System;
using System.Collections.Generic;
using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP15Iterator;

public sealed record LedgerRecord(
    string EntryCode,
    DateOnly Date,
    decimal Debit,
    decimal Credit);

public sealed record LedgerPage(IReadOnlyList<LedgerRecord> Records);

public interface ILedgerCursor
{
    bool MoveNext();
    LedgerRecord Current { get; }
}

public interface ILedgerTraversalChallenge
{
    ILedgerCursor Open(IReadOnlyList<LedgerPage> pages);
}

internal sealed class LedgerCursor : ILedgerCursor
{
    private readonly IReadOnlyList<LedgerPage> _pages;

    private int _pageIndex;
    private int _recordIndex;
    private LedgerRecord? _current;
    private bool _finished;

    public LedgerCursor(IReadOnlyList<LedgerPage> pages)
    {
        _pages = pages;
        _pageIndex = 0;
        _recordIndex = -1;
    }

    public LedgerRecord Current
    {
        get
        {
            if (_current is null)
            {
                throw new InvalidOperationException(
                    "El cursor no está posicionado sobre un registro válido.");
            }

            return _current;
        }
    }

    public bool MoveNext()
    {
        if (_finished)
            return false;

        _current = null;

        while (_pageIndex < _pages.Count)
        {
            var records = _pages[_pageIndex].Records;

            _recordIndex++;

            if (_recordIndex < records.Count)
            {
                _current = records[_recordIndex];
                return true;
            }

            // Pasar a la siguiente página.
            _pageIndex++;
            _recordIndex = -1;
        }

        _finished = true;
        return false;
    }
}

public sealed class LedgerTraversalChallenge : ILedgerTraversalChallenge
{
    public ILedgerCursor Open(IReadOnlyList<LedgerPage> pages)
    {
        ArgumentNullException.ThrowIfNull(pages);

        foreach (var page in pages)
        {
            if (page is null || page.Records is null)
            {
                throw new ArgumentException(
                    "Las páginas y sus registros son obligatorios.",
                    nameof(pages));
            }
        }

        return new LedgerCursor(pages);
    }
}
