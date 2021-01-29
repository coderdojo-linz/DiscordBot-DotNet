using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DiscordBot.Database.Attributes;

namespace DiscordBot.Database
{
    public class ObjectStoreProperties
    {
        public List<PropertyInfo> PrimaryKeys { get; private set; }
        public PropertyInfo PartitionKey { get; private set; }

        public string ContainerName { get; private set; }

        internal static ObjectStoreProperties Create(Type t)
        {
            var primaryKeys = new List<PropertyInfo>();
            PropertyInfo partitionKey = null;

            var containerName = t.GetCustomAttribute<ContainerNameAttribute>(inherit: true)?.Name ?? t.Name;

            foreach (var m in t.GetProperties())
            {
                if (!m.CanRead) continue;

                foreach (Attribute a in m.GetCustomAttributes())
                {
                    switch (a)
                    {
                        case PrimaryKeyAttribute _:
                            primaryKeys.Add(m);
                            break;

                        case PartitionKeyAttribute _ when partitionKey != null:
                            throw new InvalidOperationException("Only one PartitionKey Attribute is allowed!");
                        case PartitionKeyAttribute _:
                            partitionKey = m;
                            break;
                    }
                }
            }

            return new ObjectStoreProperties
            {
                PartitionKey = partitionKey,
                PrimaryKeys = primaryKeys,
                ContainerName = containerName
            };
        }
    }
}