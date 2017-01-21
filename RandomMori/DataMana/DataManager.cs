using System;
using System.IO;
using System.Collections.Generic;

namespace RandomMori.DataMana
{
	/// <summary>
	/// 数据管理器类
	/// </summary>
    public class DataManager
    {
		/// <summary>
		/// 工厂方法：获得类的唯一实例
		/// </summary>
		/// <returns>数据管理器唯一实例</returns>
        public static DataManager GetInstance()
        {
            return DataManager.instance;
        }

		/// <summary>
		/// 载入训练集
		/// </summary>
		/// <param name="filename">文件路径</param>
		public void LoadTrainSet(string filename = "train.csv")
        {
            ReadTrainData(filename, Base.CONSTA.Tagoffset);
        }
		
		/// <summary>
		/// 载入测试集
		/// </summary>
		/// <param name="filename">文件路径</param>
		public void LoadTestSet(string filename = "test.csv")
        {
            ReadTestData(filename);
        }
		
		/// <summary>
		/// 写出测试集判定
		/// </summary>
		/// <param name="predictList">预测结果向量</param>
		/// <param name="filename">要写出的文件路径</param>
		/// <param name="offset">预测结果向量开始遍历位置的偏移</param>
		public void WriteTestPredict(List<int> predictList, string filename = "dash.csv", int offset = 0)
        {
            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine("id,label");
            for (int i = 0; i < predictList.Count - 1; i++)
            {
                sw.WriteLine("{0},{1}", i, predictList[i] + offset);
            }
            sw.Write("{0},{1}", predictList.Count - 1, predictList[predictList.Count - 1] + offset);
            sw.Close();
        }
		
		/// <summary>
		/// 获得训练集
		/// </summary>
		/// <returns>训练集的数据包装向量</returns>
		public List<Datacell> GetTrainSet()
        {
            return rawTrainSet;
        }
		
		/// <summary>
		/// 获得测试集
		/// </summary>
		/// <returns>测试集的数据包装向量</returns>
		public List<Datacell> GetTestSet()
        {
            return testSet;
        }

		/// <summary>
		/// 读入训练数据并返回
		/// </summary>
		/// <param name="trainFile">训练文件路径</param>
		/// <param name="offset">类标的偏移量</param>
		/// <param name="jump">是否需要跳过第一行</param>
		/// <returns>数据包装向量</returns>
		private List<Datacell> ReadTrainData(string trainFile, int offset = 0, bool jump = false)
        {
            rawTrainSet.Clear();
            StreamReader sr = new StreamReader(trainFile);
            if (jump)
            {
                string jumpline = sr.ReadLine();
            }
            while (sr.EndOfStream != true)
            {
                string line = sr.ReadLine();
                string[] lineItem = line.Split(',');
                int idx = Convert.ToInt32(lineItem[0]);
                int tag = Convert.ToInt32(lineItem[4]);



                //int idx = Convert.ToInt32(lineItem[0]);
                List<double> attrs = new List<double>();
                for (int i = 0; i < Base.CONSTA.AttriNum; i++)
                {
                    attrs.Add(Convert.ToDouble(lineItem[i + 5].Trim()));
                }
                //int tag = Convert.ToInt32(lineItem[Base.CONSTA.AttriNum + 1].Trim()) - offset;
                rawTrainSet.Add(new Datacell(attrs, tag, idx));
            }
            return rawTrainSet;
        }

		// 
		/// <summary>
		/// 读入测试数据并返回
		/// </summary>
		/// <param name="testFile">测试文件路径</param>
		/// <param name="jump">是否需要跳过第一行</param>
		/// <returns>数据包装向量</returns>
		private List<Datacell> ReadTestData(string testFile, bool jump = false)
        {
            testSet.Clear();
            StreamReader sr = new StreamReader(testFile);
            if (jump)
            {
                string jumpline = sr.ReadLine();
            }
            while (sr.EndOfStream != true)
            {
                string line = sr.ReadLine();
                string[] lineItem = line.Split(',');

                int idx = Convert.ToInt32(lineItem[0]);
                int tag = Convert.ToInt32(lineItem[4]);


                //int idx = Convert.ToInt32(lineItem[0]);
                List<double> attrs = new List<double>();
                for (int i = 1; i <= Base.CONSTA.AttriNum; i++)
                {
                    attrs.Add(Convert.ToDouble(lineItem[i].Trim()));
                }
                testSet.Add(new Datacell(attrs, -1, idx));
            }
            return rawTrainSet;
        }

		/// <summary>
		/// 私有的构造器
		/// </summary>
		private DataManager() { }

		/// <summary>
		/// 测试集数据包装向量
		/// </summary>
		private List<Datacell> testSet = new List<Datacell>();

		/// <summary>
		/// 训练集数据包装向量
		/// </summary>
        private List<Datacell> rawTrainSet = new List<Datacell>();

		/// <summary>
		/// 唯一实例
		/// </summary>
        private static readonly DataManager instance = new DataManager();

		/// <summary>
		/// 抽样器实例
		/// </summary>
        private Sampler coreSp = new Sampler();
    }
}
