using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace IO
{
    public class Serializer : MonoBehaviour
    {
        private static readonly string SaveDir = Application.persistentDataPath + "/saves";

        public static bool Save(string saveName, object saveData)
        {
            if (!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
            string path = SaveDir + "/" + saveName + ".save";

            FileStream file = File.Create(path);

            try
            {
                BinaryFormatter formatter = GetBinaryFormatter();
                formatter.Serialize(file, saveData);
                file.Close();
                return true;
            }
            catch
            {
                file.Close();
                File.Delete(SaveDir + "/" + saveName + ".save");
                return false;
            }
        }

        public static object Load(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            BinaryFormatter formatter = GetBinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            try
            {
                object save = formatter.Deserialize(file);
                file.Close();
                return save;
            }
            catch
            {
                Debug.LogErrorFormat($"Failed to load file at {path}");
                file.Close();
                return null;
            }
        }
        
        //todo the BinaryFormatter is insecure. Might change this if I have enough time for the project
        private static BinaryFormatter GetBinaryFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SurrogateSelector selector = new SurrogateSelector();
            
            Vector3SerializationSurrogate vector3SerializationSurrogate = new Vector3SerializationSurrogate();
            
            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SerializationSurrogate);

            formatter.SurrogateSelector = selector;
            
            return formatter;
        }
    }
}
