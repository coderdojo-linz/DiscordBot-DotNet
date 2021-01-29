using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot.Database;
using DiscordBot.Domain.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDatabaseContainer<TEntity>(this IServiceCollection serviceCollection)
            where TEntity : DatabaseObject
        {
            serviceCollection.AddScoped<DatabaseContainer<TEntity>>();
            serviceCollection.AddScoped<IAsyncInitializable>(serviceProvider => serviceProvider.GetRequiredService<DatabaseContainer<TEntity>>());

            return serviceCollection;
        }
    }
}
