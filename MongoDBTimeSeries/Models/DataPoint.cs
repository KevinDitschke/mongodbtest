using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBTimeSeries.Models;

public class DataPoint
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }
    [BsonElement("timestamp")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get; set; }

    [BsonElement("metadata")]
    public BsonDocument Metadata { get; set; }

    [BsonElement("value")]
    public double Value { get; set; }
}