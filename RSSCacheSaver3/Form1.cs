using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using NDde.Client;
using System.Collections.Concurrent;
using System.Threading.Tasks;
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

        // 起動オプション
        private Options opts = new Options();

        // 監視開始
        private void Main()
        {
            Producer producer = new Producer(que, $"{txtCode.Text}.T");
            Consumer consumer = new Consumer(que);

            _producer = new Thread(producer.Produce);
            _consumer = new Thread(consumer.Consume);


            _producer.Start();
            _consumer.Start();
        }

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
