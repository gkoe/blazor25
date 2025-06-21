
using Base.Contracts.Persistence;
using System.ComponentModel.DataAnnotations;

namespace Base.Contracts.Validation
{
    public interface IDatabaseValidatableObject
    {
        public Task<ValidationResult> ValidateAsync(IBaseUnitOfWork unitOfWork);

    }
}
