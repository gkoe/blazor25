namespace Core.DataTransferObjects
{
    public record OrdersApiGetDto(
    int Id,
    string OrderNr,
    string CustomerFullName,
    double TotalAmount
    );

}
