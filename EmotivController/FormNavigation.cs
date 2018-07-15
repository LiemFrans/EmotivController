using EmotivController.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WebSocketSharp;

namespace EmotivController
{
    public partial class FormNavigation : Form
    {
        private Process cmdProcess;
        public FormNavigation()
        {
            InitializeComponent();
            server.RunWorkerAsync();
        }
        WebSocket ws;
        string ipAddress="127.0.0.1";
        private void btConnectServer_Click(object sender, EventArgs e)
        {
            ws = new WebSocket("ws://127.0.0.1:8080/");
            btConnectServer.Enabled = false;
            transportSignal.RunWorkerAsync();
        }

        private void transportSignal_DoWork(object sender, DoWorkEventArgs e)
        {
            ws.OnMessage += OnMessageResult;
            ws.Connect();
            ws.Send("Start");
        }
        string old_data = "";
        public void OnMessageResult(object sender, MessageEventArgs e)
        {
            if (!old_data.Equals(e.Data))
            {
                old_data = e.Data;
                Console.WriteLine(e.Data);
                DataModel data = new DataModel();
                bool isParsed = false;
                float mean = (float)4167.9950522878;
                try
                {
                    data = new JavaScriptSerializer().Deserialize<DataModel>(e.Data);
                    isParsed = true;
                    _dataAF3.Enqueue(data.AF3.value);
                    _dataAF4.Enqueue(data.AF4.value);
                    _dataF3.Enqueue(data.F3.value);
                    _dataF4.Enqueue(data.F4.value);
                    _dataF7.Enqueue(data.F7.value);
                    _dataF8.Enqueue(data.F8.value);
                    _dataFC5.Enqueue(data.FC5.value);
                    _dataFC6.Enqueue(data.FC6.value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex);
                    Console.WriteLine("error : " + ex.Message);
                }
            }
        }
        Queue<float> _dataAF3 = new Queue<float>();
        Queue<float> _dataAF4 = new Queue<float>();
        Queue<float> _dataF7 = new Queue<float>();
        Queue<float> _dataF8 = new Queue<float>();
        Queue<float> _dataF3 = new Queue<float>();
        Queue<float> _dataF4 = new Queue<float>();
        Queue<float> _dataFC5 = new Queue<float>();
        Queue<float> _dataFC6 = new Queue<float>();

        private void FormNavigation_FormClosed(object sender, FormClosedEventArgs e)
        {
            KillProcessAndChildren(cmdProcess.Id);
        }
        public void KillProcessAndChildren(int pid)
        {
            using (var searcher = new ManagementObjectSearcher
            ("Select * From Win32_Process Where ParentProcessID=" + pid))
            {
                var moc = searcher.Get();
                foreach (ManagementObject mo in moc)
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
                try
                {
                    var proc = Process.GetProcessById(pid);
                    proc.Kill();
                    Console.WriteLine("Close Server "+pid);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Server Already exit");
                }
            }
        }

        private void server_DoWork(object sender, DoWorkEventArgs e)
        {
            var psi = new ProcessStartInfo("CMD.exe", "/K python C:\\Users\\Frans\\Documents\\GitHub\\EmoGuy\\socketEmoGuy.py");
            cmdProcess = Process.Start(psi);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
