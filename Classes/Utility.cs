using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ChatApp.Classes
{
    public class Utility
    {
        public static byte[] ObjectToBytes(object obj) {

            BinaryFormatter bF = new BinaryFormatter();

            using(MemoryStream mS = new MemoryStream())
            {
                bF.Serialize(mS, obj);
                return mS.ToArray();
            }
            
        }

        public static object BytesToObject(byte[] byteAtrray) {
            BinaryFormatter bF = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream()) 
            {
                ms.Write(byteAtrray, 0, byteAtrray.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return bF.Deserialize(ms);
            }
        }


    }
}
