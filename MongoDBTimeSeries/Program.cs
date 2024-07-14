using BenchmarkDotNet.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBTimeSeries.Models;

namespace MongoDBTimeSeries;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("datapointtest");
        var collection = database.GetCollection<DataPoint>("datapoints");
        var startTime = new DateTime(2024, 7, 14, 10, 0, 0);
        var endTime = new DateTime(2024, 7, 14, 20, 0, 0);

        // await FillDatabaseWithDatapoints(collection);
        
        var results = await GetDataPoints(collection, startTime, endTime);

        foreach (var result in results)
        {
            var timestamp = result.Timestamp;
            var value = result.AverageValue;

            Console.WriteLine($"Timestamp: {timestamp}, Value: {value}");
        }
    }

    [Benchmark]
    private static async Task FillDatabaseWithDatapoints(IMongoCollection<DataPoint> collection)
    {
        var random = new Random();
        var startTime = DateTime.UtcNow;
        var dataPoints = new List<DataPoint>();
        for (int i = 0; i < 800000; i++)
        {
            var datapoint = new DataPoint
            {
                Timestamp = startTime.AddSeconds(i),
                Value = random.NextDouble(),
                Metadata = new BsonDocument()
            };
            dataPoints.Add(datapoint);
        }

        await collection.InsertManyAsync(dataPoints);
    }

    [Benchmark]
    private static async Task<List<AggregatedDataPoint>> GetDataPoints(IMongoCollection<DataPoint> collection, DateTime startTime, DateTime endTime)
    {
        
        var filter = Builders<DataPoint>.Filter.And(
            Builders<DataPoint>.Filter.Gte(dp => dp.Timestamp, startTime),
            Builders<DataPoint>.Filter.Lte(dp => dp.Timestamp, endTime)
        );

        var entries = await collection
            .Aggregate()
            .Match(filter)
            .BucketAuto(dp => dp.Timestamp, 100, g => new AggregatedDataPoint
            {
                Timestamp = g.Min(dp => dp.Timestamp),
                AverageValue = g.Average(dp => dp.Value)
            })
            .ToListAsync();

        return entries;
    }
}