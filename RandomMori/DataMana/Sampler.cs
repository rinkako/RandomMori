using System;
using System.Collections.Generic;
using System.Text;

namespace RandomMori.DataMana
{
    public class Sampler
    {
        // 构造器
        public Sampler()
        {
            //
        }

        // 获取随机值
        public static int getRandomInt(int min, int max)
        {
            return randomer.Next(min, max);
        }

        // 获得bootstrap的open列表
        public static void bootstrapOpenlist(List<int> openlist, int bootstraper, int minb = 0, int maxb = 0)
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

        private static Random randomer = new Random();
    }
}
