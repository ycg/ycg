using System;
using System.IO;
using System.Text;

namespace Ycg.Serialization
{
    public class JsonSerializer : ISerializer
    {
        public static readonly JsonSerializer Instance = new JsonSerializer();

        private JsonSerializer()
        {
        }

        public string Serialize(object obj)
        {
            return fastJSON.JSON.Instance.ToJSON(obj, new fastJSON.JSONParameters { UseExtensions = false });
        }

        public string Serialize(object obj, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public void SerializeToFile(object obj, string filePath)
        {
            this.SerializeToFile(obj, filePath, Encoding.UTF8);
        }

        public void SerializeToFile(object obj, string filePath, Encoding encoding)
        {
            string value = this.Serialize(obj);
            File.AppendAllText(filePath, value);
        }

        public T Deserialize<T>(string value)
        {
            return fastJSON.JSON.Instance.ToObject<T>(value);
        }

        public T Deserialize<T>(string value, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public T DeserializeFromFile<T>(string filePath)
        {
            return this.DeserializeFromFile<T>(filePath, Encoding.UTF8);
        }

        public T DeserializeFromFile<T>(string filePath, Encoding encoding)
        {
            string contents = File.ReadAllText(filePath);
            return this.Deserialize<T>(contents);
        }

        public object Deserialize(string value)
        {
            return fastJSON.JSON.Instance.ToObject(value);
        }

        public object Deserialize(string value, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public object DeserializeFromFile(string filePath, Encoding encoding)
        {
            throw new NotImplementedException();
        }
    }
}