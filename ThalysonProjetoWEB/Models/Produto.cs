using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ThalysonProjetoWEB.Models
{
    public class Produto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ObjectId ImageId { get; set; }
        public string ImageUrl { get; set; }
    }
}
