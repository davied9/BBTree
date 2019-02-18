# BBTree
Brothered Sized Balanced tree

current status :
	Add Remove Contains ---- completed

	but, defeated by dictionary
	those are statistics of function:

	time elapsed       dictionary    bbtree
	add                3             32
	search             1             12
	delete             1             25

maybe the whole idea that don't use key is wrong from beginning
related blog : http://blog.csdn.net/davied9/article/details/78472026

this repository kept for study and introspection

表现特性如下
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
 
 
 

