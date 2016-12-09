using System;
using System.Linq;
using RandomMori.DataMana;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RandomMori.MoriMana
{
    /// <summary>
    /// 森林类
    /// </summary>
    public class Mori
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public Mori()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("# begin split input");
            dataManager.LoadTrainSet(Base.CONSTA.trainPath);
            dataManager.LoadTestSet(Base.CONSTA.testPath);
            Console.WriteLine("# split input OK");
            Console.ResetColor();
        }
        
        /// <summary>
        /// 烧毁森林
        /// </summary>
        public void Burn()
        {
            forest.Clear();
            treeCounter = 0;
        }
        
        /// <summary>
        /// 异步生长森林
        /// </summary>
        /// <param name="treeNum">树的数量</param>
        /// <param name="handle">线程的数量</param>
        public void GrowAsync(int treeNum = 1000, int handle = 4)
        {
            // 烧毁
            if (forest.Count != 0)
            {
                this.Burn();
            }
            // 种树
            treenum = treeNum;
            workQueue = new Queue<CART>();
            for (int i = 0; i < treeNum; i++)
            {
                CART treeRef = new CART(Base.CONSTA.SampleNum);
                forest.Add(treeRef);
                workQueue.Enqueue(treeRef);
            }
            // 生长
            if (handle > 8) { handle = 8; }
            handlePool = new List<Task>();
            handlePool.Add(new Task(() => HandleAdapter(1)));
            handlePool.Add(new Task(() => HandleAdapter(2)));
            handlePool.Add(new Task(() => HandleAdapter(3)));
            handlePool.Add(new Task(() => HandleAdapter(4)));
            handlePool.Add(new Task(() => HandleAdapter(5)));
            handlePool.Add(new Task(() => HandleAdapter(6)));
            handlePool.Add(new Task(() => HandleAdapter(7)));
            handlePool.Add(new Task(() => HandleAdapter(8)));
            handlePool.RemoveRange(handle - 1, handlePool.Count - handle);
            for (int i = 0; i < handlePool.Count; i++)
            {
                handlePool[i].Start();
            }
            // 阻塞并等待所有线程回调
            Task.WaitAll(handlePool.ToArray());
            Console.WriteLine("# All thread is accomplished");
        }
        
        /// <summary>
        /// 生长任务接收器，子线程的入口
        /// </summary>
        /// <param name="handleId"></param>
        private void HandleAdapter(int handleId)
        {
            while (workQueue.Count != 0)
            {
                var myTree = workQueue.Dequeue();
                myTree.Grow((int)(Base.CONSTA.SampleNum / Base.CONSTA.Divider), Base.CONSTA.SplitNum, treeCounter++, Base.CONSTA.dt);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("At thread {0}, build tree OK, Noc： {1} ({2}%)", handleId, workQueue.Count, ((double)(treenum - workQueue.Count) / treenum * 100).ToString("0.00"));
                Console.ResetColor();
            }
            // 任务完成，等待回调
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("    >>> Waiting for callback");
            Console.ResetColor();
        }
        
        /// <summary>
        /// 投票测试森林
        /// </summary>
        /// <param name="pt">执行方式</param>
        /// <returns>准确率</returns>
        public double Classify(Base.TestType pt = Base.TestType.train)
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
        
        /// <summary>
        /// 已构造的树的总量
        /// </summary>
        private static int treeCounter = 0;

        /// <summary>
        /// 想要构造的树的总量
        /// </summary>
        private static int treenum = 0;
        
        /// <summary>
        /// 预测结果向量
        /// </summary>
        private List<int> result = null;

        /// <summary>
        /// 任务池
        /// </summary>
        private List<Task> handlePool = null;

        /// <summary>
        /// 工作队列
        /// </summary>
        private Queue<CART> workQueue = null;

        /// <summary>
        /// 数据管理器
        /// </summary>
        private DataManager dataManager = DataManager.GetInstance();

        /// <summary>
        /// 森林
        /// </summary>
        private List<CART> forest = new List<CART>();
    }
}