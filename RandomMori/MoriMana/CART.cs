using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RandomMori.Base;

namespace RandomMori.MoriMana
{
    public class CART
    {
        // 根节点
        private Node _root;
        // 数据规模
        private int scale;
        // 数据集
        private List<Datacell> data = null;
        // 最大深度
        private int mostdepth = 1;
        // 节点总数
        private int nodeNum;
        
        // 构造器
        public CART(int s)
        {
            _root = new Node();
            scale = s;
            nodeNum = 1;
            data = DataMana.DataManager.getInstance().getTrainSet();
        }

        // 生长树接口
        public void grow(int sizeofSample, int sizeofFeature, int treenumber, DashType pt)
        {
            List<int> visit = new List<int>(Base.CONSTA.AttriNum);
            List<int> classified = new List<int>(scale);
            DataMana.Sampler.bootstrapOpenlist(visit, sizeofFeature, 0, Base.CONSTA.AttriNum);
            DataMana.Sampler.bootstrapOpenlist(classified, sizeofSample, 0, Base.CONSTA.SampleNum);
            if (pt == DashType.iterative)
            {
                iterativeBuild(visit, classified, 0);
            }
            else
            {
                recursiveBuild(_root, visit, classified, 0);
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("> builded tree " + treenumber + ", At depth: " + mostdepth + " with " + nodeNum + " nodes.");
            Console.ResetColor();
        }

        // 烧树
        private void burn()
        {
            _root = new Node();
        }

        // 基尼系数
        private double Gini(double threshold, List<int> used, int feature)
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
                    if (data[i].attributes[feature] < threshold)
                    {
                        p1++;
                        count1[data[i].aTag]++;
                    }
                    else
                    {
                        p2++;
                        count2[data[i].aTag]++;
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

        // 测试接口
        public int predict(dashNode node, DashType pt)
        {
            return pt == DashType.iterative ? iterativePredict(node) : recursivePredict(node, _root);
        }

        // 递归测试函数
        private int recursivePredict(dashNode node, Node root)
        {
            if (root.left == null && root.right == null)
            {
                return root.output;
            }
            else
            {
                int dashfeature = root.cataNum;
                if (node.feature[dashfeature] < root.threshold)
                {
                    return recursivePredict(node, root.left);
                }
                else
                {
                    return recursivePredict(node, root.right);
                }
            }
        }

        // 迭代测试函数
        private int iterativePredict(dashNode node)
        {
            int output = 0;
            Stack<Node> workStack = new Stack<Node>();
            workStack.Push(_root);
            while (workStack.Count != 0)
            {
                Node currentNode = workStack.Pop();
                if (currentNode.left == null && currentNode.right == null)
                {
                    output = currentNode.output;
                    break;
                }
                else
                {
                    int dashfeature = currentNode.cataNum;
                    if (node.feature[dashfeature] < currentNode.threshold)
                    {
                        workStack.Push(currentNode.left);
                    }
                    else
                    {
                        workStack.Push(currentNode.right);
                    }
                }
            }
            return output;
        }

        // 递归生长树函数
        private void recursiveBuild(Node root, List<int> visit, List<int> classified, int level)
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
                root.left = null;
                root.right = null;
                int[] num = new int[CONSTA.ClassNum];
                for (int i = 0; i < CONSTA.ClassNum; i++)
                {
                    num[i] = 0;
                }
                for (int i = 0; i < scale; i++) 
                {
                    if (classified[i] == 0)
                    {
                        num[data[i].aTag]++;
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
                root.output = spec;
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
            // 如果数据集小于10或者深度大于20时就做判定并结束
            if (dataScanner <= 10 || level > 18) {
                root.left = null;
                root.right = null;
                int[] num = new int[CONSTA.ClassNum];
                for (int i = 0; i < CONSTA.ClassNum; i++) { num[i] = 0; }
                for (int i = 0; i < scale; i ++) 
                {
                    if (classified[i] == 0)
                        num[data[i].aTag] ++;
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
                root.output = spec;
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
                        if (data[j].attributes[i] > maxer) maxer = data[j].attributes[i];
                        else if (data[j].attributes[i] < miner) miner = data[j].attributes[i];
                    }
                    for (int c = 0; c < 50; c++)
                    {
                        double tempSplitor = miner + (maxer - miner) / 50 * c;
                        double tempEn = Gini(tempSplitor, classified, i);
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
                root.left = null;
                root.right = null;
                int[] num = new int[CONSTA.ClassNum];
                for (int i = 0; i < CONSTA.ClassNum; i++)
                {
                    num[i] = 0;
                }
                for (int i = 0;i < scale;i ++) {
                    if (classified[i] == 0)
                    {
                        num[data[i].aTag]++;
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
                root.output = spec;
                if (level > mostdepth)
                {
                    mostdepth = level;
                }
                return;
            }
            // 分裂数据到两个女儿节点
            visit[splitFeature] = 1;
            root.cataNum = splitFeature;
            root.threshold = splitValue;
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
                    if (data[i].attributes[splitFeature] < splitValue)
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
            root.left = new Node();
            root.right = new Node();
            nodeNum += 2;
            recursiveBuild(root.left, visit, classified, level);
            recursiveBuild(root.right, newVisit, newClassified, level);
            return;
        }

        // 迭代生长树函数
        private void iterativeBuild(List<int> avisit, List<int> aclassified, int alevel)
        {
            mostdepth = 1;
            Stack<iterationCell> workStack = new Stack<iterationCell>();
            workStack.Push(new iterationCell(_root, avisit, aclassified, alevel));
            while (workStack.Count != 0)
            {
                // 迭代数据
                iterationCell currentCell = workStack.Pop();
                Node currentNode = currentCell.myNode;
                List<int> visit = currentCell.visit;
                List<int> classified = currentCell.classified;
                int level = currentCell.level;
                level++;
                // 如果80%已经是该分类，那就立即判定
                List<int> voteSpace = new List<int>(CONSTA.ClassNum);
                int clearCounter = 0;
                for (int i = 0; i < 26; i++) { voteSpace.Add(0); }
                for (int i = 0; i < scale; i++)
                {
                    if (classified[i] == 1) { continue; }
                    voteSpace[data[i].aTag]++;
                    clearCounter++;
                }
                int vm = voteSpace.Max();
                if (vm > clearCounter * 0.8)
                {
                    currentNode.left = null;
                    currentNode.right = null;
                    currentNode.output = voteSpace.IndexOf(vm);
                    if (level > mostdepth)
                    {
                        mostdepth = level;
                    }
                    continue;
                }
                // 扫描是否还有未使用属性
                if (visit.TrueForAll((x) => { return x == 1; }))
                {
                    currentNode.left = null;
                    currentNode.right = null;
                    int[] num = new int[CONSTA.ClassNum];
                    for (int i = 0; i < CONSTA.ClassNum; i++)
                    {
                        num[i] = 0;
                    }
                    for (int i = 0; i < scale; i++)
                    {
                        if (classified[i] == 0)
                        {
                            num[data[i].aTag]++;
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
                    currentNode.output = spec;
                    if (level > mostdepth)
                    {
                        mostdepth = level;
                    }
                    continue;
                }
                if (classified.Count((x) => { return x == 0; }) <= 10 || level > 18)
                {
                    currentNode.left = null;
                    currentNode.right = null;
                    int[] num = new int[CONSTA.ClassNum];
                    for (int i = 0; i < CONSTA.ClassNum; i++) { num[i] = 0; }
                    for (int i = 0; i < scale; i++)
                    {
                        if (classified[i] == 0)
                            num[data[i].aTag]++;
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
                    currentNode.output = spec;
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
                            if (data[j].attributes[i] > maxer) maxer = data[j].attributes[i];
                            else if (data[j].attributes[i] < miner) miner = data[j].attributes[i];
                        }
                        for (int c = 0; c < 50; c++)
                        {
                            double tempSplitor = miner + (maxer - miner) / 50 * c;
                            double tempEn = Gini(tempSplitor, classified, i);
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
                    currentNode.left = null;
                    currentNode.right = null;
                    int[] num = new int[CONSTA.ClassNum];
                    for (int i = 0; i < CONSTA.ClassNum; i++)
                    {
                        num[i] = 0;
                    }
                    for (int i = 0; i < scale; i++)
                    {
                        if (classified[i] == 0)
                        {
                            num[data[i].aTag]++;
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
                    currentNode.output = spec;
                    if (level > mostdepth)
                    {
                        mostdepth = level;
                    }
                    continue;
                }
                // 分裂数据到两个女儿节点
                visit[splitFeature] = 1;
                currentNode.cataNum = splitFeature;
                currentNode.threshold = splitValue;
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
                        if (data[i].attributes[splitFeature] < splitValue)
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
                currentNode.left = new Node();
                currentNode.right = new Node();
                nodeNum += 2;
                workStack.Push(new iterationCell(currentNode.left, visit, classified, level));
                workStack.Push(new iterationCell(currentNode.right, newVisit, newClassified, level));
            }
        }
    }
}
