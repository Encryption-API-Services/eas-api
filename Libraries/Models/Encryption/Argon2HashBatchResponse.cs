using System.Collections.Generic;

namespace Models.Encryption
{
    public class Argon2HashBatchResponse
    {
        public List<string> HashedPasswords { get; set; }
    }
}
