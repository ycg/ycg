using System.Text;

namespace Ycg.Serialization
{
    public interface ISerializer
    {
        string Serialize(object obj);

        string Serialize(object obj, Encoding encoding);

        void SerializeToFile(object obj, string filePath);

        void SerializeToFile(object obj, string filePath, Encoding encoding);

        T Deserialize<T>(string value);
        
        T Deserialize<T>(string value, Encoding encoding);

        T DeserializeFromFile<T>(string filePath);

        T DeserializeFromFile<T>(string filePath, Encoding encoding);

        object Deserialize(string value);

        object Deserialize(string value, Encoding encoding);

        object DeserializeFromFile(string filePath, Encoding encoding);
    }
}
