using Common;
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
    public class BenchmarkMethodCache
    {
        private ConcurrentQueue<BenchmarkMethodLogger> queue { get; set; }
        private Timer timer { get; set; }
        private int interval { get; set; }
        private IDatabaseSettings databaseSettings { get; set; }
        private IMongoClient mongoClient { get; set; }
        public BenchmarkMethodCache(IDatabaseSettings databaseSettings, IMongoClient client)
        {
            this.interval = 30;
            this.databaseSettings = databaseSettings;
            this.queue = new ConcurrentQueue<BenchmarkMethodLogger>();
            this.timer = new Timer(ProcessLogs, null, TimeSpan.FromSeconds(this.interval), TimeSpan.FromSeconds(this.interval));
            this.mongoClient = client;
        }

        private async void ProcessLogs(object state)
        {
            if (this.queue.Count > 0)
            {
                await Task.Run(async () =>
                {
                    MethodBenchmarkRepository benchMarkRepo = new MethodBenchmarkRepository(this.databaseSettings, this.mongoClient);
                    List<BenchmarkMethod> logsToInsert = new List<BenchmarkMethod>();
                    while (this.queue.TryDequeue(out BenchmarkMethodLogger log))
                    {
                        BenchmarkMethod method = new BenchmarkMethod();
                        method.Details = log;
                        logsToInsert.Add(method);
                    }
                    if (logsToInsert.Count > 0)
                    {
                        await benchMarkRepo.InsertBenchmarks(logsToInsert);
                    }
                });
            }
        }
        public void AddLog(BenchmarkMethodLogger log)
        {
            this.queue.Enqueue(log);
        }
    }
}