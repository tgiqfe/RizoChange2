using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RizoChange2
{
    class ArgsParam
    {
        //  クラスパラメータ
        public int ResolutionX { get; set; }
        public int ResolutionY { get; set; }
        public string[] TargetDisplays { get; set; }
        public bool DisplayReload { get; set; }
        public string LogFile { get; set; } = 
            Environment.ExpandEnvironmentVariables("%TEMP%") + "\\RizoChange\\" + 
            "ChangeResolution_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
        public bool Runnable { get; set; }
        public bool ViewHelp { get; set; }

        //  コンストラクタ
        public ArgsParam() { }
        public ArgsParam(string[] args)
        {
            string targetDisplay = "00";

            //  オプション確認
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    switch (args[i].ToLower())
                    {
                        case "/x":
                        case "-x":
                        case "/width":
                        case "-width":
                        case "--width":
                            this.ResolutionX = int.TryParse(args[++i], out int tempX) ? tempX : 0;
                            break;
                        case "/y":
                        case "-y":
                        case "/height":
                        case "-height":
                        case "--height":
                            this.ResolutionY = int.TryParse(args[++i], out int tempY) ? tempY : 0;
                            break;
                        case "/n":
                        case "-n":
                        case "/number":
                        case "-number":
                        case "--number":
                            targetDisplay = args[++i];
                            break;
                        case "/r":
                        case "-r":
                        case "/reload":
                        case "-reload":
                        case "--reload":
                            string tempFlag = args[++i];
                            this.DisplayReload = 
                                new string[5] { "true", "1", "on", "有効", "yes" }.
                                Any(x => x.Equals(tempFlag, StringComparison.OrdinalIgnoreCase));
                            break;
                        case "/l":
                        case "-l":
                        case "/log":
                        case "-log":
                        case "--log":
                            this.LogFile = args[++i];
                            break;
                        case "/?":
                        case "-?":
                        case "-h":
                        case "/help":
                        case "--help":
                            this.ViewHelp = true;
                            break;
                    }
                }
                catch { }
            }

            //  対象のディスプレイ番号を取得
            SetDisplayArray(targetDisplay);

            //  実行可否確認
            if (!ViewHelp && ResolutionX > 0 && ResolutionY > 0)
            {
                Runnable = true;
            }
        }

        //  対象のディスプレイ番号を取得
        //  TargetDisplayの記述ルール
        /*      00      ⇒ プライマリディスプレイのみ
         *      00,01   ⇒ プライマリ、セカンダリディスプレイ
         *      0       ⇒ プライマリディスプレイのみ
         *      1       ⇒ セカンダリディスプレイのみ
         *      0,1,2   ⇒ プライマリ、セカンダリ、ターシャリディスプレイ
         */
        private void SetDisplayArray(string targetDisplay)
        {
            if (Regex.IsMatch(targetDisplay, @"^\d+(,\d+)*$"))
            {
                this.TargetDisplays = targetDisplay.Split(',').
                    Select(x => int.TryParse(x, out int tempInt) ? string.Format("{0:D2}", tempInt) : "00").
                    Distinct().
                    ToArray();
            }
            else
            {
                this.TargetDisplays = new string[1] { "00" };
            }
        }
    }
}
