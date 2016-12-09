using System;
using System.Collections.Generic;

namespace RandomMori.DataMana
{
	/// <summary>
	/// 取样器类
	/// </summary>
    public class Sampler
    {
		/// <summary>
		/// 构造器
		/// </summary>
		public Sampler() { }
		
		/// <summary>
		/// 获取一个范围随机整数值
		/// </summary>
		/// <param name="min">最小值</param>
		/// <param name="max">最大值</param>
		/// <returns>规定范围中的随机整数</returns>
		public static int getRandomInt(int min, int max)
        {
            return randomer.Next(min, max);
        }
		
		/// <summary>
		/// 获得Bootstrap的开列表
		/// </summary>
		/// <param name="openlist">开列表</param>
		/// <param name="bootstraper">挑选属性个数</param>
		/// <param name="minb">属性下标最小值</param>
		/// <param name="maxb">属性下标最大值</param>
		public static void BootstrapOpenlist(List<int> openlist, int bootstraper, int minb = 0, int maxb = 1)
        {
            for (int i = 0; i < maxb; i++)
            {
                openlist.Add(1);
            }
            for (int i = 0; i < bootstraper; )
            {
                int idx = DataMana.Sampler.getRandomInt(minb, maxb - 1);
                if (openlist[idx] == 1)
                {
                    openlist[idx] = 0;
                    i++;
                }
            }
        }

		/// <summary>
		/// 随机数生成器
		/// </summary>
        private static Random randomer = new Random();
    }
}
