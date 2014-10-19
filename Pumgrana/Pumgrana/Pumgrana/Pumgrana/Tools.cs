using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pumgrana
{
    static class Tools
    {
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static byte[] GetBytesFromObject<T>(this object obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            byte[] byteArr;

            using (var ms = new System.IO.MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                byteArr = ms.ToArray();
            }
            return byteArr;
        }

        public static object GetObjectFromByteArray<T>(byte[] array)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            var obj = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(array))
            {
                obj = (T)serializer.ReadObject(ms);
            }
            return obj;
        }
    }
}
