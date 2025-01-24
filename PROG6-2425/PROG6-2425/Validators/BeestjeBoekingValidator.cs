using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Validators;

public class BeestjeBoekingValidator : IValidator<Step2VM>
{
    private readonly UserManager<Account> _userManager;

    public BeestjeBoekingValidator(UserManager<Account> userManager)
    {
        _userManager = userManager;
    }

    public IEnumerable<ValidationResult> Validate(Step2VM model, ValidationContext context)
    {
        
        // Haal de gebruiker op met de bijbehorende KlantenKaart
        var gebruiker = context.Items["User"] != null 
            ? _userManager.Users.Include(u => u.KlantenKaart).FirstOrDefault(u => u.UserName == context.Items["User"].ToString()) 
            : null;
        var klantenkaartType = gebruiker?.KlantenKaart?.KlantenKaartTypeId ?? 1;

        var beestjes = context.Items["Beestjes"] as List<Beestje>;

        // Validatie: Selecteer minimaal één beestje
        if (model.GeselecteerdeBeestjesIds == null || !model.GeselecteerdeBeestjesIds.Any())
        {
            yield return new ValidationResult("Selecteer ten minste één beestje.",
                new[] { nameof(model.GeselecteerdeBeestjesIds) });
        }

        // Validatie: Maximaal aantal beestjes
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

        // Validatie: VIP-Dieren alleen met Platina klantenkaart
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