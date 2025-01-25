using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Services.DiscountRules;

public class AlphabetDiscountRule: IDiscountRule //TODO: Losse rules verwijderen
{
    public decimal CalculateDiscount(BoekingVM boeking, decimal currentDiscountPercentage)
    {
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var gekozenNamen = string.Concat(boeking.GekozenBeestjes.Select(b => b.Naam.ToUpper()));

        // Tel unieke letters (A-Z) die in de beestjesnamen voorkomen
        var uniekeLetters = letters.Count(c => gekozenNamen.Contains(c));
        return uniekeLetters * 2; // 2% korting per unieke letter
    }
}