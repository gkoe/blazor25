using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Core.Validations
{
    /// <summary>   
    /// Property-Validation für die CustomerNr.
    /// Die Klasse validiert die CustomerNr, indem sie sicherstellt, dass sie nur Ziffern enthält 
    /// und die Quersumme durch 10 teilbar ist.
    /// </summary>
    public class CustomerNrCheckSum : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string text = value as string ?? throw new Exception(ErrorMessage ?? "Object is null!");
            if(text.Where(c => !char.IsDigit(c)).Any())
            {
                return new ValidationResult("CustomerNr enthält nicht nur Ziffern", ["CustomerNr"]);
            }
            var summe = text.Aggregate(0, (sum, c) => sum + (c - '0'));
            if(summe % 10 != 0)
            {
                return new ValidationResult("Checksumme stimmt nicht", ["CustomerNr"]);
            }
            return ValidationResult.Success;
        }
    }
}
