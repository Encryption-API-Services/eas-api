using System.Collections.Generic;

namespace Models.Encryption
{
    public class BCryptEncryptBatchRequest
    {
        public List<string> Passwords { get; set; }
    }
}