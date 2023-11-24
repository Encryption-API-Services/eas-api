namespace Models.Encryption
{
    public class Blake2Request
    {
        public int HashSize { get; set; }
        public string DataToHash { get; set; }
    }
}