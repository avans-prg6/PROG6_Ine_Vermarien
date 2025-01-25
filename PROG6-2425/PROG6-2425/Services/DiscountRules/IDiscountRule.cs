using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services.DiscountRules;

public interface IDiscountRule
{
    decimal CalculateDiscount(BoekingVM boeking, decimal currentDiscountPercentage);
}