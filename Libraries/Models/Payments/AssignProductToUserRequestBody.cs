namespace Models.Payments
{
    public class AssignProductToUserRequestBody
    {
        public string StripeProductId { get; set; }
        public string StripePriceId { get; set; }
    }
}
