using System.ComponentModel.DataAnnotations;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Validators;

public class WoestijnBeestjesValidator: IValidator<Step2VM>
{
    public IEnumerable<ValidationResult> Validate(Step2VM model, ValidationContext context)
    {
        DateTime geboekteDatum = (DateTime)context.Items["Datum"];
        var maand = geboekteDatum.Month;
        var dag = geboekteDatum.Day;
        
        var isWinter = (maand == 12 && dag >= 1) || (maand == 1) || (maand == 2 && dag <= 28);

        var beestjes = context.Items["Beestjes"] as List<Beestje>;

        if (isWinter && beestjes != null)
        {
            foreach (var beestjeId in model.GeselecteerdeBeestjesIds)
            {
                var beestje = beestjes.FirstOrDefault(b => b.BeestjeId == beestjeId);
                if (beestje != null && beestje.Type.Equals("Woestijn", StringComparison.OrdinalIgnoreCase))
                {
                    yield return new ValidationResult("Brrrr – Veelste koud voor woestijndieren in de winter.",
                        new[] { nameof(model.GeselecteerdeBeestjesIds) });
                }
            }
        }
    }
}