using EVChargingSessionWebAPI.Models;
using MongoDB.Driver;

namespace EVChargingSessionWebAPI.Services
{
    public class EVChargingDBService
    {
        private readonly IMongoCollection<ChargingSession> _sessionCollection;
        public EVChargingDBService(IConfiguration config)
        {
           var clientConn=new MongoClient(config["MongoDB:ConnectionString"]);
           var databaseConn = clientConn.GetDatabase(config["MongoDB:DatabaseName"]);
           _sessionCollection = databaseConn.GetCollection<ChargingSession>(config["MongoDB:CollectionName"]);

        }
        //Get latest session's details
        public async Task<ChargingSession> GetChargingSessionAsync(string sessionId)
        {
            return await _sessionCollection.Find(session => session.SessionId == sessionId).FirstOrDefaultAsync();
        }
        //Get a session details by session id
        public async Task<ChargingSession> GetLatestChargingSessionAsync()
        {
            return await _sessionCollection.Find(session => true).SortByDescending(session => session.StartTime).FirstOrDefaultAsync();
        }
        //Insert a new session to DB
        public async Task StartChargingSessionAsync(ChargingSession session)
        {
            await _sessionCollection.InsertOneAsync(session);
            
        }

        //Update a session when charging is stopped
        public async Task UpdateChargingSession(string sessionId,ChargingSession session)
        {
            await _sessionCollection.ReplaceOneAsync(s => s.SessionId == sessionId, session);
        }

    }
}
