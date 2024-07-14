using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDbPerformance.Database;
using MongoDbPerformance.Models;

namespace MongoDbPerformance.MongoServices;

public class MongoDbDataService : IMongoDbDataService
{
    private readonly DatabaseContext _databaseContext;

    public MongoDbDataService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<IEnumerable<Car>> GetAllCars()
    {
        return await _databaseContext.Cars.AsNoTracking().ToListAsync();
    }

    public async Task<Car?> GetCarById(ObjectId id)
    {
        return await _databaseContext.Cars.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddCar(Car car)
    {
        await _databaseContext.Cars.AddAsync(car);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task UpdateCar(Car car)
    {
        _databaseContext.Cars.Update(car);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task DeleteCar(ObjectId id)
    {
        var car = new Car() { Id = id };
        _databaseContext.Cars.Attach(car);
        _databaseContext.Cars.Remove(car);
        await _databaseContext.SaveChangesAsync();
    }
}