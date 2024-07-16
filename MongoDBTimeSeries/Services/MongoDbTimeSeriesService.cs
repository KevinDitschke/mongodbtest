using MethodTimer;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBTimeSeries.Models;

namespace MongoDBTimeSeries.Services;

public static class MongoDbTimeSeriesService
{
    internal static async Task FillDatabaseWithDatapoints(IMongoCollection<DataPoint> collection)
    {
        var random = new Random();
        var startTime = DateTime.UtcNow;
        var dataPoints = new List<DataPoint>();
        for (int i = 0; i < 2000000; i++)
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

    [Time]
    internal static async Task<List<AggregatedDataPoint>> GetDataPoints(IMongoCollection<DataPoint> collection,
        DateTime startTime, DateTime endTime)
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

    internal static async Task EnsureCollectionIsCreated(IMongoDatabase database)
    {
        var collectionNames = await (await database.ListCollectionNamesAsync()).ToListAsync();
        if (!collectionNames.Contains("datapointsts"))
        {
            var createCollectionOptions = new CreateCollectionOptions
            {
                TimeSeriesOptions = new TimeSeriesOptions("timestamp", "metadata",
                    new Optional<TimeSeriesGranularity?>(TimeSeriesGranularity.Seconds))
            };
            await database.CreateCollectionAsync("datapointsts", createCollectionOptions);
        }
    }

    internal static async Task EnsureIndexIsCreated(IMongoCollection<BsonDocument> collection, string indexName)
    {
        var indexKeysDefinition = Builders<BsonDocument>.IndexKeys.Combine(
            Builders<BsonDocument>.IndexKeys.Descending("metadata"),
            Builders<BsonDocument>.IndexKeys.Descending("timestamp"));
        var indexModel =
            new CreateIndexModel<BsonDocument>(indexKeysDefinition,
                new CreateIndexOptions() { Name = indexName });
        await collection.Indexes.CreateOneAsync(indexModel);
    }
}