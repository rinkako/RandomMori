using System.Collections.Generic;

namespace RandomMori.MoriMana
{
	/// <summary>
	/// 树的节点类
	/// </summary>
    public class Node
    {
		/// <summary>
		/// 构造器
		/// </summary>
        public Node()
        {
            LeftChild = RightChild = null;
        }
		
		/// <summary>
		/// 此节点的分割属性在属性向量的下标
		/// </summary>
		public int CataNum;

		/// <summary>
		/// 分割阈值
		/// </summary>
		public double SplitThreshold;
		
		/// <summary>
		/// 此节点的预测判定
		/// </summary>
		public int PreditOutput;
		
		/// <summary>
		/// 左子树
		/// </summary>
		public Node LeftChild;

		/// <summary>
		/// 右子树
		/// </summary>
		public Node RightChild;
    }

	/// <summary>
	/// 样本的预测包装节点类
	/// </summary>
    public class dashNode
    {
		/// <summary>
		/// 构造器
		/// </summary>
		/// <param name="ac">真实类标</param>
		/// <param name="f">特征向量</param>
        public dashNode(int ac, List<double> f)
        {
            TrueLabel = ac;
            Feature = f;
        }

		/// <summary>
		/// 真实类标
		/// </summary>
		public int TrueLabel;
		
		/// <summary>
		/// 特征向量
		/// </summary>
        public List<double> Feature = new List<double>();
    }

	/// <summary>
	/// 迭代式计算节点类
	/// </summary>
    public class iterationCell
    {
		/// <summary>
		/// 构造器
		/// </summary>
		/// <param name="node">树节点</param>
		/// <param name="openlist">开列表</param>
		/// <param name="cf">特征向量</param>
		/// <param name="lv">高度</param>
        public iterationCell(Node node, List<int> openlist, List<int> cf, int lv)
        {
            BindingNode = node;
            Visit = openlist;
            Classified = cf;
            Level = lv;
        }
		/// <summary>
		/// 对应的树节点
		/// </summary>
        public Node BindingNode;

		/// <summary>
		/// 开列表
		/// </summary>
        public List<int> Visit;

		/// <summary>
		/// 已分类列表
		/// </summary>
        public List<int> Classified;

		/// <summary>
		/// 当前高度
		/// </summary>
        public int Level;
    }
}
