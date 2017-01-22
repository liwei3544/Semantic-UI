using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Lottery.Gather.DAL;

namespace Lottery.Gather
{
    public partial class frmMain : Form
    {
        /// <summary>
        ///  状态：true=可以采集
        /// </summary>
        public bool IsStart = false;

        /// <summary>
        /// 当前采集数据
        /// </summary>
        public Dictionary<string, string> CurrentData = new Dictionary<string, string>();

        public frmMain()
        {
            InitializeComponent();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.ToString().Contains("weblogin"))
            {
                webBrowser1.Document.GetElementById("username").SetAttribute("value", "win20160612");
                webBrowser1.Document.GetElementById("password").SetAttribute("value", "1q2w3e4r");
                webBrowser1.Document.GetElementById("loginBTN").InvokeMember("Click");
                return;
            }

            if (webBrowser1.Url.ToString().Contains("index.html"))
            {
                BeginGather();
                webBrowser1.Stop();
                return;
            }

            if (!IsStart) return;

            timerRefresh.Enabled = false;

            //解析网页
            if (webBrowser1.Document != null)
            {
                var json = webBrowser1.Document.Body.InnerHtml;
                //Logger.Debug(json);
                if (string.IsNullOrEmpty(json) || !json.StartsWith("{"))
                {
                    Logger.Error(json);
                    timerRefresh.Enabled = true;
                    return;
                }
                UyouDataInfo data = json.JSONDeserialize<UyouDataInfo>();
                if (data == null)
                {
                    Logger.Error(json);
                    timerRefresh.Enabled = true;
                    return;
                }
                else if (data.status.ToLower() == "false" && data.resultCode.ToInt32() == 3)//已退出---重新登录
                {
                    /*{"status":false,"description":"您的账户已登出,请重新登录！","resultCode":3}*/
                    webBrowser1.Navigate("http://www.hu8r.com/");
                    return;
                }
                else if (data.diagramList == null || data.diagramList.Count == 0)
                {
                    timerRefresh.Enabled = true;
                    return;
                }
                //Logger.Debug(data.JSONSerialize());
                foreach (var item in data.diagramList)
                {
                    SaveToDB(item);
                }
            }

            timerRefresh.Enabled = true;
        }


        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://www.hu8r.com/secure/game.do?method=loadRunChart&gameName=UUFFC&type=1");

        }


        private void BeginGather()
        {
            IsStart = true;
            timerRefresh.Interval = 1000 * 2;
            timerRefresh.Enabled = true;
        }

        /// <summary>
        ///  保存最后成功的
        /// </summary>
        private static int LastSaveRound = 0;

        /// <summary>
        /// 保存到数据库
        /// </summary>
        private void SaveToDB(UyouItemInfo info)
        {
            if (info.numero.ToInt32() > LastSaveRound)
            {
                if (Ub8.Add(info.numero, info.item)) LastSaveRound = info.numero.ToInt32();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //注册捕获控件的错误的处理事件 
            this.webBrowser1.Document.Window.Error +=
                new HtmlElementErrorEventHandler(Window_Error);
        }

        //对错误进行处理 
        void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true; // 阻止其他地方继续处理 
        }


    }

    [Serializable]
    public class UyouDataInfo
    {
        public List<UyouItemInfo> diagramList { set; get; }
        /// <summary>
        /// 请求结果
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 请求结果代号
        /// </summary>
        public string resultCode { get; set; }
    }
    [Serializable]
    public class UyouItemInfo
    {
        /// <summary>
        /// 号码
        /// </summary>
        public string item { set; get; }
        /// <summary>
        /// 期数
        /// </summary>
        public string numero { set; get; }
    }
}
