using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Threading;

//  PNPデバイスの有効化/無効化
namespace RizoChange2
{
    class ChangeStatePNPDevice
    {
        //  静的パラメータ
        const int WAIT_TIME = 100;

        //  クラスパラメータ
        string DeviceType { get; set; } = "";
        ManagementObject Mo { get; set; } = null;

        //  コンストラクタ
        public ChangeStatePNPDevice() { }
        public ChangeStatePNPDevice(string deviceType)
        {
            SetMO(deviceType);
        }

        //  ======== 有効化 ========
        public bool Enable(string deviceType)
        {
            SetMO(deviceType);
            return ChangeIt(true);
        }
        public bool Enable()
        {
            return ChangeIt(true);
        }

        //  ======== 無効化 ========
        public bool Disable(string deviceType)
        {
            SetMO(deviceType);
            return ChangeIt(false);
        }
        public bool Disable()
        {
            return ChangeIt(false);
        }

        //  ======== 再起動 ========
        public void Reload(string deviceType)
        {
            this.Disable(deviceType);
            Thread.Sleep(WAIT_TIME);
            this.Enable(deviceType);
        }
        public void Reload()
        {
            this.Disable();
            Thread.Sleep(WAIT_TIME);
            this.Enable();
        }
        
        //  Privateメソッド
        private void SetMO(string deviceType)
        {
            if (this.DeviceType == null || !this.DeviceType.Equals(deviceType, StringComparison.OrdinalIgnoreCase) || this.Mo == null)
            {
                this.DeviceType = deviceType;
                this.Mo = new ManagementClass("Win32_PnPEntity").
                    GetInstances().
                    OfType<ManagementObject>().
                    FirstOrDefault(x => x["PNPClass"] != null &&
                        (x["PNPClass"] as string).Equals(deviceType, StringComparison.OrdinalIgnoreCase));
            }
        }

        private bool ChangeIt(bool isEnable)
        {
            bool resultBool = false;
            try
            {
                Mo.InvokeMethod(isEnable ? "Enable" : "Disable", new object[] { 0 });
                resultBool = true;
            }
            catch { }
            return resultBool;
        }
    }
}
