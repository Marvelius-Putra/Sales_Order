namespace SalesOrder.Models
{
    public class BaseViewModel
    {
        public List<Customer> Customers { get; set; }
        public SalesOrderProperty SalesOrder { get; set; }
        public string SelectedCustomerName { get; set; } // Tambahkan properti ini
    }
}
