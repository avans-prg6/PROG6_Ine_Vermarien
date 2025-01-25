using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services.DiscountRules;

public class MaxDiscountRule: IDiscountRule
{
    public decimal CalculateDiscount(BoekingVM boeking, decimal currentDiscountPercentage)
    {
        return Math.Min(currentDiscountPercentage, 60m);
    }
}