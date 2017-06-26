using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace CIRClient
{
    class FileHelper
    {
        // 从XML配置文件里读取某个key的value
        public static string GetValueFromConf(string fileName, string key)
        {
            XDocument conf = XDocument.Load(fileName);
            XElement element = conf.Root.Element(key);
            return element.Value;
        }


        // 自动根据文件大小转换单位
        public static string ConvertBytes(long len)
        {
            string[] sizes = { "Bytes", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }
            return String.Format("{0:0.##}{1}", len, sizes[order]);
        }

        // 计算文件MD5值  返回hex格式的32字节字符串
        public static string CalcFileMD5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    string hex = BitConverter.ToString(md5.ComputeHash(stream));
                    return hex.Replace("-", "").ToLowerInvariant();
                }
            }
        }

        // 计算字符串MD5值  返回hex格式的32字节字符串
        public static string CalcStringMD5(string str)
        {
            using (var md5 = MD5.Create())
            {
                string hex = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str)));
                return hex.Replace("-", "").ToLowerInvariant();
            }
        }

        // 将string转化为Bytes 前面四个字节是int 表示这个string的字节数
        public static byte[] getBytesOfString(String str)
        {
            Byte[] dataByte = Encoding.UTF8.GetBytes(str);
            int len = dataByte.Length;
            Byte[] lenByte = BitConverter.GetBytes(len);

            Byte[] totalByte = new Byte[dataByte.Length + lenByte.Length];
            Buffer.BlockCopy(lenByte, 0, totalByte, 0, lenByte.Length);
            Buffer.BlockCopy(dataByte, 0, totalByte, lenByte.Length, dataByte.Length);
            return totalByte;
        }

        // 将file转化为Bytes 前面四个字节是int 表示这个file的字节数
        public static byte[] getBytesOfFile(String fileName)
        {
            Byte[] dataByte = File.ReadAllBytes(fileName);
            int len = dataByte.Length;
            Byte[] lenByte = BitConverter.GetBytes(len);

            Byte[] totalByte = new Byte[dataByte.Length + lenByte.Length];
            Buffer.BlockCopy(lenByte, 0, totalByte, 0, lenByte.Length);
            Buffer.BlockCopy(dataByte, 0, totalByte, lenByte.Length, dataByte.Length);
            return totalByte;
        }

        // 将string和file转化为Bytes 前面四个字节是int 表示这个file和string的总字节数
        public static byte[] getBytesOfStringAndFile(String str, String fileName)
        {
            Byte[] strByte = Encoding.UTF8.GetBytes(str);
            Byte[] dataByte = File.ReadAllBytes(fileName);
            int len = dataByte.Length + strByte.Length;
            Byte[] lenByte = BitConverter.GetBytes(len);

            Byte[] totalByte = new Byte[strByte.Length + dataByte.Length + lenByte.Length];
            Buffer.BlockCopy(lenByte, 0, totalByte, 0, lenByte.Length);
            Buffer.BlockCopy(strByte, 0, totalByte, lenByte.Length, strByte.Length);
            Buffer.BlockCopy(dataByte, 0, totalByte, lenByte.Length + strByte.Length, dataByte.Length);
            return totalByte;
        }
    }
}
