using System.Text.Json;
using System.Linq;
using YugiohCardRepository.CardFilters;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using YugiohCardRepository.DTOs;

namespace YugiohCardRepository;
public class CardRepository
{
    private readonly List<Card> _cards;

    public CardRepository(List<Card> cards)
    {
        _cards = cards;
    }

    //give em both options
    public CardRepository(string cardJsonLocation)
    {
        using FileStream fileStream = File.OpenRead(cardJsonLocation);
        CardListRoot root = JsonSerializer.Deserialize<CardListRoot>(fileStream) ?? new CardListRoot();
        _cards = root.Cards;
    }

    public List<Card> GetAllCards()
    {
        return _cards;
    }

    public List<Card> GetCards(TextFilter textFilter, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("propertyName cannot be empty or null.", nameof(propertyName));
        }

        string query = textFilter.Query;

        IQueryable<Card> cardsQuery = _cards.AsQueryable();

        if (textFilter.WholeWord)
        {
            cardsQuery = cardsQuery.Where(c => WholeWordFunction(c.GetPropertyValue<string?>(propertyName), query, textFilter.ExactCase));
        }
        else
        {
            cardsQuery = cardsQuery.Where(c => (c.GetPropertyValue<string?>(propertyName) ?? "").Contains(query, textFilter.ExactCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));
        }

        return cardsQuery.ToList();
    }

    public List<Card> GetCards(NumberFilter numberFilter, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("propertyName cannot be empty or null.", nameof(propertyName));
        }

        var cardsQuery = _cards.AsQueryable()
            .Select(c => new { card = c, numericalValue = c.GetPropertyValue<int?>(propertyName) })
            .Where(c => c.numericalValue.HasValue);

        switch (numberFilter.NumberOperator)
        {
            case NumberOperator.NotEqual:
                return cardsQuery
                    .Where(c => c.numericalValue != numberFilter.RightOperand)
                    .Select(c => c.card)
                    .ToList();
            case NumberOperator.Equal:
                return cardsQuery
                    .Where(c => c.numericalValue == numberFilter.RightOperand)
                    .Select(c => c.card)
                    .ToList();
            case NumberOperator.LessThan:
                return cardsQuery
                    .Where(c => c.numericalValue < numberFilter.RightOperand)
                    .Select(c => c.card)
                    .ToList();
            case NumberOperator.LessThanOrEqual:
                return cardsQuery
                    .Where(c => c.numericalValue <= numberFilter.RightOperand)
                    .Select(c => c.card)
                    .ToList();
            case NumberOperator.GreaterThan:
                return cardsQuery
                    .Where(c => c.numericalValue > numberFilter.RightOperand)
                    .Select(c => c.card)
                    .ToList();
            case NumberOperator.GreaterThanOrEqual:
                return cardsQuery
                    .Where(c => c.numericalValue >= numberFilter.RightOperand)
                    .Select(c => c.card)
                    .ToList();
            default:
                throw new Exception("Invalid NumberOperator passed.");
        }
    }

    private static bool WholeWordFunction(ReadOnlySpan<char> textToSearch, ReadOnlySpan<char> query, bool exactCase)
    {
        if (query.Length == 0 || query.Length > textToSearch.Length)
        {
            return false;
        }

        int start = 0;
        EqualityComparer<char>? comparer = null;
        if (!exactCase)
        {
            comparer = new EqualsIgnoreCase();
        }

        while (start + query.Length < textToSearch.Length + 1)
        {
            ReadOnlySpan<char> slice = textToSearch.Slice(start, query.Length);
            int nextCharacterIndex = start + query.Length;
            if (slice.SequenceEqual(query, comparer) && (nextCharacterIndex == textToSearch.Length || Char.IsWhiteSpace(textToSearch[nextCharacterIndex])))
            {
                return true;
            }

            ++start;
        }

        return false;
    }

    private class EqualsIgnoreCase : EqualityComparer<char>
    {
        public override bool Equals(char x, char y)
        {
            char upperX = Char.ToUpperInvariant(x);
            char upperY = Char.ToUpperInvariant(y);

            return upperX == upperY;
        }

        public override int GetHashCode([DisallowNull] char obj)
        {
            return obj; //will this work?
        }
    }
}

