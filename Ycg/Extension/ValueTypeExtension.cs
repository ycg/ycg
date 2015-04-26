using System;
/*
 
 * 目标：逐渐形成一些列的扩展方法

    主要是通过个人积累以及网络资料形成自己的类库

    2013.11.30
	    1.值类型扩展方法

	    2.准备写一点反射Table变成List

	    心得：凡是都要对NULL进行检查

    2013.12.1
	    1.昨天反射枚举值还没有搞定，没有好的方案，正在寻找，可能需要问王力同学拿以前公司代码了

	    2.值类型扩展增加对Int16，Int32，Int64转化支持。

	    3.还有什么类型需要扩展吗？
		    ①DateTime
		    ②

	    4.加好每一个方法的注释，这是必须有的而且也一定要做，o(∩_∩)o 哈哈。

	    5.今天对三种序列化方式做了总结，有JSON，XML，Binary三种。
		    添加了对三种序列化方式的支持。

	    6.22:00 二进制反序列化还有问题，不写了，要睡觉，要保证睡眠，上床之前看会书。

    2013.12.2
	    1.今天把DataTable反射成List集合这个功能做好
		    就是遇到了转换枚举的问题

    2013.12.3
	    8:00 - 已经搞定DataRow反射成实体的需求，主要就是在反射枚举和Bool类型的时候遇到了一些困难。

	    今晚回家会继续补充这方面的功能

	    今天洗澡了，真好做了个足浴，`(*∩_∩*)′。

    2013.12.4

    2013.12.5
	    1.继续研究反射Table为实体

		    精简代码并且使得代码高效。

		    最后发现循环次数还是没有降低，不过在研究Emit了，哈哈，这几天继续研究反射DataTable为实体的代码。

		    现在发现写一个通过的框架还真的不容易啊，哈哈。

		    好了，明天继续研究Emit，并实现DataReader通过反射转化为实体的代码。

		    睡觉了，睡觉之前看会瓦尔登湖，嘿嘿。


    2013.12.9

	    终于搬到公司宿舍了，也总算把住的问题搞定了，接下来就会认真完善我的个人类库框架了，加油。

	    今天参照Enterprice Library学习了一点数据访问层的东西，明天再接再厉。

	    o(∩_∩)o 哈哈
	

    2013.12.10

	    今天稍微明白了微软的Enterprise Library关于数据访问层的知识，不过知识明白了一点点，我会继续努力的。

	    明天把Parameter的缓存机制看一下，这是比较重要的，明天继续加油咯。

	    今天学习了SqlClientFactory 和 DbProviderFactory 两个类的重要原理。

	    明天继续加油了。
 * 
 *  2014-2-7
 *      
 *      重新调整Project的布局
 
 */
using Ycg.Util;

namespace Ycg.Extension
{
    public static class ValueTypeExtension
    {
        public static bool ToBool(this object value)
        {
            bool result = default(bool);
            if (value.IsNotNull())
            {
                bool.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static byte[] ToBytes(this object value)
        {
            return (byte[])value;
        }

        public static Int16 ToInt16(this object value)
        {
            Int16 result = default(Int16);
            if (value.IsNotNull())
            {
                Int16.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static Int32 ToInt32(this object value)
        {
            Int32 result = default(Int32);
            if (value.IsNotNull())
            {
                Int32.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static Int64 ToInt64(this object value)
        {
            Int64 result = default(Int64);
            if (value.IsNotNull())
            {
                Int64.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static float ToFloat(this object value)
        {
            float result = default(float);
            if (value.IsNotNull())
            {
                float.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static double ToDouble(this object value)
        {
            double result = default(double);
            if (value.IsNotNull())
            {
                double.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static decimal ToDecimal(this object value)
        {
            decimal result = default(decimal);
            if (value.IsNotNull())
            {
                decimal.TryParse(value.ToString(), out result);
            }
            return result;
        }

        public static DateTime ToDateTime(this object value)
        {
            DateTime result = default(DateTime);
            if (value.IsNotNull())
            {
                DateTime.TryParse(value.ToString(), out result);
            }
            return result;
        }
    }
}