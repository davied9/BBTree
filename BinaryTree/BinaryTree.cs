using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;
using PM = DAV.ProgramMonitor;

namespace DAV
{
    namespace Math
    {
        public class Math
        {
            /// <summary>
            /// factorial of n
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public static int Factorial(int n)
            {
                if (0 < n) return n * Factorial(n - 1);
                if (0 == n) return 1;
                return 0;   //  0 > n
            }
            /// <summary>
            /// b ** ps + b ** (ps+1) + ... + b ** pe
            /// b   for _base
            /// ps  for powerStart
            /// pe  for powerEnd
            /// </summary>
            /// <param name="_base"></param>
            /// <param name="powerStart"></param>
            /// <param name="powerEnd"></param>
            /// <returns></returns>
            public static double SumOfPower(double _base, int powerStart, int powerEnd)
            {
                if (powerEnd < powerStart) return 0;
                double sum = 0;
                for (int power = powerStart; power <= powerEnd; ++power)
                {
                    sum += System.Math.Pow(_base, power);
                }
                return sum;
            }
        }
    }

    namespace Utilities
    {
        using Exceptions;

        public enum StringAlign { Left, Center, Right }

        /// <summary>
        /// 字符串排版工具
        /// </summary>
        public class StringAligner
        {
            public StringAligner(int width, bool toTruncate = true, StringAlign alignPolicy = StringAlign.Center, char spaceHolder = ' ')
            {
                this.width = width;
                this.toTruncate = toTruncate;
                this.alignPolicy = alignPolicy;
                this.spaceHolder = spaceHolder;
            }
            /*  属性 */
            public int width { get; set; }
            public StringAlign alignPolicy
            {
                get { return _alignPolicy; }
                set
                {
                    switch (value)
                    {
                        case StringAlign.Left:
                        case StringAlign.Right:
                        case StringAlign.Center:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(string.Format(
                                "[FATAL] StringAligner.alignPolicy.set : Currently do not support align policy ( {0} ).",
                                value
                                ));
                    }
                    _alignPolicy = value;
                }
            }
            public char spaceHolder { get; set; }
            public bool toTruncate { get; set; }
            /*  静态方法 */
            public static string GetFiller(int nSpaces, char spaceHolder = ' ')
            {
                string filler = "";
                for (int ix = 0; ix < nSpaces; ++ix)
                    filler += spaceHolder;
                return filler;
            }
            public static string GetFiller(int nSpaces, string spaceHolder)
            {
                string filler = "";
                for (int ix = 0; ix < nSpaces; ++ix)
                    filler += spaceHolder;
                return filler;
            }
            public static string Fill(string str, int fillLength, char spaceHolder = ' ')
            {
                if (str.Length >= fillLength) return str;
                string spaces = "";
                for (int ix = 0; ix < fillLength - str.Length; ++ix) spaces += spaceHolder;
                return (str + spaces);
            }
            /*  接口 */
            public string align(string str)
            {
                if (width == str.Length) return str;
                if (toTruncate)
                {
                    switch (alignPolicy)
                    {
                        case StringAlign.Center:
                            return alignCenter(str);
                        case StringAlign.Left:
                            return alignLeft(str);
                        case StringAlign.Right:
                            return alignRight(str);
                        default:
                            throw new ArgumentIllegalException(string.Format(
                                "[FATAL] ConsoleDisplayer.align : Align method ( {0} ) unknown.",
                                alignPolicy));
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            public string alignCenter(string str)
            {
                int whatsleft = width - str.Length;
                int odd = whatsleft % 2;
                int left = whatsleft / 2;
                int right = whatsleft / 2 + odd;
                if (width > str.Length)
                {
                    string output = GetFiller(left, spaceHolder) + str;
                    return output + GetFiller(right, spaceHolder);
                }
                else // width < str.Length
                {
                    return str.Substring(-left, width);
                }
            }
            public string alignLeft(string str)
            {
                if (width > str.Length)
                {
                    return str + GetFiller(width - str.Length, spaceHolder);
                }
                else // width < str.Length
                {
                    return str.Substring(0, width);
                }
            }
            public string alignRight(string str)
            {
                int whatsleft = width - str.Length;
                if (width > str.Length)
                {
                    return GetFiller(whatsleft, spaceHolder) + str;
                }
                else // width < str.Length
                {
                    return str.Substring( -whatsleft, width);
                }
            }
            /*  内部组件 */
            StringAlign _alignPolicy;
        }

        /// <summary>
        /// 终端显示排版工具
        /// </summary>
        public class ConsoleDisplayer
        {
            /*  创建 */
            public ConsoleDisplayer(
                string lineBreaker = "\n",
                char spaceHolder = ' ',
                bool replaceTabs = false, 
                int nSpacesForTab = 4
                )
            {
                this.lineBreaker = lineBreaker;
                this.spaceHolder = spaceHolder;
                this.nSpacesForTab = nSpacesForTab;
                this.toReplaceTabs = replaceTabs;
            }
            /*  属性 */
            public bool toReplaceTabs
            {
                get { return _replaceTabs; }
                set
                {
                    _replaceTabs = value;
                    if (true == _replaceTabs)
                        RegenSpaceDancer();
                    else
                        _dancer = "\t";
                }
            }
            public int nSpacesForTab
            {
                get { return _nSpacesForTab; }
                set
                {
                    _nSpacesForTab = value;
                    if (_replaceTabs)
                        RegenSpaceDancer();
                }
            }
            public string lineBreaker
            {
                get { return _lineBreaker; }
                set
                {
                    if ("\n" == value || "\n\r" == value)
                        _lineBreaker = value;
                    else
                        throw new ArgumentIllegalException("Line breaker can only be \"\\n\" or \"\\n\\r\"");
                }
            }
            public char spaceHolder
            {
                get { return _spaceHolder; }
                set
                {
                    _spaceHolder = value;
                    if (_replaceTabs)
                        RegenSpaceDancer();
                }
            }
            /*  接口 */
            /// <summary>
            /// 将字符串整体后移
            /// </summary>
            /// <param name="str"></param>
            /// <param name="ntabs">后移的 tab 数目</param>
            /// <returns></returns>
            public string Retraction(string str, int ntabs)
            {
                string filler = GetFiller(ntabs);
                string output = filler + str;
                bool lastIsLineBreaker = str.EndsWith(lineBreaker);
                output = output.Replace(lineBreaker, lineBreaker + filler);
                if (lastIsLineBreaker)
                    return output.Substring(0, output.LastIndexOf(lineBreaker) + lineBreaker.Length - 1);
                else
                    return output;
            }
            public string GetFiller()
            {
                return _dancer;
            }
            public string GetFiller(int ntabs)
            {
                return StringAligner.GetFiller(ntabs, _dancer);
            }
            /*  功能实现 */
            void RegenSpaceDancer()
            {
                _dancer = "";
                for (int ix = 0; ix < nSpacesForTab; ++ix)
                    _dancer += _spaceHolder;
            }
            /*  内部组件 */
            string _lineBreaker;
            char _spaceHolder;
            bool _replaceTabs = false;
            int _nSpacesForTab;
            string _dancer = "\t";
        }

        
    }

    namespace Exceptions
    {
        /// <summary>
        /// 入参非法错误
        /// </summary>
        public class ArgumentIllegalException : ArgumentException
        {
            public ArgumentIllegalException() : base() { }
            public ArgumentIllegalException(string message) : base(message) { }
        }

        
    }

    namespace Attributes
    {
        
        public class ParameterCheckAttribute : Attribute
        {
            public override bool Match(object obj)
            {
                return base.Match(obj);
            }
            public void CheckType(object obj)
            {
                IComparable data = obj as IComparable;
                if (null == data) throw new Exception();
            }
        }
    }

    namespace DataStructure
    {
        using Utilities;
        using Exceptions;
        using DMath = DAV.Math.Math;
        using SMath = System.Math;


        /*******************************************************************************************************************
         *  兄弟二叉树数据结构
         *  以下所有均为！未！指定 Comparer 的情况。若指定了 Comparer，优先考虑 Comparer.Compare 替代 IComparable.CompareTo
         *  特性：
         *      、需要实现 IComparable 接口
         *      、排序插入(Add)：
         *          <0 插入前
         *          >0 插入后
         *          ==0 插入为 BROTHER
         *      、删除(Remove)：
         *          ==0 删除
         *          Remove(T val)
         *              删除全部 brother
         *          Remove(T val, int num)
         *              num > 0 删除前面的 num 个 brother
         *              num < 0 删除后面的 num 个 brother
         *              num == 0 不删除
         *      、判断是否包含（Contains）
         *          ==0 判定包含
         *      、允许重复 Value 插入，不区分引用重复和值重复
         *      、不允许重复 BBTreeNode 插入
         ******************************************************************************************************************/

        /// <summary>
        /// 兄弟二叉树数据结构（BrotherBinaryTree）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class  BBTree<T> : IEnumerable, IQueryable
        {
            public BBTree() { }
            public BBTree(IComparer<T> comparer)
            {
                Comparer = comparer;
            }
            /*  属性 */
            public int TotalCount /* 元素总数量（算上重复的） */{ get { return total_count; } }
            public int Count /* 元素数量 */{ get { return count; } }
            public IComparer<T> Comparer /* 比较器 */{ get; } = null;
            public BBTreeNode<T> Root /* 根节点 */ { get { return root; } }
            public Type ElementType /* 元素类型 */
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            Expression IQueryable.Expression /* 表达式 */
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            public IQueryProvider Provider /* 提供者 */
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            /*  数据操作接口 */
            public void Add(T val)                  /* 添加值 */
            {
                //  接口检测
                IComparable comparator = val as IComparable;
                if (null == comparator) throw new Exception("[FATAL] BBTree.Add(T val) : val must implement IComparable interface.");
                
                //  添加新节点
                AddNode(new BBTreeNode<T>(val));
            }
            public void Add(BBTreeNode<T> node)     /* 添加节点 */
            {
                //  树内节点不可重复添加
                if (this == node.tree) return;
                //  接口检测
                IComparable comparator = node.value as IComparable;
                if (null == comparator) throw new Exception("[FATAL] BBTree.Add(T val) : val must implement IComparable interface.");
                //  添加节点
                AddNode(node);
            }
            public int  Remove(T val)               /* 删除该值的所有节点 */
            {
                if (null == val) return 0;
                if (null == root) return 0;
                return Remove(root, val);
            }
            public void Remove(T val, int num)      /* 删除该值的给定个数节点， num < 0 从后到前的 num 个节点 */
            {
                if (0 == num) return;
                if( 0 < num)
                {

                }
                else // num > 0
                {

                }
            }
            public void Remove(BBTreeNode<T> node)  /*  */
            {
                throw new NotImplementedException();
            }
            public void Clear()                     /*  */
            {

            }
            /*  数据查询接口 */
            public int Depth()  /* 树深度 */
            {
                return GetDepth(Root);
            }
            public BBTreeNode<T> Search(T val)  /* 获取参数对应的节点 */
            {
                if (null == val) return null;
                return GetNode(root,val);
            }
            public BBTreeNode<T> Match(T val)   /* 获取该参数节点（引用相等） */
            {
                if (null == val) return null;
                return GetExactNode(root, val);
            }
            public bool Contains(T val)         /* 是否存在值 */
            {
                return (null != GetNode(root,val));
            }
            public bool ContainsExactly(T val)  /* 是否存在该对象的引用（值对象结果可能异常） */
            {
                return (null != GetExactNode(root,val));
            }
            public bool Contains(BBTreeNode<T> node)    /* 是否存在该节点 */
            {
                return (this == node.tree);
            }
            public BBTreeNode<T> First()        /* 最小节点 */
            {
                if (null != root) return GetMostLeft(root);
                return null;
            }
            public BBTreeNode<T> Last()         /* 最大节点 */
            {
                if (null != root) return GetMostRight(root);
                return null;
            }
            /*  通用接口 */
            public IEnumerator GetEnumerator() /*  */
            {
                BBTreeNode<T> refnode = root;
                Stack<BBTreeNode<T>> stack = new Stack<BBTreeNode<T>>();
                bool isMovingIn = true;

                while (null != refnode)
                {
                    if (isMovingIn) //  深入探讨
                    {
                        while (true)
                        {
                            //  左非空，往左看
                            if (null != refnode.left)
                            {
                                stack.Push(refnode);
                                refnode = refnode.left;
                                continue;
                            }
                            //  看完左边看自己
                            foreach (T val in refnode)
                            {
                                yield return val;
                            }
                            //  看完自己看右边
                            if (null != refnode.right)
                            {
                                refnode = refnode.right;
                                continue;
                            }
                            //  左空、右空、看完自己，那么向上走
                            break;
                        }
                        //  如果左边看完、自己看完、右边看完，那么向上走
                        if (0 == stack.Count) break;
                        isMovingIn = false;
                        refnode = stack.Pop();
                    }
                    else // if (isMovingIn)  ...  真相浮出
                    {
                        //  向上走，先看自己
                        foreach (T val in refnode)
                        {
                            yield return val;
                        }
                        //  再看右边子树
                        if (null != refnode.right)
                        {
                            //  右边子树有内容，我们深入
                            refnode = refnode.right;
                            isMovingIn = true;
                        }
                        else
                        {
                            //  右边子树没有内容，我们继续往上跳
                            if (0 == stack.Count) break;
                            refnode = stack.Pop();
                        }
                    }
                }
            }
            public override string ToString()   /*  */
            {
                return FullTreeView(' ');
            }
            /*  功能接口 */
            public int Compare(BBTreeNode<T> left, BBTreeNode<T> right) /*  */
            {
                if (null != Comparer) return Comparer.Compare(left.value, right.value);
                return left.CompareTo(right);
            }
            public int Compare(T left, T right) /*  */
            {
                if (null != Comparer) return Comparer.Compare(left, right);
                return (left as IComparable).CompareTo(right);
            }
            public int Compare(T left, BBTreeNode<T> right) /*  */
            {
                if (null != Comparer) return Comparer.Compare(left, right.value);
                return (left as IComparable).CompareTo(right.value);
            }
            public int Compare(BBTreeNode<T> left, T right) /*  */
            {
                if (null != Comparer) return Comparer.Compare(left.value, right);
                return (left.value as IComparable).CompareTo(right);
            }
            /********************************************************************
             * 以下部分为私有组件，不提供给用户
             ********************************************************************/
            /*  内部组件 */
            BBTreeNode<T> root = null;
            int total_count = 0;
            int count /* 仅计算不重复的内容节点数 */ = 0;
            /*  保护 */
            /*  内部实现 */
            //  ToString 功能
            void GenNodeInfo(List<LinkedList<NodeInfo>> allNodeInfo, BBTreeNode<T> node, int depth = 0, int index = 0) /* 初始化节点信息列表，用于打印输出 */
            {
                if (null == node) return;
                allNodeInfo[depth].AddLast(new NodeInfo(node, depth, index));
                GenNodeInfo(allNodeInfo, node.Left, depth + 1, 2 * index);
                GenNodeInfo(allNodeInfo, node.Right, depth + 1, 2 * index + 1);
            }
            double ComputeFullRetraction(int index, int depthIx, int maxDepth) /* 
                计算每个节点的后撤长度（以节点宽度为单位）
                index 节点横向索引（0 ~ N-1，N = 2**depthIx - 1）
                depthIx 节点深度索引(0 ~ maxDepth-1)
                maxDepth 树的最大深度*/
            {
                if (depthIx == maxDepth - 1) return index;
                int rDepthIx = maxDepth - 1 - depthIx;
                return SMath.Pow(2, rDepthIx) * index + 0.5 * DMath.SumOfPower(2, 0, rDepthIx - 1);
            }
            string GetLeftArrow(int width)  /* 绘制左箭头 */
            {
                string output_str = "/";
                if (width > 1)
                {
                    for (int ix = 0; ix < width - 2; ++ix) output_str += "-";
                    output_str += "/";
                }
                return output_str;
            }
            string GetRightArrow(int width) /* 绘制右箭头 */
            {
                string output_str = "\\";
                if (width > 1)
                {
                    for (int ix = 0; ix < width - 2; ++ix) output_str += "-";
                    output_str += "\\";
                }
                return output_str;
            }
            string MergeStringList(List<string> list, string append = "") /* 将字符串列表拼接成一个整体，每个字符串后添加一个字符串 */
            {
                string str = "";
                foreach (var s in list)
                {
                    str = str + (s + append);
                }
                return str;
            }
            string FullTreeView(char spaceHolder = ' ') /* 显示二叉树的完整 VIEW，不会根据节点节约空间 */
            {
                string title = string.Format("BBTree[{0}]", typeof(T));
                string output_str = "";
                if (0 == total_count)
                {
                    output_str = title + " is empty.";
                    return output_str;
                }
                if (1 == total_count)
                {
                    output_str = title + string.Format(" has only root : {0}.", root);
                    return output_str;
                }
                int depth = Depth();
                int width = ((int)SMath.Max(First().ToString().Length, Last().ToString().Length)) / 2 * 2 + 4; // + (int)Math.Log10(depth) + (int)Math.Log10(Count);  //  以此作为统一的长度
                StringAligner align = new StringAligner(width, alignPolicy: StringAlign.Center, spaceHolder: ' ');
                List<string> levelStrings = new List<string>(2 * depth - 1);
                List<LinkedList<NodeInfo>> allNodeInfo = new List<LinkedList<NodeInfo>>(depth);
                //  initialize level strings && all node information
                for (int ix = 0; ix < 2 * depth - 1; ++ix) levelStrings.Add("");
                for (int ix = 0; ix < depth; ++ix) allNodeInfo.Add(new LinkedList<NodeInfo>());
                //  Gen node informations (depth, index)
                GenNodeInfo(allNodeInfo, Root);
                //  Generate string
                int strLenMax = 0;
                foreach (var level in allNodeInfo)
                {
                    foreach (var node in level)
                    {
                        //  retraction for this node
                        double retraction = ComputeFullRetraction(node.index, node.depth, depth) * width;
                        int retraction_apply = (int)retraction;
                        levelStrings[2 * node.depth] = StringAligner.Fill(levelStrings[2 * node.depth], retraction_apply, spaceHolder);
                        levelStrings[2 * node.depth] = levelStrings[2 * node.depth].Insert(retraction_apply, align.align(node.node.ToString()));
                        //  refresh max length
                        if (strLenMax < retraction_apply + width) strLenMax = retraction_apply + width;
                        //  add arrows
                        if (node.depth != depth - 1)
                        {
                            // left arrow
                            if (null != node.node.left)
                            {
                                double retraction_left_node = ComputeFullRetraction(2 * node.index, node.depth + 1, depth) * width;
                                int left_arrow_start = (int)retraction_left_node + width / 2;
                                int left_arrow_end = retraction_apply + width / 2;
                                int leftArrowWidth = left_arrow_end - left_arrow_start + 1;
                                string leftArrow = GetLeftArrow(leftArrowWidth);
                                levelStrings[2 * node.depth + 1] = StringAligner.Fill(levelStrings[2 * node.depth + 1], left_arrow_start, spaceHolder);
                                levelStrings[2 * node.depth + 1] = levelStrings[2 * node.depth + 1].Insert(left_arrow_start, leftArrow);
                            }
                            // right arrow
                            if (null != node.node.right)
                            {
                                double retraction_right_node = ComputeFullRetraction(2 * node.index + 1, node.depth + 1, depth) * width;
                                int right_arrow_start = retraction_apply + width / 2 + 1;
                                int right_arrow_end = (int)retraction_right_node + width / 2;
                                int rightArrowWidth = right_arrow_end - right_arrow_start + 1;
                                string rightArrow = GetRightArrow(rightArrowWidth);
                                levelStrings[2 * node.depth + 1] = StringAligner.Fill(levelStrings[2 * node.depth + 1], right_arrow_start, spaceHolder);
                                levelStrings[2 * node.depth + 1] = levelStrings[2 * node.depth + 1].Insert(right_arrow_start, rightArrow);
                            }
                        }
                    }
                }

                string sharpFiller = StringAligner.GetFiller((strLenMax - title.Length - 10) / 2 + 1, '#');

                output_str += string.Format("{1} BEGIN : {0} {1}\n", title, sharpFiller);
                output_str += MergeStringList(levelStrings, "\n");
                output_str += string.Format("{1}  END  : {0} {1}\n", title, sharpFiller);

                return output_str;
            }
            //  Size Banlanced Tree 特性
            void LeftRotate(BBTreeNode<T> node)     /* 左旋操作， 节点限制 ： 1、本树节点； 2、非空节点 */
            {
                if (null == node) return;
                // get info
                BBTreeNode<T> right = node.right;
                if (null == right) return;

                BBTreeNode<T> right_left = right.left;
                // operation
                if (root == node) root = right;
                else node.SetParentChild(right);

                right.left = node;
                right.size = node.size;

                node.right = right_left;
                node.size = node.GetSizeFromChildren();
            }
            void RightRotate(BBTreeNode<T> node)    /* 右旋操作， 节点限制 ： 1、本树节点； 2、非空节点 */
            {
                if (null == node) return;

                BBTreeNode<T> left = node.left;
                if (null == left) return;

                BBTreeNode<T> left_right = left.right;

                if (root == node) root = left;
                else node.SetParentChild( left );

                left.right = node;
                left.size = node.size;

                node.left = left_right;
                node.size = node.GetSizeFromChildren();
            }
            void LeftRotateTree()   /* 整个树的左旋操作，从根节点开始。 单独重写是因为此方法不使用 parent 属性 */
            {
                if (null == root) return;
                BBTreeNode<T> pivot = root;
                BBTreeNode<T> right = pivot.right;
                if (null == right) return;

                BBTreeNode<T> right_left = right.left;

                root = right;

                right.left = pivot;
                right.size = pivot.size;

                pivot.right = right_left;
                pivot.size = pivot.GetSizeFromChildren();
            }
            void RightRotateTree()  /* 整个树的右旋操作，从根节点开始。 单独重写是因为此方法不使用 parent 属性 */
            {
                if (null == root) return;
                BBTreeNode<T> pivot = root;
                BBTreeNode<T> left = pivot.left;
                if (null == left) return;

                BBTreeNode<T> left_right = left.right;

                root = left;

                left.right = pivot;
                left.size = pivot.size;

                pivot.left = left_right;
                pivot.size = pivot.GetSizeFromChildren();
            }
            void Maintain(BBTreeNode<T> node, bool focusOnLeft) /* 维护 SBT 特性的 Size */
            {
                /*  这里特别需要注意的与原文中的区别在哪里？
                 *      原文中若该 node 不存在，那么得到的节点 size 应该为 0
                 *      所以不能直接返回，而需要继续 maintain
                 */
                if (null == node) return;
                if (focusOnLeft)
                {
                    if (node.SizeLeftLeft() > node.SizeRight())
                    { 
                        RightRotate(node);
                    }
                    else if (node.SizeLeftRight() > node.SizeRight())
                    {
                        LeftRotate(node.left);
                        RightRotate(node);
                    }
                    else return;
                }
                else
                {
                    if (node.SizeRightRight() > node.SizeLeft())
                    {
                        LeftRotate(node);
                    }
                    else if (node.SizeRightLeft() > node.SizeLeft())
                    {
                        RightRotate(node.right);
                        LeftRotate(node);
                    }
                    else return;
                }
                Maintain(node.left, true);
                Maintain(node.right, false);
                Maintain(node, true);
                Maintain(node, false);
            }
            //  树的增删
            bool AddRoot(BBTreeNode<T> node)        /* 添加根节点 */
            {
                /*  第一个，作为根节点 */
                if (0 == total_count)
                {
                    //  添加为根节点
                    root = node;
                    node.size = 1;
                    //  更新统计信息
                    total_count = 1;
                    count = 1;
                    return true;
                }
                return false;
            }
            void AddNodeRepMeth(BBTreeNode<T> node) /* 添加一个可用节点【循环方式】 */
            {
                if (null == node) return;
                node.tree = this;
                /*  第一个，作为根节点 */
                if (AddRoot(node)) return;
                /*  按照其位置进行插入 */
                BBTreeNode<T> refnode = root;
                ++refnode.size;
                int comparision = 0;
                while (true)    /*  这里保持通过循环查找的插入方式，原因是我们的比较需要使用 Comparer，递归方法需要多次转换 */
                {
                    //  完成比较
                    comparision = Compare(node, refnode);
                    //  根据对比结果向下查找
                    if (comparision > 0)
                    {
                        if (null != refnode.right)
                        {   //  向前寻找
                            refnode = refnode.right;
                            ++refnode.size;
                        }
                        else
                        {   //  前插
                            refnode.right = node;
                            node.size = 1;
                            break;
                        }
                    }
                    else if (comparision < 0)
                    {
                        if (null != refnode.left)
                        {   //  向后寻找
                            refnode = refnode.left;
                            ++refnode.size;
                        }
                        else
                        {   //  后插
                            refnode.left = node;
                            node.size = 1;
                            break;
                        }
                    }
                    else // comparision == 0
                    {   //  插入兄弟链条末尾
                        refnode.AddBrother(node);
                        node.size = 0;
                        --count;
                        break;
                    }
                }
                //  计数
                ++total_count;
                ++count;
                //  Maintain
                /* //  way 1 : push into stack before maintain
                Dictionary<BBTreeNode<T>, bool> dict = new Dictionary<BBTreeNode<T>, bool>();
                BBTreeNode<T> p = node;
                while (null != (p = p.parent))
                    dict.Add(p, Compare(node, p) < 0);
                foreach (var pair in dict)
                    Maintain(pair.Key, pair.Value);
                */
                //  way 2 : one way straight up
                BBTreeNode<T> p = node;
                while (null != (p = p.parent))
                {
                    Maintain(p, Compare(node, p) < 0);
                }
            }
            void AddNode(BBTreeNode<T> node)        /* 递归方法插入节点 */
            {
                node.tree = this;
                /*  第一个，作为根节点 */
                if (AddRoot(node)) return;
                //  Add && maintain
                Insert(root, node);
                //  count
                ++total_count;
                ++count;
            }
            void Insert(BBTreeNode<T> searchNode, BBTreeNode<T> nodeToAdd)  /* 【不直接调用】递归方法插入节点的底层操作，
                【不直接调用】仅作为 AddRecursively(BBTreeNode<T> node) 的底层辅助方法 */
            {
                ++searchNode.size;
                int comp = Compare(nodeToAdd, searchNode);
                if (0 == comp)
                {
                    searchNode.AddBrother( nodeToAdd );
                    FixParentsSize(searchNode, -1);
                    --count;
                    return;
                }
                if (comp > 0)
                {
                    if (null != searchNode.right)
                    {
                        Insert(searchNode.right, nodeToAdd);
                        Maintain(searchNode, false);
                    }
                    else
                    {
                        searchNode.right = nodeToAdd;
                        nodeToAdd.size = 1;
                    }
                }
                else    //  comp < 0
                {
                    if (null != searchNode.left)
                    {
                        Insert(searchNode.left, nodeToAdd);
                        Maintain(searchNode, true);
                    }
                    else
                    {
                        searchNode.left = nodeToAdd;
                        nodeToAdd.size = 1;
                    }
                }
            }
            void Move(BBTreeNode<T> searchNode, BBTreeNode<T> nodeToMove)   /* 【不直接调用】递归方法移动节点，用于删除时，两侧节点均存在时候的操作，
                【不直接调用】仅作为 Remove(BBTreeNode<T> searchNode,  T val) 底层辅助方法，不考虑相等的情况，因为在应用场景不存在；解决思路——报错 */
            {
                int comp = Compare(nodeToMove, searchNode);
                searchNode.size += nodeToMove.size;
                if (comp > 0)
                {
                    if(null != searchNode.right)
                        Move(searchNode.right, nodeToMove);
                    else
                        searchNode.right = nodeToMove;
                    Maintain(searchNode, false);
                }
                else if(comp < 0)
                {
                    if (null != searchNode.left)
                        Move(searchNode.left, nodeToMove);
                    else
                        searchNode.left = nodeToMove;
                    Maintain(searchNode, true);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("[FATAL] DAV.DataStructure.BBTree.Move(BBTreeNode<T> searchNode, BBTreeNode<T> nodeToMove) : node to move not have brothers in tree.");
                }

            }
            int  Remove(BBTreeNode<T> searchNode,  T val)       /* 递归方法删除数据（删除该值的全部数据） */
            {
                if (null == searchNode) return 0;
                int comp = Compare(val, searchNode.value);
                int nRemoved = 0;
                if (comp > 0)
                {
                    nRemoved = Remove(searchNode.right, val);
                    Maintain(searchNode, true);
                }
                else  if(comp < 0)
                {
                    nRemoved = Remove(searchNode.left, val);
                    Maintain(searchNode, false);
                }
                else // if (0 == comp)
                {
                    //  update count
                    nRemoved = searchNode.BrotherCount;
                    --count;
                    total_count -= nRemoved;
                    //  update size
                    FixParentsSize(searchNode, -1);
                    //  remove node
                    if (searchNode.IsLeaf)
                    {
                        if (searchNode.IsRoot) root = null;
                        else searchNode.SetParentChild(null);
                    }
                    else if (searchNode.IsFull)
                    {
                        if (searchNode.left.size < searchNode.right.size)
                        {   //  左侧子树更小，那么把左子树添加到右子树的最前端；同时，右子树替代原来的节点，成为子树的顶层
                            if (searchNode.IsRoot) root = searchNode.right;
                            else searchNode.SetParentChild(searchNode.right);
                            //  移动
                            Move(searchNode.right, searchNode.left);
                        }
                        else
                        {   //  右侧子树更小，那么把右子树添加到左子树的最后端； 同时，左子树替代原来的节点，成为子树的顶层
                            if (searchNode.IsRoot) root = searchNode.left;
                            else searchNode.SetParentChild(searchNode.left);
                            //  移动
                            Move(searchNode.left, searchNode.right);
                        }
                    }   // if (searchNode.IsFull)
                    else if (null != searchNode.left)
                    {
                        if (searchNode.IsRoot) root = searchNode.left;
                        else searchNode.SetParentChild(searchNode.left);
                    }
                    else if (null != searchNode.right)
                    {
                        if (searchNode.IsRoot) root = searchNode.right;
                        else searchNode.SetParentChild(searchNode.right);
                    }
                    // dismiss brothers
                    searchNode.Dismiss();
                }
                return nRemoved;
            }
            void FixParentsSize(BBTreeNode<T> bottom, int fixVal)   /* include bottom */
            {
                BBTreeNode<T> p = bottom;
                do { p.size += fixVal; }
                while (null != (p = p.parent));
            }
            //  查询
            BBTreeNode<T> GetNode(BBTreeNode<T> searchNode, T val)
            {
                if (null == searchNode) return null;

                int comp = Compare(val, searchNode.value);

                if (0 == comp) return searchNode;
                else if (comp < 0) return GetNode(searchNode.left, val);
                else /*comp>0*/ return GetNode(searchNode.right, val);
            }
            BBTreeNode<T> GetExactNode(BBTreeNode<T> searchNode, T val)
            {
                if (null == searchNode) return null;

                int comp = Compare(val, searchNode.value);

                if (0 == comp) return searchNode.GetExactBrother(val);
                else if (comp < 0) return GetExactNode(searchNode.left, val);
                else /*comp>0*/ return GetExactNode(searchNode.right, val);

            }
            BBTreeNode<T> GetNodeRepMeth(T val) /*  */
            {
                BBTreeNode<T> refnode = root;
                while (true)
                {
                    int comp = Compare(val, refnode.value);
                    if (0 == comp)
                        return refnode;
                    else if (comp > 0)
                    {
                        if (null != refnode.right)
                            refnode = refnode.right;
                        else
                            return null;
                    }
                    else if (comp < 0)
                    {
                        if (null != refnode.left)
                            refnode = refnode.left;
                        else
                            return null;
                    }
                }
            }
            BBTreeNode<T> GetExactNodeRepMeth(T val) /*  */
            {
                BBTreeNode<T> exact = GetNodeRepMeth(val);
                if (null == exact) return null;
                while (null != exact)
                {
                    if (ReferenceEquals(val, exact.value))
                        return exact;
                    else
                        exact = exact.nextBrother;
                }
                return null;
            }
            BBTreeNode<T> GetMostLeft(BBTreeNode<T> node)       /* 获取当前节点最左端节点的位置 */
            {
                if (null != node.left) return GetMostLeft(node.left);
                return node;
            }
            BBTreeNode<T> GetMostRight(BBTreeNode<T> node)      /* 获取当前节点最右端节点的位置 */
            {
                if (null != node.right) return GetMostRight(node.right);
                return node;
            }
            int GetDepth(BBTreeNode<T> node, int refDepth = 0)  /* 获取某节点能延伸的最大深度。 refDepth -- 该节点在当前树的深度索引*/
            {
                if (null == node) return refDepth;
                return SMath.Max(GetDepth(node.left, refDepth + 1), GetDepth(node.right, refDepth + 1));
            }
            class NodeInfo  /*  用于转换为字符串时的信息统计 */
            {
                public NodeInfo(BBTreeNode<T> node, int depth, int index)
                {
                    this.node = node;
                    this.depth = depth;
                    this.index = index;
                }
                public NodeInfo(BBTreeNode<T> node, int depth, int start, int end)
                {
                    this.node = node;
                    this.depth = depth;
                    this.start = start;
                    this.end = end;
                }
                public BBTreeNode<T> node { get; set; }
                public int depth { get; set; }
                public int index { get; set; }
                public int start { get; set; }
                public int end { get; set; }
            }
        }

       


        /// <summary>
        /// 二叉树节点数据结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class BBTreeNode<T> : IComparable, IEnumerable
        {
            /*  通用接口 */
            public int CompareTo(object obj)
            {
                //  支持与 T 的比较，以及 BinaryTreeNode<T> 的比较
                //  获取比较器
                IComparable comparator = value as IComparable;
                //  先尝试以 节点 结构比较
                BBTreeNode<T> node = obj as BBTreeNode<T>;
                if(null != node) return comparator.CompareTo(node.value);
                //  再尝试以 内容 结构比较
                return comparator.CompareTo(obj);
            }
            public IEnumerator GetEnumerator()
            {
                BBTreeNode<T> node = FirstBrother();
                yield return node.value;
                while(null !=(node = node.nextBrother)) yield return node.value;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override string ToString()
            {
                return string.Format("S{0} {1}", size, value);
            }
            /*  二叉树特性接口 */
            /*  brother 特性接口*/

            /*  二叉树属性 */
            public T Value { get { return value; } }
            public BBTree<T> Tree { get { return tree; } }
            public BBTreeNode<T> Root
            {
                get
                {
                    if (null == tree) return null;
                    return tree.Root;
                }
            }
            public int Size { get { return FirstBrother().size; } }
            public BBTreeNode<T> Parent { get { return FirstBrother().parent; } }
            public BBTreeNode<T> Left { get { return FirstBrother().left; } }
            public BBTreeNode<T> Right { get { return FirstBrother().right; } }
            public bool IsLeaf { get { return (null == Left && null == Right); } }
            public bool IsFull  /* 儿孙满堂 */ { get { return (null != Left && null != Right); } }
            public bool IsRoot { get { return (null == Parent); } }
            public bool IsSubTree { get { return (null != Right && null != Left); } }
            public bool IsRightChild    /* 判定是否为父亲的右子节点 */
            {
                get
                {
                    if (IsRoot) return false;
                    if (this == Parent.Right) return true;
                    return false;
                }
            }
            public bool IsLeftChild     /* 判定是否为父亲的左子节点 */
            {
                get
                {
                    if (IsRoot) return false;
                    if (this == Parent.Left) return true;
                    return false;
                }
            }
            /*  Brother 属性 */
            public int BrotherCount { get { return FirstBrother().brotherCount; } }
            public BBTreeNode<T> NextBrother { get { return nextBrother; } }
            public BBTreeNode<T> PreviousBrother { get { return previousBrother; } }
            /*  二叉树特性操作属性 */
            internal T value { get; set; }
            internal BBTree<T> tree { get; set; } = null;
            internal int size { get; set; } = 0;
            internal BBTreeNode<T> parent { get { return _parent; } }
            internal BBTreeNode<T> left
            {
                get { return _left; }
                set
                {
                    if (value == _left) return;
                    if (null != _left && this == _left._parent)
                        _left._parent = null;
                    _left = value;
                    if (null != _left)
                        _left._parent = this;
                }
            }
            internal BBTreeNode<T> right
            {
                get { return _right; }
                set
                {
                    if (value == _right) return;
                    if (null != _right && this == _right._parent)
                        _right._parent = null;
                    _right = value;
                    if (null != _right)
                        _right._parent = this;
                }
            }
            /*  brother 特性操作属性*/
            internal int brotherCount { get { return _brotherCount; } }
            internal BBTreeNode<T> nextBrother
            {
                get { return _nextBrother; }
            }
            internal BBTreeNode<T> previousBrother
            {
                get { return _previousBrother; }
            }
            /*  brother 特性内部接口*/
            internal BBTreeNode<T> FirstBrother()
            {
                BBTreeNode<T> node = this;
                while(node._previousBrother != null)
                {
                    node = node._previousBrother;
                }
                return node;
            }
            internal BBTreeNode<T> LastBrother()
            {
                BBTreeNode<T> node = this;
                while (node._nextBrother != null)
                {
                    node = node._nextBrother;
                }
                return node;
            }
            internal void AddBrother(BBTreeNode<T> bro)     /* 不考虑 bro 可能存在的链表，仅添加当前兄弟到列表 */
            {
                if (!bro.IsAlone()) throw new ArgumentIllegalException(
                    "[FATAL] DAV.DataStructure.BBTreeNode<T>.AddBrother(BBTreeNode<T> bro) : bro must be alone.");
                //  shake hands
                Contact(LastBrother(), bro);
                //  count
                ++FirstBrother()._brotherCount;
            }
            internal void JointBrotherhood(BBTreeNode<T> fb)    /* 与别的兄弟会结盟，当然我还是老大 */
            {
                throw new NotImplementedException();
            }
            internal bool RemoveBrother(BBTreeNode<T> bro)
            {
                throw new NotImplementedException();
            }
            internal int RemoveBrother(int num) /* num < 0 从尾部删除 abs(num) 个，返回删除个数（>0） */
            {

                return 0;
            }
            internal BBTreeNode<T> GetExactBrother(T val)
            {
                BBTreeNode<T> bro = FirstBrother();
                do
                {
                    if (ReferenceEquals(val, bro.value)) return bro;
                }
                while (null != (bro = bro._nextBrother));

                return null;
            }
            /*  内部使用接口 */
            internal BBTreeNode(T value)
            {
                //  非空
                if (null == value) throw new Exception("[FATAL]  BinaryTreeNode<T>.BinaryTreeNode(T value) : Null value can't create a proper node.");
                //  确保 IComparable 接口实现
                IComparable comparator = value as IComparable;
                if (null == comparator) throw new Exception(string.Format("[FATAL]  BinaryTreeNode<T>.BinaryTreeNode(T value) : Type {0} should implement IComparable interface.", typeof(T)));
                this.value = value;
                _brotherCount = 1;
            }
            internal void Dismiss()     /* 遣散整个兄弟会 */
            {
                BBTreeNode<T> bro = FirstBrother();
                BBTreeNode<T> next = bro._nextBrother;
                while(next != null)
                {
                    bro.Dispose();
                    bro = next;
                    next = bro._nextBrother;
                }
                bro.Dispose();
            }
            internal void Usurp()       /* 从老大那里夺取兄弟会信息，并干掉老大 */
            {
                BBTreeNode<T> first = FirstBrother();
                //  树相关信息
                tree = first.tree;
                size = first.size;
                _parent = first._parent;
                _left = first._left;
                _right = first._right;
                //  链表相关
                _brotherCount = first._brotherCount - 1;
                Contact(_previousBrother, _nextBrother);
                Contact(this, first._nextBrother);
            }
            internal int SizeLeftRight()
            {
                if (null != left && null != left.right) return left.right.size;
                return 0;
            }
            internal int SizeRightLeft()
            {
                if (null != right && null != right.left) return right.left.size;
                return 0;
            }
            internal int SizeLeftLeft()
            {
                if (null != left && null != left.left) return left.left.size;
                return 0;
            }
            internal int SizeRightRight()
            {
                if (null != right && null != right.right) return right.right.size;
                return 0;
            }
            internal int SizeRight()
            {
                if (null != right) return right.size;
                return 0;
            }
            internal int SizeLeft()
            {
                if (null != left) return left.size;
                return 0;
            }
            internal int GetSizeFromChildren()
            {
                if (null != left && null != right)
                    return left.size + right.size + 1;
                else if (null != right)
                    return right.size + 1;
                else if (null != left)
                    return left.size + 1;
                else
                    return 1;
            }
            internal bool SetParentChild(BBTreeNode<T> node)
            {
                if (IsLeftChild)
                {
                    Parent.left = node;
                    return true;
                }
                else if (IsRightChild)
                {
                    Parent.right = node;
                    return true;
                }
                else return false;
            }
            /*  SBT 特性私有组件 */
            BBTreeNode() { }
            BBTreeNode<T> _parent = null;
            BBTreeNode<T> _left = null;
            BBTreeNode<T> _right = null;
            /*  brother 特性私有组件*/
            int _brotherCount = 0;
            BBTreeNode<T> _nextBrother = null;
            BBTreeNode<T> _previousBrother = null;
            /*  底层功能实现 */
            bool IsAlone()  /* 判定是否为孤胆英雄 */
            {
                return (
                    1 == _brotherCount
                    && null == _nextBrother
                    && null == _previousBrother
                    );
            }
            void Dispose()  /* 清场，破坏性极大，谨慎驾驶 */
            {
                value = default(T);
                tree = null;
                size = 0;
                _parent = null;
                _left = null;
                _right = null;
                _brotherCount = 0;
                _nextBrother = null;
                _previousBrother = null;
            }
            void Contact(BBTreeNode<T> pre, BBTreeNode<T> post) /* 将两个兄弟联络起来 */
            {
                if(null != pre) pre._nextBrother = post;
                if(null != post) post._previousBrother = pre;
            }
        }
        
        /// <summary>
        /// standard Sized Balanced Tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class SBTree<T>
        {
            public SBTree() { throw new NotImplementedException(); }
        }

    }


}

