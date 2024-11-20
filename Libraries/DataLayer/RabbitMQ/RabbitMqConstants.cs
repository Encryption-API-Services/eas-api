namespace DataLayer.RabbitMQ
{
    public class RabbitMqConstants
    {
        public class Queues
        {
            public const string ActivateUser = "ActivateUser";
            public const string ForgotPassword = "ForgotPassword";
            public const string LockedOutUsers = "LockedOutUsers";
            public const string CCInformationChanged = "CCInformationChanged";
            public const string Email2FAHotpCode = "Email2FAHotpCode";
            public const string EmergencyKit = "EmergencyKit";
            public const string EmergencyKitRecovery = "EmgergencyKitRecovery";
            public const string LogRequest = "LogRequest";
        }
    }
}
