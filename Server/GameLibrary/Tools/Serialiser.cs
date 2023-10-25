using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace GameLibrary.Tools
{
    public static class Serializer
    {
        public static async Task<byte[]> SerializeAsync<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    serializer.WriteObject(memoryStream, obj);
                    await memoryStream.FlushAsync();

                    if (memoryStream.Length > 0)
                    {
                        return memoryStream.ToArray();
                    }
                    else
                    {
                        throw new InvalidOperationException("Serialization failed: Empty data");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Serialization error: {ex.Message}");
                throw;
            }
        }

        public static async Task<T> DeserializeAsync<T>(byte[] data)
        {
            try
            {
                if (data == null || data.Length == 0)
                {
                    throw new ArgumentException("Invalid data");
                }

                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(memoryStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                throw;
            }
        }
    }
}


