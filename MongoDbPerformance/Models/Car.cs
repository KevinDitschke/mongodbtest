using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace MongoDbPerformance.Models;

[Collection("cars")]
public class Car
{
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
}