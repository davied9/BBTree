﻿using System;
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
                AddNodeRecur(new BBTreeNode<T>(val));
            }
            public void Add(BBTreeNode<T> node)     /* 添加节点 */
            {
                //  树内节点不可重复添加
                if (this == node.tree) return;
                //  接口检测
                IComparable comparator = node.value as IComparable;
                if (null == comparator) throw new Exception("[FATAL] BBTree.Add(T val) : val must implement IComparable interface.");
                //  添加节点
                AddNodeRecur(node);
            }
            public int  Remove(T val)               /* 删除该值的所有节点 */
            {
                if (null == val) return 0;
                BBTreeNode<T> node = GetNode(val);
                if (null == node) return 0;


                return 0;
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
            public void LeftRotate()                /*  */
            {
                LeftRotateTree();
            }
            public void RightRotate()               /*  */
            {
                RightRotateTree();
            }
            /*  数据查询接口 */
            public int Depth()  /* 树深度 */
            {
                return GetDepth(Root);
            }
            public BBTreeNode<T> Search(T val)  /* 获取参数对应的节点 */ { return GetNode(val); }
            public BBTreeNode<T> Match(T val)   /* 获取该参数节点（引用相等） */ { return GetExactNode(val); }
            public bool Contains(T val)         /* 是否存在值 */
            {
                return (null != GetNode(val));
            }
            public bool ContainsExactly(T val)  /* 是否存在该对象的引用（值对象结果可能异常） */
            {
                return (null != GetExactNode(val));
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
                else if (node == node.parent.left) node.parent.left = right;
                else /*if (node == node.parent.right)*/ node.parent.right = right;

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
                else if (node == node.parent.left) node.parent.left = left;
                else /*if (node == node.parent.right)*/ node.parent.right = left;

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
                    if (node.SizeRightLeft() > node.SizeLeft())
                    {
                        LeftRotate(node);
                    }
                    else if (node.SizeRightRight() > node.SizeLeft())
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
            void AddNode(BBTreeNode<T> node)        /* 添加一个可用节点【循环方式】 */
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
                        refnode.lastBrother.nextBrother = node;
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
            void AddNodeRecur(BBTreeNode<T> node)   /* 递归方法插入节点 */
            {
                node.tree = this;
                /*  第一个，作为根节点 */
                if (AddRoot(node)) return;
                //  Add && maintain
                InsertRecur(root, node);
                //  count
                ++total_count;
                ++count;
            }
            void InsertRecur(BBTreeNode<T> searchNode, BBTreeNode<T> nodeToAdd) /* 【不直接调用】递归方法插入节点的底层操作，仅作为 AddRecursively(BBTreeNode<T> node) 的底层辅助方法 */
            {
                int comp = Compare(nodeToAdd, searchNode);
                if (0 == comp)
                {
                    searchNode.lastBrother.nextBrother = nodeToAdd;
                    nodeToAdd.size = 0;
                    --count;
                    return;
                }
                ++searchNode.size;
                if (comp > 0)
                {
                    if (null != searchNode.right)
                    {
                        InsertRecur(searchNode.right, nodeToAdd);
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
                        InsertRecur(searchNode.left, nodeToAdd);
                        Maintain(searchNode, true);
                    }
                    else
                    {
                        searchNode.right = nodeToAdd;
                        nodeToAdd.size = 1;
                    }
                }
            }
            int  RemoveRecur(BBTreeNode<T> searchNode,  T val)   /* 递归方法删除数据（删除该值的全部数据） */
            {
                if (null == searchNode) return 0;
                int comp = (val as IComparable).CompareTo(searchNode.value);
                int res = 0;
                if(0 == comp)
                {
                    res = searchNode.BrotherCount;
                    if (searchNode.IsLeaf)
                    {
                        if (searchNode.IsRoot) root = null;
                        else if (searchNode.IsLeftChild) searchNode.parent.left = null;
                        else /*if (searchNode.IsRightChild)*/ searchNode.parent.right = null;
                    }
                    else if (null != searchNode.left)
                    {
                        if (searchNode.IsRoot) root = searchNode.left;
                        else if (searchNode.IsLeftChild) searchNode.parent.left = searchNode.left;
                        else /*if (searchNode.IsRightChild)*/ searchNode.parent.right = searchNode.left;
                    }
                    else if (null != searchNode.right)
                    {
                        if (searchNode.IsRoot) root = searchNode.right;
                        else if (searchNode.IsLeftChild) searchNode.parent.left = searchNode.right;
                        else /*if (searchNode.IsRightChild)*/ searchNode.parent.right = searchNode.right;
                    }
                    else /* null != searchNode.left && null != searchNode.right */
                    {


                    }
                    searchNode.tree = null;
                }
                else if (comp >0)
                {
                    res = RemoveRecur(searchNode.right, val);
                }
                else    //  comp < 0
                {
                    res = RemoveRecur(searchNode.left, val);
                }
                if (0 != res) searchNode.size -= res;
                return res;
            }
            //  查询
            BBTreeNode<T> GetNode(T val) /*  */
            {
                BBTreeNode<T> refnode = root;
                while (true)
                {
                    int comparision = Compare(val, refnode.value);
                    if (0 == comparision)
                        return refnode;
                    else if (comparision > 0)
                    {
                        if (null != refnode.right)
                            refnode = refnode.right;
                        else
                            return null;
                    }
                    else if (comparision < 0)
                    {
                        if (null != refnode.left)
                            refnode = refnode.left;
                        else
                            return null;
                    }
                }
            }
            BBTreeNode<T> GetExactNode(T val) /*  */
            {
                BBTreeNode<T> exact = GetNode(val);
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
            BBTreeNode<T> GetMostLeft(BBTreeNode<T> node)   /* 获取当前节点最左端节点的位置 */
            {
                if (null != node.left) return GetMostLeft(node.left);
                return node;
            }
            BBTreeNode<T> GetMostRight(BBTreeNode<T> node)  /* 获取当前节点最右端节点的位置 */
            {
                if (null != node.right) return GetMostLeft(node.right);
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
                BBTreeNode<T> node = firstBrother;
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
            public int Size { get { return firstBrother.size; } }
            public BBTreeNode<T> Parent { get { return firstBrother.parent; } }
            public BBTreeNode<T> Left { get { return firstBrother.left; } }
            public BBTreeNode<T> Right { get { return firstBrother.right; } }
            public bool IsLeaf { get { return (null == Left && null == Right); } }
            public bool IsRoot { get { return (null == Parent); } }
            public bool IsRightChild
            {
                get
                {
                    if (IsRoot) return false;
                    if (this == Parent.Right) return true;
                    return false;
                }
            }
            public bool IsLeftChild
            {
                get
                {
                    if (IsRoot) return false;
                    if (this == Parent.Left) return true;
                    return false;
                }
            }
            /*  Brother 属性 */
            public int BrotherCount { get { return firstBrother.brotherCount; } }
            public BBTreeNode<T> NextBrother { get { return nextBrother; } }
            public BBTreeNode<T> PreviousBrother { get { return previousBrother; } }
            public BBTreeNode<T> FirstBrother { get { return firstBrother; } }
            public BBTreeNode<T> LastBrother { get { return lastBrother; } }
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
                set
                {
                    //  若配置为空，那么相当于把自己设置为队列尾巴
                    if(null == value)
                    {
                        _firstBrother._lastBrother = this;
                        _nextBrother = null;
                        return;
                    }
                    //  其他情况，我们需要以下一个的 last 作为新的 last
                    _firstBrother._lastBrother = value._lastBrother;
                    _nextBrother = value;
                }
            }
            internal BBTreeNode<T> previousBrother
            {
                get { return _previousBrother; }
                set
                {
                    //  若配置为空，那么相当于把自己设置为队首
                    if(null == value)
                    {
                        _firstBrother = this;
                        _previousBrother = null;
                        return;
                    }
                    //  其他情况，我们将上一个的 first 作为新的 first
                    _firstBrother = value._firstBrother;
                    _previousBrother = value;
                }
            }
            internal BBTreeNode<T> firstBrother
            {
                get { return _firstBrother; }
            }
            internal BBTreeNode<T> lastBrother
            {
                get { return _lastBrother; }
            }
            /*  brother 特性内部接口*/
            internal void AddBrother(BBTreeNode<T> bro)
            {
                _lastBrother._nextBrother = bro;
                _lastBrother = bro;
                ++_firstBrother._brotherCount;
            }
            internal bool RemoveBrother(BBTreeNode<T> bro)
            {
                return false;
            }
            internal int RemoveBrother(int num) /* num < 0 从尾部删除 abs(num) 个，返回删除个数（>0） */
            {
                return 0;
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
                _firstBrother = this;
                _lastBrother = this;
                _brotherCount = 1;
            }
            internal void Dispose()
            {
                value = default(T);
                tree = null;
                size = 1;
                _parent = null;
                _left = null;
                _right = null;
                _nextBrother = null;
                _previousBrother = null;
                _firstBrother = null;
                _lastBrother = null;
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
            /*  SBT 特性私有组件 */
            BBTreeNode() { }
            BBTreeNode<T> _parent = null;
            BBTreeNode<T> _left = null;
            BBTreeNode<T> _right = null;
            /*  brother 特性私有组件*/
            int _brotherCount = 0;
            BBTreeNode<T> _nextBrother = null;
            BBTreeNode<T> _previousBrother = null;
            BBTreeNode<T> _firstBrother;
            BBTreeNode<T> _lastBrother;
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

