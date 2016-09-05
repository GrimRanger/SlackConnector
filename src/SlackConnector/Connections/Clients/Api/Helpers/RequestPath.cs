using System;
using System.Collections.Generic;
using System.Reflection;

namespace SlackConnector.Connections.Clients.Api.Helpers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RequestPath : Attribute
    {
        public readonly string Path;
        public RequestPath(string requestPath)
        {
            Path = requestPath;
        }

        private static readonly Dictionary<Type, RequestPath> Paths = new Dictionary<Type, RequestPath>();

        public static RequestPath GetRequestPath<T>()
        {
            Type t = typeof(T);
            if (Paths.ContainsKey(t))
                return Paths[t];

            TypeInfo info = t.GetTypeInfo();

            RequestPath path = info.GetCustomAttribute<RequestPath>();
            if (path == null) throw new InvalidOperationException($"No valid request path for {t.Name}");

            Paths.Add(t, path);
            return path;
        }
    }
}
