using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Services.Contracts;
using Core.DataTransferObjects;

namespace WebAssembly.Client.Pages;
public partial class OrdersPage
{
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IOrdersApiService OrdersApiService { get; set; } = null!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    private MudDataGrid<OrdersApiGetDto>? _dataGrid = null;
    public string NameFilter { get; set; } = string.Empty;

    private async Task<GridData<OrdersApiGetDto>> ServerReload(GridState<OrdersApiGetDto> state)
    {
        OrdersApiGetDto[] data = await OrdersApiService.GetPageAsync(NameFilter, state.Page * state.PageSize, state.PageSize);
        var totalItems = await OrdersApiService.GetCountAsync(NameFilter);
        return new GridData<OrdersApiGetDto>
        {
            TotalItems = totalItems,
            Items = data
        };
    }

    async Task DeleteAsync(int id)
    {
        await OrdersApiService.DeleteAsync(id);
        _dataGrid?.ReloadServerData();
    }

    void AddCustomer()
    {
        NavigationManager.NavigateTo("/addcustomerpage", true);
    }

    void SearchOrders()
    {
        _dataGrid?.ReloadServerData();
    }



}
