using Roro.Activities;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public class XmlSerializerHelper
    {
        private static Type[] _extraTypes;

        private static XmlSerializer GetSerializer<T>()
        {
            if (_extraTypes is null)
            {
                _extraTypes = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(x =>
                                    x.GetName().Name.StartsWith(typeof(Activity).Namespace) ||
                                    x.GetName().Name.Equals(typeof(XmlSerializerHelper).Namespace))
                                .SelectMany(x => x.GetTypes())
                                    .Where(x => !x.IsInterface)
                                    .Where(x => !x.IsGenericType)
                                    .Where(x =>
                                        typeof(Node).IsAssignableFrom(x) ||
                                        typeof(Port).IsAssignableFrom(x) ||
                                        typeof(Argument).IsAssignableFrom(x)).ToArray();
            }

            return new XmlSerializer(typeof(T), _extraTypes);
        }

        public static T ToObject<T>(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                return (T)GetSerializer<T>().Deserialize(reader);
            }
        }

        public static string ToString<T>(T obj)
        {
            using (var writer = new StringWriter())
            {
                var ser = GetSerializer<T>();
                ser.Serialize(writer, obj);
                return writer.ToString();
            }
        }
    }
}