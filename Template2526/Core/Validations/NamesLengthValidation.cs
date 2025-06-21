using System.ComponentModel.DataAnnotations;

namespace Core.Validations
{
    /// <summary>
    /// Objektvalidiation für die Properties FirstName und LastName
    /// Die Gesamtlänge der beiden Properties muss mindestens 5 Zeichen betragen.
    /// </summary>
    public class NamesLengthValidation: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ArgumentNullException.ThrowIfNull(validationContext);
            var instance = validationContext.ObjectInstance ?? throw new Exception(ErrorMessage ?? "Object is null!");
            var firstNameProperty = instance.GetType().GetProperty("FirstName");
            var lastNameProperty = instance.GetType().GetProperty("LastName");
            if (firstNameProperty == null || lastNameProperty == null)
            {
                return new ValidationResult("Required properties 'FirstName' and 'LastName' not found in the object.");
            }
            // Werte aus den Properties auslesen
            var firstName = (firstNameProperty.GetValue(instance) as string) ?? "";
            var lastName = (lastNameProperty.GetValue(instance) as string) ?? "";
            if (firstName.Length + lastName.Length < 5)
            {
                return new ValidationResult("FirstName und LastName müssen gemeinsam mindestens 5 Zeichen lang sein", ["FirstName", "LastName"]);
            }

            return ValidationResult.Success;
        }
    }
}
