using API.ControllerLogic;
using API.ControllersLogic;
using DataLayer.Cache;
using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using DataLayer.RabbitMQ;
using DataLayer.Redis;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MongoDB.Driver;
using Validation.UserSettings;

namespace API.Config
{
    public class ConfigureServicesHelper
    {
        private IServiceCollection _services { get; set; }

        public ConfigureServicesHelper(IServiceCollection services)
        {
            this._services = services;
        }
        public void Setup()
        {
            this._services.AddHttpContextAccessor();
            SetupMongoClient();
            SetupTransient();
            SetupSingleton();
            SetupScoped();
            SetupKestralAndIISOptions();
        }

        private void SetupMongoClient()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("Connection")));
            settings.MinConnectionPoolSize = 1;
            settings.MaxConnectionPoolSize = 500;
            MongoClient client = new MongoClient(settings);
            this._services.AddSingleton<IMongoClient, MongoClient>(s =>
            {
                return client;
            });
        }
        private void SetupTransient()
        {

        }
        private void SetupSingleton()
        {
            this._services.AddSingleton<IRedisClient, RedisClient>();
            this._services.AddSingleton<IDatabaseSettings, DatabaseSettings>();
            this._services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            this._services.AddSingleton<LogRequestCache>();
            this._services.AddSingleton<BenchmarkMethodCache>();
            this._services.AddSingleton<RabbitMQConnection>();
        }
        private void SetupScoped()
        {
            //  Repositories
            this._services.AddScoped<IUserRepository, UserRepository>();
            this._services.AddScoped<IMethodBenchmarkRepository, MethodBenchmarkRepository>();
            this._services.AddScoped<ICreditRepository, CreditRepository>();
            this._services.AddScoped<IFailedLoginAttemptRepository, FailedLoginAttemptRepository>();
            this._services.AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();
            this._services.AddScoped<ILogRequestRepository, LogRequestRepository>();
            this._services.AddScoped<IHotpCodesRepository, HotpCodesRepository>();
            this._services.AddScoped<ISuccessfulLoginRepository, SuccessfulLoginRepository>();
            this._services.AddScoped<ICASExceptionRepository, CASExceptionRepository>();
            this._services.AddScoped<INewsletterRepository, NewsletterRepository>();
            this._services.AddScoped<IProductRepository, ProductRepository>();
            this._services.AddScoped<IPriceRepository, PriceRepository>();
            this._services.AddScoped<IBenchmarkSDKMethodRepository, BenchmarkSDKMethodRepository>();

            // Controller Logic
            this._services.AddScoped<IUserRegisterControllerLogic, UserRegisterControllerLogic>();
            this._services.AddScoped<IUserLoginControllerLogic, UserLoginControllerLogic>();
            this._services.AddScoped<ICreditControllerLogic, CreditControllerLogic>();
            this._services.AddScoped<ITwoFAControllerLogic, TwoFAControllerLogic>();
            this._services.AddScoped<IUIDataControllerLogic, UIDataControllerLogic>();
            this._services.AddScoped<ITokenControllerLogic, TokenControllerLogic>();
            this._services.AddScoped<IPaymentsControllerLogic, PaymentsControllerLogic>();
            this._services.AddScoped<IPasswordControllerLogic, PasswordControllerLogic>();
            this._services.AddScoped<IUserSettingsControllerLogic, UserSettingsControllerLogic>();
            this._services.AddScoped<IBenchmarkSDKMethodControllerLogic, BenchmarkSDKMethodControllerLogic>();
            this._services.AddScoped<IApiKeyControllerLogic, ApiKeyControllerLogic>();
            this._services.AddScoped<IAuthenticationControllerLogic, AuthenticationControllerLogic>();

            // Validaton
            this._services.AddScoped<UserSettingsValidation>();

            //Rabbit MQ Queues
            this._services.AddScoped<ActivateUserQueuePublish>();
            this._services.AddScoped<ForgotPasswordQueuePublish>();
            this._services.AddScoped<LockedOutUserQueuePublish>();
            this._services.AddScoped<CreditCardInformationChangedQueuePublish>();
        }

        private void SetupKestralAndIISOptions()
        {
            this._services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = null;
            });

            this._services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = null;
            });
        }
    }
}