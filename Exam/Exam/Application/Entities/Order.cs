namespace Exam.Application.Entities
{
    public class Order
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public Order() { }
        public Order(string productId, int amount)
        {
            Id = Guid.NewGuid().ToString();
            ProductId = productId;
            Amount = amount;
            Status = "InProcessing";
            CreatedAt = DateTime.Now;
        }
    }
}
