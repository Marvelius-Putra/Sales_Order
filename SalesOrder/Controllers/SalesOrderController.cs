using Microsoft.AspNetCore.Mvc;
using SalesOrder.Interfaces;
using SalesOrder.Models;
using SalesOrder.Repositories;
using System.Linq;
using System.Collections.Generic;
using SalesOrder.Extensions;

namespace SalesOrder.Controllers
{
    public class SalesOrderController : Controller
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly ILogger<SalesOrderController> _logger;

        public SalesOrderController(ISalesOrderRepository salesOrderRepository, ILogger<SalesOrderController> logger)
        {
            _salesOrderRepository = salesOrderRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get All Customers data to Create Page.
        /// </summary>
        public IActionResult Create()
        {
            _logger.LogStart<SalesOrderController>();
            var customers = _salesOrderRepository.GetAllCustomers();
            ViewBag.Customers = customers;

            _logger.LogSuccess().LogFinish<SalesOrderController>();
            return View();
        }

        /// <summary>
        /// Bring Saler Data to editPage.
        /// </summary>
        public IActionResult Edit(int id)
        {
            _logger.LogStart<SalesOrderController>();
            var salesOrder = _salesOrderRepository.GetOrderById(id);
            var customers = _salesOrderRepository.GetAllCustomers();

            if (salesOrder == null)
            {
                return NotFound();
            }

            // Cari customer berdasarkan CustomerId
            var selectedCustomer = customers.FirstOrDefault(c => c.Id == salesOrder.CustomerId);

            var model = new BaseViewModel
            {
                Customers = customers.ToList(),
                SalesOrder = new SalesOrderProperty
                {
                    Id = salesOrder.Id,
                    OrderNumber = salesOrder.OrderNumber,
                    OrderDate = salesOrder.OrderDate,
                    CustomerId = salesOrder.CustomerId,
                    Address = salesOrder.Address,
                    Items = salesOrder.Items
                },
                SelectedCustomerName = selectedCustomer?.Name ?? "Unknown Customer"
            };
            _logger.LogSuccess().LogFinish<SalesOrderController>();
            return View(model);
        }    

    }
}
