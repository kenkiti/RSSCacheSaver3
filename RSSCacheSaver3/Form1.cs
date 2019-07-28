using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;

using Model;

namespace RSSCacheSaver3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Model.Buffer que = new Model.Buffer(100);
        private Thread _producer;
        private Thread _consumer;

        private delegate void SafeCallDelegate(string text);

        // 起動オプション
        private Options opts = new Options();

        // 監視開始
        private void Main()
        {
            string[] s = new string[] { $"{txtCode.Text}.T", "2121.T" };
            Producer producer = new Producer(que, s);
            Consumer consumer = new Consumer(que);

            consumer.Tick += new Consumer.TickEventHandler(this.Consumer_OnTick);

            _producer = new Thread(producer.Produce);
            _consumer = new Thread(consumer.Consume);

            _producer.Start();
            _consumer.Start();
        }

        private void Consumer_OnTick(object sender, TickEventArgs e)
        {
            WriteTextbox(e.Message);
        }
        #region スレッドセーフなテキストボックス処理
        private void WriteTextbox(string s)
        {
            if (textBox1.IsDisposed) { return; }
            if (textBox1.InvokeRequired)
            {
                Invoke(new SafeCallDelegate(WriteLog), new object[] { s });
            }

        }
        private void WriteLog(string s)
        {
            textBox1.AppendText($"{s}{Environment.NewLine}");
        }
        #endregion

        private void StopThread()
        {
            _producer.Abort();
            _consumer.Abort();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            txtCode.Text = "3697";
            lblCount.Text = "";
            Options();
            if (opts.Code != "" && opts.Code != null) { txtCode.Text = opts.Code; }
        }

        /// <summary>
        /// コマンドライン引数を解析する
        /// </summary>
        public void Options()
        {
            string[] i_args = Environment.GetCommandLineArgs();
            var result = CommandLine.Parser.Default.ParseArguments<Options>(i_args) as CommandLine.Parsed<Options>;

            if (result != null)
            {
                opts.Code = result.Value.Code;
                opts.AutoStart = result.Value.AutoStart;

                //解析に成功した時は、解析結果を表示
                Console.WriteLine(string.Format("code: {0}\r\nstart: {1}\r\n",
                        opts.Code, opts.AutoStart));
            }
            else
            {
                //解析に失敗
                Console.WriteLine("コマンドライン引数の解析に失敗");
            }
        }

        // 停止ボタン
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopThread();

            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        // 開始ボタン
        private void btnStart_Click(object sender, EventArgs e)
        {

            //Byte[] b = client.Request("銘柄名称", 1, 6000);
            //this.Text = Encoding.Default.GetString(b);
            Main();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        // 自動処理
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (opts.AutoStart)
            {
                btnStart_Click(sender, e);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopThread();
        }
    }
}
