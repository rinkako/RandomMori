using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomMori
{
    public class Datacell
    {
        // 构造函数
        public Datacell(List<double> iattr, int itag)
        {
            attribute = iattr;
            tag = itag;
        }
        public List<double> attribute = new List<double>();
        public int tag = -1;
    }

    public class Moricell
    {
        // 构造函数
        public Moricell(List<double> subattr, int stag)
        {
            subAttribute = subattr;
            tag = stag;
        }
        public List<double> subAttribute = new List<double>();
        public int tag = -1;
    }

    public class MoriTreeNode
    {
        // 构造函数
        public MoriTreeNode(List<Moricell> icellList, int isplitor, double ientropy)
        {
            cellList = icellList;
            splitor = isplitor;
            entropy = ientropy;
        }
        public List<Moricell> cellList = new List<Moricell>();
        public int splitor = -1;
        public double entropy = 0;
    }

    public class Mori
    {
        // 构造函数
        public Mori()
        {

        }
        // 追加一棵树
        public void addTree(MoriTreeNode tree)
        {
            mori.Add(tree);
        }
        // 清空森林
        public void clear()
        {
            mori.Clear();
        }
        // 数量
        public int count()
        {
            return mori.Count;
        }
        public List<MoriTreeNode> mori = new List<MoriTreeNode>();
    }
}
