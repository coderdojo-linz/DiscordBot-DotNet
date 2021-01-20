using DiscordBot.Domain.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace DiscordBot.Database
{
    public interface IDatabaseService
    {
        /// <summary>
        /// Get a simple, connected Cosmos-Client.
        /// </summary>
        CosmosClient Client { get; }

        /// <summary>
        /// The configuration in the appsettings.json.
        /// </summary>
        IOptions<DatabaseSettings> Configuration { get; }

        /// <summary>
        /// Get the Database for All Object Types
        /// </summary>
        /// <returns></returns>
        Microsoft.Azure.Cosmos.Database GetDatabase();

        /// <summary>
        ///  Prepares a Container for object Storage of the given Type
        /// </summary>
        /// <typeparam name="TContainer">The Type you want to store</typeparam>
        /// <returns></returns>
        DatabaseContainer<TContainer> GetContainer<TContainer>() where TContainer : DatabaseObject;
    }
}