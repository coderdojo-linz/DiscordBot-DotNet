using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            var type = typeof(T);
            var reactionNames = new List<string>();

            var passAll = string.IsNullOrEmpty(reactionName);
            if (!passAll)
            {
                reactionNames.Add(reactionName);
            }

            var attribute = type.GetCustomAttribute<ReactionAttribute>();
            if (attribute != null)
            {
                if (attribute.FilterMode == ReactionFilter.Select)
                {
                    passAll = false;
                    if (attribute.ReactionNames.Any())
                    {
                        reactionNames.AddRange(attribute.ReactionNames);
                    }

                    if (!reactionNames.Any())
                    {
                        throw new ArgumentException($"Filtermode for '{type.FullName}' is set to '{ReactionFilter.Select}' but no reaction filter name is set!");
                    }
                }
            }

            if (passAll)
            {
                RegisterInternal(type, string.Empty);
                return this;
            }

            foreach (var name in reactionNames)
            {
                RegisterInternal(type, name);
            }

            return this;
        }

        private void RegisterInternal(Type type, string reactionName)
        {
            if (!_modules.TryGetValue(reactionName, out var moduleList) || moduleList == null)
            {
                moduleList = new HashSet<Type>();
                _modules[reactionName] = moduleList;
            }

            if (moduleList.Add(type))
            {
                _serviceProvider?.Add(new ServiceDescriptor(type, type, ServiceLifetime.Transient));
            }
        }

        public IEnumerable<Type> GetRegisteredTypes(string reactionName)
        {
            var baseEnumerable = Enumerable.Empty<Type>();

            if (_modules.TryGetValue(reactionName, out var moduleList))
            {
                baseEnumerable = baseEnumerable.Concat(moduleList);
            }

            if (_modules.TryGetValue(string.Empty, out var defaultList))
            {
                baseEnumerable = baseEnumerable.Concat(defaultList);
            }

            return baseEnumerable;
        }

        public ReactionModuleRegistry AutoDiscover(Func<Type, bool> pre = null)
        {
            pre ??= x => true;

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Append(Assembly.Load("DiscordBot.Modules"))
                .Distinct()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(ReactionModuleBase).IsAssignableFrom(x))
                .Where(x => pre(x));

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<ReactionAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                if (attribute.FilterMode == ReactionFilter.PassAll)
                {
                    RegisterInternal(type, String.Empty);
                    continue;
                }

                foreach (var reactionName in attribute.ReactionNames)
                {
                    TryRegisterAliases(type, reactionName);

                    RegisterInternal(type, reactionName);
                }
            }

            return this;
        }

        private void TryRegisterAliases(Type type, string reactionName)
        {
            var parsed = GEmojiSharp.Emoji.Get(reactionName);
            if (parsed.IsCustom)
            {
                return;
            }

            RegisterInternal(type, parsed.Raw);
            foreach (var alias in parsed.Aliases ?? Array.Empty<string>())
            {
                RegisterInternal(type, alias);
            }
        }
    }
}