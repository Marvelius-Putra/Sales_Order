namespace SalesOrder.Models
{
    public class Customer : BaseViewModel
    {
        public int Id { get; set; }  // Ensure this property exists
        public string Name { get; set; }
    }
}
