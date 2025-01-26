using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PROG6_2425.Models;
using PROG6_2425.Services;
using PROG6_2425.ViewModels;

namespace PROG6_2425.Validators;

public class BeestjeBoekingValidator : IValidator<Step2VM>
{
    
    private readonly List<IValidator<Step2VM>> _validators;

    public BeestjeBoekingValidator()
    {
        _validators = new List<IValidator<Step2VM>>
        {
            new MaxBeestjesValidator(),
            new VipBeestjesValidator(),
            new PinguinWeekendValidator(),
            new WoestijnBeestjesValidator(),
            new LeeuwIJsbeerBoerderijValidator()
        };
    }

    public IEnumerable<ValidationResult> Validate(Step2VM model, ValidationContext context)
    {
        foreach (var validator in _validators)
        {
            foreach (var validationResult in validator.Validate(model, context))
            {
                yield return validationResult;
            }
        }
    }
}