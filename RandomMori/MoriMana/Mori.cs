using System;
using System.Linq;
using RandomMori.DataMana;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RandomMori.MoriMana
{
    public class Mori
    {
        // 构造器
        public Mori()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("# begin split input");
            dataManager.LoadTrainSet(Base.CONSTA.trainPath);
            dataManager.LoadTestSet(Base.CONSTA.testPath);
            Console.WriteLine("# split input OK");
            Console.ResetColor();
        }

        // 烧毁森林
        public void burn()
        {
            forest.Clear();
            treeCounter = 0;
        }

        // 异步生长森林
        public void grow(int treeNum = 1000, int handle = 4)
        {
            // 烧毁
            if (forest.Count != 0)
            {
                this.burn();
            }
            // 种树
            treenum = treeNum;
            workFlow = new Queue<CART>();
            for (int i = 0; i < treeNum; i++)
            {
                CART treeRef = new CART(Base.CONSTA.SampleNum);
                forest.Add(treeRef);
                workFlow.Enqueue(treeRef);
            }
            // 生长
            if (handle > 8) { handle = 8; }
            handlePool = new List<Task>();
            handlePool.Add(new Task(() => handleAdapter(1)));
            handlePool.Add(new Task(() => handleAdapter(2)));
            handlePool.Add(new Task(() => handleAdapter(3)));
            handlePool.Add(new Task(() => handleAdapter(4)));
            handlePool.Add(new Task(() => handleAdapter(5)));
            handlePool.Add(new Task(() => handleAdapter(6)));
            handlePool.Add(new Task(() => handleAdapter(7)));
            handlePool.Add(new Task(() => handleAdapter(8)));
            handlePool.RemoveRange(handle - 1, handlePool.Count - handle);
            for (int i = 0; i < handlePool.Count; i++)
            {
                handlePool[i].Start();
            }
            // 阻塞并等待所有线程回调
            Task.WaitAll(handlePool.ToArray());
            Console.WriteLine("# All thread is accomplished");
        }

        // 生长任务
        private void handleAdapter(int handleId)
        {
            while (workFlow.Count != 0)
            {
                var myTree = workFlow.Dequeue();
                myTree.Grow((int)(Base.CONSTA.SampleNum / Base.CONSTA.Divider), Base.CONSTA.SplitNum, treeCounter++, Base.CONSTA.dt);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("At thread {0}, build tree OK, Noc： {1} ({2}%)", handleId, workFlow.Count, ((double)(treenum - workFlow.Count) / treenum * 100).ToString("0.00"));
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("    >>> Waiting for callback");
            Console.ResetColor();
        }

        // 投票测试森林：训练集
        public double classify(Base.TestType pt = Base.TestType.train)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(">>> Running predicting job for " + pt.ToString());
            Console.ResetColor();
            int dataBoundary = pt == Base.TestType.train ? dataManager.GetTrainSet().Count : dataManager.GetTestSet().Count;
            int encounter = 0;
            result = new List<int>();
            List<int> outBuffer = new List<int>();
            int bound = Math.Min(treenum, forest.Count);
            for (int i = 0; i < dataBoundary; i++) 
            {
                List<int> voteSpace = new List<int>(Base.CONSTA.ClassNum);
                for (int j = 0; j < Base.CONSTA.ClassNum; j++) { voteSpace.Add(0); }
                for (int j = 0; j < bound; j++)
                {
                    Datacell dc = pt == Base.TestType.train ? dataManager.GetTrainSet()[i] : dataManager.GetTestSet()[i];
                    dashNode va = new dashNode(dc.Label, dc.Attributes);
                    voteSpace[forest[j].Predict(va, Base.CONSTA.dt)]++;
                }
                result.Add(voteSpace.IndexOf(voteSpace.Max()));
                if (pt == Base.TestType.train)
                {
                    encounter += result.Last() == dataManager.GetTrainSet()[i].Label ? 1 : 0;
                }
                else
                {
                    outBuffer.Add(result.Last());
                }
            }
            if (pt == Base.TestType.test)
            {
                dataManager.WriteTestPredict(outBuffer, "dash.csv", Base.CONSTA.Tagoffset);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(">>> Predicted OK.");
            Console.ResetColor();
            return (double)encounter / (double)dataBoundary;
        }

        // 森林计数器
        private static int treeCounter = 0;
        private static int treenum = 0;
        // 结果向量
        private List<int> result = null;
        // 任务池
        private List<Task> handlePool = null;
        // 工作队列
        private Queue<CART> workFlow = null;
        // 数据管理器
        private DataManager dataManager = DataManager.GetInstance();
        // 森林
        private List<CART> forest = new List<CART>();
    }
}