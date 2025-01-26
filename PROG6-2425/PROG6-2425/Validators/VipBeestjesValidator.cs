using System.ComponentModel.DataAnnotations;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Validators;

public class VipBeestjesValidator: IValidator<Step2VM>
{
    public IEnumerable<ValidationResult> Validate(Step2VM model, ValidationContext context)
    {
        var gebruiker = context.Items["User"] as Account;
        var klantenkaartType = gebruiker?.KlantenKaart?.KlantenKaartTypeId ?? 1;
        var beestjes = context.Items["Beestjes"] as List<Beestje>;

        if (beestjes != null)
        {
            var geselecteerdeBeestjes = 
                beestjes.Where(b => model.GeselecteerdeBeestjesIds.Contains(b.BeestjeId)).ToList();

            if (geselecteerdeBeestjes.Any(b => b.Type == "VIP") && klantenkaartType != 4)
            {
                yield return new ValidationResult("Je hebt een Platina klantenkaart nodig om VIP-dieren te boeken.",
                    new[] { nameof(model.GeselecteerdeBeestjesIds) });
            }
        }
    }
}