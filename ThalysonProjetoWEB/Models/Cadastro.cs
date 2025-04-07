using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ThalysonProjetoWEB.Models
{
    public class Cadastro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ID { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [BsonElement("usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [BsonElement("senha")]
        public string Password { get; set; }
    }
}
