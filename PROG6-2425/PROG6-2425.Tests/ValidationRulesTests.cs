using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Moq;
using PROG6_2425.Models;
using PROG6_2425.Validators;
using PROG6_2425.ViewModels;
using Xunit;

namespace PROG6_2425.Tests;

public class ValidationRulesTests
{
    private readonly Mock<UserManager<Account>> _userManagerMock;
    private readonly BeestjeBoekingValidator _validator;

    public ValidationRulesTests()
    {
        _userManagerMock = new Mock<UserManager<Account>>(
            Mock.Of<IUserStore<Account>>(), null, null, null, null, null, null, null, null);

        _validator = new BeestjeBoekingValidator(_userManagerMock.Object);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenNoBeestjeSelected()
    {
        var model = new Mock<Step2VM>();
        model.SetupGet(m => m.GeselecteerdeBeestjesIds).Returns(() => null);

        var context = new ValidationContext(model.Object);

        var results = _validator.Validate(model.Object, context).ToList();

        Assert.Single(results);
        Assert.Equal("Selecteer ten minste één beestje.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTooManyBeestjesSelected_WithoutKlantenkaart()
    {
        var model = new Mock<Step2VM>();
        model.SetupGet(m => m.GeselecteerdeBeestjesIds).Returns(new List<int> { 1, 2, 3, 4 });

        var context = new ValidationContext(model.Object)
        {
            Items = { { "User", "testuser" } }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 1 } }
        }.AsQueryable());

        var results = _validator.Validate(model.Object, context).ToList();

        Assert.Single(results);
        Assert.Equal("Maximaal 3 beestjes toegestaan zonder klantenkaart.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenVIPDierenSelected_WithoutKlantenKaart()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1 }
        };
        var context = new ValidationContext(model)
        {
            Items =
            {
                { "User", "testuser" },
                { "Beestjes", new List<Beestje> { new Beestje { BeestjeId = 1, Type = "VIP" } } }
            }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 3 } }
        }.AsQueryable());

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Je hebt een Platina klantenkaart nodig om VIP-dieren te boeken.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTooManyBeestjesSelected_WithSilverKlantenkaart()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1, 2, 3, 4, 5 } // Meer dan toegestaan voor zilver
        };
        var context = new ValidationContext(model)
        {
            Items = { { "User", "testuser" } }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 2 } }
        }.AsQueryable());

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Maximaal 4 beestjes toegestaan met een zilveren klantenkaart.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldPass_WhenBeestjesWithinLimit_WithSilverKlantenkaart()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1, 2, 3, 4 } // Precies het maximum voor zilver
        };
        var context = new ValidationContext(model)
        {
            Items = { { "User", "testuser" } }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 2 } }
        }.AsQueryable());

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Empty(results); // Geen foutmeldingen
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenVIPDierenSelected_WithSilverKlantenKaart()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1 }
        };
        var context = new ValidationContext(model)
        {
            Items =
            {
                { "User", "testuser" },
                { "Beestjes", new List<Beestje> { new Beestje { BeestjeId = 1, Type = "VIP" } } }
            }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 2 } }
        }.AsQueryable());

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Je hebt een Platina klantenkaart nodig om VIP-dieren te boeken.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldPass_WhenBeestjesWithinLimit_WithGoldKlantenkaart()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1, 2, 3, 4, 5, 6 }
        };
        var context = new ValidationContext(model)
        {
            Items = { { "User", "testuser" } }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 3 } }
        }.AsQueryable());

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenVIPDierenSelected_WithGoldKlantenKaart()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1 }
        };
        var context = new ValidationContext(model)
        {
            Items =
            {
                { "User", "testuser" },
                { "Beestjes", new List<Beestje> { new Beestje { BeestjeId = 1, Type = "VIP" } } }
            }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 3 } }
        }.AsQueryable());

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Je hebt een Platina klantenkaart nodig om VIP-dieren te boeken.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldPass_WhenBeestjesWithinLimit_WithPlatinumKlantenkaart()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1, 2, 3, 4, 5, 6 }
        };
        var context = new ValidationContext(model)
        {
            Items = { { "User", "testuser" } }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 4 } }
        }.AsQueryable());

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Empty(results); // Geen foutmeldingen
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenVIPDierenSelected_WithoutPlatinaKaart()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1 }
        };
        var context = new ValidationContext(model)
        {
            Items =
            {
                { "User", "testuser" },
                { "Beestjes", new List<Beestje> { new Beestje { BeestjeId = 1, Type = "VIP" } } }
            }
        };

        _userManagerMock.Setup(um => um.Users).Returns(new List<Account>
        {
            new Account { UserName = "testuser", KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 2 } }
        }.AsQueryable());

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Je hebt een Platina klantenkaart nodig om VIP-dieren te boeken.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPinguinSelectedOnWeekend()
    {
        // Arrange
        var model = new Step2VM
        {
            GeselecteerdeBeestjesIds = new List<int> { 1 }
        };
        var context = new ValidationContext(model)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 25) }, // Zaterdag
                { "Beestjes", new List<Beestje> { new Beestje { BeestjeId = 1, Naam = "Pinguïn" } } }
            }
        };

        // Act
        var results = _validator.Validate(model, context).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Dieren in pak werken alleen doordeweeks. Kies een andere datum.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPinguinSelectedOnWeekday()
    {
        var model = new Mock<Step2VM>();
        model.SetupGet(m => m.GeselecteerdeBeestjesIds).Returns(new List<int> { 1 });

        var context = new ValidationContext(model.Object)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 22) },
                { "Beestjes", new List<Beestje> { new Beestje { BeestjeId = 1, Naam = "Pinguïn" } } }
            }
        };

        var results = _validator.Validate(model.Object, context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenDesertAnimalsSelectedInWinter()
    {
        var model = new Mock<Step2VM>();
        model.SetupGet(m => m.GeselecteerdeBeestjesIds).Returns(new List<int> { 1 });

        var context = new ValidationContext(model.Object)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 12, 1) }, // Wintermaand
                { "Beestjes", new List<Beestje> { new Beestje { BeestjeId = 1, Type = "Woestijn" } } }
            }
        };

        var results = _validator.Validate(model.Object, context).ToList();

        Assert.Single(results);
        Assert.Equal("Brrrr – Veelste koud voor woestijndieren in de winter.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldPass_WhenDesertAnimalsSelectedInSummer()
    {
        var model = new Mock<Step2VM>();
        model.SetupGet(m => m.GeselecteerdeBeestjesIds).Returns(new List<int> { 1 });

        var context = new ValidationContext(model.Object)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 7, 1) }, // Zomermaand
                { "Beestjes", new List<Beestje> { new Beestje { BeestjeId = 1, Type = "Woestijn" } } }
            }
        };

        var results = _validator.Validate(model.Object, context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void ValidateLeeuwIJsbeerAndBoerderij_ShouldReturnError_WhenLeeuwOrIJsbeerAndBoerderijdierAreSelected()
    {
        var model = new Mock<Step2VM>();

        model.SetupGet(m => m.GeselecteerdeBeestjesIds).Returns(new List<int> { 1 });

        // Mock Beestje list
        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 1, Naam = "Leeuw", Type = "Wild" },
            new Beestje { BeestjeId = 2, Naam = "Koe", Type = "Boerderij" }
        };

        var context = new ValidationContext(model);

        // Act
        var results = _validator.Validate(model.Object, context).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal(
            "Nom nom nom – Je kunt geen boerderijdieren boeken wanneer je een Leeuw of IJsbeer hebt geselecteerd.",
            results.First().ErrorMessage);
    }

    [Fact]
    public void ValidateLeeuwIJsbeerAndBoerderij_ShouldNotReturnError_WhenOnlyLeeuwOrIJsbeerAreSelected()
    {
        // Arrange
        var model = new Mock<Step2VM>();
        model.SetupGet(m => m.GeselecteerdeBeestjesIds).Returns(new List<int> { 1 });

        // Mock Beestje list
        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 1, Naam = "Leeuw", Type = "Wild" }
        };

        var context = new ValidationContext(model);

        // Act
        var results = _validator.Validate(model.Object, context).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal(
            "Nom nom nom – Je kunt geen boerderijdieren boeken wanneer je een Leeuw of IJsbeer hebt geselecteerd.",
            results.First().ErrorMessage);
    }

    [Fact]
    public void ValidateLeeuwIJsbeerAndBoerderij_ShouldPass_WhenNoLeeuwOrIJsbeerAreSelected()
    {
        // Arrange
        var model = new Mock<Step2VM>();
        model.SetupGet(m => m.GeselecteerdeBeestjesIds).Returns(new List<int> { 1 });

        // Mock Beestje list
        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 2, Naam = "Koe", Type = "Boerderij" }
        };

        var context = new ValidationContext(model);

        // Act
        var results = _validator.Validate(model.Object, context).ToList();

        // Assert
        Assert.Null(results);
    }
}