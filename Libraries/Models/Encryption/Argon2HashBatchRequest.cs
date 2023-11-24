using System.Collections.Generic;

namespace Models.Encryption
{
    public class Argon2HashBatchRequest
    {
        public List<string> Passwords { get; set; }
    }
}