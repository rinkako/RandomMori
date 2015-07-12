using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RandomMori.DataMana;

namespace RandomMori.DataMana
{
    public class DataManager
    {
        // 构造器
        private DataManager()
        {
            //
        }

        // 工厂
        public static DataManager getInstance()
        {
            return instance;
        }

        // 载入训练集
        public void loadTrainSet(string filename = "train.csv")
        {
            readTrainData(filename, Base.CONSTA.Tagoffset);
        }

        // 载入测试集
        public void loadTestSet(string filename = "test.csv")
        {
            readTestData(filename);
        }

        // 写出测试集判定
        public void writeTestPredict(List<int> predictList, string filename = "dash.csv", int offset = 0)
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

        // 获得训练集
        public List<Datacell> getTrainSet()
        {
            return rawTrainSet;
        }

        // 获得测试集
        public List<Datacell> getTestSet()
        {
            return testSet;
        }

        // 训练数据读入并返回
        private List<Datacell> readTrainData(string trainFile, int offset = 0, bool jump = true)
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
                List<double> attrs = new List<double>();
                for (int i = 1; i <= Base.CONSTA.AttriNum; i++)
                {
                    attrs.Add(Convert.ToDouble(lineItem[i].Trim()));
                }
                int tag = Convert.ToInt32(lineItem[Base.CONSTA.AttriNum + 1].Trim()) - offset;
                rawTrainSet.Add(new Datacell(attrs, tag, idx));
            }
            return rawTrainSet;
        }

        // 测试数据读入并返回
        private List<Datacell> readTestData(string testFile, bool jump = true)
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
                List<double> attrs = new List<double>();
                for (int i = 1; i <= Base.CONSTA.AttriNum; i++)
                {
                    attrs.Add(Convert.ToDouble(lineItem[i].Trim()));
                }
                testSet.Add(new Datacell(attrs, -1, idx));
            }
            return rawTrainSet;
        }

        private List<Datacell> testSet = new List<Datacell>();
        private List<Datacell> rawTrainSet = new List<Datacell>();
        private static readonly DataManager instance = new DataManager();
        private Sampler coreSp = new Sampler();
    }
}
