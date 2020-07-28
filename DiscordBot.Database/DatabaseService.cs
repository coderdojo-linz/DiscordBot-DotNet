using System;
using DiscordBot.Domain.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Database
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IOptions<DatabaseSettings> _configuration;

        public DatabaseService(IOptions<DatabaseSettings> configuration, ILogger<DatabaseService> logger)
        {
            _configuration = configuration;
            try
            {
                Client = new CosmosClient(_configuration.Value.Endpoint, _configuration.Value.Key);
                logger.LogInformation("Erfolgreich mit der Datenbank verbunden!");
            }
            catch (Exception e)
            {
                Client = null;
                logger.LogError(e, "Fehler beim Laden der Datenbank!");
            }
        }

        public CosmosClient Client { get; }

        public Microsoft.Azure.Cosmos.Database GetDatabase()
        {
            if (Client == null)
            {
                return null;
            }
            return Client.GetDatabase(_configuration.Value.Name);
        }

        public DatabaseContainer<TContainer> GetContainer<TContainer>(string name = null) where TContainer : DatabaseObject
        {
            return new DatabaseContainer<TContainer>(_configuration, name, Client, DatabaseHelpers.GetProperties<TContainer>());
        }
    }
}