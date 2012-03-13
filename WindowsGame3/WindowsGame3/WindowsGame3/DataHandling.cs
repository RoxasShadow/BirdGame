using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace WindowsGame3 {
    class DataHandling {
        public static bool Save(string filename, object data) {
            FileStream stream = File.Open(filename, FileMode.OpenOrCreate);
            new BinaryFormatter().Serialize(stream, data);
            stream.Close();
            return File.Exists(filename);
        }

        public static Object Load(string filename) {
            FileStream stream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read);
            Object data = new BinaryFormatter().Deserialize(stream);
            stream.Close();
            return data;
        }

        public static bool Delete(string filename) {
            if(File.Exists(filename))
                File.Delete(filename);
            return File.Exists(filename);
        }

        public static bool Exists(string filename) {
            return File.Exists(filename);
        }
    }
}
