namespace Models.Payments
{
    public class CreatePriceRequestBody
    {
        public string ProductId { get; set; }
        public long Price { get; set; }
    }
}
