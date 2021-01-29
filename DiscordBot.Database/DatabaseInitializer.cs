//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//using DiscordBot.Database.Attributes;

//namespace DiscordBot.Database
//{
//    //public class DatabaseInitializer
//    //{
//    //    private readonly IEnumerable<Type> _allTypes;
//    //    private readonly IDatabaseService _databaseService;

//    //    public DatabaseInitializer
//    //    (
//    //        IEnumerable<Type> allTypes,
//    //        IDatabaseService databaseService,
            
//    //    )
//    //    {
//    //        _allTypes = allTypes;
//    //        _databaseService = databaseService;
//    //    }

//    //    public async Task InitializeAsync()
//    //    {
//    //        var allAttributes = _allTypes
//    //            .Select(x => new
//    //            {
//    //               ContainerName = x.GetCustomAttribute<ContainerNameAttribute>(),
//    //               PartitionKey = x.GetCustomAttribute<PartitionKeyAttribute>()

//    //            })
//    //            .Where(x => x != null)
//    //            .Select(x => x.Name);

//    //        var db = _databaseService.GetDatabase();

//    //        foreach (var attribute in allAttributes)
//    //        {
//    //            string partitionKey = $"/{_props.PartitionKey?.Name ?? "partkey"}";
//    //            await db.CreateContainerIfNotExistsAsync(attribute, partitionKey);
//    //        }
//    //    }
//    }
////}