using System;
using System.Threading.Tasks;
using DiscordBot.Domain.Configuration;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Database
{
    public class DatabaseService : IDatabaseService, IAsyncInitializable
    {
        public DatabaseSettings Configuration { get; }
        public CosmosClient Client { get; }
        
        int IAsyncInitializable.Priority => 1;

        public DatabaseService(IOptions<DatabaseSettings> configuration, ILogger<DatabaseService> logger)
        {
            Configuration = configuration.Value;

            try
            {
                if (!string.IsNullOrEmpty(Configuration.ConnectionString))
                {
                    Client = new CosmosClient(Configuration.ConnectionString);
                }
                else if (!(string.IsNullOrEmpty(Configuration.Endpoint) ||
                           string.IsNullOrEmpty(Configuration.Key)))
                {
                    Client = new CosmosClient(Configuration.Endpoint, Configuration.Key);
                }
                else
                {
                    throw new Exception("No configuration given");
                }
                logger.LogInformation("Erfolgreich mit der Datenbank verbunden!");
            }
            catch (Exception e)
            {
                Client = null;
                logger.LogError(e, "Fehler beim Laden der Datenbank!");
            }
        }

  

        public async Task InitializeAsync()
        {
            await Client.CreateDatabaseIfNotExistsAsync(Configuration.Name);
        }

        public Microsoft.Azure.Cosmos.Database GetDatabase()
        {
            return Client?.GetDatabase(Configuration.Name);
        }

        public DatabaseContainer<TContainer> GetContainer<TContainer>() where TContainer : DatabaseObject
        {
            return new DatabaseContainer<TContainer>(this);
        }
    }
}