using Microsoft.Azure.Cosmos;

namespace DiscordBot.Domain.Database
{
    public interface IDatabaseService
    {
        /// <summary>
        /// Get a simple, connected Cosmos-Client.
        /// </summary>
        CosmosClient Client { get; }

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
        DatabaseContainer<TContainer> GetContainer<TContainer>(string name = null) where TContainer : DatabaseObject;
    }
}