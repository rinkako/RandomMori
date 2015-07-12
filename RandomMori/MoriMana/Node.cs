using System;
using System.Collections.Generic;
using System.Text;

namespace RandomMori.MoriMana
{
    public class Node
    {
        public Node()
        {
            left = right = null;
        }
        // 此节点的分割熟悉
        public int cataNum;
        // 分割阈值
        public double threshold;
        // 此节点的判定
        public int output;
        // 左子树
        public Node left;
        // 右子树
        public Node right;
    }

    public class dashNode
    {
        public dashNode(int ac, List<double> f)
        {
            acceptTag = ac;
            feature = f;
        }
        // 真实值
        public int acceptTag;
        // 测试属性集
        public List<double> feature = new List<double>();
    }

    public class iterationCell
    {
        public iterationCell(Node node, List<int> openlist, List<int> cf, int lv)
        {
            myNode = node;
            visit = openlist;
            classified = cf;
            level = lv;
        }
        public Node myNode;
        public List<int> visit;
        public List<int> classified;
        public int level;
    }
}
