using System.ComponentModel.DataAnnotations;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Validators;

public class PinguinWeekendValidator : IValidator<Step2VM>
{
    public IEnumerable<ValidationResult> Validate(Step2VM model, ValidationContext context)
    {
        DateTime geboekteDatum = (DateTime)context.Items["Datum"];
        var isWeekend = geboekteDatum.DayOfWeek == DayOfWeek.Saturday || geboekteDatum.DayOfWeek == DayOfWeek.Sunday;
        var beestjes = context.Items["Beestjes"] as List<Beestje>;

        if (isWeekend && beestjes != null)
        {
            foreach (var beestjeId in model.GeselecteerdeBeestjesIds)
            {
                var beestje = beestjes.FirstOrDefault(b => b.BeestjeId == beestjeId);
                if (beestje != null && beestje.Naam.Equals("Pinguïn", StringComparison.OrdinalIgnoreCase))
                {
                    yield return new ValidationResult("Dieren in pak werken alleen doordeweeks. Kies een andere datum.",
                        new[] { nameof(model.GeselecteerdeBeestjesIds) });
                }
            }
        }
    }
}