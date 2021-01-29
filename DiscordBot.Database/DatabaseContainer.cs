using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;


namespace DiscordBot.Database
{
    public class DatabaseContainer<T> : IAsyncInitializable
        where T : DatabaseObject
    {
        private readonly ObjectStoreProperties _props;
        private readonly string _name;
        private readonly Microsoft.Azure.Cosmos.Database _database;
        private Container _container;

        public DatabaseContainer(IDatabaseService databaseService)
        {
            _props = DatabaseHelpers.GetProperties<T>();
            _name = _props.ContainerName;

            if (databaseService.Client == null)
                _database = null;
            else
                _database = databaseService.GetDatabase();

            _container = null;
        }

        public async Task InitializeAsync()
        {
            if (_container == null)
            {
                string partitionKey = $"/{_props.PartitionKey?.Name ?? "partkey"}";
                _container = await _database.CreateContainerIfNotExistsAsync(_name, partitionKey);
            }
        }

        /// <summary>
        /// Queries the database with the string <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The query to send</param>
        /// <returns>Array of type <typeparamref name="T"/></returns>
        public async Task<T[]> Query(string query)
        {
            if (_database == null)
            {
                throw new Exception("Cannot use database. Have you provided the right configuration?");
            }
            await InitializeAsync();

            return (await _container.GetItemQueryIterator<T>(query).ReadNextAsync()).ToArray();
        }

        /// <summary>
        /// Lists all entries in the database.
        /// </summary>
        /// <returns>Array of type <typeparamref name="T"/> with all entries</returns>
        public async Task<T[]> ReadAll()
        {
            if (_database == null)
            {
                throw new Exception("Cannot use database. Have you provided the right configuration?");
            }
            await InitializeAsync();

            return await Query("select * from db");
        }

        /// <summary>
        /// Inserts the provided <paramref name="item"/> into the database.
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <returns>The provided <paramref name="item"/></returns>
        public async Task<T> Insert(T item)
        {
            if (_database == null)
            {
                throw new Exception("Cannot use database. Have you provided the right configuration?");
            }
            await InitializeAsync();

            DatabaseHelpers.CalculateStorageKeys(_props, item);
            await _container.CreateItemAsync(item, DatabaseHelpers.CalculatePartitionKey(_props, item));
            return item;
        }

        /// <summary>
        /// Updates the <paramref name="item"/> if it exists, else inserts it.
        /// </summary>
        /// <param name="item">The item to update or insert</param>
        /// <returns>The provided <paramref name="item"/></returns>
        public async Task<T> Upsert(T item)
        {
            if (_database == null)
            {
                throw new Exception("Cannot use database. Have you provided the right configuration?");
            }
            await InitializeAsync();

            DatabaseHelpers.CalculateStorageKeys(_props, item);
            PartitionKey partKey = DatabaseHelpers.CalculatePartitionKey(_props, item);

            await _container.UpsertItemAsync(item, partKey);

            return item;
        }

        /// <summary>
        /// Deletes the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to delete</param>
        /// <returns>The provided <paramref name="item"/></returns>
        public async Task<T> Delete(T item)
        {
            if (_database == null)
            {
                throw new Exception("Cannot use database. Have you provided the right configuration?");
            }
            await InitializeAsync();

            DatabaseHelpers.CalculateStorageKeys(_props, item);
            PartitionKey partKey = DatabaseHelpers.CalculatePartitionKey(_props, item);

            await _container.DeleteItemAsync<T>(item.Id, partKey);

            return item;
        }
    }
}