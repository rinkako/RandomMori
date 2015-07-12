using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RandomMori.Base;
using RandomMori.DataMana;
using RandomMori.MoriMana;

namespace RandomMori
{
    class MoriDriver
    {
        // 私有构造器
        private MoriDriver()
        {
            //
        }

        // 工厂
        public static MoriDriver getInstance()
        {
            return syncObject;
        }

        // 异步生成森林
        public void dashInPara()
        {
            DateTime dt = DateTime.Now;
            Mori m = new Mori();
            m.grow(CONSTA.Treenum, CONSTA.Thread);
            double acc = m.classify(TestType.train);
            m.classify(TestType.test);
            double cost = (DateTime.Now - dt).TotalSeconds;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Acc: {0} / {1}", (1 - acc).ToString("0.0000"), acc.ToString("0.0000"));
            Console.WriteLine("Cost: {0}s, Dash type: {1}", cost.ToString("0.00"), CONSTA.dt.ToString());
            Console.ResetColor();
        }

        // 同步生成森林
        public void dashInSerial()
        {
            CONSTA.Thread = 1;
            this.dashInPara();
        }

        // 训练集
        public List<Datacell> trainSet = new List<Datacell>();
        // 唯一实例
        private static readonly MoriDriver syncObject = new MoriDriver();
    }
}
