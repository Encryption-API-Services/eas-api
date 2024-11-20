using DataLayer.Mongo;
using DataLayer.Mongo.Repositories;
using DataLayer.RabbitMQ;
using Log_Service;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);

LogServiceInfisicalEnvironment.Setup();

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IDatabaseSettings, DatabaseSettings>();
builder.Services.AddSingleton<RabbitMQConnection>();
builder.Services.AddSingleton<ILogRequestRepository, LogRequestRepository>();
builder.Services.AddSingleton<LogRequestQueueSubscribe>();
MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("Connection")));
settings.MinConnectionPoolSize = 1;
settings.MaxConnectionPoolSize = 500;
MongoClient client = new MongoClient(settings);
builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
{
    return client;
});
var host = builder.Build();
host.Run();
