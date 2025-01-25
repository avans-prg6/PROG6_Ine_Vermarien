namespace PROG6_2425.Tests;

using Moq;
using Xunit;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;
using PROG6_2425.Services;
using System;
using System.Collections.Generic;
using System.Linq;

public class DiscountServiceTests
{
    private readonly DiscountService _discountService = new();

    [Fact]
    public void CalculateTypeDiscount_ShouldApply10Percent_WhenThreeSameTypeBeestjes()
    {
        // Arrange
        var boekingMock = new Mock<BoekingVM>();
        boekingMock.Setup(b => b.GekozenBeestjes).Returns(new List<Beestje>
        {
            new Beestje { Type = "Hond" },
            new Beestje { Type = "Hond" },
            new Beestje { Type = "Hond" }
        });

        // Act
        decimal discount = _discountService.CalculateTotalDiscount(boekingMock.Object, null);

        // Assert
        Assert.Contains("10% korting omdat er 3 dieren van hetzelfde type zijn.", boekingMock.Object.KortingDetails);
        Assert.Equal(10, discount);
    }

    [Fact]
    public void CalculateEendDiscount_ShouldApply50Percent_WhenEendAndChanceMatches()
    {
        // Arrange
        var boekingMock = new Mock<BoekingVM>();
        boekingMock.Setup(b => b.GekozenBeestjes).Returns(new List<Beestje>
        {
            new Beestje { Naam = "Eend" }
        });

        var randomMock = new Mock<Random>();
        randomMock.Setup(r => r.Next(1, 7)).Returns(1); // Mock the random chance to always hit

        var discountServiceWithMockedRandom = new DiscountService();

        // Act
        decimal discount = discountServiceWithMockedRandom.CalculateTotalDiscount(boekingMock.Object, null);

        // Assert
        Assert.Contains("50% korting omdat er een 'Eend' aanwezig is en de kans in jouw voordeel was.", boekingMock.Object.KortingDetails);
        Assert.Equal(50, discount);
    }

    [Theory]
    [InlineData(DayOfWeek.Monday, 15)]
    [InlineData(DayOfWeek.Tuesday, 15)]
    [InlineData(DayOfWeek.Wednesday, 0)]
    public void CalculateWeekdayDiscount_ShouldApply15Percent_OnMondayOrTuesday(DayOfWeek day, decimal expectedDiscount)
    {
        // Arrange
        var boekingMock = new Mock<BoekingVM>();
        boekingMock.Setup(b => b.Datum).Returns(new DateTime(2025, 1, 1).AddDays((int)day - 1));

        // Act
        decimal discount = _discountService.CalculateTotalDiscount(boekingMock.Object, null);

        // Assert
        if (expectedDiscount > 0)
        {
            Assert.Contains("15% voor een boeking op maandag of dinsdag", boekingMock.Object.KortingDetails);
        }
        Assert.Equal(expectedDiscount, discount);
    }

    [Fact]
    public void CalculateLetterDiscount_ShouldApply2PercentPerSequentialLetter()
    {
        // Arrange
        var boekingMock = new Mock<BoekingVM>();
        boekingMock.Setup(b => b.GekozenBeestjes).Returns(new List<Beestje>
        {
            new Beestje { Naam = "ABCD" }, // 4 sequential letters
            new Beestje { Naam = "AXYZ" }  // Only 1 sequential letter
        });

        // Act
        decimal discount = _discountService.CalculateTotalDiscount(boekingMock.Object, null);

        // Assert
        Assert.Contains("2% per opvolgende letter", boekingMock.Object.KortingDetails);
        Assert.Equal(10, discount); // (4 + 1) * 2 = 10
    }

    [Fact]
    public void CalculateKlantenKaartDiscount_ShouldApply10Percent_WhenAccountHasKlantenKaart()
    {
        // Arrange
        var gebruikerMock = new Mock<Account>();
        gebruikerMock.Setup(g => g.KlantenKaart).Returns(new KlantenKaart());

        var boekingMock = new Mock<BoekingVM>();
        boekingMock.Setup(b => b.GekozenBeestjes).Returns(new List<Beestje>());

        // Act
        decimal discount = _discountService.CalculateTotalDiscount(boekingMock.Object, gebruikerMock.Object);

        // Assert
        Assert.Contains("10% omdat je een klantenkaart hebt", boekingMock.Object.KortingDetails);
        Assert.Equal(10, discount);
    }

    [Fact]
    public void CalculateTotalDiscount_ShouldNotExceed60Percent()
    {
        // Arrange
        var boekingMock = new Mock<BoekingVM>();
        boekingMock.Setup(b => b.GekozenBeestjes).Returns(new List<Beestje>
        {
            new Beestje { Type = "Hond" },
            new Beestje { Type = "Hond" },
            new Beestje { Type = "Hond" },
            new Beestje { Naam = "Eend" }
        });
        boekingMock.Setup(b => b.Datum).Returns(new DateTime(2025, 1, 20)); // Monday

        var gebruikerMock = new Mock<Account>();
        gebruikerMock.Setup(g => g.KlantenKaart).Returns(new KlantenKaart());

        // Act
        decimal discount = _discountService.CalculateTotalDiscount(boekingMock.Object, gebruikerMock.Object);

        // Assert
        Assert.True(discount <= 60);
        Assert.Equal(60, discount);
    }
}