using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EVChargingSessionWebAPI.Models
{
    public class ChargingSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string SessionId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public double EnergyConsumed { get; set; }
    }
}
