using System.ComponentModel.DataAnnotations;
using PROG6_2425.Models;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Validators;

public class LeeuwIJsbeerBoerderijValidator: IValidator<Step2VM>
{
    public IEnumerable<ValidationResult> Validate(Step2VM model, ValidationContext context)
    {
        var beestjes = context.Items["Beestjes"] as List<Beestje>;

        bool hasLeeuwOrIJsbeer = model.GeselecteerdeBeestjesIds.Any(beestjeId =>
        {
            var beestje = beestjes?.FirstOrDefault(b => b.BeestjeId == beestjeId);
            return beestje != null && (beestje.Naam.Equals("Leeuw", StringComparison.OrdinalIgnoreCase) || beestje.Naam.Equals("IJsbeer", StringComparison.OrdinalIgnoreCase));
        });

        if (hasLeeuwOrIJsbeer)
        {
            bool hasBoerderijdier = model.GeselecteerdeBeestjesIds.Any(beestjeId =>
            {
                var beestje = beestjes?.FirstOrDefault(b => b.BeestjeId == beestjeId);
                return beestje != null && beestje.Type.Equals("Boerderij", StringComparison.OrdinalIgnoreCase);
            });

            if (hasBoerderijdier)
            {
                yield return new ValidationResult("Nom nom nom – Je kunt geen boerderijdieren boeken wanneer je een Leeuw of IJsbeer hebt geselecteerd.",
                    new[] { nameof(model.GeselecteerdeBeestjesIds) });
            }
        }
    }
}