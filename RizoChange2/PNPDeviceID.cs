using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace RizoChange2
{
    class PNPDeviceID
    {
        //  クラスパラメータ
        public string DeviceType { get; set; }
        public string[] HardwareIDs { get; set; }

        //  コンストラクタ
        public PNPDeviceID() { }
        public PNPDeviceID(string deviceType)
        {
            SetID(deviceType);
        }

        //  ハードウェアID取得用静的メソッド
        public static string[] GetID(string deviceType)
        {
            return new PNPDeviceID(deviceType).HardwareIDs;
        }

        //  Privateメソッド
        private void SetID(string deviceType)
        {
            List<string> hardwareIDList = new List<string>();
            if (this.DeviceType == null || !this.DeviceType.Equals(deviceType, StringComparison.OrdinalIgnoreCase) || 
                this.HardwareIDs == null || this.HardwareIDs.Length == 0)
            {
                foreach (ManagementObject mo in new ManagementClass("Win32_PnPEntity").
                GetInstances().
                OfType<ManagementObject>().
                Where(x => x["PNPClass"] != null &&
                    (x["PNPClass"] as string).Equals(deviceType, StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (string hardwareID in mo["HardwareID"] as string[])
                    {
                        hardwareIDList.Add(hardwareID.Substring(hardwareID.LastIndexOf("\\") + 1));
                    }
                }
            }
            this.HardwareIDs = hardwareIDList.ToArray();
        }
    }
}
