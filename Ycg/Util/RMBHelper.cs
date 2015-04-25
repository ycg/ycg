//using System.Linq;
//using Ycg.Extension;

//namespace Ycg.Util
//{
//    public class RMBHelper
//    {
//        private static readonly string[] chineseNumbers = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
//        private static readonly string[] chinesePlaces = { "分", "角", "圆", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };

//        /// <summary>
//        /// 人民币数字转中文
//        /// </summary>
//        /// <param name="money"></param>
//        /// <returns></returns>
//        public static string ToChineseChars(double money)
//        {
//            string strMoney = (money * 100).ToString();
//            int valueLength = strMoney.Length;
//            string tempMoney = string.Empty;
//            string chineseChars = string.Empty;

//            for (int index = 0; index < valueLength; index++)
//            {
//                int number = strMoney.Substring(valueLength - index - 1, 1).ToInt16();
//                tempMoney += chinesePlaces[index] + chineseNumbers[number];
//            }

//            chineseChars = tempMoney.Aggregate(chineseChars, (current, a) => a + current);
//            return Replace(chineseChars);
//        }

//        private static string Replace(string chineseChars)
//        {
//            chineseChars = chineseChars.Replace("零仟零佰零拾零圆", "圆");
//            chineseChars = chineseChars.Replace("零仟零佰零拾零万", "万");
//            chineseChars = chineseChars.Replace("零零", "零");
//            chineseChars = chineseChars.Replace("零圆", "圆");
//            chineseChars = chineseChars.Replace("零拾", "零");
//            chineseChars = chineseChars.Replace("零佰", "零");
//            chineseChars = chineseChars.Replace("零仟", "零");
//            chineseChars = chineseChars.Replace("零万", "万");
//            chineseChars = chineseChars.Replace("零亿", "亿");
//            chineseChars = chineseChars.Replace("零角零分", "整");
//            chineseChars = chineseChars.Replace("零分", "");
//            chineseChars = chineseChars.Replace("零角", "");
//            return chineseChars;
//        }
//    }
//}
