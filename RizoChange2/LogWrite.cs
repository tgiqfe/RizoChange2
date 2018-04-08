using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace RizoChange2
{
    class LogWrite
    {
        //  クラスパラメータ
        public string LogDir { get; set; }
        public string LogFile { get; set; }
        private bool Writable { get; set; }
        private Encoding Enc = Encoding.GetEncoding("Shift_JIS");

        //  コンストラクタ
        public LogWrite() { }
        public LogWrite(string logFile)
        {
            this.LogDir = Path.GetDirectoryName(logFile);
            this.LogFile = logFile;
            Init();
        }

        //  ログ準備
        public void Init()
        {
            if (LogDir != null && LogFile != null)
            {
                //  ログ格納フォルダーを作成
                if (!Directory.Exists(LogDir)) { Directory.CreateDirectory(LogDir); }

                //  書き込み可否確認
                try
                {
                    string checkFile =
                        LogDir + Path.DirectorySeparatorChar + "check_" +
                        BitConverter.ToString(MD5.Create().
                            ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.Ticks.ToString()))).
                            Replace("-", "") + ".log";
                    File.Create(checkFile).Close();
                    File.Delete(checkFile);
                    this.Writable = true;
                }
                catch { }
            }
        }

        //  ログ記述 (一行、日付有り) ※複数スレッド/プロセスからの重複アクセスは起こらないものとする
        public void WriteLine(string message)
        {
            try
            {
                string messageStr = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + message;
                if (Writable)
                {
                    using (StreamWriter sw = new StreamWriter(LogFile, true, Enc))
                    {
                        sw.WriteLine(messageStr);
                    }
                }
            }
            catch { }
        }

        //  ログ記述 (日付無し) ※複数スレッド/プロセスからの重複アクセスは起こらないものとする
        public void WriteRaw(string message)
        {
            try
            {
                if (Writable)
                {
                    using (StreamWriter sw = new StreamWriter(LogFile, true, Enc))
                    {
                        sw.Write(message);
                    }
                }
            }
            catch { }
        }
    }
}
