using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticLogLibrary
{
    public class MongoLogDestination : ILogDestination
    {
        private IMongoCollection<MongoLog> Collection;

        public MongoLogDestination(String mongoConnection, String collectionName)
        {
            var url = new MongoUrl(mongoConnection);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            Collection = db.GetCollection<MongoLog>(collectionName);
        }

        public void AddLog(LogMessage message)
        {
            Collection.InsertOne(new MongoLog(message));
        }
    }

    public class MongoLog
    {

        public MongoLog(LogMessage log)
        {
            Id = ObjectId.GenerateNewId();
            Log = log;
        }

        public ObjectId Id { get; set; }

        public LogMessage Log { get; set; }
    }
}
