using System;
using System.Collections.Generic;
using RandomMori.Base;
using RandomMori.MoriMana;

namespace RandomMori
{
	/// <summary>
	/// 控制器类：负责前端与后台的交互
	/// </summary>
    class MoriDriver
    {
		/// <summary>
		/// 私有构造器
		/// </summary>
		private MoriDriver() { }
		
		/// <summary>
		/// 工厂方法：获得类的唯一实例
		/// </summary>
		/// <returns>控制器唯一实例</returns>
        public static MoriDriver getInstance()
        {
            return MoriDriver.syncObject;
        }
		
		/// <summary>
		/// 异步生成森林
		/// </summary>
		public void DashInPara()
        {
            DateTime dt = DateTime.Now;
            Mori m = new Mori();
            m.GrowAsync(CONSTA.Treenum, CONSTA.Thread);
            double acc = m.Classify(TestType.train);
            //m.Classify(TestType.test);
            double cost = (DateTime.Now - dt).TotalSeconds;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Acc: {0} / {1}", (1 - acc).ToString("0.0000"), acc.ToString("0.0000"));
            Console.WriteLine("Cost: {0}s, Dash type: {1}", cost.ToString("0.00"), CONSTA.dt.ToString());
            Console.ResetColor();
        }
		
		/// <summary>
		/// 同步生成森林
		/// </summary>
		public void DashInSerial()
        {
            CONSTA.Thread = 1;
            this.DashInPara();
        }
		
		/// <summary>
		/// 训练集向量
		/// </summary>
        public List<Datacell> trainSet = new List<Datacell>();

		/// <summary>
		/// 唯一实例
		/// </summary>
		private static readonly MoriDriver syncObject = new MoriDriver();
    }
}
