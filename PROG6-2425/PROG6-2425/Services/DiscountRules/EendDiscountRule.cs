using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services.DiscountRules;

public class EendDiscountRule: IDiscountRule
{
    private readonly Random _random = new Random();

    public decimal CalculateDiscount(BoekingVM boeking, decimal currentDiscountPercentage)
    {
        if (boeking.GekozenBeestjes.Any(b => b.Naam.Equals("Eend", StringComparison.OrdinalIgnoreCase)))
        {
            var chance = _random.Next(1, 7); // Willekeurige waarde tussen 1 en 6
            if (chance == 1)
            {
                return currentDiscountPercentage + 50m;
            }
        }
        return currentDiscountPercentage;
    }
}