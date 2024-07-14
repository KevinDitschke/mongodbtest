using BenchmarkDotNet.Attributes;
using MongoDbPerformance.Database;
using MongoDbPerformance.Models;
using MongoDbPerformance.MongoServices;

namespace MongoDbPerformance;

class Program
{
    private static readonly Random _random = new Random();

    [Benchmark]
    static async Task Main(string[] args)
    {
        await using var dbContext = new DatabaseContext();
        var mongoDbDataService = new MongoDbDataService(dbContext);
        // for (int i = 0; i < 10000; i++)
        // {
        //     await mongoDbDataService.AddCar(new Car { Name = RandomString(5) });
        // }

        var cars = await mongoDbDataService.GetAllCars();
        foreach (var car in cars)
        {
            Console.WriteLine($"Id: {car.Id}, Name: {car.Name}");
        }
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}