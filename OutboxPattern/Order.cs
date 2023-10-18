namespace OutboxPattern
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public int OrderDate { get; set; }
        public int OrderReference { get; set; }
    }

    public class OutboxMessage
    {
        public int Id { get; set; }
        public string EventName { get; set; }

        public string MessageContent { get; set; }

        public bool? Processed { get; set; }

        public DateTime? ProcessedOn { get; set; }
    }

    public class OrderEvent
    {
        public Guid OrderId { get; set; }
        public int OrderDate { get; set; }
        public int OrderReference { get; set; }
    }

    public class OrderRequest
    {
        public int OrderDate { get; set; }
        public int OrderReference { get; set; }
    }
}
