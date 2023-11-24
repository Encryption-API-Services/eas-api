using System;

namespace Validation.Networking
{
    public static class IPAddressExtension
    {
        public static string ConvertContextToLocalHostIp(string loopback)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(loopback))
            {
                if (loopback.Equals("::1"))
                {
                    result = "127.0.0.1";
                }
                else
                {
                    result = loopback;
                }
            }
            else
            {
                throw new Exception("Please provide an appropriate loopback IP address alternative as a parameter");
            }
            return result;
        }
    }
}
