using DiscordBot.Services.Base;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Services.ReactionBase
{
    public class ReactionModuleRegistry
    {
        private Dictionary<string, HashSet<Type>> _modules = new Dictionary<string, HashSet<Type>>();

        public ReactionModuleRegistry Register<T>(string reactionName = null) where T : ReactionModuleBase
        {
            reactionName ??= string.Empty;

            if (!_modules.TryGetValue(reactionName, out var moduleList) || moduleList == null)
            {
                moduleList = new HashSet<Type>();
                _modules[reactionName] = moduleList;
            }

            moduleList.Add(typeof(T));
            return this;
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