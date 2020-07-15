using System;

namespace DiscordBot.Domain.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PartitionKeyAttribute : Attribute
    {
    }
}