using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PROG6_2425.Attributes;

public class NoDateInPastAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        DateTime datum = (DateTime)value;
        
        if (DateTime.Today > datum)
        {
            return new ValidationResult("Datum kan niet in het verleden liggen");

        }

        return null;

    }
}