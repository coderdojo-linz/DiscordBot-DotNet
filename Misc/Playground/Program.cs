using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Net.Http;

namespace Playground
{
    /// <summary>
    ///
    ///  User -> Controller -> Services -> DatabaseAccesObjects (DAO)
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            var controller = new Controller
            (
                new Service
                (
                    new Dao()
                )
            );

            var provider = ConfigureServices();

            var controller1 = provider.GetService<Controller>();

            Console.WriteLine("Hello World!");
        }

        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddOptions()

                .AddSingleton<Dao>()
                .AddSingleton<Service>()
                .AddSingleton<Controller>()
                ;

            return serviceCollection.BuildServiceProvider();
        }
    }

    public class Controller
    {
        private readonly Service _service;

        public Controller(Service service)
        {
            _service = service;
        }

        public int MySuperFunc()
        {
            return _service.DoStuff();
        }
    }

    public class Service
    {
        private readonly Dao _mydao;

        public Service(Dao mydao)
        {
            _mydao = mydao;
        }

        public int DoStuff()
        {
            return _mydao.GetValue() * 2;
        }
    }

    public class Dao
    {
        private Random _random;

        public Dao()
        {
            _random = new Random();
        }

        public int GetValue()
        {
            return _random.Next(100);
        }
    }
}