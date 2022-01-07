using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

public class ToolsManager
{
    static MethodInfo SerializeEntityMethod = typeof(ToolsManager).GetMethod("SerializeEntityToList");
    static MethodInfo DeserializeEntityMethod = typeof(ToolsManager).GetMethod("DeserializeEntityList");

    static MethodInfo SerializeBytesMethod = typeof(ToolsManager).GetMethod("Serialize");
    static MethodInfo DeserializeBytesMethod = typeof(ToolsManager).GetMethod("Deserialize");

    static Action<string> Logger;

    public static int ParseInt32(string str, int defaultValue = -1)
    {
        int result;
        return Int32.TryParse(str, out result) ? result : defaultValue;
    }
    public static T Deserialize<T>(byte[] param)
    {
        using (MemoryStream ms = new MemoryStream(param))
        {
            IFormatter br = new BinaryFormatter();
            return (T)br.Deserialize(ms);
        }
    }

    public static T Serialize<T>(byte[] param)
    {
        using (MemoryStream ms = new MemoryStream(param))
        {
            IFormatter br = new BinaryFormatter();
            return (T)br.Deserialize(ms);
        }
    }

}
