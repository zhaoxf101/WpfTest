using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomBA
{
    class Util
    {
        public static bool RegisterAutoStart(string fileName, string nodeName, bool autoStart)
        {
            var isSuccess = false;

            try
            {
                using (var keyHKCU = Microsoft.Win32.Registry.CurrentUser)
                {
                    var keyRun = keyHKCU.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    if (autoStart)
                    {
                        keyRun.SetValue(nodeName, $@"""{fileName}""");//加入注册，参数一为注册节点名称(随意)  
                    }
                    else
                    {
                        keyRun.DeleteValue(nodeName, false);//删除该注册节点    
                    }

                    isSuccess = true;
                }
            }
            catch (Exception)
            {
                return isSuccess;
            }

            return isSuccess;
        }
    }
}
