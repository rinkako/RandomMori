using System;
using System.Collections.Generic;
using System.Text;

namespace RandomMori
{
	/// <summary>
	/// 样本数据包装类
	/// </summary>
    public class Datacell
    {
		/// <summary>
		/// 构造器
		/// </summary>
		/// <param name="paraFeature">特征向量</param>
		/// <param name="paraTag">类标</param>
		/// <param name="paraIdx">样本序号</param>
        public Datacell(List<double> paraFeature, int paraTag, int paraIdx)
        {
            Attributes = paraFeature;
            Label = paraTag;
            SampleId = paraIdx;
        }

		/// <summary>
		/// 特征向量
		/// </summary>
		public List<double> Attributes = new List<double>();

		/// <summary>
		/// 类标
		/// </summary>
        public int Label = -1;

		/// <summary>
		/// 样本编号
		/// </summary>
        public int SampleId = -1;
    }
}
