using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMA_ETL
{
    public static class DataETLer
    {
        public static void ETL(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            FileStream wfs = new FileStream("ETL_" + path, FileMode.Create);
            StreamWriter sw = new StreamWriter(wfs);
            DataETLer.DataList = new List<DataPackage>();
            DataETLer.NormalParamList = new List<NormalPackage>();
            DataETLer.AccuArray = new double[130];
            DataETLer.Counter = 0;
            for (int i = 0; i < 130; i++)
            {
                DataETLer.AccuArray[i] = 0;
            }
            // 读入数据
            while (sr.EndOfStream != true)
            {
                var aline = sr.ReadLine();
                DataETLer.Counter++;
                var lineitem = aline.Split(',');
                Console.WriteLine("Dealing: " + lineitem[0]);
                // 处理基本属性的转义
                int newtype = ParseTypeStringToInt(lineitem[1]);
                double orgamt = Convert.ToDouble(lineitem[2]);
                double newamt = Convert.ToDouble(lineitem[3]);
                bool amtFlag = orgamt > newamt;
                // 创建数据包装
                DataPackage dp = new DataPackage()
                {
                    id = lineitem[0],
                    org_amt = orgamt,
                    new_amt = newamt,
                    amt_flag = amtFlag,
                    new_type = newtype
                };
                for (int i = 4; i <= 133; i++)
                {
                    var v = Convert.ToDouble(lineitem[i]);
                    dp.properties[i - 4] = v;
                    DataETLer.AccuArray[i - 4] += v;
                }
                DataETLer.DataList.Add(dp);
            }
            // 计算统计量
            for (int i = 0; i < 130; i++)
            {
                DataETLer.NormalizePrepare(i);
            }
            // 输出
            for (int i = 0; i < DataETLer.DataList.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DataETLer.DataList[i].id).Append(",");
                sb.Append(DataETLer.DataList[i].new_type).Append(",");
                sb.Append(DataETLer.DataList[i].org_amt).Append(",");
                sb.Append(DataETLer.DataList[i].new_amt).Append(",");
                sb.Append(DataETLer.DataList[i].amt_flag == true ? "1" : "0");
                for (int j = 0; j < 130; j++)
                {
                    var nv = DataETLer.NormalizeLinear(DataETLer.DataList[i].properties[j], j);
                    sb.Append(",").Append(nv.ToString("0.00000000"));
                }
                sw.WriteLine(sb);
            }
            sw.Close();
            wfs.Close();
            sr.Close();
            fs.Close();
        }

        public static int ParseTypeStringToInt(string str)
        {
            if (str == "2 - U33YJHZ") { return 1; }
            else if (str == "DA_SAN_YUAN_53") { return 2; }
            else if (str == "FEI_YONG_19") { return 3; }
            else { return 0; }
        }

        public static double Normalize(double orgValue, int indexOfProperty)
        {
            return (orgValue - DataETLer.NormalParamList[indexOfProperty].mean) / DataETLer.NormalParamList[indexOfProperty].sd;
        }

        public static double NormalizeLinear(double orgValue, int indexOfProperty)
        {
            var fm = DataETLer.NormalParamList[indexOfProperty].maxv - DataETLer.NormalParamList[indexOfProperty].minv;
            if (fm == 0) { return 0; }
            return (orgValue - DataETLer.NormalParamList[indexOfProperty].minv) / fm;
        }

        public static void NormalizePrepare(int indexOfProperty)
        {
            //// 均值
            //var mean = DataETLer.AccuArray[indexOfProperty] / DataETLer.Counter;
            //// 方差
            //double acc = 0;
            //for (int i = 0; i < DataETLer.Counter; i++)
            //{
            //    acc += Math.Pow(DataETLer.DataList[i].properties[indexOfProperty] - mean, 2.0);
            //}
            //double standardVariance = Math.Sqrt(acc / DataETLer.Counter);
            // 最值
            double cmax = double.MinValue;
            double cmin = double.MaxValue;
            for (int i = 0; i < DataETLer.Counter; i++)
            {
                if (DataETLer.DataList[i].properties[indexOfProperty] > cmax)
                {
                    cmax = DataETLer.DataList[i].properties[indexOfProperty];
                }
                if (DataETLer.DataList[i].properties[indexOfProperty] < cmin)
                {
                    cmin = DataETLer.DataList[i].properties[indexOfProperty];
                }
            }
            // 存起来
            NormalPackage np = new NormalPackage()
            {
                //mean = mean,
                //sd = standardVariance,
                maxv = cmax,
                minv = cmin
            };
            DataETLer.NormalParamList.Add(np);
        }

        private static double Counter;

        private static List<DataPackage> DataList;

        private static List<NormalPackage> NormalParamList;

        private static double[] AccuArray;

    }

    class NormalPackage
    {
        public double mean = 0;
        public double sd = 0;
        public double maxv = Double.MinValue;
        public double minv = Double.MaxValue;
    }

    class DataPackage
    {
        public string id = "ERROR";
        public int new_type = 0;
        public double org_amt = 0;
        public double new_amt = 0;
        public bool amt_flag = false;
        public double[] properties = new double[130];
    }
}
