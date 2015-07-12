using System;
using System.Collections.Generic;
using System.Text;

namespace RandomMori
{
    public class Datacell
    {
        // 构造函数
        public Datacell(List<double> paraFeature, int paraTag, int paraIdx)
        {
            attributes = paraFeature;
            aTag = paraTag;
            dataIdx = paraIdx;
        }
        // 特征向量
        public List<double> attributes = new List<double>();
        // 真实值
        public int aTag = -1;
        // 序号
        public int dataIdx = -1;
    }
}
