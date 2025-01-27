using Microsoft.AspNetCore.Mvc;
using SalesOrder.Extensions;
using SalesOrder.Interfaces;
using SalesOrder.Models;
using System.Net;

namespace SalesOrder.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestOrderController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISalesOrderRepository _salesOrderRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestOrderController"/> class.
        /// </summary>
        public RequestOrderController(ISalesOrderRepository salesOrderRepository, ILogger<HomeController> logger)
        {
            _salesOrderRepository = salesOrderRepository;
            _logger = logger;
        }

        /// <summary>
        /// Save the Added Data.
        /// </summary>
        [HttpPost]
        [Route("SaveOrder")]
        public IActionResult SaveOrder([FromBody] SalesOrderProperty request)
        {
            _logger.LogStart<RequestOrderController>();
            try
            {
                _salesOrderRepository.SaveOrder(request);
                _logger.LogSuccess().LogFinish<RequestOrderController>();
                return Json(new { success = true, message = "Order berhasil disimpan." });
            }
            catch (Exception ex)
            {
                _logger.LogFailed(ex);
                return Json(new { success = false, message = $"Gagal menyimpan order: {ex.Message}" });
            }
        }

        /// <summary>
        /// Modify Sales data in Database.
        /// </summary>
        [HttpPost]
        [Route("UpdateSales")]
        public IActionResult UpdateSales([FromBody] SalesOrderProperty request)
        {
            _logger.LogStart<RequestOrderController>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.OrderNumber) || request.CustomerId <= 0 || request.OrderDate == DateTime.MinValue)
                {
                    return BadRequest(new { success = false, message = "Missing required order information." });
                }
                _salesOrderRepository.UpsertSalesOrder(request);

                _logger.LogSuccess().LogFinish<RequestOrderController>();
                return Ok(new { success = true, message = "Order successfully updated." });
            }
            catch (Exception ex)
            {
                _logger.LogFailed(ex);
                return StatusCode(500, new { success = false, message = $"Failed to update order: {ex.Message}" });
            }
        }

        /// <summary>
        /// Delete sales order data.
        /// </summary>
        [HttpDelete]
        [Route("DeleteOrder")]
        public IActionResult DeleteOrder(int id)
        {
            _logger.LogStart<RequestOrderController>();
            try
            {
                _salesOrderRepository.DeleteOrderById(id);

                _logger.LogSuccess().LogFinish<RequestOrderController>();
                return Json(new { success = true, message = "Order berhasil dihapus." });
            }
            catch (Exception ex)
            {
                _logger.LogFailed(ex);
                return Json(new { success = false, message = $"Gagal menghapus order: {ex.Message}" });
            }
        }

        /// <summary>
        /// Delete sales order item.
        /// </summary>
        [HttpDelete]
        [Route("DeleteItems")]
        public IActionResult DeleteItems([FromBody] List<SalesOrderItem> request)
        {
            _logger.LogStart<RequestOrderController>();
            try
            {
                var itemIds = request.Select(item => item.Id).ToList();  

                if (!itemIds.Any())
                {
                    return BadRequest(new { success = false, message = "No items to delete." });
                }

                _salesOrderRepository.DeleteItemsById(itemIds);

                _logger.LogSuccess().LogFinish<RequestOrderController>();
                return Json(new { success = true, message = "Items berhasil dihapus." });
            }
            catch (Exception ex)
            {
                _logger.LogFailed(ex);
                return Json(new { success = false, message = $"Gagal menghapus item: {ex.Message}" });
            }
        }
    }
}
