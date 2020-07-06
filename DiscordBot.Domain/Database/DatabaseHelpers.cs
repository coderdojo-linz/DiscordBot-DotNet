using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Cosmos;

namespace DiscordBot.Domain.Database
{
    public static class DatabaseHelpers
    {
        public static PartitionKey CalculatePartitionKey<T>(ObjectStoreProperties props, T databaseObject) where T : DatabaseObject
        {
            if (props.PartitionKey == null)
            {
                return new PartitionKey(databaseObject.PartKey);
            }

            object partKeyValue = props.PartitionKey.GetMethod.Invoke(databaseObject, null);
            if (props.PartitionKey.PropertyType == typeof(string))
                return new PartitionKey((string)partKeyValue);
            if (props.PartitionKey.PropertyType == typeof(bool))
                return new PartitionKey((bool)partKeyValue);
            if (props.PartitionKey.PropertyType == typeof(double))
                return new PartitionKey((double)partKeyValue);

            return new PartitionKey(partKeyValue?.ToString());
        }

        public static void CalculateStorageKeys<T>(ObjectStoreProperties props, T storeObject) where T : DatabaseObject
        {
            if (storeObject.Id != null) return; // id is already set - nothing to do

            byte[] primaryKeyHash;
            if (props.PrimaryKeys.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var key in props.PrimaryKeys)
                {
                    builder.Append(key.GetMethod.Invoke(storeObject, null)?.ToString() ?? "null");
                }

                primaryKeyHash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));
            }
            else
            {
                throw new InvalidOperationException("Cannot Store objects without at least one primary Key");
            }

            if (props.PartitionKey != null)
            {
                storeObject.Id = Convert.ToBase64String(primaryKeyHash);
            }
            else
            {
                storeObject.PartKey = Convert.ToBase64String(primaryKeyHash, 0, 5);
                storeObject.Id = Convert.ToBase64String(primaryKeyHash, 5, primaryKeyHash.Length - 5);
            }
        }

        private static readonly Dictionary<Type, ObjectStoreProperties> _knownTypes = new Dictionary<Type, ObjectStoreProperties>();
        public static ObjectStoreProperties GetProperties<T>() where T : class
        {
            if (!_knownTypes.ContainsKey(typeof(T)))
            {
                _knownTypes[typeof(T)] = ObjectStoreProperties.Create(typeof(T));
            }

            return _knownTypes[typeof(T)];
        }
    }
}
