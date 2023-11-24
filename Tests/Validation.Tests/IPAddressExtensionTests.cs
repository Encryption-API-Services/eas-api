using System;
using Validation.Networking;
using Xunit;

namespace Validation.Tests
{
    public class IPAddressExtensionTests
    {
        [Fact]
        public void GetLocalHostIP()
        {
            string loopback = IPAddressExtension.ConvertContextToLocalHostIp("::1");
            Assert.Equal(loopback, "127.0.0.1");
        }

        [Fact]
        public void GetLocalhostException()
        {
            Assert.Throws<Exception>(() => IPAddressExtension.ConvertContextToLocalHostIp(""));
        }
    }
}
