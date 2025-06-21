using Base.Contracts.Persistence;
using Base.Contracts.Validation;
using Base.Entities;
using Core.Contracts.Persistence;
using Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    public class Customer : EntityObject, IDatabaseValidatableObject
    {
        [CustomerNrCheckSum]
        [Required]
        public string CustomerNr { get; set; } = string.Empty;

        [NamesLengthValidation]
        [MaxLength(20)]
        public string LastName { get; set; } = string.Empty;

        [NamesLengthValidation]
        [MaxLength(20)]
        public string FirstName { get; set; } = string.Empty ;

        public async Task<ValidationResult> ValidateAsync(IBaseUnitOfWork unitOfWork)
        {
            IUnitOfWork uow = (IUnitOfWork)unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            if (!await uow.Customers.IsFullNameUniqueAsync(FirstName, LastName))
            {
                return new ValidationResult("Customer Name ist nicht einzigartig", [nameof(FirstName), nameof(LastName)]);
            }

            return ValidationResult.Success!;
        }

    }


}
