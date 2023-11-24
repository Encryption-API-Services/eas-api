using System.Collections.Generic;

namespace Models.Encryption
{
    public class SCryptEncryptBatchResponse
    {
        public List<string> HashedPasswords { get; set; }
    }
}
