using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace DiscordBot.Domain.Database
{
    public class DatabaseContainer<T> where T : DatabaseObject
    {
        private readonly string _name;
        private readonly Microsoft.Azure.Cosmos.Database _database;
        private readonly ObjectStoreProperties _props;
        private Container _container;
        public Container Container;

        public DatabaseContainer(IOptions<DatabaseSettings> configuration, string name, CosmosClient client, ObjectStoreProperties props)
        {
            if (name != null && name != "")
            {
                _name = name;
            }
            else
            {
                _name = props.ContainerName;
            }
            _database = client.GetDatabase(configuration.Value.Name);
            _props = props;
            InitAsync().Wait();
        }
        private async Task InitAsync()
        {
            string partitionKey = $"/{_props.PartitionKey?.Name ?? "partkey"}";
            _container = await _database.CreateContainerIfNotExistsAsync(_name, partitionKey);
        }

        public T[] Query(string query) => _container.GetItemQueryIterator<T>(query).ReadNextAsync().Result.ToArray();
        public T[] ReadAll() => Query("select * from db");

        public T Insert(T item)
        {
            DatabaseHelpers.CalculateStorageKeys(_props, item);
            _container.CreateItemAsync(item, DatabaseHelpers.CalculatePartitionKey(_props, item));
            return item;
        }

        public T Upsert(T item)
        {
            DatabaseHelpers.CalculateStorageKeys(_props, item);
            PartitionKey partKey = DatabaseHelpers.CalculatePartitionKey(_props, item);

            _container.UpsertItemAsync(item, partKey).Wait();

            return item;
        }

        public T Delete(T item)
        {
            DatabaseHelpers.CalculateStorageKeys(_props, item);
            PartitionKey partKey = DatabaseHelpers.CalculatePartitionKey(_props, item);

            _container.DeleteItemAsync<T>(item.Id, partKey).Wait();

            return item;
        }
    }
}