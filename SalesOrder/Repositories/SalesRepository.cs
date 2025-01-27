using Dapper;
using Microsoft.Extensions.Options;
using SalesOrder.Controllers;
using SalesOrder.Extensions;
using SalesOrder.Interfaces;
using SalesOrder.Models;
using System.Data;
namespace SalesOrder.Repositories
{
    public class SalesOrderRepository : BaseRepository, ISalesOrderRepository
    {
        private readonly ILogger<SalesOrderRepository> _logger;
        public SalesOrderRepository(IConfiguration configuration, IOptions<AppSettings> appSettings, ILogger<SalesOrderRepository> logger) : base(configuration, appSettings)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets all Customer Sales Data.
        /// </summary>
        public IEnumerable<CustomerSalesOrder> GetAll()
        {
            _logger.LogStart<SalesOrderRepository>();
            using var connection = CreateConnection();
            string storedProcedure = Constants.ProcedureName.GetCustomerSalesOrders;

            _logger.LogSuccess().LogFinish<SalesOrderRepository>();
            return connection.Query<CustomerSalesOrder>(
                storedProcedure,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        /// <summary>
        /// Gets all Customer data.
        /// </summary>
        public IEnumerable<Customer> GetAllCustomers()
        {
            _logger.LogStart<SalesOrderRepository>();
            using var connection = CreateConnection();
            string storedProcedure = Constants.ProcedureName.GetCustomers;

            _logger.LogSuccess().LogFinish<SalesOrderRepository>();
            return connection.Query<Customer>(
                storedProcedure,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        /// <summary>
        /// Save Sales Order Data.
        /// </summary>
        public void SaveOrder(Models.SalesOrderProperty request)
        {
            _logger.LogStart<SalesOrderRepository>();
            using var connection = CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            using var transaction = connection.BeginTransaction();
            try
            {
                var orderId = connection.ExecuteScalar<int>(
                    Constants.ProcedureName.InsertSalesOrder,
                    new
                    {
                        OrderNo = request.OrderNumber,
                        request.OrderDate,
                        request.CustomerId,
                        request.Address
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                if (orderId <= 0)
                {
                    throw new Exception("Failed to insert sales order.");
                }

                foreach (var item in request.Items)
                {
                    if (string.IsNullOrWhiteSpace(item.ItemName) || item.Quantity <= 0 || item.Price <= 0)
                    {
                        throw new Exception("Invalid item data.");
                    }

                    connection.Execute(
                        Constants.ProcedureName.InsertSalesOrderItem,
                        new
                        {
                            OrderId = orderId,
                            item.ItemName,
                            item.Quantity,
                            item.Price
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure
                    );
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogFailed(ex);
                transaction.Rollback();
                throw new Exception($"Error saving order: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all sales order data by ID.
        /// </summary>
        public SalesOrderProperty GetOrderById(int id)
        {
            _logger.LogStart<SalesOrderRepository>();
            using var connection = CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var multi = connection.QueryMultiple(
                Constants.ProcedureName.GetSalesOrderByID,
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            var salesOrder = multi.ReadFirstOrDefault<SalesOrderProperty>();
            if (salesOrder != null)
            {
                salesOrder.Items = multi.Read<SalesOrderItem>().ToList();
            }
            _logger.LogSuccess().LogFinish<SalesOrderRepository>();
            return salesOrder;
        }

        /// <summary>
        /// Update and Insert sales order data.
        /// </summary>
        public void UpsertSalesOrder(SalesOrderProperty request)
        {
            _logger.LogStart<SalesOrderRepository>();
            using var connection = CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var transaction = connection.BeginTransaction();
            try
            {
                // Upsert Sales Order
                connection.ExecuteScalar<int>(
                    Constants.ProcedureName.UpdateSalesOrder, 
                    new
                    {
                        Order_Id = request.Id, 
                        OrderNo = request.OrderNumber,
                        request.OrderDate,
                        request.CustomerId,
                        request.Address
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );                

                // Upsert Sales Order Items
                foreach (var item in request.Items)
                {
                    if (string.IsNullOrWhiteSpace(item.ItemName))
                    {
                        throw new Exception("Item name cannot be null or empty.");
                    }

                    if (item.Quantity <= 0 || item.Price <= 0)
                    {
                        throw new Exception($"Invalid item data for {item.ItemName}. Quantity and Price must be greater than 0.");
                    }

                    connection.Execute(
                        Constants.ProcedureName.UpsertSalesOrderItem, 
                        new
                        {
                            ItemId = item.Id, 
                            OrderId = request.Id, 
                            item.ItemName,
                            item.Quantity,
                            item.Price
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure
                    );
                }
                _logger.LogSuccess().LogFinish<SalesOrderRepository>();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogFailed(ex);
                transaction.Rollback();
                throw new Exception($"Error upserting sales order: {ex.Message}");
            }
        }

        /// <summary>
        ///Delete sales order data.
        /// </summary>
        public void DeleteOrderById(int id)
        {
            _logger.LogStart<SalesOrderRepository>();
            using var connection = CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            using var transaction = connection.BeginTransaction();
            try
            {
                var rowsAffected = connection.Execute(
                    Constants.ProcedureName.DeleteOrderById, 
                    new { SO_ORDER_ID = id },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                if (rowsAffected <= 0)
                {
                    throw new Exception("Failed to delete sales order or sales order not found.");
                }
                _logger.LogSuccess().LogFinish<SalesOrderRepository>();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogFailed(ex);
                transaction.Rollback();
                throw new Exception($"Error deleting sales order: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all sales order items data.
        /// </summary>
        public void DeleteItemsById(List<int> itemIds)
        {
            _logger.LogStart<SalesOrderRepository>();
            using var connection = CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var transaction = connection.BeginTransaction();
            try
            {
                var idsString = string.Join(",", itemIds); 

                var rowsAffected = connection.Execute(
                    Constants.ProcedureName.DeleteItemsById, 
                    new { SO_ITEM_ID = idsString }, 
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                if (rowsAffected <= 0)
                {
                    throw new Exception("Failed to delete items or items not found.");
                }
                _logger.LogSuccess().LogFinish<SalesOrderRepository>();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogFailed(ex);
                transaction.Rollback();
                throw new Exception($"Error deleting items: {ex.Message}");
            }
        }
    }
}
