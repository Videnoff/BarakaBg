namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BarakaBg.Data.Models;
    using BarakaBg.Data.Models.Enums;

    public interface IOrdersService
    {
        public Task CreateAsync<T>(T model, string userId);

        public Task<string> CompleteOrderAsync(string userId);

        public Task<bool> SetOrderStatusAsync(string id, string status);

        public IEnumerable<T> TakeOrdersByUserId<T>(string userId, int page, int ordersToTake);

        public IEnumerable<T> TakeOrdersByCondition<T>(OrderCondition condition, int page, int ordersToTake);

        public IEnumerable<T> TakeProcessingAndUnprocessedOrders<T>(int page, int ordersToTake);

        public IEnumerable<T> TakeDeletedOrders<T>(int page, int ordersToTake);

        public IEnumerable<T> GetMostBoughtProducts<T>(int productsToTake);

        public int GetOrdersCountByUserId(string userId);

        public int GetOrdersCountByCondition(OrderCondition condition);

        public int GetDeletedOrdersCount();

        public T GetById<T>(string id);

        public Order GetProcessingOrderByUserId(string userId);

        public PayForm GetPayFormById(string id);

        public bool UserHasOrder(string userId, string orderId);

        public Task FulfillOrderById(string orderId, string stripeId);

        public Task CancelAnyProcessingOrders(string userId);

        public Task<bool> DeleteAsync(string id);

        public Task<bool> UndeleteAsync(string id);
    }
}
