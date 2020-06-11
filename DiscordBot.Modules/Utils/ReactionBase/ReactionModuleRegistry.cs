using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules.Utils.ReactionBase
{
    public class ReactionModuleRegistry
    {
        private readonly IServiceCollection _serviceProvider;

        private Dictionary<string, HashSet<Type>> _modules = new Dictionary<string, HashSet<Type>>();

        public ReactionModuleRegistry(IServiceCollection serviceProvider = null)
        {
            _serviceProvider = serviceProvider;
        }

        public ReactionModuleRegistry Register<T>(string reactionName = null) where T : ReactionModuleBase
        {
            var reactionNames = typeof(T).GetCustomAttributes<ReactionAttribute>()
                .SelectMany(x => x.ReactionName)
                .Append(reactionName)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            if (!reactionNames.Any())
            {
                RegisterInternal<T>(string.Empty);
                return this;
            }

            foreach (var name in reactionNames)
            {
                RegisterInternal<T>(name);
            }

            return this;
        }

        private void RegisterInternal<T>(string reactionName) where T : ReactionModuleBase
        {
            if (!_modules.TryGetValue(reactionName, out var moduleList) || moduleList == null)
            {
                moduleList = new HashSet<Type>();
                _modules[reactionName] = moduleList;
            }

            if (moduleList.Add(typeof(T)))
            {
                _serviceProvider?.Add(new ServiceDescriptor(typeof(T), typeof(T), ServiceLifetime.Transient));
            }
        }

        public IEnumerable<Type> GetRegisteredTypes(string reactionName)
        {
            var baseEnumerable = Enumerable.Empty<Type>();

            if (_modules.TryGetValue(reactionName, out var moduleList))
            {
                baseEnumerable = baseEnumerable.Concat(moduleList);
            }

            if (_modules.TryGetValue(reactionName, out var defaultList))
            {
                baseEnumerable = baseEnumerable.Concat(defaultList);
            }

            return baseEnumerable;
        }
    }
}