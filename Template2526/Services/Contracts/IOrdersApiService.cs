using Core.DataTransferObjects;

namespace Services.Contracts
{
    public interface IOrdersApiService
    {
        Task<OrdersApiGetDto[]> GetPageAsync(string filter, int skip, int take);
        Task<int> GetCountAsync(string filter);

        Task DeleteAsync(int id);
    }
}
