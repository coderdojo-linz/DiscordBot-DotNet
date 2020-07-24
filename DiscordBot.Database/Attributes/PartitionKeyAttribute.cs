using System;

namespace DiscordBot.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PartitionKeyAttribute : Attribute
    {
    }
}