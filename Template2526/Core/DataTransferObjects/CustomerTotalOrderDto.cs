namespace Core.DataTransferObjects
{
    public class CustomerTotalOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public int NumberOfOrders { get; set; }
        public double TotalSales { get; set; }
    }
}
