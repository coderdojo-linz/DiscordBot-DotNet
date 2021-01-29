using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiscordBot.Database;

namespace DiscordBot.Services
{
    public class AsyncInitializationService
    {
        private readonly IEnumerable<IAsyncInitializable> _asyncInitializables;

        public AsyncInitializationService(IEnumerable<IAsyncInitializable> asyncInitializables)
        {
            _asyncInitializables = asyncInitializables;
        }

        public async Task InitializeAllAsync()
        {
            foreach (var asyncInitializable in _asyncInitializables.OrderByDescending(x => x.Priority))
            {
                await asyncInitializable.InitializeAsync();
            }
        }
    }
}