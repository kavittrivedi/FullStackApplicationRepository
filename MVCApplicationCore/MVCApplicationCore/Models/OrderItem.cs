namespace MVCApplicationCore.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
    }
}
