using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHelper
{
    /// <summary>
    /// 计算机信息类
    /// </summary>
    public class ComputerInfo
    {
        private static ComputerInfo instance;
        private static readonly object _lock = new object();
        private ComputerInfo(){ }
        public static ComputerInfo CreateComputer
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new ComputerInfo();
                        }
                    }
                }
                return instance;
            }
        }

        #region 【计算机属性】

        /// <summary>
        /// CPU型号
        /// </summary>
        public string ProcessorName => GetCPU().ProcessorName;
        /// <summary>
        /// CPU主频
        /// </summary>
        public string BaseFrequency => GetCPU().BaseFrequency;
        /// <summary>
        /// CPU核心数
        /// </summary>
        public string CoresCount => GetCPU_Count();
        /// <summary>
        /// 计算机内存
        /// </summary>
        public string PhisicalMemory => GetPhisicalMemory();
        /// <summary>
        /// 主硬盘容量
        /// </summary>
        public string DiskSize => GetDiskSize();
        /// <summary>
        /// 计算机型号
        /// </summary>
        public string Product => GetProduct();
        /// <summary>
        /// 主屏幕分辨率
        /// </summary>
        public string ScreenResolution => GetScreenResolution();
        /// <summary>
        /// 显卡型号
        /// </summary>
        public string VideoProcessor => GetVideoController().VideoProcessor;
        /// <summary>
        /// 显存容量
        /// </summary>
        public string AdapterRAM => GetVideoController().AdapterRAM;
        /// <summary>
        /// 操作系统版本
        /// </summary>
        public string OSVersion => GetOS_Version();

        #endregion

        #region 【CPU信息】
        /// <summary>
        /// 查找cpu的名称，主频
        /// </summary>
        /// <returns></returns>
        private (string ProcessorName, string BaseFrequency) GetCPU()
        {
            (string ProcessorName, string BaseFrequency) result = ("未知", "未知");
            try
            {
                string str = string.Empty;
                ManagementClass mcCPU = new ManagementClass(WindowsAPIType.Win32_Processor.ToString());
                ManagementObjectCollection mocCPU = mcCPU.GetInstances();
                foreach (ManagementObject m in mocCPU)
                {
                    string name = m[WindowsAPIKeys.Name.ToString()].ToString();
                    string[] parts = name.Split('@');
                    result.ProcessorName = parts[0].Split('-')[0] + "处理器";
                    result.BaseFrequency = parts[1];
                    break;
                }

            }
            catch
            {

            }
            return result;
        }
        #endregion

        #region 【CPU核心数】
        /// <summary>
        /// 获取cpu核心数
        /// </summary>
        /// <returns></returns>
        private string GetCPU_Count()
        {
            string str = "查询失败";
            try
            {
                uint coreCount = 0;
                ManagementObjectCollection mos = new ManagementObjectSearcher("Select * from " +
 WindowsAPIType.Win32_Processor.ToString()).Get();
                foreach (var item in mos)
                {
                    coreCount += uint.Parse(item[WindowsAPIKeys.NumberOfCores.ToString()].ToString());
                }
                str = UpperNumberConvert(coreCount) + "核";
            }
            catch
            {

            }
            return str;
        }
        #endregion

        #region 【系统内存】
        /// <summary>
        /// 获取系统内存大小
        /// </summary>
        /// <returns>内存大小（单位M）</returns>
        private string GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
            {
                Query = new SelectQuery(WindowsAPIType.Win32_PhysicalMemory.ToString(), "",
 new string[] { WindowsAPIKeys.Capacity.ToString() })//设置查询条件 
            };   //用于查询一些如系统信息的管理对象 
            ManagementObjectCollection collection = searcher.Get();   //获取内存容量 
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties[WindowsAPIKeys.Capacity.ToString()].Value != null)
                {
                    try
                    {
                        capacity += long.Parse(baseObj.Properties[WindowsAPIKeys.Capacity.ToString()].Value.ToString());
                    }
                    catch
                    {
                        return "查询失败";
                    }
                }
            }
            return UnitConvert(capacity, 1024.0);
        }
        #endregion

        #region 【主硬盘容量】
        /// <summary>
        /// 获取硬盘容量
        /// </summary>
        private string GetDiskSize()
        {
            string result = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                string hdId = string.Empty;
                ManagementClass mc = new ManagementClass(WindowsAPIType.win32_DiskDrive.ToString());
                ManagementObjectCollection mcC = mc.GetInstances();
                foreach (ManagementObject m in mcC)
                {
                    long capacity = Convert.ToInt64(m[WindowsAPIKeys.Size.ToString()].ToString());
                    sb.Append(UnitConvert(capacity, 1000.0) + "+");
                }
                result = sb.ToString().TrimEnd('+');
            }
            catch
            {

            }
            return result;
        }
        #endregion

        #region 【计算机型号】
        /// <summary>
        /// 计算机型号
        /// </summary>
        private string GetProduct()
        {
            string str = "查询失败";
            try
            {
                string hdId = string.Empty;
                ManagementClass mc = new ManagementClass(WindowsAPIType.Win32_ComputerSystemProduct.ToString());
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject m in moc)
                {
                    str = m[WindowsAPIKeys.Name.ToString()].ToString(); break;
                }
            }
            catch
            {

            }
            return str;
        }
        #endregion

        #region 【主屏幕分辨率】
        /// <summary>
        /// 获取分辨率
        /// </summary>
        private string GetScreenResolution()
        {
            string result = "xxxx*xxxx";
            try
            {
                string hdId = string.Empty;
                ManagementClass mc = new ManagementClass(WindowsAPIType.Win32_DesktopMonitor.ToString());
                ManagementObjectCollection mcC = mc.GetInstances();
                foreach (ManagementObject m in mcC)
                {
                    result = m[WindowsAPIKeys.ScreenWidth.ToString()].ToString() + "*" +
m[WindowsAPIKeys.ScreenHeight.ToString()].ToString();
                    break;
                }
            }
            catch
            {

            }
            return result;
        }
        #endregion

        #region 【显卡信息】
        /// <summary>
        /// 显卡 芯片,显存大小
        /// </summary>
        private (string VideoProcessor, string AdapterRAM) GetVideoController()
        {
            (string VideoProcessor, string AdapterRAM) result = ("未知","未知");
            try
            {

                ManagementClass mc = new ManagementClass(WindowsAPIType.Win32_VideoController.ToString());
                ManagementObjectCollection mcC = mc.GetInstances();
                foreach (ManagementObject m in mcC)
                {
                    result.VideoProcessor = m[WindowsAPIKeys.VideoProcessor.ToString()].ToString();
                    result.AdapterRAM = UnitConvert(Convert.ToInt64(m[WindowsAPIKeys.AdapterRAM.ToString()].ToString()), 1024.0);
                    break;
                }
            }
            catch
            {

            }
            return result;
        }
        #endregion

        #region 【操作系统版本】
        /// <summary>
        /// 操作系统版本
        /// </summary>
        private string GetOS_Version()
        {
            string str = "Windows 10";
            try
            {
                string hdId = string.Empty;
                ManagementClass mc = new ManagementClass(WindowsAPIType.Win32_OperatingSystem.ToString());
                ManagementObjectCollection mcC = mc.GetInstances();
                foreach (ManagementObject m in mcC)
                {
                    str = m[WindowsAPIKeys.Name.ToString()].ToString().Split('|')[0].Replace("Microsoft", "");
                    break;
                }
            }
            catch
            {

            }
            return str;
        }
        #endregion

        #region 【数据转换】

        /// 将字节转换为GB
        /// </summary>  
        /// <param name="size">字节值</param>  
        /// <param name="mod">除数，硬盘除以1000，内存除以1024</param>  
        /// <returns></returns>  
        private string UnitConvert(double size, double mod)
        {
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return Math.Round(size, 2) + units[i];
        }

        /// <summary>
        /// 将数字转换为大写
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string UpperNumberConvert(uint num)
        {
            string[] uppers = new string[] { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
            string result = "";            
            if (num <= 10)
            {
                result = uppers[num];
            }
            else if (num < 20)
            {
                result = uppers[10] + uppers[num % 10];
            }
            else if (num == 20)
            {
                result = uppers[2] + uppers[10];
            }
            //else if (num >= 20 && num <100)
            //{
            //    if (num % 10 == 0)
            //    {
            //        result = uppers[num/10] + uppers[10];
            //    }
            //    else
            //    {
            //        result = uppers[num / 10] + uppers[10] + uppers[num % 10];
            //    }
            //}
            else
            {
                result = num.ToString();
            }
            return result;
        }

        #endregion
    }

    #region 【WindowsAPI名称】
    /// <summary>
    /// windows api 名称
    /// </summary>
    enum WindowsAPIType
    {
        /// <summary>
        /// 内存
        /// </summary>
        Win32_PhysicalMemory,
        /// <summary>
        /// cpu
        /// </summary>
        Win32_Processor,
        /// <summary>
        /// 硬盘
        /// </summary>
        win32_DiskDrive,
        /// <summary>
        /// 电脑型号
        /// </summary>
        Win32_ComputerSystemProduct,
        /// <summary>
        /// 分辨率
        /// </summary>
        Win32_DesktopMonitor,
        /// <summary>
        /// 显卡
        /// </summary>
        Win32_VideoController,
        /// <summary>
        /// 操作系统
        /// </summary>
        Win32_OperatingSystem

    }
    #endregion

    #region 【WindowsAPI属性】
    enum WindowsAPIKeys
    {
        /// <summary>
        /// 名称
        /// </summary>
        Name,
        /// <summary>
        /// 显卡芯片
        /// </summary>
        VideoProcessor,
        /// <summary>
        /// 显存大小
        /// </summary>
        AdapterRAM,
        /// <summary>
        /// 分辨率宽
        /// </summary>
        ScreenWidth,
        /// <summary>
        /// 分辨率高
        /// </summary>
        ScreenHeight,
        /// <summary>
        /// 电脑型号
        /// </summary>
        Version,
        /// <summary>
        /// 硬盘容量
        /// </summary>
        Size,
        /// <summary>
        /// 内存容量
        /// </summary>
        Capacity,
        /// <summary>
        /// cpu核心数
        /// </summary>
        NumberOfCores
    }
    #endregion
}
