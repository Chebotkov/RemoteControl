using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServerPart
{
    public static class DataBinary
    {
        public static byte[] GetBinaryRepresentation<T>(T obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                bytes = memoryStream.ToArray();
            }

            return bytes;
        }

        public static byte[] GetBinaryRepresentationWX<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, obj);
                bytes = memoryStream.ToArray();
            }

            return bytes;
        }

        

        public static T GetNormalRepresentation<T>(byte[] bytes)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static T GetNormalRepresentation<T>(byte[] bytes, int startIndex, int length)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(bytes, startIndex, length))
            {
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
