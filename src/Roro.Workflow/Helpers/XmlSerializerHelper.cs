using Roro.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public class XmlSerializerHelper
    {
        private static XmlSerializer _xmlSerializer;

        private static XmlSerializer GetSerializer<T>()
        {
            if (_xmlSerializer is null)
            {
                var extraTypes = AppDomain.CurrentDomain.GetAssemblies()
                                     .Where(x =>
                                         x.GetName().Name.StartsWith(typeof(Activity).Namespace) ||
                                         x.GetName().Name.Equals(typeof(XmlSerializerHelper).Namespace))
                                     .SelectMany(x => x.GetTypes())
                                         .Where(t => !t.IsInterface)
                                         .Where(t =>
                                             typeof(Node).IsAssignableFrom(t) ||
                                             typeof(Port).IsAssignableFrom(t)).ToArray();

                _xmlSerializer = new XmlSerializer(typeof(T), extraTypes);
            }
            return _xmlSerializer;
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
                GetSerializer<T>().Serialize(writer, obj);
                return writer.ToString();
            }
        }
    }
}