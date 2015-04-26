using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Ycg.Util;

namespace Ycg.Serialization
{
    public class XMLSerializer : ISerializer
    {
        public static readonly XMLSerializer Instance = new XMLSerializer();
        
        private XMLSerializer()
        {
        }

        public string Serialize(object obj)
        {
            return this.Serialize(obj, Encoding.UTF8);
        }

        public string Serialize(object obj, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XMLSerializeInternal(stream, obj, encoding);
                stream.Position = 0;
                return encoding.GetString(stream.ToArray());
            }
        }

        public void SerializeToFile(object obj, string filePath)
        {
            this.SerializeToFile(obj, filePath, Encoding.UTF8);
        }

        public void SerializeToFile(object obj, string filePath, Encoding encoding)
        {
            using (FileStream fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                XMLSerializeInternal(fileStream, obj, encoding);
            }
        }

        public T Deserialize<T>(string value)
        {
            return this.Deserialize<T>(value, Encoding.UTF8);
        }

        public T Deserialize<T>(string value, Encoding encoding)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream(encoding.GetBytes(value)))
            {
                return (T)xmlSerializer.Deserialize(stream);
            }
        }

        public T DeserializeFromFile<T>(string filePath)
        {
            return this.DeserializeFromFile<T>(filePath, Encoding.UTF8);
        }

        public T DeserializeFromFile<T>(string filePath, Encoding encoding)
        {
            string value = File.ReadAllText(filePath);
            using (MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(value)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(memoryStream);
            }

            //Another Code Write: 
            //using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            //{
            //    using (StreamReader streamReader = new StreamReader(fileStream))
            //    {
            //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            //        return (T)xmlSerializer.Deserialize(streamReader);
            //    }
            //}
        }

        public object Deserialize(string value)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(string value, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public object DeserializeFromFile(string filePath, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        private void XMLSerializeInternal(Stream stream, object obj, Encoding encoding)
        {
            if (obj.IsNull() || encoding.IsNull()) throw new ArgumentNullException("obj");
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = "\r\n",
                Encoding = encoding,
                IndentChars = "    "
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
            {
                xmlSerializer.Serialize(xmlWriter, obj);
            }
        }
    }
}