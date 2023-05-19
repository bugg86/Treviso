using MongoDB.Bson;
using Treviso.Domain.Mongo.Models.Interfaces;

namespace Treviso.Domain.Mongo.Models;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }
    public DateTime CreatedAt => Id.CreationTime;
}