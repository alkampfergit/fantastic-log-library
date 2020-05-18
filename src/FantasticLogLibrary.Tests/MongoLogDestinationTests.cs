using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FantasticLogLibrary;
using NUnit.Framework;

namespace FantasticLogLibrary.Tests
{
    public class MongoLogDestinationTests
    {
        private IMongoCollection<MongoLog> _coll;
        private MongoLogDestination _sut;

        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            string connection = "mongodb://localhost/test-logger";
            var url = new MongoUrl(connection);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            _coll = db.GetCollection<MongoLog>("log");
            db.DropCollection("log");

            _sut = new MongoLogDestination(connection, "log");
        }


        [Test]
        public void basic_save_of_log()
        {
            _sut.AddLog(new LogMessage(LogLevel.Info, "test", DateTime.UtcNow));
            Assert.That(_coll.AsQueryable().Count(), Is.EqualTo(1)); 
        }
    }
}
