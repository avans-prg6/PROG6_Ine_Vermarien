using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Validators;

public class MaxBeestjesValidator : IValidator<Step2VM>
{
    public IEnumerable<ValidationResult> Validate(Step2VM model, ValidationContext context)
    {
        var gebruiker = context.Items["User"] as Account;
        var klantenkaartType = gebruiker?.KlantenKaart?.KlantenKaartTypeId ?? 1;

        if (model.GeselecteerdeBeestjesIds == null || model.GeselecteerdeBeestjesIds.IsNullOrEmpty())
        {
            yield return new ValidationResult("Selecteer ten minste één beestje",
                new[] { nameof(model.GeselecteerdeBeestjesIds) });
        }
        if (klantenkaartType == 1 && model.GeselecteerdeBeestjesIds.Count > 3)
        {
            yield return new ValidationResult("Maximaal 3 beestjes toegestaan zonder klantenkaart.",
                new[] { nameof(model.GeselecteerdeBeestjesIds) });
        }

        if (klantenkaartType == 2 && model.GeselecteerdeBeestjesIds.Count > 4)
        {
            yield return new ValidationResult("Maximaal 4 beestjes toegestaan met een zilveren klantenkaart.",
                new[] { nameof(model.GeselecteerdeBeestjesIds) });
        }
    }
}