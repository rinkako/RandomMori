using System;
using System.Collections.Generic;
using System.Text;

namespace RandomMori.Base
{
	/// <summary>
	/// 为全局数据和设置提供通用的容器
	/// </summary>
    public class CONSTA
    {
		/// <summary>
		/// 初始化函数
		/// </summary>
		/// <param name="cn">类标总数</param>
		/// <param name="an">属性总数</param>
		/// <param name="mn">最小样本数量</param>
		/// <param name="sn">样本数量</param>
		/// <param name="of">类标在内存中和在文件中的偏移量</param>
		/// <param name="tr">树的数量</param>
		/// <param name="th">线程数量</param>
		/// <param name="dv">生长一棵树所需样本占总样本的几分之一</param>
        public static void init(int cn, int an, int mn, int sn, int of, int tr, int th, double dv)
        {
            ClassNum = cn;
            AttriNum = an;
            MinSampleNum = mn;
            SampleNum = sn;
            Tagoffset = of;
            Treenum = tr;
            Thread = th;
            Divider = dv;
            SplitNum = (int)(Math.Sqrt(an) * 0.5);
        }

		/// <summary>
		/// 训练文件路径
		/// </summary>
        public static string trainPath = "train.csv";

		/// <summary>
		/// 测试文件路径
		/// </summary>
        public static string testPath = "test.csv";

		/// <summary>
		/// 执行算法的模式
		/// </summary>
        public static DashType dt = DashType.iterative;

		/// <summary>
		/// 生长一棵树所需样本占总样本的几分之一
		/// </summary>
		public static double Divider = 3;

		/// <summary>
		/// 类标总数
		/// </summary>
        public static int ClassNum = 2;

		/// <summary>
		/// 属性总数
		/// </summary>
        public static int AttriNum = 130;

		/// <summary>
		/// 数据集分裂的最小样本数
		/// </summary>
        public static int MinSampleNum = 32;

		/// <summary>
		/// 样本总数
		/// </summary>
        public static int SampleNum = 1000000;

		/// <summary>
		/// 类标在内存中和在文件中的偏移量
		/// </summary>
		public static int Tagoffset = 0;

		/// <summary>
		/// 树的数量
		/// </summary>
        public static int Treenum = 500;

		/// <summary>
		/// 线程数量
		/// </summary>
        public static int Thread = 4;

		/// <summary>
		/// 分裂数量
		/// </summary>
        public static int SplitNum = (int)(Math.Sqrt(CONSTA.AttriNum) * 0.5);
    }

	/// <summary>
	/// 枚举：递归或迭代
	/// </summary>
    public enum DashType
    {
		/// <summary>
		/// 迭代
		/// </summary>
        iterative,
		/// <summary>
		/// 递归
		/// </summary>
        recursive
    }

	/// <summary>
	/// 枚举：运行方式
	/// </summary>
    public enum TestType
    {
		/// <summary>
		/// 训练
		/// </summary>
        train,
		/// <summary>
		/// 测试
		/// </summary>
        test
    }
}
