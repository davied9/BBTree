using System.IO;
using System.Collections.Generic;
using System;

namespace DAV
{

    /// <summary>
    /// 程序记录器，在每次调用程序的时候，在指定位置创建文件记录程序运行情况
    /// </summary>
    public class ProgramMonitor
    {
        //  构造
        private ProgramMonitor(string filePath)
        {
            
        }
        private ProgramMonitor() { }
        //  外部接口
        /// <summary>
        /// 程序日志路径
        /// </summary>
        public static string logFilePath { get { return _logFilePath; } }
        /// <summary>
        /// 将程序日志重定向至指定根目录
        /// </summary>
        /// <param name="root"></param>
        public static void Redirect(string root)
        {
            if (isOpen)
            {
                Close();
                _logFileRoot = root;
                _logFilePath = null;
                Open();
            }
            else
            {
                _logFileRoot = root;
            }

        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public static void Close()
        {
            if (!isOpen) return;
            _fileWriter.Flush();
            _fileWriter.Close(); _fileWriter = null;
            _file.Close(); _file = null;
        }
        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="info">信息字符串</param>
        public static void Log(string info, bool toFlush = false)
        {
            if (!isOpen) Open();
#if THREADSAFE
            lock (_lock)
            {
#endif
                _fileWriter.WriteLine(info);
                if (toFlush)
                {
                    _fileWriter.Flush();
                }                
#if THREADSAFE
            }
#endif
        }
        /// <summary>
        /// 将消息从缓冲器写入文件
        /// </summary>
        public static void Flush()
        {
            _fileWriter.Flush();
        }
        /// <summary>
        /// 记录信息，并关闭文件
        /// </summary>
        /// <param name="info"></param>
        public static void KeyLog(string info)
        {
            if (!isOpen) Open();
#if THREADSAFE
            lock (_lock)
            {
#endif
                _fileWriter.WriteLine(info);
                _fileWriter.Flush();
#if THREADSAFE
            }
#endif
            Close();
        }
        //  基本信息
        /// <summary>
        /// 保存路径下能够存储的最大文件个数，超过此个数时，按照创建时间删除旧文件
        /// </summary>
        private static int _maxFileNum = 5;
        /// <summary>
        /// 记录文件根目录
        /// </summary>
        private static string _logFileRoot = ".";
        /// <summary>
        /// 记录文件的存储文件夹
        /// </summary>
        private static string _logFileDir = "ProgramLogFiles";
        /// <summary>
        /// 记录文件的标识
        /// </summary>
        private static string _logFileTag = "ProgramLog";
        /// <summary>
        /// 记录文件的后缀名
        /// </summary>
        private static string _logFileExt = ".log";
        /// <summary>
        /// 时间戳的格式
        /// </summary>
        private static string _logFileTimeTagFormat = "_yyyyMMdd_HHmmss";
        /// <summary>
        /// 本次记录文件的文件名
        /// </summary>
        private static string _logFilePath = null;
        /// <summary>
        /// 记录的文件流
        /// </summary>
        private static FileStream _file = null;
        /// <summary>
        /// 记录文件流的写对象
        /// </summary>
        private static StreamWriter _fileWriter = null;
        //  初始化
        /// <summary>
        /// 从文件路径读取配置信息
        /// </summary>
        /// <param name="filePath"></param>
        private static void ReadConfig(string filePath)
        {

        }
        /// <summary>
        /// 是否完成了初始化
        /// </summary>
        private static bool _initialized { get { return (null != _logFilePath); } }
        /// <summary>
        /// 以 【logFilePath + logFileTag + time + logFileExt】 为文件名初始化文件
        /// </summary>
        private static void Initialize()
        {
            CheckDir();
            //  创建新的文件
            string timeStr = DateTime.Now.ToString(_logFileTimeTagFormat);
            _logFilePath += _logFileRoot + "/" + _logFileDir + "/" + _logFileTag + timeStr + _logFileExt;
        }
        /// <summary>
        /// 检查路径文件夹是否满足要求
        /// </summary>
        private static void CheckDir()
        {
            string logFileDir = _logFileRoot + "/" + _logFileDir;
            //  查找 log 路径下的所有记录文件，以文件名的【tag】和【ext】为标志；
            if (Directory.Exists(logFileDir))
            {
                string[] files = Directory.GetFiles(logFileDir);
                List<string> logs = new List<string>(); //  将要被删除的文件的列表
                int tagLen = _logFileTag.Length;
                int extLen = _logFileExt.Length;
                foreach (string filepath in files)
                {
                    string file = Path.GetFileName(filepath);
                    if (file.Length < tagLen + extLen) continue;
                    if (file.Substring(0, tagLen) == _logFileTag /* Substring 是不获取位置上的最后一个字符的 */
                        && file.Substring(file.Length - extLen, extLen) == _logFileExt)
                    {
                        logs.Add(file);
                    }
                }
                //  将时间最早的文件删除
                if (logs.Count >= _maxFileNum)
                {
                    logs.Sort();
                    int n = 0, n2Del = logs.Count - _maxFileNum + 1;
                    foreach (string toDel in logs)
                    {
                        File.Delete(logFileDir + "/" + toDel);
                        if (++n == n2Del) break;
                    }

                }
            }
            else
            {
                Directory.CreateDirectory(logFileDir);
            }
        }
        //  二次开启
        /// <summary>
        /// 记录文件是否打开
        /// </summary>
        public static bool isOpen { get { return (null != _fileWriter); } }
        private static void Open()
        {
            if (!_initialized) Initialize();
            _file = new FileStream(_logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            _file.Seek(0, SeekOrigin.End);
            _fileWriter = new StreamWriter(_file);
        }
        //  多线程安全
#if THREADSAFE
        private static object _lock = new object();
#endif
        //  单例
        private static ProgramMonitor _instance = null;
    }
    

    

}
