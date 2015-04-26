using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Ycg.Util;

namespace Ycg.Serialization
{
    public class BinarySerializer : ISerializer
    {
        public static readonly BinarySerializer Instance = new BinarySerializer();

        private BinarySerializer()
        {
        }

        public string Serialize(object obj)
        {
            return this.Serialize(obj, Encoding.UTF8);
        }

        public string Serialize(object obj, Encoding encoding)
        {
            if (obj.IsNull()) throw new ArgumentNullException();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());

                //return Convert.ToString(memoryStream.ToArray().ToString());  The code don't effect.
                //知道为什么不行
                //①首先没有byte[]这个类型的重载
                //②为什么没有重载却编译通过了，因为编译器会自动调用string类型的重载
                //③string的值就是byte[]
            }
        }

        public void SerializeToFile(object obj, string filePath)
        {
            this.SerializeToFile(obj, filePath, Encoding.UTF8);
        }

        public void SerializeToFile(object obj, string filePath, System.Text.Encoding encoding)
        {
            if (obj.IsNull()) throw new ArgumentNullException();
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter binarFormatter = new BinaryFormatter();
                binarFormatter.Serialize(fileStream, obj);
            }
        }

        public T Deserialize<T>(string value)
        {
            return (T)this.Deserialize(value);
        }

        public T Deserialize<T>(string value, System.Text.Encoding encoding)
        {
            return (T)this.Deserialize(value, encoding);
        }

        public T DeserializeFromFile<T>(string filePath)
        {
            return (T)this.DeserializeFromFile(filePath, Encoding.UTF8);
        }

        public T DeserializeFromFile<T>(string filePath, System.Text.Encoding encoding)
        {
            return (T)this.DeserializeFromFile(filePath, encoding);
        }

        public object Deserialize(string value)
        {
            return this.Deserialize(value, Encoding.UTF8);
        }

        public object Deserialize(string value, System.Text.Encoding encoding)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("Value");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Position = 0;
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memoryStream);
            }
        }

        public object DeserializeFromFile(string filePath, System.Text.Encoding encoding)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                fileStream.Position = 0;
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(fileStream);
            }
        }
    }
}