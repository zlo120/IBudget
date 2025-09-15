using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IBudget.Core.Model
{
    public class BaseModel
    {
        [BsonId]
        public ObjectId? Id { get; set; }
    }
}
