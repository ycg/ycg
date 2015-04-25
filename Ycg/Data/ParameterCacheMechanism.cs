using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

/*
 
 * 2013.12.16
 * 
 *      Parameter有这几个功能
 *      1.克隆Parameter实例
 *      2.存储一定的Parameter实例到内存中去
 *      3.通过Key从内存中获取相应的Parameter实例
 *      
 * 2013.12.18
 * 
 *      Parameter的可以是以ConnectionString + CommandText为Key的
 *      
 * 2013.12.22
 * 
 *      1. 终于Parameter的缓存机制有了一定的认识了
 *      
 * 2013.12.23
 * 
 *      1. 这里的参数缓存只是针对存储过程的缓存
 *      2. 但是有一个还没搞懂的问题：我传过来的值怎么跟缓存里面的对象一一对应的
 *      
 * 2013.12.24
 * 
 *      这边为什么要Clone一下呢？一开始我也没想的通
         不过当我遇到一个"当前的Parameter已经在另一个ParameterCollection中被引用了"异常的时候，想通了
         也就是说我们必须Clone一个新的Parameter
         如果不Clone就必须保证线程同步才行
 * 
 *      ★
 *      其实数据库参数缓存在性能上一定不会提升多少。
 *      
 *      最终重要的是它使得用户调用变的简单，用户只要知道存储过程名和对应的参数值就可以了，并不需要参数的类型，长度，返回类型等等
 *      所以我认为这个才是数据库参数缓存的精华所在
 *      
 *      睡觉了，现在已经是 0：49了，明天TMD又要困了，艹
 *      主要是今晚脑子涌现了很多思想，明天继续加油了，o(∩_∩)o 哈哈
 *      
 * 2013.12.29
 * 
 *      1. 这边存在一个很严重的Bug
 *      2. 当定义一个存储过程的时候，只有返回参数，没有其它的，就会出现问题
 *      3. 解决办法：通过判断参数的返回类型来做
 *      
 *      对参数的返回状态要有个清除的认识：
 *       成员名称 说明  
 *       ----------------------------------------
 *       ① Input 参数是输入参数。 
 *       ② Output 参数是输出参数。 
 *       ③ InputOutput 参数既能输入，也能输出。  
 *       ④ ReturnValue 参数表示诸如存储过程、内置函数或用户定义函数之类的操作的返回值。 
 *       
 *      .Net中的参数定义为形式参数 而把存储过程的参数定义为实际参数
    
        数据库存储过程的实际参数如果没有默认值则形式参数必须传值给实际参数

        但是如果形式参数的类型为ParameterDirection.Output 则传给实际参数的永远是空值

        果形式参数的类型为ParameterDirection.ReturnValue 则形式参数不会传值给实际参数 实际参数必须有默认值  否则代码会报错

        如果形式参数类型为ParameterDirection.InputOutput 或者 ParameterDirection.Output 则实际参数必须有output 关键字

        另外需要注意的是在.net中 System.DBNull.Value表示数据库参数为空值 而不是null
 * 
 *      cmd.Parameters.Add(
            new SqlParameter("ReturnValue", SqlDbType.Int, 4,
            ParameterDirection.ReturnValue, false, 0, 0,
            string.Empty, DataRowVersion.Default, null));
        这段代码也是加入参数，但是这个参数的类型是返回值类型（sql参数类型有Input Output InputOutput ReturnValue  四种类型，默认是input型的。）
 *      你也可以定义 output型的，
 *      但每个sql 执行完成后都会有一个returnvalue型的返回参数，这里new了一个sqlParameter("ReturnValue"参数的名称， SqlDbType.Int参数的类型整型， 
 *      4参数的大小4字节,
 *      ParameterDirection.ReturnValue参数的类型为ReturnValue型， false是否可空, 0小数点左右二侧的总位数, 0总小数位数，string.Empty 源列的名称, 
 *      DataRowVersion.Default 自己查，null一个 Object它是 SqlParameter 的值）

        如果你不需要 ReturnValue类型的参数，当然可去掉，一般我们都是不加这个参数的。包括微软自己的示例源码一般也不加。除非有用，但一般不用的
 * 
 */

namespace Ycg.Data
{
    public class ParameterCacheMechanism
    {
        private readonly Dictionary<string, IDataParameter[]> _parameterCache = new Dictionary<string, IDataParameter[]>();

        public void AssignParameterValues(DbCommand command, IList<ParameterMappingInfo> parameterNamesAndValues)
        {
            if (command.Parameters.Count > 0) command.Parameters.Clear();
            IDataParameter[] cloneParametersFromCache = this.GetParametersFromCache(command.Connection.ConnectionString, command.CommandText);

            for (int index = 0; index < cloneParametersFromCache.Length; index++)
            {
                cloneParametersFromCache[index].Value = this.GetParameterValue(cloneParametersFromCache[index].ParameterName, parameterNamesAndValues) ?? DBNull.Value;
                command.Parameters.Add(cloneParametersFromCache[index]);
            }
        }

        private object GetParameterValue(string parameterName, IList<ParameterMappingInfo> parameterNamesAndValues)
        {
            if (parameterNamesAndValues != null && parameterNamesAndValues.Count > 0)
            {
                foreach (ParameterMappingInfo parameterMappingInfo in parameterNamesAndValues)
                {
                    if (parameterName == parameterMappingInfo.ParameterName)
                    {
                        return parameterMappingInfo.ParameterValue;
                    }
                }
            }
            return null;
        }

        private IDataParameter[] GetParametersFromCache(string connectionString, string stroeProcedureName)
        {
            string key = this.CreateKey(connectionString, stroeProcedureName);
            IDataParameter[] parameters = this._parameterCache[key];
            return this.CloneParameters(parameters);
        }

        public void SetParametersToCache(DbCommand command)
        {
            IDataParameter[] parameters = new IDataParameter[command.Parameters.Count];
            for (int index = 0; index < command.Parameters.Count; index++)
            {
                parameters[index] = this.CloneParameter(command.Parameters[index]);
            }
            this.SetParametersToCache(command.Connection.ConnectionString, command.CommandText, parameters);
        }

        private void SetParametersToCache(string connectionString, string stroeProcedureName, params IDataParameter[] parameters)
        {
            string key = this.CreateKey(connectionString, stroeProcedureName);
            this._parameterCache.Add(key, parameters);
        }

        public bool HasParametersFromCache(DbCommand command)
        {
            string storedProcedureName = command.CommandText;
            string connectionString = DataBaseFactory.DBConfigurationInfo.ConnectionString; 
            return this.HasParametersFromCache(connectionString, storedProcedureName);
        }

        private bool HasParametersFromCache(string connectionString, string stroeProcedureName)
        {
            string key = this.CreateKey(connectionString, stroeProcedureName);
            return this._parameterCache.ContainsKey(key);
        }

        private string CreateKey(string connectionString, string commandText)
        {
            return connectionString + ":" + commandText;
        }

        #region Clone Parameters

        public IDataParameter CloneParameter(IDataParameter originalParameter)
        {
            return (IDataParameter)((ICloneable)originalParameter).Clone();
        }

        public IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
        {
            IDataParameter[] parameters = new IDataParameter[originalParameters.Length];

            for (int i = 0; i < originalParameters.Length; i++)
            {
                parameters[i] = this.CloneParameter(originalParameters[i]);
            }

            return parameters;
        }

        #endregion
    }
}
