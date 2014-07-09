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

namespace Pumgrana
{
    public static class IsolatedStorageOperations
    {
        public static string InputCacheName = "List_Cache_Files";
        public static string CacheDirName = "Pumgrana_Cache";
        public static void Save<T>(this T obj, string file)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = null;
            List<string> list = IsolatedStorageSettings.ApplicationSettings[InputCacheName] as List<string>;
            try
            {
                stream = storage.OpenFile(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Write);
                if (list.Contains(stream.Name) == false)
                {
                        list.Add(stream.Name);
                }
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
                stream.Close();
                storage.Dispose();
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException e)
            {
                System.Diagnostics.Debug.WriteLine("Error while writing the file " + file + ": " + e.Message);
                storage.DeleteFile(file);
            }
            finally
            {
                storage.Dispose();
                if (stream != null)
                {
                   stream.Close();
                }
            }
        }

        public static T Load<T>(string file)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            T obj = Activator.CreateInstance<T>();

            if (storage.FileExists(file))
            {
                System.Diagnostics.Debug.WriteLine("Load page " + file + " from local");
                IsolatedStorageFileStream stream = null;
                try
                {
                    stream = storage.OpenFile(file, FileMode.Open, FileAccess.Read);
                    XmlSerializer serializer = new XmlSerializer(typeof(T));

                    obj = (T)serializer.Deserialize(stream);
                }
                catch (Exception)
                {
                    return obj;
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                return obj;
            }
            System.Diagnostics.Debug.WriteLine("Load page " + file + " from web");
            return obj;
        }
    }
}
