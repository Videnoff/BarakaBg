﻿namespace BarakaBg.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using BarakaBg.Data.Models.Enums;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Orders;
    using Microsoft.AspNetCore.Mvc;

    public class OrdersController : AdministrationController
    {
        private const int ItemsPerPage = 6;
        private const string AreaName = "Administration";
        private const string ControllerName = "Orders";

        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        [HttpGet("/Administration/Orders/Unprocessed/{pageNumber?}")]
        public IActionResult Unprocessed(int pageNumber = 1)
        {
            if (pageNumber <= 0)
            {
                return this.Unprocessed();
            }

            var processingAndUnprocessedOrders =
                this.ordersService.TakeProcessingAndUnprocessedOrders<OrderCheckViewModel>(pageNumber, ItemsPerPage);
            var unprocessedOrdersCount = this.ordersService.GetOrdersCountByCondition(OrderCondition.Unprocessed);
            var processingOrdersCount = this.ordersService.GetOrdersCountByCondition(OrderCondition.Processing);

            var viewModel = new OrderListViewModel
            {
                ItemsCount = unprocessedOrdersCount + processingOrdersCount,
                ItemsPerPage = ItemsPerPage,
                PageNumber = pageNumber,
                Orders = processingAndUnprocessedOrders,
                Area = AreaName,
                Controller = ControllerName,
                Action = nameof(this.Unprocessed),
            };

            return this.View(viewModel);
        }

        [HttpGet("/Administration/Orders/Processed/{pageNumber?}")]
        public IActionResult Processed(int pageNumber = 1)
        {
            if (pageNumber <= 0)
            {
                return this.Processed();
            }

            var processedOrders =
                this.ordersService.TakeOrdersByCondition<OrderCheckViewModel>(OrderCondition.Processed, pageNumber,
                    ItemsPerPage);
            var processedOrdersCount = this.ordersService.GetOrdersCountByCondition(OrderCondition.Processed);

            var viewModel = new OrderListViewModel
            {
                ItemsCount = processedOrdersCount,
                ItemsPerPage = ItemsPerPage,
                PageNumber = pageNumber,
                Orders = processedOrders,
                Area = AreaName,
                Controller = ControllerName,
                Action = nameof(this.Processed),
            };

            return this.View(viewModel);
        }

        [HttpGet("/Administration/Orders/Delivered/{pageNumber?}")]
        public IActionResult Delivered(int pageNumber = 1)
        {
            if (pageNumber <= 0)
            {
                return this.Delivered();
            }

            var deliveredOrders =
                this.ordersService.TakeOrdersByCondition<OrderCheckViewModel>(OrderCondition.Delivered, pageNumber,
                    ItemsPerPage);
            var deliveredOrdersCount = this.ordersService.GetOrdersCountByCondition(OrderCondition.Delivered);

            var viewModel = new OrderListViewModel
            {
                ItemsCount = deliveredOrdersCount,
                ItemsPerPage = ItemsPerPage,
                PageNumber = pageNumber,
                Orders = deliveredOrders,
                Area = AreaName,
                Controller = ControllerName,
                Action = nameof(this.Delivered),
            };

            return this.View(viewModel);
        }

        [HttpGet("/Administration/Orders/Deleted/{pageNumber?}")]
        public IActionResult Deleted(int pageNumber = 1)
        {
            if (pageNumber <= 0)
            {
                return this.Deleted();
            }

            var deletedOrders = this.ordersService.TakeDeletedOrders<OrderCheckViewModel>(pageNumber, ItemsPerPage);
            var deletedOrdersCount = this.ordersService.GetDeletedOrdersCount();

            var viewModel = new OrderListViewModel
            {
                ItemsCount = deletedOrdersCount,
                ItemsPerPage = ItemsPerPage,
                PageNumber = pageNumber,
                Orders = deletedOrders,
                Area = AreaName,
                Controller = ControllerName,
                Action = nameof(this.Deleted),
            };

            return this.View(viewModel);
        }

        public IActionResult Details(string id)
        {
            var order = this.ordersService.GetById<OrderInfoViewModel>(id);

            if (order == null)
            {
                this.TempData["Error"] = "Order not found.";
                return this.RedirectToAction(nameof(this.Unprocessed));
            }

            return this.View(order);
        }

        public async Task<IActionResult> SetStatus(string id, string status)
        {
            var actionResult = await this.ordersService.SetOrderStatusAsync(id, status);

            if (actionResult)
            {
                this.TempData["Alert"] = "Sucessfully changed order status.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem changing the order status.";
            }

            return this.RedirectToAction(status.ToString());
        }

        public async Task<IActionResult> Delete(string id)
        {
            var deleteResult = await this.ordersService.DeleteAsync(id);
            if (deleteResult)
            {
                this.TempData["Alert"] = "Successfully deleted order.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem deleting the order.";
            }

            return this.RedirectToAction(nameof(this.Unprocessed));
        }

        public async Task<IActionResult> Undelete(string id)
        {
            var undeleteResult = await this.ordersService.UndeleteAsync(id);
            if (undeleteResult)
            {
                this.TempData["Alert"] = "Successfully undeleted order.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem undeleting the order.";
            }

            return this.RedirectToAction(nameof(this.Deleted));
        }
    }
}
