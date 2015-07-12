using System;
using System.Collections.Generic;
using System.Text;

namespace RandomMori.Base
{
    public class CONSTA
    {
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
        public static string trainPath = "train.csv";
        public static string testPath = "test.csv";
        public static DashType dt = DashType.iterative;
        public static double Divider = 3;
        public static int ClassNum = 26;
        public static int AttriNum = 617;
        public static int MinSampleNum = 32;
        public static int SampleNum = 6238;
        public static int Tagoffset = 1;
        public static int Treenum = 500;
        public static int Thread = 4;
        public static int SplitNum = (int)(Math.Sqrt(CONSTA.AttriNum) * 0.5);
    }

    public enum DashType
    {
        iterative,
        recursive
    }

    public enum TestType
    {
        train,
        test
    }
}
