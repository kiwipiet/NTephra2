using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NTephra2.Tests
{
    /// <summary>
    ///     Easily get access to embedded resources
    /// </summary>
    internal static class EmbeddedResource
    {
        /// <summary>
        ///     Return a resource stream
        /// </summary>
        /// <param name="assembly">The assembly the resource is embedded in</param>
        /// <param name="name">FQDN or the resource</param>
        /// <returns>Resource Stream</returns>
        public static StreamReader GetStream(this Assembly assembly, string name)
        {
            return (from resName in assembly.GetManifestResourceNames()
                where resName.EndsWith(name)
                select new StreamReader(assembly.GetManifestResourceStream(resName)))
                .FirstOrDefault();
        }

        /// <summary>
        ///     Return a resource stream from the same assembly as <see cref="Hp.Nis.Personalisation.EmbeddedResource" />
        /// </summary>
        /// <param name="name">FQDN or the resource</param>
        /// <returns>Resource Stream</returns>
        public static StreamReader GetStream(this string name)
        {
            return GetStream(typeof(EmbeddedResource).Assembly, name);
        }

        /// <summary>
        ///     Returns a resource string
        /// </summary>
        /// <param name="assembly">The assembly the resource is embedded in</param>
        /// <param name="name">FQDN or the resource</param>
        /// <returns>resource string</returns>
        public static string GetString(this Assembly assembly, string name)
        {
            using (var sr = GetStream(assembly, name))
            {
                var data = sr.ReadToEnd();
                return data;
            }
        }

        /// <summary>
        ///     Returns a resource string from the same assembly as <see cref="Hp.Nis.Personalisation.EmbeddedResource" />
        /// </summary>
        /// <param name="name">FQDN or the resource</param>
        /// <returns>resource string</returns>
        public static string GetString(this string name)
        {
            return GetString(typeof(EmbeddedResource).Assembly, name);
        }

        /// <summary>
        ///     Returns a resource string array
        /// </summary>
        /// <param name="assembly">The assembly the resource is embedded in</param>
        /// <param name="name">FQDN or the resource</param>
        /// <returns>resource string array</returns>
        public static string[] GetStringArray(this Assembly assembly, string name)
        {
            var s = assembly.GetString(name);
            return s.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        /// <summary>
        ///     Returns a resource string from the same assembly as <see cref="EmbeddedResource" />
        /// </summary>
        /// <param name="name">FQDN or the resource</param>
        /// <returns>resource string array</returns>
        public static string[] GetStringArray(string name)
        {
            return GetStringArray(typeof(EmbeddedResource).Assembly, name);
        }

    }
}