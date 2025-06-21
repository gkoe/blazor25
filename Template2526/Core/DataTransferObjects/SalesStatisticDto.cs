namespace Core.DataTransferObjects
{
    public class SalesStatisticDto
    {
        public double TotalSales { get; set; }
        public string BestProduct { get; set; } = string.Empty;
        public double BestProductSales { get; set; }
        public List<CustomerTotalOrderDto> CustomerTotalOrders { get; set; } = [];
    }
}
