using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using NUnit.Framework;
using YugiohCardRepository;
using YugiohCardRepository.CardFilters;
using YugiohCardRepository.DTOs;

namespace YugiohCardRepositoryTests;

[TestFixture]
public class YugiohCardRepositoryTests
{
    const string JSON_FILE_PATH = "/Users/ismaelalmaguer/yugioh-cards-2022-03-07.json";
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private CardRepository _repository;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    //reused test variables
    const string DARK_MAGICIAN_DESC_SUBSTRING = "in terms of attack and defense";
    const int CASTLE_OF_DARK_ILLUSIONS_ATK = 920;
    const int HIGHEST_ATK = 5000;
    const int JSON_LENGTH = 11784;

    [SetUp]
    public void Setup()
    {
        _repository = new(JSON_FILE_PATH);
    }

    [Test]
    public void GetAllCards_ReturnsAllCards()
    {
        List<Card> result = _repository.GetAllCards();

        //number will change if u use a different dated json file
        Assert.AreEqual(JSON_LENGTH, result.Count);
    }

    [Test]
    public void GetCards_NoWholeWordNoCaseMatch_ReturnsDarkMagician()
    {
        List<Card> result = _repository.GetCards(new TextFilter(DARK_MAGICIAN_DESC_SUBSTRING) { ExactCase = false, WholeWord = false }, "Description");

        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Dark Magician", result[0].Name);
        });
    }

    [Test]
    public void GetCards_NoWholeWordCaseMatch_ReturnsDarkMagician()
    {
        List<Card> result = _repository.GetCards(new TextFilter(DARK_MAGICIAN_DESC_SUBSTRING) { ExactCase = true, WholeWord = false }, "Description");

        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Dark Magician", result[0].Name);
        });
    }

    [Test]
    public void GetCards_WholeWordNoCaseMatch_ReturnsNothing()
    {
        List<Card> result = _repository.GetCards(new TextFilter(DARK_MAGICIAN_DESC_SUBSTRING) { ExactCase = false, WholeWord = true }, "Description");

        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void GetCards_WholeWordCaseMatch_ReturnsNothing()
    {
        List<Card> result = _repository.GetCards(new TextFilter(DARK_MAGICIAN_DESC_SUBSTRING) { ExactCase = true, WholeWord = true }, "Description");

        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void GetCards_ExactNumberATK_ReturnsCastleOfDarkIllusions()
    {
        List<Card> result = _repository.GetCards(new NumberFilter(NumberOperator.Equal, CASTLE_OF_DARK_ILLUSIONS_ATK), "AttackPoints");

        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Castle of Dark Illusions", result[0].Name);
        });
    }
    
    [Test]
    public void GetCards_GreaterThan5000ATK_ReturnsNothing()
    {
        List<Card> result = _repository.GetCards(new NumberFilter(NumberOperator.GreaterThan, HIGHEST_ATK), "AttackPoints");

        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void GetCards_GreaterThanOrEqualTo5000ATK_ReturnsNineCards()
    {
        //https://www.db.yugioh-card.com/yugiohdb/card_search.action?ope=1&sess=1&rp=20&keyword=&stype=1&ctype=&othercon=2&starfr=&starto=&pscalefr=&pscaleto=&linkmarkerfr=&linkmarkerto=&link_m=2&atkfr=5000&atkto=&deffr=&defto=

        List<Card> result = _repository.GetCards(new NumberFilter(NumberOperator.GreaterThanOrEqual, HIGHEST_ATK), "AttackPoints");

        Assert.AreEqual(9, result.Count);
    }

    [Test]
    public void GetCards_LessThan0ATK_ReturnsNothing()
    {
        List<Card> result = _repository.GetCards(new NumberFilter(NumberOperator.LessThan, 0), "AttackPoints");

        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void GetCards_LessThanOrEqualToNegative1ATK_ReturnsNothing()
    {
        List<Card> result = _repository.GetCards(new NumberFilter(NumberOperator.LessThanOrEqual, -1), "AttackPoints");

        Assert.AreEqual(0, result.Count);
    }

    //relies on GetAllCards_ReturnsAllCards passing to be accurate
    [Test]
    public void GetCards_NotEqualToNonExistingId_ReturnsAllCards()
    {
        List<Card> result = _repository.GetCards(new NumberFilter(NumberOperator.NotEqual, -1), "Id");

        Assert.AreEqual(JSON_LENGTH, result.Count);
    }

    [Test]
    [TestCase("Name")]
    [TestCase("Description")]
    [TestCase("CardType")]
    [TestCase("Race")]
    [TestCase("Attribute")]
    public void GetAllCards_StringPropertyIsPopulatedAtLeastOnce(string propertyName)
    {
        List<Card> result = _repository.GetAllCards();

        Type myType = typeof(Card);
        PropertyInfo? propertyInfo = myType.GetProperty(propertyName);

        Assert.That(() => result.Any(c => !string.IsNullOrWhiteSpace((string?)propertyInfo.GetValue(c))));
    }

    [Test]
    [TestCase("AttackPoints")]
    [TestCase("DefensePoints")]
    [TestCase("Level")]
    [TestCase("LinkRating")]
    [TestCase("Scale")]
    public void GetAllCards_NullableIntPropertyIsPopulatedAtLeastOnce(string propertyName)
    {
        List<Card> result = _repository.GetAllCards();

        Type myType = typeof(Card);
        PropertyInfo? propertyInfo = myType.GetProperty(propertyName);

        Assert.That(() => result.Any(c => {
            int? value = (int?)propertyInfo.GetValue(c);

            return value.HasValue;
        }));
    }

    [Test]
    public void GetAllCards_IdPropertyIsPopulatedEveryTime()
    {
        List<Card> result = _repository.GetAllCards();

        Assert.That(() => result.All(c => c.Id != 0));
    }

    [Test]
    public void GetAllCards_LinkMarkersPropertyIsPopulatedAtLeastOnce()
    {
        List<Card> result = _repository.GetAllCards();

        Assert.That(() => result.Any(c => c.LinkMarkers != null && c.LinkMarkers.Any()));
    }
}
