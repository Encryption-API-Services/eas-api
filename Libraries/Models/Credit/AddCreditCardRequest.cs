namespace Models.Credit
{
    public class AddCreditCardRequest
    {
        public string creditCardNumber { get; set; }
        public string expirationMonth { get; set; }
        public string expirationYear { get; set; }
        public string SecurityCode { get; set; }
    }
}
