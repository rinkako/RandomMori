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
            attributes = iattr;
            klass = itag;
        }
        public List<double> attributes = new List<double>();
        public int klass = -1;
    }

    public class Moricell
    {
        // 构造函数
        public Moricell(List<double> subattr, int stag)
        {
            attributes = subattr;
            klass = stag;
        }
        public List<double> attributes = new List<double>();
        public int klass = -1;
    }

    public class MoriTreeNode
    {
        // 构造函数
        public MoriTreeNode(int attr, int val)
        {
            branchAttr = attr;
            attriveVal = val;
            staticstic[0] = staticstic[1] = 0;
        }
        // 划分属性
        public int branchAttr;
        // 到达该节点的属性值
        public int attriveVal;
        // 根据最佳属性的分支节点
        public List<MoriTreeNode> childs = new List<MoriTreeNode>();
        // 该分支下的决策结果，用于叶子节点
        public int decisionVal;
        // 统计该决策下的数据（民主派和共和派的个数）
        public int[] staticstic = new int[121];
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
