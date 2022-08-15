using MongoDB.Driver;
using MongoDbTools.MongoDb;
using MongoDbTools.Test1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTools.Test1
{
    internal class Test1
    {
        private readonly MongoDbSettings _dbSettings;

        public Test1()
        {
            _dbSettings = new MongoDbSettings
            {
                Database = "myDb",
                ConnectionString = "mongodb://localhost:27017"
            };

            //AddSampleData();
            GetCollections().ToList().ForEach(x => Console.WriteLine(x));
        }


        async void AddSampleData()
        {
            await new MongoDbRepositoryBase<User>(_dbSettings).AddAsync(new User
            {
                Name = "test 2",
                LastName = "guest",
                UserType = UserType.Premium,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        Label = "myAddress1",
                        AddressType = AddressType.Home,
                        Description = "İstanbul"
                    },
                    new Address
                    {
                        Label = "myAddress2",
                        AddressType = AddressType.Work,
                        Description = "Ayazağa"
                    }
                }
            });
        }


        string[] GetCollections()
        {
            var client = new MongoClient(_dbSettings.ConnectionString);
            var db = client.GetDatabase(_dbSettings.Database);

            return db.ListCollections().ToList().Select(x => x["name"].ToString()).ToArray();
        }
    }
}
