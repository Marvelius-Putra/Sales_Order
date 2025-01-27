using Microsoft.AspNetCore.Mvc;
using SalesOrder.Interfaces;
using SalesOrder.Models;
using SalesOrder.Repositories;
using System;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SalesOrder.Extensions;

namespace SalesOrder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISalesOrderRepository _salesOrderRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController(ILogger<HomeController> logger, ISalesOrderRepository salesOrderRepository)
        {
            _logger = logger;
            _salesOrderRepository = salesOrderRepository;
        }

        /// <summary>
        /// Gets all sales order data.
        /// </summary>
        public IActionResult Index()
        {
            var salesOrders = _salesOrderRepository.GetAll();
            return View(salesOrders);
        }

        /// <summary>
        /// Privacy Page.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Error Page.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Pagination View Data Method (5 items per page).
        /// </summary>
        [HttpGet]
        public IActionResult GetPagedOrders(int pageNumber = 1, int pageSize = 5, string keywords = "", string orderDate = "")
        {
            _logger.LogStart<HomeController>();
            var query = _salesOrderRepository.GetAll().AsQueryable();
            if (!string.IsNullOrEmpty(keywords))
            {
                query = query.Where(order =>
                    order.OrderNumber.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                    order.Customer.Contains(keywords, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(orderDate))
            {
                var orderDateParts = orderDate.Split('/');
                if (orderDateParts.Length == 2)
                {
                    query = query.Where(order =>
                        order.OrderDate.Month == int.Parse(orderDateParts[1]) &&
                        order.OrderDate.Day == int.Parse(orderDateParts[0]));
                }
                else if (orderDateParts.Length == 3)
                {
                    query = query.Where(order =>
                        order.OrderDate.Date == DateTime.ParseExact(orderDate, "d/M/yyyy", CultureInfo.InvariantCulture).Date);
                }
            }

            var totalRecords = query.Count();

            var orders = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(order => new
                {
                    order.Id,
                    order.OrderNumber,
                    order.OrderDate,
                    order.Customer
                })
                .ToList();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            _logger.LogSuccess().LogFinish<HomeController>();
            return Json(new
            {
                data = orders,
                totalPages,
                currentPage = pageNumber
            });
        }
    }
}
