using System.Collections.Generic;

namespace Models.Encryption
{
    public class BCryptEncryptBatchResponse
    {
        public List<string> HashedPasswords { get; set; }
    }
}
