using System;
namespace YugiohCardRepository.CardFilters
{
	public class TextFilter
	{
        public string Query { get; init; }
        public bool ExactCase { get; init; }
        public bool WholeWord { get; init; }

        public TextFilter(string query)
        {
			if (string.IsNullOrWhiteSpace(query))
				throw new ArgumentException("Argument cannot be null or empty.", nameof(query));

			Query = query;
		}
	}
}

