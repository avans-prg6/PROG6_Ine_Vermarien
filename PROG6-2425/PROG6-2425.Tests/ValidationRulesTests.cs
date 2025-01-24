using PROG6_2425.Models;
using Xunit;

namespace PROG6_2425.Tests;

public class ValidationRulesTests
{
    [Fact]
    public void Should_Not_Allow_Leeuw_With_Boerderijdier()
    {
        // Arrange
        var validator = new BoekingValidator();
        var beestjes = new List<Beestje>
        {
            new Beestje { Naam = "Leeuw", Type = "Wild" },
            new Beestje { Naam = "Koe", Type = "Boerderijdier" }
        };
        
        // Act
        var result = validator.ValidateBeestjesCombination(beestjes);
        
        // Assert
        Assert.False(result, "Expected Leeuw to not be allowed with Boerderijdier.");
    }
}