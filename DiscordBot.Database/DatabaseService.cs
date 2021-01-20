using System;
using DiscordBot.Domain.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Database
{
    public class DatabaseService : IDatabaseService
    {
        public DatabaseService(IOptions<DatabaseSettings> configuration, ILogger<DatabaseService> logger)
        {
            Configuration = configuration;
            try
            {
                Client = new CosmosClient(Configuration.Value.Endpoint, Configuration.Value.Key);
                logger.LogInformation("Erfolgreich mit der Datenbank verbunden!");
            }
            catch (Exception e)
            {
                Client = null;
                logger.LogError(e, "Fehler beim Laden der Datenbank!");
            }
        }

        public IOptions<DatabaseSettings> Configuration { get; }
        public CosmosClient Client { get; }

        public Microsoft.Azure.Cosmos.Database GetDatabase()
        {
            if (Client == null)
            {
                return null;
            }
            return Client.GetDatabase(Configuration.Value.Name);
        }

        public DatabaseContainer<TContainer> GetContainer<TContainer>() where TContainer : DatabaseObject
        {
            return new DatabaseContainer<TContainer>(this);
        }
    }
}