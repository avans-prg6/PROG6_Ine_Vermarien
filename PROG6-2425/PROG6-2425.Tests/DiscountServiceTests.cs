namespace PROG6_2425.Tests;

using Moq;
using Xunit;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;
using PROG6_2425.Services;
using System;
using System.Collections.Generic;

public class DiscountServiceTests
{
    [Fact]
    public void CalculateTypeDiscount_ShouldApply10Percent_WhenThreeSameTypeBeestjes()
    {
        // Arrange
        Mock<BoekingVM> boeking = new Mock<BoekingVM>();
        DiscountService discountService = new();

        boeking.Object.GekozenBeestjes = new List<Beestje>
        {
            new Beestje { Type = "Hond" },
            new Beestje { Type = "Hond" },
            new Beestje { Type = "Hond" }
        };

        // Act
        decimal discount = discountService.CalculateTypeDiscount(boeking.Object);

        // Assert
        Assert.Equal(10, discount);
    }

    [Theory]
    [InlineData(DayOfWeek.Monday, 15)]
    [InlineData(DayOfWeek.Tuesday, 15)]
    [InlineData(DayOfWeek.Wednesday, 0)]
    public void CalculateWeekdayDiscount_ShouldApply15Percent_OnMondayOrTuesday(DayOfWeek day, decimal expectedDiscount)
    {
        //TODO: aanpassen, test of kortingsregel werkt niet
        Mock<BoekingVM> boeking = new Mock<BoekingVM>();
        DiscountService discountService = new();
        // Arrange

        boeking.Object.Datum = new DateTime(2025, 1, 1).AddDays((int)day - 3);
        Console.WriteLine(boeking.Object.Datum);
        // Act
        decimal discount = discountService.CalculateWeekdayDiscount(boeking.Object);
        
        Assert.Equal(expectedDiscount, discount);
    }

    [Fact]
    public void CalculateLetterDiscount_ShouldApply2PercentPerSequentialLetter()
    {
        // Arrange
        Mock<BoekingVM> boeking = new Mock<BoekingVM>();
        DiscountService discountService = new();

        boeking.Object.GekozenBeestjes = new List<Beestje>
        {
            new Beestje { Naam = "ABCD" }, // 4 sequential letters
            new Beestje { Naam = "AXYZ" } // Only 1 sequential letter
        };
        
        // Act
        decimal discount = discountService.CalculateLetterDiscount(boeking.Object);

        // Assert
        Assert.Equal(10, discount); // (4 + 1) * 2 = 10
    }

    [Fact]
    public void CalculateKlantenKaartDiscount_ShouldApply10Percent_WhenDiscountServicenAccountHasKlantenKaart()
    {
        // Arrange

        Mock<BoekingVM> boeking = new Mock<BoekingVM>();
        DiscountService discountService = new();
        Mock<Account> gebruikerMock = new Mock<Account>();
        gebruikerMock.Object.KlantenKaart = new KlantenKaart()
            {KlantenKaartTypeId = 1};
        
        boeking.Object.GekozenBeestjes = new List<Beestje>();
        
        // Act
        decimal discount = discountService.CalculateKlantenKaartDiscount(gebruikerMock.Object);

        // Assert
        Assert.Equal(10, discount);
    }

    [Fact]
    public void CalculateTotalDiscount_ShouldNotExceed60Percent()
    {
        // Arrange
        Mock<BoekingVM> boeking = new Mock<BoekingVM>();
        DiscountService discountService = new();
        Mock<Account> gebruikerMock = new Mock<Account>();
        gebruikerMock.Object.KlantenKaart = new KlantenKaart()
            {KlantenKaartTypeId = 4};
        boeking.Object.GekozenBeestjes = new List<Beestje>
        {
            new Beestje { Naam= "Hond", Type = "Hond" },
            new Beestje { Naam= "Hond", Type = "Hond" },
            new Beestje { Naam= "Hond", Type = "Hond" },
            new Beestje { Naam = "Eend" },
            new Beestje { Naam = "ABCDEFGHIJKLMNOP" } 
        };
        boeking.Object.Datum = new DateTime(2025, 1, 20); // Monday

        // Act
        decimal discount = discountService.CalculateTotalDiscount(boeking.Object, gebruikerMock.Object);

        // Assert
        Assert.True(discount <= 60);
        Assert.Equal(60, discount);
    }
}