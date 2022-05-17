using System;
using System.Text.Json.Serialization;

namespace YugiohCardRepository.DTOs
{
	public class CardListRoot
	{
        [JsonPropertyName("data")]
        public List<Card> Cards { get; set; } = new List<Card>();
    }
}

