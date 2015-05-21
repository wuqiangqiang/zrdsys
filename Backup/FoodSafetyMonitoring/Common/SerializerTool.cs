using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FoodSafetyMonitoring.Common
{
    /// <summary>
    /// 序列化和反序列化工具
    /// </summary>
    public class SerializerTool
    {
        //将类型序列化为文件
        public static void SerializeToFile<T>(T t, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, t);
                fs.Flush();
            }
        }


        //将文件反序列化为类型
        public static T DeserializeFromFile<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default(T);
            }
            else
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        if (fs.Length != 0)
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            return (T)formatter.Deserialize(fs);
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                }
                catch (Exception e)
                {
                    return default(T);
                }

            }
        }

        //将文件反序列化为类型
        public static T DeserializeFromFile<T>(FileStream fs)
        {

            BinaryFormatter formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(fs);


        }
    }
}
