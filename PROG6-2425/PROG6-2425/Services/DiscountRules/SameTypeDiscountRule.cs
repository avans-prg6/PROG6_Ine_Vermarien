using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services.DiscountRules;

public class SameTypeDiscount : IDiscountRule
{
    public decimal CalculateDiscount(BoekingVM boeking, decimal currentDiscountPercentage)
    {
        var korting = boeking.GekozenBeestjes
            .GroupBy(b => b.Type)
            .Where(g => g.Count() >= 3)
            .Sum(_ => 10); // 10% korting voor elk type met 3+ dieren

        return korting;
    }
}