using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ThalysonProjetoWEB.Models
{
    public class Pessoa
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ID { get; set; }

        [Required]
        [BsonElement("nome")]
        public string Nome { get; set; }

        [Required]
        [BsonElement("idade")]
        public int Idade { get; set; }
    }
}
