using System.ComponentModel.DataAnnotations;

namespace PROG6_2425.Validators;

public interface IValidator<T>
{
    IEnumerable<ValidationResult> Validate(T instance, ValidationContext context);
}