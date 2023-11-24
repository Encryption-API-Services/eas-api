using DataLayer.Mongo;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataLayer.Cache
{
    public class LogRequestCache
    {
        private ConcurrentQueue<LogRequest> queue { get; set; }
        private Timer timer { get; set; }
        private int interval { get; set; }
        private IDatabaseSettings databaseSettings { get; set; }
        private IMongoClient mongoClient { get; set; }

        public LogRequestCache(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            this.interval = 30;
            this.databaseSettings = databaseSettings;
            this.queue = new ConcurrentQueue<LogRequest>();
            this.timer = new Timer(ProcessRequests, null, TimeSpan.FromSeconds(this.interval), TimeSpan.FromSeconds(this.interval));
            this.mongoClient = client;
        }

        private async void ProcessRequests(object state)
        {
            if (this.queue.Count > 0)
            {
                await Task.Run(async () =>
                {
                    LogRequestRepository logRequestRepo = new LogRequestRepository(this.databaseSettings, this.mongoClient);
                    List<LogRequest> requestsToInsert = new List<LogRequest>();
                    while (this.queue.TryDequeue(out LogRequest request))
                    {
                        requestsToInsert.Add(request);
                    }
                    if (requestsToInsert.Count > 0)
                    {
                        await logRequestRepo.InsertRequests(requestsToInsert);
                    }
                });
            }
        }
        public void AddRequest(LogRequest request)
        {
            this.queue.Enqueue(request);
        }
    }
}
