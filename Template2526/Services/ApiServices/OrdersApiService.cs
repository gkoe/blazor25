using Core.DataTransferObjects;
using Newtonsoft.Json;
using Services.Contracts;
using System.Net.Http.Json;
using System.Text;

namespace Services.ApiServices
{
    public class OrdersApiService(HttpClient httpClient) : IOrdersApiService
    {
        private readonly HttpClient _httpClient = httpClient;
        const string _baseUrl = "api/orders";

        /// <summary>
        /// Eine Seite von Orders von der API laden
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Seite von Orders</returns>
        /// <exception cref="Exception">Fehler werden über Exception gemeldet</exception>
        public async Task<OrdersApiGetDto[]> GetPageAsync(string filter, int skip, int take)
        {
            // für streaming responses, die nicht gesamt in den Speicher geladen werden sollen
            //var response = await _httpClient.GetFromJsonAsAsyncEnumerable<OrdersApiGetDto>($"{_baseUrl}/getpage?filter={filter}&skip={skip}&take={take}");
            var response = await _httpClient.GetAsync($"{_baseUrl}/getpage?filter={filter}&skip={skip}&take={take}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"GetPageAsync, Error {response.StatusCode}, {response.ReasonPhrase}");
            }
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OrdersApiGetDto[]>(contentTemp) ?? [];
            return result;
        }

        public async Task<int> GetCountAsync(string filter)
        {
            var count = await _httpClient.GetFromJsonAsync<int>($"{_baseUrl}/count?filter={filter}");
            return count;
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Order, DeleteAsync, Error {response.StatusCode}, {response.ReasonPhrase}");
            }
        }

    }
}
