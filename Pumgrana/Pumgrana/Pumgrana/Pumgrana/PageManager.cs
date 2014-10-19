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
        private static string FolderCacheName = "Pumgrana_Cache";

        public static void Save<T>(this T obj, string filename)
        {
            // Get the data to serialize
            byte[] data = Tools.GetBytesFromObject<T>(obj);

            // Get the local folder
            IsolatedStorageFile local = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                // Create a new file
                using (var fs = local.CreateFile(filename))
                {
                    if (fs != null)
                    {
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                        fs.Close();
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
            }
        }

        public static T Load<T>(string filename)
        {
            // Get the local folder.
            IsolatedStorageFile local = IsolatedStorageFile.GetUserStoreForApplication();

            if (local != null)
            {
                if (local.FileExists(filename) == true)
                {
                    System.Diagnostics.Debug.WriteLine("Load " + filename + " from local");

                    // Get the file.
                    using (var fs = local.OpenFile(filename, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data = new byte[fs.Length];
                        fs.Read(data, 0, data.Length);
                        using (var ms = new MemoryStream(data))
                        {
                            var dataobject = Activator.CreateInstance<T>();
                            System.Runtime.Serialization.DataContractSerializer ser = new System.Runtime.Serialization.DataContractSerializer(dataobject.GetType());
                            dataobject = (T)ser.ReadObject(ms);
                            return dataobject;
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Load " + filename + " from web");
                    T voidobject = Activator.CreateInstance<T>();
                    return voidobject;
                }
            }
            T voidobject2 = Activator.CreateInstance<T>();
            return voidobject2;
        }

       public static void ClearCache()
        {
            IsolatedStorageFile local = IsolatedStorageFile.GetUserStoreForApplication();

            string[] files = local.GetFileNames();
            foreach (string file in files)
            {
                if (local.FileExists(file))
                {
                    local.DeleteFile(file);
                    System.Diagnostics.Debug.WriteLine("File " + file + " deleted");
                }
            }
        }
    }
}
