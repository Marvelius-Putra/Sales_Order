namespace SalesOrder.Models
{
    public class SalesOrderProperty
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string Address { get; set; }
        public List<SalesOrderItem> Items { get; set; }
    }
    public class SalesOrderItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}