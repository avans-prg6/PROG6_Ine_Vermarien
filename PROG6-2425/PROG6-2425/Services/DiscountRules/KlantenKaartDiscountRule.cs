using Microsoft.AspNetCore.Identity;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services.DiscountRules;

public class KlantenKaartDiscountRule: IDiscountRule
{
    private readonly UserManager<Account> _userManager;
    public KlantenKaartDiscountRule(UserManager<Account> userManager)
    {
        _userManager = userManager;
    }
    public decimal CalculateDiscount(BoekingVM boeking, decimal currentDiscountPercentage)
    {
        Account gebruiker = _userManager.FindByIdAsync(boeking.GebruikerId).Result;
        if (gebruiker != null)
        {
            if (gebruiker.KlantenKaart != null)
            {
                return 10; // 10% korting voor klanten met een klantenkaart

            }
        }

        return 0;
    }
}
