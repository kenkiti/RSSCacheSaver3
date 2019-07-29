using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.Linq;

using Model;
using System.IO;

namespace RSSCacheSaver3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Model.Buffer que = new Model.Buffer(1000);
        private Thread _producer;
        private Thread _consumer;

        private delegate void SafeCallDelegate(string text);

        // 起動オプション
        private Options opts = new Options();

        // 監視開始
        private void Main()
        {
            // 監視する銘柄コードを配列で取得
            //string[] s = new string[] { $"{txtCode.Text}.T", "9984.T", "7803.T" };
            string[] arrayCodes = lstCodes.Items.OfType<string>().ToArray();

            // Producer へは、キューと銘柄コードを渡す
            Producer producer = new Producer(que, arrayCodes);
            // Consumer へは、キューとSQLを渡す（TODO)
            Consumer consumer = new Consumer(que);

            consumer.Tick += new Consumer.TickEventHandler(this.Consumer_OnTick);

            _producer = new Thread(producer.Produce);
            _consumer = new Thread(consumer.Consume);

            _producer.Start();
            _consumer.Start();
        }

        private void Consumer_OnTick(object sender, TickEventArgs e)
        {
            // TODO 
            //invoke の書き方リファクタリング
            var s = $"{e.Topic}";
            WriteTextbox(s);
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
            if (_producer != null)
            {
                _producer.Abort();
                _consumer.Abort();
            }
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
            btnLoad_Click(sender, e);

            if (opts.AutoStart)
            {
                btnStart_Click(sender, e);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopThread();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            lstCodes.Items.Add(txtCode.Text);
            lsvCodes.Items.Add(txtCode.Text);
        }


        private void btnRemove_Click(object sender, EventArgs e)
        {
            var i = lstCodes.SelectedIndex;
            if (i>-1)
            {
                lstCodes.Items.RemoveAt(i);
            }
        }

        // TEST
        private void button1_Click(object sender, EventArgs e)
        {
            foreach(var item in lstCodes.Items)
            {
                textBox1.AppendText($"[{item}] ");
            }
            textBox1.AppendText($"{Environment.NewLine}");


            var s = (Int64)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            textBox1.AppendText(
                $"{s}" +
                $"{Environment.NewLine}" +
                $"{DateTimeOffset.FromUnixTimeMilliseconds(s).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff")}" +
                $"{Environment.NewLine}");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            const string sPath = "save.txt";

            using (FileStream fs = new FileStream(sPath, FileMode.Open))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var item in lstCodes.Items)
                    {
                        sw.WriteLine(item.ToString());
                    }
                }

            }
            MessageBox.Show("Programs saved!");
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            // TODO 
            //リストビューに銘柄名称が表示されると良い
            lstCodes.Items.Clear();

            using (FileStream fs = new FileStream("save.txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        lstCodes.Items.Add(line);
                    }
                }
            }
        }
    }
}
