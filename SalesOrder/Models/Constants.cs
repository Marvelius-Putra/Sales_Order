namespace SalesOrder.Models
{
    public class Constants
    {
        public class ProcedureName
        {
            public const string GetCustomers = "dbo.GET_CUSTOMERS";
            public const string GetCustomerSalesOrders = "dbo.GET_CUSTOMER_SALES_ORDERS";

            public const string InsertSalesOrder = "dbo.INSERT_SALES_ORDER";
            public const string InsertSalesOrderItem = "dbo.INSERT_SALES_ORDER_ITEM";
            public const string GetSalesOrderByID = "dbo.GET_SALES_ORDER_BY_ID";

            public const string UpdateSalesOrder = "dbo.UPDATE_SALES_ORDER";
            public const string UpsertSalesOrderItem = "dbo.UPSERT_SALES_ORDER_ITEM";

            public const string DeleteOrderById = "dbo.DELETE_ORDER_BY_ID";
            public const string DeleteItemsById = "dbo.DELETE_ORDER_ITEM_BY_ID";
        }
    }
}
