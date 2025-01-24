using System.ComponentModel.DataAnnotations;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Attributes;

public class NoPinguinOnWeekendsValidator : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is BoekingVM bookingVM)
        {
            var errors = new List<string>();

            var geboekteDatum = bookingVM.Datum;

            if (geboekteDatum == null)
            {
                return new ValidationResult("Kon datum niet vinden");
            }

            // Controleer of de datum in het weekend valt
            var isWeekend = geboekteDatum.DayOfWeek == DayOfWeek.Saturday || geboekteDatum.DayOfWeek == DayOfWeek.Sunday;
            if (isWeekend)
            {
                // Controleer de GekozenBeestjes lijst
                if (bookingVM.GekozenBeestjes != null && bookingVM.GekozenBeestjes.Any())
                {
                    foreach (var beestje in bookingVM.GekozenBeestjes)
                    {
                        if (beestje.Naam.Equals("Pinguïn", StringComparison.OrdinalIgnoreCase))
                        {
                            return new ValidationResult("Dieren in pak werken alleen doordeweeks. Kies een andere datum.");
                        }
                    }
                }
            }
            if (errors.Any())
            {
                return new ValidationResult(string.Join("\n", errors));
            }
        }

        // Als alles geldig is, geef succes terug
        return ValidationResult.Success;
    }
}