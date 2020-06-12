using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules.Utils.ReactionBase
{
    public static class ReactionBaseExtensions
    {
        public static IServiceCollection AddReactionModules(this IServiceCollection serviceCollection)
        {
            var reactionRegistry = new ReactionModuleRegistry(serviceCollection);

            reactionRegistry.AutoDiscover();

            serviceCollection.AddSingleton(reactionRegistry);
            return serviceCollection;
        }
    }
}