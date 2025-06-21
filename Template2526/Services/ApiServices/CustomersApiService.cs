using Core.DataTransferObjects;
using Newtonsoft.Json;
using Services.Contracts;
using System.Net.Http.Json;
using System.Net;
using System.Text;

namespace Srevices.ApiServices
{
    public class CustomersApiService(HttpClient httpClient) : ICustomersApiService
    {
        private readonly HttpClient _httpClient = httpClient;
        const string _baseUrl = "api/customers";

        public async Task<CustomerApiGetDto> AddAsync(CustomerApiPostDto customerPostDto)
        {
            var content = JsonConvert.SerializeObject(customerPostDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/add", bodyContent);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CustomerApiGetDto>(contentTemp) ??
                        new CustomerApiGetDto(0, "", "", "");
            return result;
        }


        /// <summary>
        /// Check if the full name (first name and last name) is unique in the database.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public async Task<bool> IsFullNameUniqueAsync(string firstName, string lastName)
        {
            bool isUnique = await _httpClient.GetFromJsonAsync<bool>($"{_baseUrl}/isfullnameunique?firstname={firstName}&lastname={lastName}");
            return isUnique;
        }
    }
}
