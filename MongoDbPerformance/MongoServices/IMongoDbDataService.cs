using MongoDB.Bson;
using MongoDbPerformance.Models;

namespace MongoDbPerformance.MongoServices;

public interface IMongoDbDataService
{
    Task<IEnumerable<Car>> GetAllCars();
    Task<Car?> GetCarById(ObjectId id);
    Task AddCar(Car car);
    Task UpdateCar(Car car);
    Task DeleteCar(ObjectId id);
}