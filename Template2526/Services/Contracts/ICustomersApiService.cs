using Base.Entities;
using Core.Entities;
using Core.Validations;
using System.ComponentModel.DataAnnotations;

namespace Services.Contracts
{
    public class CustomerApiPostDto 
    {
        [CustomerNrCheckSum]
        [Required]
        public string CustomerNr { get; set; } = string.Empty;
        [Required]

        [NamesLengthValidation]
        [MaxLength(20)]
        public string LastName { get; set; } = string.Empty;

        [NamesLengthValidation]
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; } = string.Empty;
    }

    public record CustomerApiGetDto(int Id, string CustomerNr, string FirstName,string LastName);

    public interface ICustomersApiService
    {
        Task<CustomerApiGetDto> AddAsync(CustomerApiPostDto customerPostDto);
        Task<bool> IsFullNameUniqueAsync(string firstName, string lastName);
    }
}
