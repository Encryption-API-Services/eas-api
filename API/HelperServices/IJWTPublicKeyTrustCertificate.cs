namespace API.HelperServices
{
    public interface IJWTPublicKeyTrustCertificate
    {
        void CreatePublicKeyTrustCertificate(string publicKey, string userId);
    }
}
