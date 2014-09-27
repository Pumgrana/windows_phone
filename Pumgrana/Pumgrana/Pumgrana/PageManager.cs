using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;
using Windows.Storage;

namespace Pumgrana
{
    public static class IsolatedStorageOperations
    {
        public static async void Save<T>(this T obj, string filename)
        {
            // Get the data to serialize
            byte[] data = Tools.GetBytesFromObject<T>(obj);

            // Get the local folder
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new file named DataFile.txt.
            var file = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            // Write the data from the textbox.
            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(data, 0, data.Length);
                s.Close();
            }            
        }

        public static async Task<T> Load<T>(string filename)
        {
            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (local != null)
            {
                string data;

                // Get the file.
                try
                {
                    var file = await local.OpenStreamForReadAsync(filename);

                    System.Diagnostics.Debug.WriteLine("Load " + filename + " from local");

                    // Read the data.
                    StreamReader sr = new StreamReader(file);
                    data = sr.ReadToEnd();
                    sr.Close();
                    var obj = (T)(Tools.GetObjectFromByteArray<T>(Tools.GetBytes(data)));
                    return obj;
                }
                catch (System.IO.FileNotFoundException e)
                {
                    System.Diagnostics.Debug.WriteLine("Load " + filename + " from web");
                    T voidobject = Activator.CreateInstance<T>();
                    return voidobject;
                }
            }
            T voidobject2 = Activator.CreateInstance<T>();
            return voidobject2;
        }
    }
}
