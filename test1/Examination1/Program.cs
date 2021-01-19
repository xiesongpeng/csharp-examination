using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Examination1
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args != null && args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                Console.WriteLine("请输入程序同目录文件名或完整路径：");
                path = Console.ReadLine();
            }

            int count = GetClusterCount(path);


            Console.WriteLine(count);
            Console.ReadLine();
        }
        public class NumberClass { public int Number; }
        public class RefClass { public NumberClass obj; }
        /// <summary>
        /// 获取簇的数量
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetClusterCount(string path)
        {
            List<string> list = new List<string>();

            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line.Trim());
            }
            int rowCount = list.Count;
            int colCount = list[0].Length;

            //标记所属簇编号
            int[,] matrixNumber = new int[rowCount, colCount];
            //扫描相对坐标集
            List<int[]> xyList = new List<int[]>();
            xyList.Add(new int[2] { -1, 0 });
            xyList.Add(new int[2] { -1, -1 });
            xyList.Add(new int[2] { 0, -1 });
            xyList.Add(new int[2] { 1, -1 });

            //存储簇及其关联簇，有关联的以关联最小编号为其最终所属簇
            Dictionary<int, RefClass> numberDict = new Dictionary<int, RefClass>();
            ReadOnlySpan<char> span;
            int x, y;
            bool flag;
            int key, key2;
            for (int row = 0; row < list.Count; row++)
            {
                span = list[row].AsSpan();
                for (int col = 0; col < span.Length; col++)
                {
                    flag = span[col] == '+';
                    if (flag)
                    {
                        RefClass tempObj;
                        if (matrixNumber[row, col] == 0)
                        {
                            key = numberDict.Count + 1;
                            tempObj = new RefClass() { obj = new NumberClass() { Number = key } };
                        }
                        else
                        {
                            key = matrixNumber[row, col];
                            tempObj = numberDict[key];
                        }
                        key2 = key;
                        foreach (var xy in xyList)
                        {
                            y = row + xy[1];
                            x = col + xy[0];
                            if (y >= 0 && y < rowCount && x >= 0 && x < colCount)
                            {
                                if (row == 1 && col == 2)
                                {
                                }
                                if (matrixNumber[y, x] != 0 && matrixNumber[y, x] != key)
                                {
                                    var num0 = tempObj;
                                    var num1 = numberDict[matrixNumber[y, x]];

                                    //大编号关联为小编号所属簇
                                    if (num0.obj.Number < num1.obj.Number)
                                    {
                                        num1.obj = num0.obj;
                                    }
                                    else if (num0.obj.Number > num1.obj.Number)
                                    {
                                        if (key2 == key)
                                        {
                                            num0.obj = num1.obj;
                                            key2 = num0.obj.Number;
                                        }
                                        else
                                        {
                                            num0.obj.Number = num1.obj.Number;
                                        }
                                    }
                                }
                            }
                        }
                        if (key == key2)//扫描不到则新增簇
                        {
                            numberDict.Add(key, tempObj);
                        }
                        matrixNumber[row, col] = numberDict[key2].obj.Number;
                    }
                }
            }
            //返回无关联簇的，即最父级簇个数
            return numberDict.Where(p => p.Key == p.Value.obj.Number).Count();//String
        }
    }
}
