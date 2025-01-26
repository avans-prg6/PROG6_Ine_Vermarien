using System.ComponentModel.DataAnnotations;
using Moq;
using PROG6_2425.Models;
using PROG6_2425.Validators;
using PROG6_2425.ViewModels;
using Xunit;

namespace PROG6_2425.Tests;

public class ValidationRulesTests
{
    private BeestjeBoekingValidator _validator;

    [Fact]
    public void Validate_ShouldReturnError_WhenNoBeestjeSelected()
    {
        _validator = new BeestjeBoekingValidator();

        Mock<Step2VM> model = new Mock<Step2VM>();
        model.Object.GeselecteerdeBeestjesIds = new List<int>();
        Mock<Account> gebruikerMock = new Mock<Account>();

        gebruikerMock.Object.KlantenKaart = new KlantenKaart()
        {
            KlantenKaartTypeId = 2,
            KlantenKaartType = new KlantenKaartType()
            {
                Id = 2,
                Naam = "Zilver"
            }
        };
        model.Object.BeschikbareBeestjes = new List<Beestje>()
        {
            new Beestje { Naam = "Hond", Type = "Hond" },
            new Beestje { Naam = "Hond", Type = "Hond" },
            new Beestje { Naam = "Hond", Type = "Hond" },
        };
        model.Object.AlleBeestjes = new List<Beestje>()
        {
            new Beestje { Naam = "Hond", Type = "Hond" },
            new Beestje { Naam = "Hond", Type = "Hond" },
            new Beestje { Naam = "Hond", Type = "Hond" },
        };

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 1) },
                { "User", gebruikerMock },
                { "Beestjes", model.Object.BeschikbareBeestjes }
            }
        };
        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Single(results);
        Assert.Equal("Selecteer ten minste één beestje", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTooManyBeestjesSelected_WithoutKlantenkaart()
    {
        _validator = new BeestjeBoekingValidator();

        Mock<Step2VM> model = new Mock<Step2VM>();

        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1, 2, 3, 4 };

        Mock<Account> gebruikerMock = new Mock<Account>();
        gebruikerMock.Object.KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 1 };

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 1) },
                { "User", gebruikerMock.Object },
                { "Beestjes", model.Object.BeschikbareBeestjes }
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Single(results);
        Assert.Equal("Maximaal 3 beestjes toegestaan zonder klantenkaart.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenVIPDierenSelected_WithoutKlantenKaart()
    {
        _validator = new BeestjeBoekingValidator();

        var model = new Mock<Step2VM>();
        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        Mock<Account> gebruikerMock = new Mock<Account>();

        model.Object.BeschikbareBeestjes = new List<Beestje>()

        {
            new Beestje { BeestjeId = 1, Naam = "Hond", Type = "VIP" },
            new Beestje { BeestjeId = 2, Naam = "Hond", Type = "Hond" },
        };
        model.Object.AlleBeestjes = new List<Beestje>()
        {
            new Beestje { BeestjeId = 1, Naam = "Hond", Type = "VIP" },
            new Beestje { BeestjeId = 2, Naam = "Hond", Type = "Hond" },
        };

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 1) },
                { "User", gebruikerMock.Object },
                { "Beestjes", model.Object.BeschikbareBeestjes }
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Single(results);
        Assert.Equal("Je hebt een Platina klantenkaart nodig om VIP-dieren te boeken.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenVIPDierenSelected_WithZilverKlantenKaart()
    {
        // Arrange
        _validator = new BeestjeBoekingValidator();

        var model = new Mock<Step2VM>();
        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        Mock<Account> gebruikerMock = new Mock<Account>();
        gebruikerMock.Object.KlantenKaart = new KlantenKaart() { KlantenKaartTypeId = 2 };

        model.Object.BeschikbareBeestjes = new List<Beestje>()

        {
            new Beestje { BeestjeId = 1, Naam = "Hond", Type = "VIP" },
            new Beestje { BeestjeId = 2, Naam = "Hond", Type = "Hond" },
        };
        model.Object.AlleBeestjes = new List<Beestje>()
        {
            new Beestje { BeestjeId = 1, Naam = "Hond", Type = "VIP" },
            new Beestje { BeestjeId = 2, Naam = "Hond", Type = "Hond" },
        };

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", DateTime.Today },
                { "User", gebruikerMock.Object },
                { "Beestjes", model.Object.BeschikbareBeestjes }
            }
        };

        // Act
        var results = _validator.Validate(model.Object, validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Je hebt een Platina klantenkaart nodig om VIP-dieren te boeken.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenVIPDierenSelected_WithGoudenKlantenKaart()
    {
        // Arrange
        _validator = new BeestjeBoekingValidator();

        var model = new Mock<Step2VM>();
        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        Mock<Account> gebruikerMock = new Mock<Account>();
        gebruikerMock.Object.KlantenKaart = new KlantenKaart() { KlantenKaartTypeId = 3 };

        model.Object.BeschikbareBeestjes = new List<Beestje>()
        {
            new Beestje { BeestjeId = 1, Naam = "Hond", Type = "VIP" },
            new Beestje { BeestjeId = 2, Naam = "Hond", Type = "Hond" },
        };

        model.Object.AlleBeestjes = new List<Beestje>()
        {
            new Beestje { BeestjeId = 1, Naam = "Hond", Type = "VIP" },
            new Beestje { BeestjeId = 2, Naam = "Hond", Type = "Hond" },
        };

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 1) },
                { "User", gebruikerMock.Object },
                { "Beestjes", model.Object.BeschikbareBeestjes }
            }
        };

        // Act
        var results = _validator.Validate(model.Object, validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("Je hebt een Platina klantenkaart nodig om VIP-dieren te boeken.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_WhenVIPDierenSelected_WithPlatinaKaart()
    {
        _validator = new BeestjeBoekingValidator();

        var model = new Mock<Step2VM>();
        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        Mock<Account> gebruikerMock = new Mock<Account>();
        gebruikerMock.Object.KlantenKaart = new KlantenKaart() { KlantenKaartTypeId = 4 };

        model.Object.BeschikbareBeestjes = new List<Beestje>()

        {
            new Beestje { BeestjeId = 1, Naam = "Hond", Type = "VIP" },
            new Beestje { BeestjeId = 2, Naam = "Hond", Type = "Hond" },
        };
        model.Object.AlleBeestjes = new List<Beestje>()
        {
            new Beestje { BeestjeId = 1, Naam = "Hond", Type = "VIP" },
            new Beestje { BeestjeId = 2, Naam = "Hond", Type = "Hond" },
        };

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 1) },
                { "User", gebruikerMock.Object },
                { "Beestjes", model.Object.BeschikbareBeestjes }
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTooManyBeestjesSelected_WithSilverKlantenkaart()
    {
        _validator = new BeestjeBoekingValidator();
        var model = new Mock<Step2VM>();

        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1, 2, 3, 4, 5 };

        Mock<Account> gebruikerMock = new Mock<Account>();
        gebruikerMock.Object.Naam = "zilverUser";
        gebruikerMock.Object.KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 2 };

        var validationContext = new ValidationContext(model)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 1) },
                { "User", gebruikerMock.Object },
                { "Beestjes", model.Object.BeschikbareBeestjes },
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Single(results);
        Assert.Equal("Maximaal 4 beestjes toegestaan met een zilveren klantenkaart.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldPass_WhenBeestjesWithinLimit_WithSilverKlantenkaart()
    {
        _validator = new BeestjeBoekingValidator();
        var model = new Mock<Step2VM>();

        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1, 2, 3, 4 };

        Mock<Account> gebruikerMock = new Mock<Account>();
        gebruikerMock.Object.Naam = "zilverUser";
        gebruikerMock.Object.KlantenKaart = new KlantenKaart { KlantenKaartTypeId = 2 };

        var validationContext = new ValidationContext(model)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 1) },
                { "User", gebruikerMock.Object },
                { "Beestjes", model.Object.BeschikbareBeestjes },
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPinguinSelectedOnWeekend()
    {
        _validator = new BeestjeBoekingValidator();
        var model = new Mock<Step2VM>();

        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 1, Naam = "Pinguïn", Type = "Sneeuw" }
        };

        Mock<Account> gebruikerMock = new Mock<Account>();

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 25) }, // Zaterdag
                { "User", gebruikerMock.Object },
                { "Beestjes", beestjes }
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Single(results);
        Assert.Equal("Dieren in pak werken alleen doordeweeks. Kies een andere datum.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPinguinSelectedOnWeekday()
    {
        _validator = new BeestjeBoekingValidator();
        var model = new Mock<Step2VM>();

        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 1, Naam = "Pinguïn", Type = "Sneeuw" }
        };

        Mock<Account> gebruikerMock = new Mock<Account>();

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 22) }, // Zaterdag
                { "User", gebruikerMock.Object },
                { "Beestjes", beestjes }
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenDesertAnimalsSelectedInWinter()
    {
        _validator = new BeestjeBoekingValidator();

        var model = new Mock<Step2VM>();

        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 1, Naam = "Slang", Type = "Woestijn" }
        };

        Mock<Account> gebruikerMock = new Mock<Account>();

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 22) }, // Zaterdag
                { "User", gebruikerMock.Object },
                { "Beestjes", beestjes }
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Single(results);
        Assert.Equal("Brrrr – Veelste koud voor woestijndieren in de winter.", results.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldPass_WhenDesertAnimalsSelectedInSummer()
    {
        _validator = new BeestjeBoekingValidator();

        var model = new Mock<Step2VM>();

        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 1, Naam = "Slang", Type = "Woestijn" }
        };

        Mock<Account> gebruikerMock = new Mock<Account>();

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 7, 1) }, // Zomermaand
                { "User", gebruikerMock.Object },
                { "Beestjes", beestjes }
            }
        };

        var results = _validator.Validate(model.Object, validationContext).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void ValidateLeeuwIJsbeerAndBoerderij_ShouldReturnError_WhenLeeuwOrIJsbeerAndBoerderijdierAreSelected()
    {
        _validator = new BeestjeBoekingValidator();
        var model = new Mock<Step2VM>();
        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1, 2 };

        // Mock Beestje list
        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 1, Naam = "Leeuw", Type = "Wild" },
            new Beestje { BeestjeId = 2, Naam = "Koe", Type = "Boerderij" }
        };
        Mock<Account> gebruikerMock = new Mock<Account>();

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 22) }, // Zaterdag
                { "User", gebruikerMock.Object },
                { "Beestjes", beestjes }
            }
        };

        // Act
        var results = _validator.Validate(model.Object, validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal(
            "Nom nom nom – Je kunt geen boerderijdieren boeken wanneer je een Leeuw of IJsbeer hebt geselecteerd.",
            results.First().ErrorMessage);
    }

    [Fact]
    public void ValidateLeeuwIJsbeerAndBoerderij_ShouldPass_WhenOnlyLeeuwOrIJsbeerAreSelected()
    {
        // Arrange
        _validator = new BeestjeBoekingValidator();

        var model = new Mock<Step2VM>();
        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        // Mock Beestje list
        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 1, Naam = "Leeuw", Type = "Wild" }
        };
        Mock<Account> gebruikerMock = new Mock<Account>();

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 22) }, // Zaterdag
                { "User", gebruikerMock.Object },
                { "Beestjes", beestjes }
            }
        };

        // Act
        var results = _validator.Validate(model.Object, validationContext).ToList();

        // Assert
       Assert.Empty(results);
    }

    [Fact]
    public void ValidateLeeuwIJsbeerAndBoerderij_ShouldPass_WhenNoLeeuwOrIJsbeerAreSelected()
    {
        // Arrange
        _validator = new BeestjeBoekingValidator();

        var model = new Mock<Step2VM>();
        model.Object.GeselecteerdeBeestjesIds = new List<int> { 1 };

        // Mock Beestje list
        var beestjes = new List<Beestje>
        {
            new Beestje { BeestjeId = 2, Naam = "Koe", Type = "Boerderij" }
        };
        
        Mock<Account> gebruikerMock = new Mock<Account>();

        var validationContext = new ValidationContext(model, null, null)
        {
            Items =
            {
                { "Datum", new DateTime(2025, 1, 22) }, // Zaterdag
                { "User", gebruikerMock.Object },
                { "Beestjes", beestjes }
            }
        };
        
        // Act
        var results = _validator.Validate(model.Object, validationContext).ToList();

        // Assert
        Assert.Empty(results);
    }
}