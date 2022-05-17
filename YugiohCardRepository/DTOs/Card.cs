using System;
using System.Reflection;
using System.Text.Json.Serialization;

namespace YugiohCardRepository.DTOs
{
	public class Card
	{
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("desc")]
        public string? Description { get; set; }
        [JsonPropertyName("type")]
        public string? CardType { get; set; }
        [JsonPropertyName("atk")]
        public int? AttackPoints { get; set; }
        [JsonPropertyName("def")]
        public int? DefensePoints { get; set; }
        [JsonPropertyName("lvl")]
        public int? Level { get; set; }
        [JsonPropertyName("race")]
        public string? Race { get; set; }
        [JsonPropertyName("attribute")]
        public string? Attribute { get; set; }
        [JsonPropertyName("linkval")]
        public int? LinkRating { get; set; }
        [JsonPropertyName("linkmarkers")]
        public string[]? LinkMarkers { get; set; }
        [JsonPropertyName("scale")]
        public int? Scale { get; set; }

        //maybe we can use a source generator if its faster instead of reflection

        //https://stackoverflow.com/a/10283288
        public T? GetPropertyValue<T>(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("propertyName cannot be empty or null.", nameof(propertyName));
            }

            Type myType = typeof(Card);
            PropertyInfo? propertyInfo = myType.GetProperty(propertyName);

            if (propertyInfo is null)
            {
                throw new Exception($"Value found for property name {propertyName} was null.");
            }

            //this will throw an exception if it cant convert
            T? value = (T?)propertyInfo.GetValue(this);

            return value;
        }
    }
}

