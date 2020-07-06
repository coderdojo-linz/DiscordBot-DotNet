using System;

namespace DiscordBot.Domain.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContainerNameAttribute : Attribute
    {
        public string Name { get; set; }

        public ContainerNameAttribute(string name)
        {
            Name = name;
        }
    }
}