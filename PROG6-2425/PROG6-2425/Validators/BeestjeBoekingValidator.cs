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
            ? _userManager.Users.Include(u => u.KlantenKaart)
                .FirstOrDefault(u => u.UserName == context.Items["User"].ToString())
            : null;
        var klantenkaartType = gebruiker?.KlantenKaart?.KlantenKaartTypeId ?? 1;

        var beestjes = context.Items["Beestjes"] as List<Beestje>; // Deze regel moet nog correct werken

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

        // Roep de weekendvalidatie aan
        var weekendValidationResult = ValidatePinguinOnWeekend(model, context, beestjes);
        if (weekendValidationResult != null)
        {
            yield return weekendValidationResult;
        }
        var desertValidationResult = ValidateDesetAnimalsInWinter(model, context, beestjes);
        if (desertValidationResult != null)
        {
            yield return desertValidationResult;
        }

    }

    private ValidationResult ValidatePinguinOnWeekend(Step2VM model, ValidationContext context, List<Beestje> beestjes)
    {
        DateTime geboekteDatum = (DateTime)context.Items["Datum"];

        // Controleer of de datum in het weekend valt
        var isWeekend = geboekteDatum.DayOfWeek == DayOfWeek.Saturday || geboekteDatum.DayOfWeek == DayOfWeek.Sunday;
        if (isWeekend)
        {
            if (model.GeselecteerdeBeestjesIds != null && model.GeselecteerdeBeestjesIds.Any())
            {
                foreach (var beestjeId in model.GeselecteerdeBeestjesIds)
                {
                    var beestje = beestjes?.FirstOrDefault(b => b.BeestjeId == beestjeId);
                    if (beestje != null && beestje.Naam.Equals("Pinguïn", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ValidationResult("Dieren in pak werken alleen doordeweeks. Kies een andere datum.",
                            new[] { nameof(model.GeselecteerdeBeestjesIds) });
                    }
                }
            }
        }

        return null;
    }

    private ValidationResult ValidateDesetAnimalsInWinter(Step2VM model, ValidationContext context,
        List<Beestje> beestjes)
    {
        DateTime geboekteDatum = (DateTime)context.Items["Datum"];

        // Datum niet aanwezig
        if (geboekteDatum == null)
        {
            return new ValidationResult("De geboekte datum mag niet leeg zijn.");
        }

        // wintermaanden
        var maand = geboekteDatum.Month;
        var isWinter = maand == 10 || maand == 11 || maand == 12 || maand == 1 || maand == 2;

        if (isWinter)
        {
            // check per beestje
            if (model.GeselecteerdeBeestjesIds != null && model.GeselecteerdeBeestjesIds.Any())
            {
                foreach (var beestjeId in model.GeselecteerdeBeestjesIds)
                {
                    var beestje = beestjes?.FirstOrDefault(b => b.BeestjeId == beestjeId);
                    if (beestje.Type.Equals("Woestijn", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ValidationResult("Brrrr – Veelste koud voor woestijndieren in de winter.");
                    }
                }
            }
        }
        return null;
    }
}