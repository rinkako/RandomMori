using System;
using System.Collections.Generic;
using System.Linq;
using RandomMori.Base;

namespace RandomMori.MoriMana
{
	/// <summary>
	/// CART树类，维护一棵树的生命周期
	/// </summary>
    public class CART
    {
		/// <summary>
		/// 构造器
		/// </summary>
		/// <param name="s">尺寸系数</param>
		public CART(int s)
        {
            rootNode = new Node();
            scale = s;
            nodeNum = 1;
            data = DataMana.DataManager.GetInstance().GetTrainSet();
        }
		
		/// <summary>
		/// 生长这棵树直到收敛
		/// </summary>
		/// <param name="sizeofSample">样本尺寸</param>
		/// <param name="sizeofFeature">单样本的维度尺寸</param>
		/// <param name="treenumber">这棵树的编号</param>
		/// <param name="pt">构造方式：迭代或递归</param>
		public void Grow(int sizeofSample, int sizeofFeature, int treenumber, DashType pt)
        {
            List<int> visit = new List<int>(Base.CONSTA.AttriNum);
            List<int> classified = new List<int>(scale);
            DataMana.Sampler.BootstrapOpenlist(visit, sizeofFeature, 0, Base.CONSTA.AttriNum);
            DataMana.Sampler.BootstrapOpenlist(classified, sizeofSample, 0, Base.CONSTA.SampleNum);
            if (pt == DashType.iterative)
            {
                IterativeBuild(visit, classified, 0);
            }
            else
            {
                RecursiveBuild(rootNode, visit, classified, 0);
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("> builded tree " + treenumber + ", At depth: " + mostdepth + " with " + nodeNum + " nodes.");
            Console.ResetColor();
        }

		/// <summary>
		/// 预测一个样本
		/// </summary>
		/// <param name="node">由样本构造出来的预测包装节点</param>
		/// <param name="pt">预测的方式：迭代或递归</param>
		/// <returns>预测的类标</returns>
		public int Predict(dashNode node, DashType pt)
		{
			return pt == DashType.iterative ? IterativePredict(node) : RecursivePredict(node, rootNode);
		}

		/// <summary>
		/// 焚烧毁灭这棵树
		/// </summary>
		private void Burn()
        {
            rootNode = new Node();
        }
		
		/// <summary>
		/// 计算基尼系数
		/// </summary>
		/// <param name="threshold">分裂的阈值</param>
		/// <param name="used">特征脏位向量</param>
		/// <param name="feature">要计算的特征在特征向量的下标</param>
		/// <returns>该特征在当前集合的基尼系数</returns>
		private double GetGini(double threshold, List<int> used, int feature)
        {
            int p1 = 0;
            int[] count1 = new int[CONSTA.ClassNum];
            int p2 = 0;
            int[] count2 = new int[CONSTA.ClassNum];
            int sum = 0;
            for (int i = 0; i < CONSTA.ClassNum; i ++) {
                count1[i] = 0;
                count2[i] = 0;
            }
            for (int i = 0; i < scale; i++)
            {
                if (used[i] == 0)
                {
                    sum++;
                    if (data[i].Attributes[feature] < threshold)
                    {
                        p1++;
                        count1[data[i].Label]++;
                    }
                    else
                    {
                        p2++;
                        count2[data[i].Label]++;
                    }
                }
            }
            double s1 = 0;
            double s2 = 0;
            for (int i = 0; i < CONSTA.ClassNum; i++)
            {
                s1 += (double)count1[i] / p1 * (double)count1[i] / p1;
                s2 += (double)count2[i] / p2 * (double)count2[i] / p2;
            }
            return (double)p1 / sum * (1 - s1) + (double)p2 / sum * (1 - s2);
        }
		
		/// <summary>
		/// 递归测试
		/// </summary>
		/// <param name="node">预测包装节点</param>
		/// <param name="root">递归节点</param>
		/// <returns>预测类标</returns>
		private int RecursivePredict(dashNode node, Node root)
        {
            if (root.LeftChild == null && root.RightChild == null)
            {
                return root.PreditOutput;
            }
            else
            {
                int dashfeature = root.CataNum;
                if (node.Feature[dashfeature] < root.SplitThreshold)
                {
                    return RecursivePredict(node, root.LeftChild);
                }
                else
                {
                    return RecursivePredict(node, root.RightChild);
                }
            }
        }

		/// <summary>
		/// 迭代测试
		/// </summary>
		/// <param name="node">预测包装节点</param>
		/// <returns>预测类标</returns>
		private int IterativePredict(dashNode node)
        {
            int output = 0;
            Stack<Node> workStack = new Stack<Node>();
            workStack.Push(rootNode);
            while (workStack.Count != 0)
            {
                Node currentNode = workStack.Pop();
                if (currentNode.LeftChild == null && currentNode.RightChild == null)
                {
                    output = currentNode.PreditOutput;
                    break;
                }
                else
                {
                    int dashfeature = currentNode.CataNum;
                    if (node.Feature[dashfeature] < currentNode.SplitThreshold)
                    {
                        workStack.Push(currentNode.LeftChild);
                    }
                    else
                    {
                        workStack.Push(currentNode.RightChild);
                    }
                }
            }
            return output;
        }

		/// <summary>
		/// 递归生长树
		/// </summary>
		/// <param name="root">递归节点</param>
		/// <param name="visit">维度下标的脏位向量</param>
		/// <param name="classified">已分类向量</param>
		/// <param name="level">当前高度</param>
		private void RecursiveBuild(Node root, List<int> visit, List<int> classified, int level)
        {
            // 扫描是否还有未使用属性
            level++;
            int featureScanner = 0;
            for (featureScanner = 0; featureScanner < CONSTA.AttriNum; featureScanner++)
            {
                if (visit[featureScanner] == 0)
                    break;
            }
            // 如果所有属性都用完了就做判定并结束
            if (featureScanner == CONSTA.AttriNum)
            {
                root.LeftChild = null;
                root.RightChild = null;
                int[] num = new int[CONSTA.ClassNum];
                for (int i = 0; i < CONSTA.ClassNum; i++)
                {
                    num[i] = 0;
                }
                for (int i = 0; i < scale; i++) 
                {
                    if (classified[i] == 0)
                    {
                        num[data[i].Label]++;
                    }
                }
                int maxx = 0;
                int spec = 0;
                for (int i = 0;i < CONSTA.ClassNum;i ++) {
                    if (num[i] > maxx){
                        maxx = num[i];
                        spec = i;
                    }
                }
                root.PreditOutput = spec;
                if (level > mostdepth)
                {
                    mostdepth = level;
                }
                return;
            }
            int dataScanner = 0;
            for (int j = 0; j < scale; j++)
            {
                if (classified[j] == 0) { dataScanner++; }
            }
            // 如果数据集小于10或者深度大于20时就做判定并结束，避免过拟合
            if (dataScanner <= 10 || level > 18) {
                root.LeftChild = null;
                root.RightChild = null;
                int[] num = new int[CONSTA.ClassNum];
                for (int i = 0; i < CONSTA.ClassNum; i++) { num[i] = 0; }
                for (int i = 0; i < scale; i ++) 
                {
                    if (classified[i] == 0)
                        num[data[i].Label] ++;
                }
                int maxx = 0;
                int spec = 0;
                for (int i = 0; i < CONSTA.ClassNum; i++)
                {
                    if (num[i] > maxx){
                        maxx = num[i];
                        spec = i;
                    }
                }
                root.PreditOutput = spec;
                if (level > mostdepth)
                {
                    mostdepth = level;
                }
                return;
            }
            // 计算最小基尼
            double splitValue = 0;
            int splitFeature = -1;
            double minGini = 999999;
            for (int i = 0; i < CONSTA.AttriNum; i++)
            {
                if (visit[i] == 0)
                {
                    double maxer = -9999.0, miner = 9999.0;
                    for (int j = 0; j < CONSTA.SampleNum; j++)
                    {
                        if (classified[j] == 1) { continue; }
                        if (data[j].Attributes[i] > maxer) maxer = data[j].Attributes[i];
                        else if (data[j].Attributes[i] < miner) miner = data[j].Attributes[i];
                    }
                    for (int c = 0; c < 50; c++)
                    {
                        double tempSplitor = miner + (maxer - miner) / 50 * c;
                        double tempEn = GetGini(tempSplitor, classified, i);
                        if (tempEn < minGini)
                        {
                            minGini = tempEn;
                            splitValue = tempSplitor;
                            splitFeature = i;
                        }
                    }
                }
            }
            // 已经没法分裂了
            if (splitFeature == -1) 
            {
                root.LeftChild = null;
                root.RightChild = null;
                int[] num = new int[CONSTA.ClassNum];
                for (int i = 0; i < CONSTA.ClassNum; i++)
                {
                    num[i] = 0;
                }
                for (int i = 0;i < scale;i ++) {
                    if (classified[i] == 0)
                    {
                        num[data[i].Label]++;
                    }
                }
                int maxx = 0;
                int spec = 0;
                for (int i = 0; i < CONSTA.ClassNum; i++)
                {
                    if (num[i] > maxx) {
                        maxx = num[i];
                        spec = i;
                    }
                }
                root.PreditOutput = spec;
                if (level > mostdepth)
                {
                    mostdepth = level;
                }
                return;
            }
            // 分裂数据到两个女儿节点
            visit[splitFeature] = 1;
            root.CataNum = splitFeature;
            root.SplitThreshold = splitValue;
            List<int> newVisit = new List<int>(CONSTA.AttriNum);
            List<int> newClassified = new List<int>(scale);
            for (int i = 0; i < CONSTA.AttriNum; i++)
            { 
                newVisit.Add(visit[i]);
            }
            for (int i = 0; i < scale; i++)
            {
                newClassified.Add(classified[i]);
            }
            for (int i = 0; i < scale; i++)
            {
                if (classified[i] == 0)
                {
                    if (data[i].Attributes[splitFeature] < splitValue)
                    {
                        classified[i] = 0;
                        newClassified[i] = 1;
                    }
                    else
                    {
                        classified[i] = 1;
                        newClassified[i] = 0;
                    }
                }
            }
            // 构造女儿节点们
            root.LeftChild = new Node();
            root.RightChild = new Node();
            nodeNum += 2;
            RecursiveBuild(root.LeftChild, visit, classified, level);
            RecursiveBuild(root.RightChild, newVisit, newClassified, level);
            return;
        }

		/// <summary>
		/// 迭代生长树
		/// </summary>
		/// <param name="avisit">维度下标的脏位向量</param>
		/// <param name="aclassified">已分类向量</param>
		/// <param name="alevel">当前高度</param>
		private void IterativeBuild(List<int> avisit, List<int> aclassified, int alevel)
        {
            mostdepth = 1;
            Stack<iterationCell> workStack = new Stack<iterationCell>();
            workStack.Push(new iterationCell(rootNode, avisit, aclassified, alevel));
            while (workStack.Count != 0)
            {
                // 迭代数据
                iterationCell currentCell = workStack.Pop();
                Node currentNode = currentCell.BindingNode;
                List<int> visit = currentCell.Visit;
                List<int> classified = currentCell.Classified;
                int level = currentCell.Level;
                level++;
                // 如果80%已经是该分类，那就立即判定
                List<int> voteSpace = new List<int>(CONSTA.ClassNum);
                int clearCounter = 0;
                for (int i = 0; i < 26; i++) { voteSpace.Add(0); }
                for (int i = 0; i < scale; i++)
                {
                    if (classified[i] == 1) { continue; }
                    voteSpace[data[i].Label]++;
                    clearCounter++;
                }
                int vm = voteSpace.Max();
                if (vm > clearCounter * 0.8)
                {
                    currentNode.LeftChild = null;
                    currentNode.RightChild = null;
                    currentNode.PreditOutput = voteSpace.IndexOf(vm);
                    if (level > mostdepth)
                    {
                        mostdepth = level;
                    }
                    continue;
                }
                // 扫描是否还有未使用属性
                if (visit.TrueForAll((x) => { return x == 1; }))
                {
                    currentNode.LeftChild = null;
                    currentNode.RightChild = null;
                    int[] num = new int[CONSTA.ClassNum];
                    for (int i = 0; i < CONSTA.ClassNum; i++)
                    {
                        num[i] = 0;
                    }
                    for (int i = 0; i < scale; i++)
                    {
                        if (classified[i] == 0)
                        {
                            num[data[i].Label]++;
                        }
                    }
                    int maxx = 0;
                    int spec = 0;
                    for (int i = 0; i < CONSTA.ClassNum; i++)
                    {
                        if (num[i] > maxx)
                        {
                            maxx = num[i];
                            spec = i;
                        }
                    }
                    currentNode.PreditOutput = spec;
                    if (level > mostdepth)
                    {
                        mostdepth = level;
                    }
                    continue;
                }
                if (classified.Count((x) => { return x == 0; }) <= 10 || level > 18)
                {
                    currentNode.LeftChild = null;
                    currentNode.RightChild = null;
                    int[] num = new int[CONSTA.ClassNum];
                    for (int i = 0; i < CONSTA.ClassNum; i++) { num[i] = 0; }
                    for (int i = 0; i < scale; i++)
                    {
                        if (classified[i] == 0)
                            num[data[i].Label]++;
                    }
                    int maxx = 0;
                    int spec = 0;
                    for (int i = 0; i < CONSTA.ClassNum; i++)
                    {
                        if (num[i] > maxx)
                        {
                            maxx = num[i];
                            spec = i;
                        }
                    }
                    currentNode.PreditOutput = spec;
                    if (level > mostdepth)
                    {
                        mostdepth = level;
                    }
                    continue;
                }
                // 计算最小基尼
                double splitValue = 0;
                int splitFeature = -1;
                double minGini = 999999;
                for (int i = 0; i < CONSTA.AttriNum; i++)
                {
                    if (visit[i] == 0)
                    {
                        double maxer = -9999.0, miner = 9999.0;
                        for (int j = 0; j < CONSTA.SampleNum; j++)
                        {
                            if (classified[j] == 1) { continue; }
                            if (data[j].Attributes[i] > maxer) maxer = data[j].Attributes[i];
                            else if (data[j].Attributes[i] < miner) miner = data[j].Attributes[i];
                        }
                        for (int c = 0; c < 50; c++)
                        {
                            double tempSplitor = miner + (maxer - miner) / 50 * c;
                            double tempEn = GetGini(tempSplitor, classified, i);
                            if (tempEn < minGini)
                            {
                                minGini = tempEn;
                                splitValue = tempSplitor;
                                splitFeature = i;
                            }
                        }
                    }
                }
                // 已经没法分裂了
                if (splitFeature == -1)
                {
                    currentNode.LeftChild = null;
                    currentNode.RightChild = null;
                    int[] num = new int[CONSTA.ClassNum];
                    for (int i = 0; i < CONSTA.ClassNum; i++)
                    {
                        num[i] = 0;
                    }
                    for (int i = 0; i < scale; i++)
                    {
                        if (classified[i] == 0)
                        {
                            num[data[i].Label]++;
                        }
                    }
                    int maxx = 0;
                    int spec = 0;
                    for (int i = 0; i < CONSTA.ClassNum; i++)
                    {
                        if (num[i] > maxx)
                        {
                            maxx = num[i];
                            spec = i;
                        }
                    }
                    currentNode.PreditOutput = spec;
                    if (level > mostdepth)
                    {
                        mostdepth = level;
                    }
                    continue;
                }
                // 分裂数据到两个女儿节点
                visit[splitFeature] = 1;
                currentNode.CataNum = splitFeature;
                currentNode.SplitThreshold = splitValue;
                List<int> newVisit = new List<int>(CONSTA.AttriNum);
                List<int> newClassified = new List<int>(scale);
                for (int i = 0; i < CONSTA.AttriNum; i++)
                {
                    newVisit.Add(visit[i]);
                }
                for (int i = 0; i < scale; i++)
                {
                    newClassified.Add(classified[i]);
                }
                for (int i = 0; i < scale; i++)
                {
                    if (classified[i] == 0)
                    {
                        if (data[i].Attributes[splitFeature] < splitValue)
                        {
                            classified[i] = 0;
                            newClassified[i] = 1;
                        }
                        else
                        {
                            classified[i] = 1;
                            newClassified[i] = 0;
                        }
                    }
                }
                // 构造女儿节点
                currentNode.LeftChild = new Node();
                currentNode.RightChild = new Node();
                nodeNum += 2;
                workStack.Push(new iterationCell(currentNode.LeftChild, visit, classified, level));
                workStack.Push(new iterationCell(currentNode.RightChild, newVisit, newClassified, level));
            }
        }

		/// <summary>
		/// 树的根节点
		/// </summary>
		private Node rootNode;

		/// <summary>
		/// 数据规模
		/// </summary>
		private int scale;

		/// <summary>
		/// 数据集
		/// </summary>
		private List<Datacell> data = null;

		/// <summary>
		/// 最大深度
		/// </summary>
		private int mostdepth = 1;

		/// <summary>
		/// 节点总数
		/// </summary>
		private int nodeNum;
	}
}
