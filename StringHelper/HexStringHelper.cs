using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringHelper
{
    public class HexStringHelper
    {
        /// <summary>
        /// 将Byte数组转换为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="withBackspace">字节间是否包含空格</param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes, bool withBackspace = true)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (withBackspace)
                        sb.Append(bytes[i].ToString("X2") + " ");
                    else
                        sb.Append(bytes[i].ToString("X2"));
                }
                hexString = sb.ToString();
            }
            return hexString;
        }

        /// <summary>
        /// 将字符串转化为Byte数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string text)
        {
            text = text.Replace(" ", "").ToUpper();
            if ((text.Length > 1) && RegexHelper.IsMatch(text, "^[0-9A-F]$"))
            {
                byte[] bytes = new byte[text.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = (Convert.ToByte(text.Substring(i * 2, 2), 16));
                }
                return bytes;
            }
            else
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// 将给定的字符串按字节反转
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string ReverseStr(string text)
        {
            if (text.Length % 2 != 0) text = "0" + text;
            string tmp = "";
            for (int i = text.Length - 2; i >= 0; i = i - 2)
            {
                tmp += text.Substring(i, 2);
            }
            return tmp;
        }
    }
}
