using System.ComponentModel.DataAnnotations;

namespace PROG6_2425.Validators;

public interface IStepValidator<T>
{
    IEnumerable<ValidationResult> Validate(T model, ValidationContext context);

}