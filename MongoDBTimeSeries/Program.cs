using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBTimeSeries.Models;
using MongoDBTimeSeries.Services;

namespace MongoDBTimeSeries;

class Program
{
    static async Task Main(string[] args)
    {
        var startTime = DateTime.UtcNow.AddHours(-3);
        var endTime = DateTime.UtcNow.AddHours(3);
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("datapointtest");
        
        var timeSeriesCollection = await SetupTimeSeriesCollection(database);
        var collection = await SetupDocumentCollection(database);
        // await MongoDbTimeSeriesService.FillDatabaseWithDatapoints(timeSeriesCollection);
        // await MongoDbCollectionService.FillDatabaseWithDatapoints(collection);

        await PrintTimeSeriesDatapoints(timeSeriesCollection, startTime, endTime);
        await PrintCollectionDatapoints(collection, startTime, endTime);
    }

    private static async Task PrintCollectionDatapoints(IMongoCollection<DataPoint> collection, DateTime startTime, DateTime endTime)
    {
        var points = await MongoDbCollectionService.GetDataPoints(collection, startTime, endTime);

        foreach (var result in points)
        {
            var timestamp = result.Timestamp;
            var value = result.AverageValue;

            Console.WriteLine($"Timestamp: {timestamp}, Value: {value}");
        }
    }

    private static async Task PrintTimeSeriesDatapoints(IMongoCollection<DataPoint> timeSeriesCollection, DateTime startTime,
        DateTime endTime)
    {
        var results = await MongoDbTimeSeriesService.GetDataPoints(timeSeriesCollection, startTime, endTime);

        foreach (var result in results)
        {
            var timestamp = result.Timestamp;
            var value = result.AverageValue;

            Console.WriteLine($"Timestamp: {timestamp}, Value: {value}");
        }
    }

    private static async Task<IMongoCollection<DataPoint>> SetupDocumentCollection(IMongoDatabase database)
    {
        var collection = database.GetCollection<DataPoint>("datapoints");
        await MongoDbTimeSeriesService.EnsureIndexIsCreated(database.GetCollection<BsonDocument>("datapoints"), "datapointindex");
        return collection;
    }

    private static async Task<IMongoCollection<DataPoint>> SetupTimeSeriesCollection(IMongoDatabase database)
    {
        await MongoDbTimeSeriesService.EnsureCollectionIsCreated(database);
        var timeSeriesCollection = database.GetCollection<DataPoint>("datapointsts");
        await MongoDbTimeSeriesService.EnsureIndexIsCreated(database.GetCollection<BsonDocument>("datapointsts"), "datapointindex");
        return timeSeriesCollection;
    }
}