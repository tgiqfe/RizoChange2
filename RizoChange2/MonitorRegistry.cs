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

        //  コンストラクタ
        public MonitorRegistry()
        {
            //this.MonitorIDList = new List<string>();
            this.MonitorList = new List<Monitor>();
        }

        //  現在接続中モニタ全部のハードウェアIDを持つレジストリキーを取得
        public void GetConnectedMonitor()
        {
            string[] monitorIDs = PNPDeviceID.GetID("Monitor");
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(REG_CONFIGURATION))
            {
                foreach (string subKeyName in regKey.GetSubKeyNames())
                {
                    List<string> tempIDList = new List<string>(monitorIDs);
                    string[] tempKeys = Regex.Split(subKeyName, @"[\+\*]");    //  +と*で分割。先頭文字列チェックだけなので、^以降はここでは気にしない
                    if (tempIDList.Count == tempKeys.Length)
                    {
                        foreach (string tempKey in tempKeys)
                        {
                            tempIDList.Remove(
                                tempIDList.FirstOrDefault(x => tempKey.StartsWith(x, StringComparison.OrdinalIgnoreCase)));
                        }
                        if (tempIDList.Count == 0)
                        {
                            MonitorList.Add(new Monitor(
                                subKeyName,
                                (long)regKey.OpenSubKey(subKeyName).GetValue(PARAM_Timestamp, 0)));
                        }
                    }
                }
            }
        }

        //  取得したレジストリキーの正当性チェック
        public void CheckRegistryKey()
        {
            MD5 md5 = MD5.Create();
            for (int i = MonitorList.Count - 1; i >= 0; i--)
            {
                string hashedKey = BitConverter.ToString(
                    md5.ComputeHash(Encoding.UTF8.GetBytes(MonitorList[i].DeviceID_pre))).Replace("-", "");
                if (!MonitorList[i].DeviceID_suf.Equals(hashedKey, StringComparison.OrdinalIgnoreCase))
                {
                    MonitorList.RemoveAt(i);
                }
            }
        }

        //  レジストリキーから最新のキーを取得
        public void LatestRegistryKey()
        {
            LatestMonitor = MonitorList.OrderByDescending(x => x.TimeStamp).ToArray()[0];
        }



        //  先にタイムスタンプをチェックしたほうが良さそう。
        //  タイムスタンプが最新で、尚且つ接続中のモニター情報と一致しているのがアタリの模様



    }

    //  モニター情報格納用クラス
    class Monitor
    {
        public string DeviceID { get; set; }
        public string DeviceID_pre { get; set; }
        public string DeviceID_suf { get; set; }
        public DateTime TimeStamp { get; set; }

        public Monitor() { }
        public Monitor(string deviceID, long timeStamp)
        {
            this.DeviceID = deviceID;
            this.DeviceID_pre = deviceID.Substring(0, deviceID.IndexOf("^"));
            this.DeviceID_suf = deviceID.Substring(deviceID.IndexOf("^") + 1);
            this.TimeStamp = DateTime.FromFileTime(timeStamp);
        }
    }
}
