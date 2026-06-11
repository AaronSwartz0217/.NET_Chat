using Furion;
using System;
using System.Reflection;

namespace Chat.Desktop;

public class SingleFilePublish : ISingleFilePublish
{
    public Assembly[] IncludeAssemblies()
    {
        return Array.Empty<Assembly>();
    }

    public string[] IncludeAssemblyNames()
    {
        return
        [
            "Chat.Application",
            "Chat.Core",
        ];
    }
}