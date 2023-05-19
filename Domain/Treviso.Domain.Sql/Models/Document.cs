using MongoDB.Bson;
using Treviso.Domain.Sql.Models.Interfaces;

namespace Treviso.Domain.Sql.Models;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }
    public DateTime CreatedAt => Id.CreationTime;
}