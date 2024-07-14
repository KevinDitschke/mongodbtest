namespace MongoDBTimeSeries.Models;

public class AggregatedDataPoint
{
    public DateTime Timestamp { get; set; }
    public double AverageValue { get; set; }
}