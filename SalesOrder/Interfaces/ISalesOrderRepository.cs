using SalesOrder.Models;

namespace SalesOrder.Interfaces
{
    public interface ISalesOrderRepository
    {
        IEnumerable<CustomerSalesOrder> GetAll();
        IEnumerable<Customer> GetAllCustomers();

        void SaveOrder(Models.SalesOrderProperty request);
        public SalesOrderProperty GetOrderById(int id);

        public void UpsertSalesOrder(SalesOrderProperty request);
        public void DeleteOrderById(int id);
        public void DeleteItemsById(List<int> itemIds);
    }
}