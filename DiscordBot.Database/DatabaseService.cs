using DiscordBot.Domain.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace DiscordBot.Database
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IOptions<DatabaseSettings> _configuration;

        public DatabaseService(IOptions<DatabaseSettings> configuration)
        {
            _configuration = configuration;
            Client = new CosmosClient(_configuration.Value.Endpoint,
                        _configuration.Value.Key);
        }

        public CosmosClient Client { get; }

        public Microsoft.Azure.Cosmos.Database GetDatabase()
        {
            return Client.GetDatabase(_configuration.Value.Name);
        }

        public DatabaseContainer<TContainer> GetContainer<TContainer>(string name = null) where TContainer : DatabaseObject
        {
            return new DatabaseContainer<TContainer>(_configuration, name, Client, DatabaseHelpers.GetProperties<TContainer>());
        }
    }
}