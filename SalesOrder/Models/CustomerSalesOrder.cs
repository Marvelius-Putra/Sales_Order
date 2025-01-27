namespace SalesOrder.Models
{
    public class CustomerSalesOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Customer { get; set; }
    }

}
