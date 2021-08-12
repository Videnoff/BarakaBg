namespace BarakaBg.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Data.Models.Enums;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Orders;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.EntityFrameworkCore;

    public class OrdersService : IOrdersService
    {
        private readonly IDeletableEntityRepository<Order> ordersRepository;
        private readonly IRepository<ProductOrder> orderProductsRepository;
        private readonly IProductsService productsService;
        private readonly IShoppingBagService shoppingBagService;

        public OrdersService(
            IDeletableEntityRepository<Order> ordersRepository,
            IShoppingBagService shoppingBagService,
            IRepository<ProductOrder> orderProductsRepository,
            IProductsService productsService)
        {
            this.ordersRepository = ordersRepository;
            this.shoppingBagService = shoppingBagService;
            this.orderProductsRepository = orderProductsRepository;
            this.productsService = productsService;
        }

        public async Task CreateAsync<T>(T model, string userId)
        {
            await this.CancelAnyProcessingOrders(userId);

            var order = AutoMapperConfig.MapperInstance.Map<Order>(model);
            order.UserId = userId;

            await this.ordersRepository.AddAsync(order);
            await this.ordersRepository.SaveChangesAsync();
        }

        public async Task<string> CompleteOrderAsync(string userId)
        {
            var order = this.GetProcessingOrderByUserId(userId);
            if (order == null)
            {
                return null;
            }

            var shoppingBagProducts =
                await this.shoppingBagService.GetAllProductsAsync<ShoppingBagProductViewModel>(true, null, userId);
            if (shoppingBagProducts == null || shoppingBagProducts.Count() == 0)
            {
                return null;
            }

            foreach (var shoppingBagProduct in shoppingBagProducts)
            {
                var orderProduct = new ProductOrder
                {
                    Order = order,
                    ProductId = shoppingBagProduct.ProductId,
                    Quantity = shoppingBagProduct.Quantity,
                    Price = shoppingBagProduct.ProductPrice,
                };

                if (!this.OrderHasProduct(order.Id, shoppingBagProduct.ProductId))
                {
                    order.Products.Add(orderProduct);
                }
            }

            order.TotalPrice = order.Products.Sum(x => x.Quantity * x.Price) + order.DeliveryPrice;

            if (order.PayForm == PayForm.CashOnDelivery || order.PayCondition == PayCondition.Paid)
            {
                await this.shoppingBagService.DeleteAllProductsAsync(userId);
                order.Condition = OrderCondition.Unprocessed;

                var orderViewModel = this.GetById<OrderViewModel>(order.Id);
                orderViewModel.Condition = order.Condition;
                orderViewModel.TotalPrice = order.TotalPrice;
            }

            this.ordersRepository.Update(order);
            await this.ordersRepository.SaveChangesAsync();

            return order.Id;
        }

        public async Task<bool> SetOrderStatusAsync(string id, string condition)
        {
            var order = this.GetOrderById(id);
            if (order == null)
            {
                return false;
            }

            var conditionResult = Enum.TryParse<OrderCondition>(condition, out var conditionParsed);
            if (!conditionResult)
            {
                return false;
            }

            order.Condition = conditionParsed;
            if (conditionParsed == OrderCondition.Delivered)
            {
                order.IsDelivered = true;
                order.DeliveredOn = DateTime.UtcNow;
                order.PayCondition = PayCondition.Paid;
            }
            else
            {
                order.IsDelivered = false;
                order.DeliveredOn = null;
            }

            this.ordersRepository.Update(order);
            await this.ordersRepository.SaveChangesAsync();

            return true;
        }

        public IEnumerable<T> TakeOrdersByUserId<T>(string userId, int page, int ordersToTake) =>
            this.ordersRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * ordersToTake)
                .Take(ordersToTake)
                .To<T>().ToList();

        public IEnumerable<T> TakeOrdersByCondition<T>(OrderCondition orderCondition, int page, int ordersToTake) =>
            this.ordersRepository.AllAsNoTracking()
                .Where(x => x.Condition == orderCondition)
                .OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * ordersToTake)
                .Take(ordersToTake)
                .To<T>()
                .ToList();

        public IEnumerable<T> TakeProcessingAndUnprocessedOrders<T>(int page, int ordersToTake) =>
            this.ordersRepository.AllAsNoTracking()
                .Where(x => x.Condition == OrderCondition.Processing || x.Condition == OrderCondition.Unprocessed)
                .OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * ordersToTake)
                .Take(ordersToTake)
                .To<T>().ToList();

        public IEnumerable<T> TakeDeletedOrders<T>(int page, int ordersToTake) =>
            this.ordersRepository.AllAsNoTrackingWithDeleted()
                .Where(x => x.IsDeleted)
                .OrderByDescending(x => x.DeletedOn)
                .Skip((page - 1) * ordersToTake)
                .Take(ordersToTake)
                .To<T>().ToList();

        public IEnumerable<T> GetMostBoughtProducts<T>(int productsToTake)
        {
            var productsIds = this.orderProductsRepository.AllAsNoTracking()
                .Where(x => x.Order.Condition == OrderCondition.Delivered)
                .GroupBy(x => x.ProductId)
                .Select(x => new
                {
                    ProductId = x.Key,
                    Total = x.Count(),
                })
                .OrderByDescending(x => x.Total)
                .Take(productsToTake)
                .ToList();

            var products = new List<T>();

            foreach (var product in productsIds)
            {
                var mappedProduct = this.productsService.GetById<T>(product.ProductId);
                products.Add(mappedProduct);
            }

            return products;
        }

        public int GetOrdersCountByUserId(string userId) =>
            this.ordersRepository.AllAsNoTracking()
                .Count(x => x.UserId == userId);

        public int GetOrdersCountByCondition(OrderCondition condition) =>
            this.ordersRepository.AllAsNoTracking()
                .Count(x => x.Condition == condition);

        public int GetDeletedOrdersCount() =>
            this.ordersRepository.AllAsNoTrackingWithDeleted()
                .Count(x => x.IsDeleted);

        public T GetById<T>(string id) =>
            this.ordersRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>().FirstOrDefault();

        public Order GetProcessingOrderByUserId(string userId) =>
            this.ordersRepository.All()
                .Include(x => x.Products)
                .FirstOrDefault(x => x.UserId == userId && x.Condition == OrderCondition.Processing);

        public PayForm GetPayFormById(string id) =>
            this.ordersRepository.AllAsNoTracking()
                .FirstOrDefault(x => x.Id == id)
                .PayForm;

        public bool UserHasOrder(string userId, string orderId) =>
            this.ordersRepository.AllAsNoTracking()
                .Any(x => x.UserId == userId && x.Id == orderId);

        public async Task FulfillOrderById(string orderId, string stripeId)
        {
            var order = this.GetOrderById(orderId);

            order.PayCondition = PayCondition.Paid;
            order.StripeId = stripeId;

            this.ordersRepository.Update(order);
            await this.ordersRepository.SaveChangesAsync();
        }

        public async Task CancelAnyProcessingOrders(string userId)
        {
            var order = this.GetProcessingOrderByUserId(userId);
            if (order != null)
            {
                order.Condition = OrderCondition.Cancelled;
                this.ordersRepository.Update(order);
                await this.ordersRepository.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var order = this.GetOrderById(id);
            if (order == null)
            {
                return false;
            }

            this.ordersRepository.Delete(order);
            await this.ordersRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UndeleteAsync(string id)
        {
            var order = this.GetDeletedOrderById(id);
            if (order == null)
            {
                return false;
            }

            this.ordersRepository.Undelete(order);
            await this.ordersRepository.SaveChangesAsync();
            return true;
        }

        private Order GetOrderById(string id) =>
            this.ordersRepository.All()
                .FirstOrDefault(x => x.Id == id);

        private Order GetDeletedOrderById(string id) =>
            this.ordersRepository.AllAsNoTrackingWithDeleted()
                .Where(x => x.IsDeleted && x.Id == id)
                .FirstOrDefault();

        private bool OrderHasProduct(string orderId, int productId) =>
            this.ordersRepository.AllAsNoTracking()
                .Any(x => x.Id == orderId && x.Products.Any(x => x.ProductId == productId));
    }
}
