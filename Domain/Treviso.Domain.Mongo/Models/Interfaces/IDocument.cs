using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Treviso.Domain.Mongo.Models.Interfaces;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    ObjectId Id { get; set; }
    DateTime CreatedAt { get; }
}