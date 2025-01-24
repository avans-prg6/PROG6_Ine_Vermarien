using System.ComponentModel.DataAnnotations;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Attributes;

public class NoDesertAnimalsInWinterValidator : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var errors = new List<string>();

        // ViewModel geldig
        if (value is BoekingVM bookingVM)
        {
            var geboekteDatum = bookingVM.Datum;

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
                if (bookingVM.GekozenBeestjes != null && bookingVM.GekozenBeestjes.Any())
                {
                    foreach (var beestje in bookingVM.GekozenBeestjes)
                    {
                        if (beestje.Type.Equals("Woestijn", StringComparison.OrdinalIgnoreCase))
                        {
                            return new ValidationResult("Brrrr – Veelste koud voor woestijndieren in de winter.");
                        }
                    }
                }
                
                if (errors.Any())
                {
                    return new ValidationResult(string.Join("\n", errors));
                }
            }
        }

        return ValidationResult.Success;
    }
}