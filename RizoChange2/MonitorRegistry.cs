using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace RizoChange2
{
    class MonitorRegistry
    {
        //  クラスパラメータ
        const string REG_CONFIGURATION = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers\Configuration";
        const string PARAM_PrimSurfSize_cx = "PrimSurfSize.cx";
        const string PARAM_PrimSurfSize_cy = "PrimSurfSize.cy";
        const string PARAM_Timestamp = "Timestamp";

        public List<Monitor> MonitorList { get; set; }
        public Monitor LatestMonitor { get; set; }
        public bool IsChanged { get; set; }

        //  コンストラクタ
        public MonitorRegistry() { }

        //  レジストリに登録されているモニタ情報を取得
        public void CheckRegMonitor()
        {
            List<Monitor> tempMonitorList = new List<Monitor>();
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(REG_CONFIGURATION))
            {
                foreach (string subKeyName in regKey.GetSubKeyNames())
                {
                    tempMonitorList.Add(new Monitor(
                        subKeyName,
                        (long)regKey.OpenSubKey(subKeyName).GetValue(PARAM_Timestamp, 0)));
                }
            }

            //  逆ソート
            this.MonitorList = tempMonitorList.OrderByDescending(x => x.TimeStamp).ToList();

            //  接続中モニターのハードウェアIDを取得
            string[] monitorIDs = PNPDeviceID.GetID("Monitor");

            //  レジストリキーとハードウェアIDとの整合性チェック
            Func<string, bool> checkMonitorID = (monitorKey) =>
            {
                foreach (string tempKey in Regex.Split(monitorKey, @"[\+\*]"))
                {
                    if (monitorIDs.Any(x => !tempKey.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        return false;
                    }
                }
                return true;
            };
            MD5 md5 = MD5.Create();
            foreach (Monitor monitor in MonitorList)
            {
                if (checkMonitorID(monitor.MonitorID_pre))
                {
                    string hashKey = BitConverter.ToString(
                        md5.ComputeHash(Encoding.UTF8.GetBytes(monitor.MonitorID_pre))).Replace("-", "");
                    if (monitor.MonitorID_suf.Equals(hashKey, StringComparison.OrdinalIgnoreCase))
                    {
                        LatestMonitor = monitor;
                        break;
                    }
                }
            }
        }

        //  解像度を変更
        public void ChangeRegResolution(string[] targetMonitors, int resolutionX, int resolutionY)
        {
            foreach (string targetMonitor in targetMonitors)
            {
                try
                {
                    using (RegistryKey regKey =
                        Registry.LocalMachine.OpenSubKey(REG_CONFIGURATION + "\\" + LatestMonitor.MonitorID + "\\" + targetMonitor, true))
                    {
                        if (resolutionX != (int)regKey.GetValue(PARAM_PrimSurfSize_cx, 0) ||
                            resolutionY != (int)regKey.GetValue(PARAM_PrimSurfSize_cy, 0))
                        {
                            IsChanged = true;
                            regKey.SetValue(PARAM_PrimSurfSize_cx, resolutionX, RegistryValueKind.DWord);
                            regKey.SetValue(PARAM_PrimSurfSize_cy, resolutionY, RegistryValueKind.DWord);
                        }
                    }
                }
                catch { }
            }
        }
    }

    //  モニター情報格納用クラス
    class Monitor
    {
        public string MonitorID { get; set; }
        public string MonitorID_pre { get; set; }
        public string MonitorID_suf { get; set; }
        public DateTime TimeStamp { get; set; }

        public Monitor() { }
        public Monitor(string deviceID, long timeStamp)
        {
            this.MonitorID = deviceID;
            this.MonitorID_pre = deviceID.Substring(0, deviceID.IndexOf("^"));
            this.MonitorID_suf = deviceID.Substring(deviceID.IndexOf("^") + 1);
            this.TimeStamp = DateTime.FromFileTime(timeStamp);
        }
    }
}
