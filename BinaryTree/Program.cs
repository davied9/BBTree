using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DAV.Utilities;
using DAV.DataStructure;
using System.IO;
using PM = DAV.ProgramMonitor;

namespace BinaryTree
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestCase000();
            TestCase001();
            //TestCase002();
            //TestCase003();
            //TestCase004();
            Console.ReadKey();
        }

        /*  查看通用数据结构在操作时的特性 */
        static void TestCase000()
        {
            //  查看 Dictionary 在添加元素时，key 值使用的判定依据
            Console.WriteLine("Test case 000 ####################################################### " );
            Dictionary<Data, bool> test = new Dictionary<Data, bool>();
            Data d3 = new Data(3);
            Data d1 = new Data(1);
            Data d0 = new Data(0);
            Data d4 = new Data(4);
            Data d2 = new Data(2);
            Data d01 = new Data(0);
            Data d02 = new Data(0);
            test[d1] = true;
            test[d2] = true;
            test[d3] = true;
            test[d0] = true;
            test[d4] = true;

            Console.WriteLine("Dictionary :");
            Show(test.Keys);
            Console.WriteLine("d2 > d4? : {0}", d2.GetHashCode() > d4.GetHashCode());
            Console.WriteLine("d0 > d2? : {0}", d0.GetHashCode() > d2.GetHashCode());
            //  结论：
            //      1、GetHashCode 的比较与返回值相关，这里是 index
            //      2、遍历的顺序与添加的顺序一致，与 key 值无关


            
            LinkedList<Data> list = new LinkedList<Data>();
            list.AddLast(d0);
            list.AddLast(d1);
            list.AddLast(d2);
            list.AddLast(d3);
            list.AddLast(d4);
            LinkedListNode<Data> itr = list.First;

            SortedSet<Data> sortedSet = new SortedSet<Data>(new DataComparer());
            Console.WriteLine("\n\nBegin build SortedSet ...");
            sortedSet.Add(d1);
            sortedSet.Add(d2);
            sortedSet.Add(d3);
            sortedSet.Add(d0);
            sortedSet.Add(d01);
            sortedSet.Add(d02);
            sortedSet.Add(d4);
            Console.WriteLine("End build SortedSet ...");
            Console.WriteLine("SortedSet :");
            Show(sortedSet);
            //  结论 ： 添加时进行排序
            Console.WriteLine("\n\n");

            Console.WriteLine("Contains d1  ? : {0}", sortedSet.Contains(d1));
            Console.WriteLine("Contains d01 ? : {0}", sortedSet.Contains(d01));


            Console.WriteLine("\n\nBegin remove SortedSet ...");
            sortedSet.Remove(d01);
            sortedSet.Remove(d02);
            sortedSet.Remove(d3);
            Console.WriteLine("End remove SortedSet ...");
            Console.WriteLine("SortedSet (after remove):");
            Show(sortedSet);

            Console.WriteLine("\n\nBegin re-add SortedSet ...");
            sortedSet.Add(d01);
            sortedSet.Add(d02);
            sortedSet.Add(d3);
            Console.WriteLine("End re-add SortedSet ...");
            Console.WriteLine("SortedSet (Re-added):");
            Show(sortedSet);
            
            Console.WriteLine("\n\nBegin alter SortedSet ...");
            d01.num = 11;
            d02.num = 12;
            d3.num = 33; // d3.index = 0
            Console.WriteLine("End alter SortedSet ...");
            Console.WriteLine("SortedSet (after alter):");
            Show(sortedSet);

            
            Console.WriteLine("\n\nBegin remove SortedSet after altered ...");
            sortedSet.Remove(d01);
            sortedSet.Remove(d02);
            sortedSet.Remove(d3);
            Console.WriteLine("End remove SortedSet after altered ...");
            Console.WriteLine("SortedSet (after alter && remove):");
            Show(sortedSet);
            /*  结论 ： 
             *      1、修改后顺序不变，且不提供重新排序的方法，参考(after alter)
             *      2、如果创建 SortedSet 时指定了 Comparer，那么后续的 Add、Remove、Contains 操作，均使用此 Comparer
             *      3、如果修改了 item 的内容，不能正确的删除，因为改变了遍历的特性，参考(after alter && remove)
            */
        }

        /*  关于二叉树的特性 */
        static void TestCase001()
        {
            Console.WriteLine("\n\nTest case 001 ####################################################### ");
            BBTree<Data> tree = new BBTree<Data>(new DataComparer());
            //BBTree<Data> tree = new BBTree<Data>();
            int nums = 15;
            for(int ix = 0; ix < nums; ++ix)
            {
                tree.Add(new Data(ix));
            }
            Data d31 = new Data(3);
            Data d32 = new Data(3);
            Data d33 = new Data(3);
            Data d34 = new Data(3);

            tree.Add(d31);
            tree.Add(d32);
            tree.Add(d33);
            tree.Add(d34);

            BBTreeNode<int> jnode = new BBTreeNode<int>(4);

            Console.WriteLine("\n\nInitialized :");
            Show(tree);
            Console.WriteLine("Depth of tree : {0}", tree.Depth());
            Console.WriteLine("tree Contains d31 {0} ? {1}", d31, tree.Contains(d31));
            Console.WriteLine("tree ContainsExactly d31 {0} ? {1}", d31, tree.ContainsExactly(d31));
            //Console.WriteLine("Num of Brothers of d32 is {0}", tree.Match(d32).BrotherCount);
            //Console.WriteLine("fist brother of d32 is {0}", tree.Match(d32).FirstBrother);
            //Console.WriteLine("last of brother of d32 is {0}", tree.Match(d32).LastBrother);
            //Console.WriteLine("next of brother of d32 is {0}", tree.Match(d32).NextBrother);
            //Console.WriteLine("previous of brother of d32 is {0}", tree.Match(d32).PreviousBrother);
            Console.WriteLine("Count of nodes in this tree : {0}", tree.TotalCount);
            Console.WriteLine("Unique Count of nodes in this tree : {0}", tree.Count);
            Console.WriteLine("First node : {0}", tree.First());
            Console.WriteLine("Last node : {0}", tree.Last());

            d31.num = 31;
            d32.num = 32;
            Console.WriteLine("\n\nAfter alter:");
            Show(tree);

            //  these are in file
            FileStream fs = new FileStream("tree.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("\n\nWe now see the whole tree :");
            sw.WriteLine(tree);
            Show(tree, sw);
            sw.Flush();

            tree.LeftRotate();
            sw.WriteLine("\n\nafter left rotation :");
            sw.WriteLine(tree);
            Show(tree, sw);
            sw.Flush();

            tree.RightRotate();
            sw.WriteLine("\n\nafter right rotation :");
            sw.WriteLine(tree);
            Show(tree, sw);
            sw.Flush();

            tree.RightRotate();
            sw.WriteLine("\n\nafter right rotation 2 time :");
            sw.WriteLine(tree);
            Show(tree, sw);
            sw.Flush();

            sw.Close(); sw = null;
            fs.Close(); fs = null;
            
        }

        //  测试 \r，可以完成覆盖写的操作
        static void TestCase002()
        {
            Console.WriteLine("\n\nTest case 002 ####################################################### ");

            //  测试 \r，可以完成覆盖写的操作
            string str = "joke 1 joke 2 joke 3\r";
            Console.Write(str);
            Thread.Sleep(2000);
            str = "joke 4 joke 5 joke 6\r";
            Console.Write(str);
            Thread.Sleep(2000);
            str = "joke 7 joke 8 joke 9\r";
            Console.Write(str);
            Thread.Sleep(2000);


        }


        /*  测试 Utilities 里的一些特性 */
        static void TestCase003()
        {
            Console.WriteLine("\n\nTest case 003 ####################################################### ");
            ConsoleDisplayer displayer = new ConsoleDisplayer(
                replaceTabs : true,
                nSpacesForTab : 4,
                spaceHolder : '$',
                lineBreaker : "\n"
                );
            string str = "joke 1 joke 2 joke 3\n";
            str += "joke 4 joke 5 joke 6\n";
            str += "joke 7 joke 8 joke 9\ntest";

            for(int ix = 0; ix < 5; ++ix)
            {
                string str_p = displayer.Retraction(str, ix);
                Console.WriteLine(str_p);
            }
            

        }

        /*  StringAligner 特性测试*/
        static void TestCase004()
        {
            Console.WriteLine("\n\nTest case 004 ####################################################### ");

            //  Test format string print
            Console.WriteLine("string format testing");
            Console.WriteLine("{0,-16}{1,16}", "testforfun", "testforfun");
            StringAligner al = new StringAligner(5, spaceHolder: '@');
            Console.WriteLine("Center Aligned : {0}", al.alignCenter("js"));
            Console.WriteLine("Left   Aligned : {0}", al.alignLeft("js"));
            Console.WriteLine("Right  Aligned : {0}", al.alignRight("js"));
            al.width = 4;
            Console.WriteLine("Center Aligned : {0}", al.alignCenter("js"));
            Console.WriteLine("Left   Aligned : {0}", al.alignLeft("js"));
            Console.WriteLine("Right  Aligned : {0}", al.alignRight("js"));
            
            Console.WriteLine("Center Aligned : {0}", al.alignCenter("this_i"));
            Console.WriteLine("Left   Aligned : {0}", al.alignLeft("this_i"));
            Console.WriteLine("Right  Aligned : {0}", al.alignRight("this_i"));
            al.width = 6;
            Console.WriteLine("Center Aligned : {0}", al.alignCenter("this_i"));
            Console.WriteLine("Left   Aligned : {0}", al.alignLeft("this_i"));
            Console.WriteLine("Right  Aligned : {0}", al.alignRight("this_i"));
        }




        static void Show(IEnumerable dataset, StreamWriter sw = null)
        {
            int counter = 0;
            if(null == sw) Console.WriteLine("index   :   value");
            else sw.WriteLine("index   :   value");
            foreach (var data in dataset)
            {
                if (null == sw) Console.WriteLine("{0} : {1}", counter++, data);
                else sw.WriteLine("{0} : {1}", counter++, data);
            }
        }
        
    }
    public class Data : IComparable
    {
        public Data(int num)
        {
            this.num = num;
            index = counter++;
        }
        public static int counter = 0;
        public int index = 0;
        public int num = 0;

        public int CompareTo(object obj)
        {
            Console.WriteLine("Data.CompareTo called by {0} with {1}.", this, obj);
            Data data = obj as Data;
            if (null == data) throw new Exception("[FATAL] Data.CompareTo(objec) : Data can only compare with Data.");
            int ret = this.num - data.num;
            //if (0 == ret) ret = -1; // 关键行 ： 修改判断相等时的处理
            return ret; 
        }
        public override bool Equals(object obj)
        {
            Console.WriteLine("Data.Equals called by {0} with {1}.", this, obj);
            Data outsider = obj as Data;
            if (null == outsider) throw new Exception("[FATAL] ");
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return string.Format("I {0} D {1}", index, num);
            //return string.Format("Data( {0}, {1} )", index, num);
        }
        public override int GetHashCode()
        {
            Console.WriteLine("Data.GetHashCode() of {0} called!!!", this);
            //return base.GetHashCode();
            return index;
        }
    }
    public class DataComparer : IComparer<Data>
    {
        public int Compare(Data d1, Data d2)
        {
            Console.WriteLine("DataComparer.Compare called with {0} and {1}.", d1, d2);
            int ret = d1.num - d2.num;
            //int ret = d2.num - d1.num;
            //if (0 == ret) ret = -1; // 关键行 ： 修改判断相等时的处理
            return ret;
        }
    }
    public class NotComparable
    {

    }
}
