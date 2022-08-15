using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbTools.MongoDb;
using static MongoDbTools.MongoDb.MongoDbAttributes;

namespace MongoDbTools.Test1.Models
{
    [BsonCollection("Users")]
    internal class User : MongoDbEntity
    {
        [BsonElement("Name")]
        [BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }


        [BsonElement("LastName")]
        [BsonRepresentation(BsonType.String)]
        public string? LastName { get; set; }


        public int Age { get; set; }


        public UserType UserType { get; set; }


        public List<Address> Addresses { get; set; }
    }

    public enum UserType
    {
        Guest,
        Premium
    }
}
