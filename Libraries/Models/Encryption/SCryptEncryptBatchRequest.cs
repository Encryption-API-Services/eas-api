using System.Collections.Generic;

namespace Models.Encryption
{
    public class SCryptEncryptBatchRequest
    {
        public List<string> Passwords { get; set; }
    }
}
