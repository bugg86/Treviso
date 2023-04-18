using MongoDB.Bson;
using oTSPA.Domain.Mongo.Models.Interfaces;

namespace oTSPA.Domain.Mongo.Models;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }
    public DateTime CreatedAt => Id.CreationTime;
}