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
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WebSocketSharp;

namespace EmotivController
{
    public partial class FormNavigation : Form
    {
        private Process cmdProcess;
        private bool isOK = false;
        private bool isRunning = false;
        private WebSocket ws;
        private string old_data = "";
        private string ipAddress = "127.0.0.1";
        private string selectedArrow = "kiri";
        private List<float> _dataAF3 = new List<float>();
        private List<float> _dataAF4 = new List<float>();
        private List<float> _dataF7 = new List<float>();
        private List<float> _dataF8 = new List<float>();
        private List<float> _dataF3 = new List<float>();
        private List<float> _dataF4 = new List<float>();
        private List<float> _dataFC5 = new List<float>();
        private List<float> _dataFC6 = new List<float>();
        private static int _frekuensiSampling = 128;
        private static int _second = 5;
        private static int _jumlahData = _frekuensiSampling * _second;
        private Stopwatch watch;
        public FormNavigation()
        {
            InitializeComponent();
            serverStart.RunWorkerAsync();
        }
        private void btConnect_Click(object sender, EventArgs e)
        {
            if (ws == null || !ws.IsAlive)
            {
                ws = new WebSocket("ws://" + ipAddress + ":8080/");
                btConnect.Enabled = false;
                btfromZero.Enabled = true;
                btLangsungTraining.Enabled = true;
                ws.Connect();
            }
        }

        public void OnMessageResult(object sender, MessageEventArgs e)
        {
            if (watch.Elapsed.Seconds != 5)
            {
                old_data = e.Data;
                DataModel data = new DataModel();
                bool isParsed = false;
                float mean = (float)4167.9950522878;
                try
                {
                    data = new JavaScriptSerializer().Deserialize<DataModel>(e.Data);
                    isParsed = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex);
                    Console.WriteLine("error : " + ex.Message);
                }
                if (isParsed)
                {
                    _dataAF3.Add(data.AF3.value - mean);
                    _dataAF4.Add(data.AF3.value - mean);
                    _dataF3.Add(data.F3.value - mean);
                    _dataF4.Add(data.F4.value - mean);
                    _dataF7.Add(data.F7.value - mean);
                    _dataF8.Add(data.F8.value - mean);
                    _dataFC5.Add(data.FC5.value - mean);
                    _dataFC6.Add(data.FC6.value - mean);
                }

            }
            else if (watch.Elapsed.Seconds==5)
            {
                ws.Close();
                watch.Stop();
                Console.WriteLine(watch.Elapsed.Seconds);
                Console.WriteLine(_dataAF3.Count());
                Console.WriteLine(_dataAF4.Count());
                Console.WriteLine(_dataF3.Count());
                Console.WriteLine(_dataF4.Count());
            }
        }

        private void FormNavigation_FormClosed(object sender, FormClosedEventArgs e)
        {
            killProcessAndChildren(cmdProcess.Id);
        }

        public void killProcessAndChildren(int pid)
        {
            using (var searcher = new ManagementObjectSearcher
            ("Select * From Win32_Process Where ParentProcessID=" + pid))
            {
                var moc = searcher.Get();
                foreach (ManagementObject mo in moc)
                {
                    killProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
                try
                {
                    var proc = Process.GetProcessById(pid);
                    proc.Kill();
                    Console.WriteLine("Close Server " + pid);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Server Already exit");
                }
            }
        }
                
        private void btfromZero_Click(object sender, EventArgs e)
        {
            btKiri.Enabled = true;
        }

        private void btLangsungTraining_Click(object sender, EventArgs e)
        {
            btTraining.Enabled = true;
        }

        //private void initializedList()
        //{
        //    _dataFC5 = new List<float>();
        //    _dataAF4 = new List<float>();
        //    _dataF7 = new List<float>();
        //    _dataF8 = new List<float>();
        //    _dataF3 = new List<float>();
        //    _dataF4 = new List<float>();
        //    _dataFC5 = new List<float>();
        //    _dataFC6 = new List<float>();
        //}

        private void clearList()
        {
            _dataAF3.Clear();
            _dataAF4.Clear();
            _dataF3.Clear();
            _dataF4.Clear();
            _dataF7.Clear();
            _dataF8.Clear();
            _dataFC5.Clear();
            _dataFC6.Clear();
        }

        private void btKiri_Click(object sender, EventArgs e)
        {
            watch = new Stopwatch();
            if (!_dataAF3.Any())
            {
                clearList();
            }
            if (ws.IsAlive) { 
                ws.Connect();
            }
            progressBar.Maximum = _jumlahData;
            watch.Start();
            transportSignal.RunWorkerAsync();
        }

        //serverStart DO WORK
        private void serverStart_DoWork(object sender, DoWorkEventArgs e)
        {
            var psi = new ProcessStartInfo("CMD.exe", "/K python C:\\Users\\Frans\\Documents\\GitHub\\EmoGuy\\socketEmoGuy.py");
            cmdProcess = Process.Start(psi);
        }
        
        //transport Signal DO WORK
        private void transportSignal_DoWork(object sender, DoWorkEventArgs e)
        {
            ws.OnMessage += OnMessageResult;
            isRunning = true;
            ws.Send("START");
        }

        //Preprocessing

        private double[] DFT(double[] signal)
        {
            double[] _re = new double[_jumlahData];
            double[] _im = new double[_jumlahData];
            double[] _hasilKuadrat = new double[_jumlahData];
            double[] _dft = new double[_jumlahData];
            Parallel.For(0, _jumlahData, i =>
            {
                _re[i] = 0;
                _im[i] = 0;
                for(int j = 0; j < _jumlahData; i++)
                {
                    _re[i] = _re[i] + signal[j] * Math.Cos(2 * Math.PI * j * i) / _jumlahData;
                    _im[j] = _im[j] - signal[j] * Math.Sin(2 * Math.PI * j * i) / _jumlahData;
                }
                _hasilKuadrat[i] = Math.Sqrt(_re[i]) + Math.Sqrt(_im[i]);
                _dft[i] = Math.Sqrt(_hasilKuadrat[i]) / _jumlahData;
            });
            return _dft;
        }

        double _frequencyCutOff = 0.7;

        private double[] filterBPFOrde4(double[] signal)
        {
            double r = 0;
            double theta = 2 * Math.PI * (_frequencyCutOff / _frekuensiSampling);
            double[] bpfOrde4 = new double[_jumlahData];
            double gain = ((1 - r) * Math.Sqrt(1 - 2 * r * Math.Cos(2 * theta) + r * r)) / 4 * Math.Abs(Math.Sin(theta));
            //for (int i = 0; i < _jumlahData; i++)
            Parallel.For(0, _jumlahData, i =>
            {
                    switch (i)
                    {
                        case 0:
                            bpfOrde4[i] = 4 * r * Math.Cos(theta) * 0 - 4 * r * r * Math.Sqrt(Math.Cos(theta)) * 0 - 2 * r * r * 0 + 4 * Math.Pow(r, 3) * Math.Cos(theta) * 0
                                 - Math.Pow(r, 4) * 0 + gain * (signal[i] - 2 * signal[0] + signal[0]);
                            break;
                        case 1:
                            bpfOrde4[i] = 4 * r * Math.Cos(theta) * bpfOrde4[i - 1] - 4 * r * r * Math.Sqrt(Math.Cos(theta)) * bpfOrde4[0] - 2 * r * r * bpfOrde4[0] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * bpfOrde4[0]
                                 - Math.Pow(r, 4) * bpfOrde4[0] + gain * (signal[i] - 2 * signal[0] + signal[0]);
                            break;
                        case 2:
                            bpfOrde4[i] = 4 * r * Math.Cos(theta) * bpfOrde4[i - 1] - 4 * r * r * Math.Sqrt(Math.Cos(theta)) * bpfOrde4[i - 2] - 2 * r * r * bpfOrde4[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * bpfOrde4[0]
                                 - Math.Pow(r, 4) * bpfOrde4[0] + gain * (signal[i] - 2 * signal[i - 2] + signal[0]);
                            break;
                        case 3:
                            bpfOrde4[i] = 4 * r * Math.Cos(theta) * bpfOrde4[i - 1] - 4 * r * r * Math.Sqrt(Math.Cos(theta)) * bpfOrde4[i - 2] - 2 * r * r * bpfOrde4[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * bpfOrde4[i - 3]
                                 - Math.Pow(r, 4) * bpfOrde4[0] + gain * (signal[i] - 2 * signal[i - 2] + signal[0]);
                            break;
                        case 4:
                            bpfOrde4[i] = 4 * r * Math.Cos(theta) * bpfOrde4[i - 1] - 4 * r * r * Math.Sqrt(Math.Cos(theta)) * bpfOrde4[i - 2] - 2 * r * r * bpfOrde4[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * bpfOrde4[i - 3]
                                 - Math.Pow(r, 4) * bpfOrde4[i - 4] + gain * (signal[i] - 2 * signal[i - 2] + signal[i - 4]);
                            break;
                        default:
                            bpfOrde4[i] = 4 * r * Math.Cos(theta) * bpfOrde4[i - 1] - 4 * r * r * Math.Sqrt(Math.Cos(theta)) * bpfOrde4[i - 2] - 2 * r * r * bpfOrde4[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * bpfOrde4[i - 3]
                                 - Math.Pow(r, 4) * bpfOrde4[i - 4] + gain * (signal[i] - 2 * signal[i - 2] + signal[i - 4]);
                            break;
                    }
            });
            return bpfOrde4;
        }

        private double[] filterBPFOrde6(double[] signal)
        {
            double r = 0;
            double theta = 2 * Math.PI * _frequencyCutOff * _frekuensiSampling;
            double[] bpfOrde6 = new double[_jumlahData];
            //for (int i = 0; i <_jumlahData; i++)
            Parallel.For(0, _jumlahData, i =>
            {
                switch (i)
                {
                    case 0:
                        bpfOrde6[i] = 6 * r * Math.Cos(theta) * 0 - Math.Pow(r, 2) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * 0 + 4 * Math.Pow(r, 3) * Math.Cos(theta) * (3 + 2 * Math.Sqrt(Math.Cos(theta))) * 0 - Math.Pow(r, 4) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * 0 + 6 * Math.Pow(r, 5) * Math.Cos(theta) * 0 + Math.Pow(r, 6) * 0 + signal[i] - 3 * signal[0] + 3 * signal[0] - 1 * signal[0];
                        break;
                    case 1:
                        bpfOrde6[i] = 6 * r * Math.Cos(theta) * bpfOrde6[i - 1] - Math.Pow(r, 2) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[0] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * (3 + 2 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[0] - Math.Pow(r, 4) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[0] + 6 * Math.Pow(r, 5) * Math.Cos(theta) * bpfOrde6[0] + Math.Pow(r, 6) * bpfOrde6[0] + signal[i] - 3 * signal[0] + 3 * signal[0] - 1 * signal[0];
                        break;
                    case 2:
                        bpfOrde6[i] = 6 * r * Math.Cos(theta) * bpfOrde6[i - 1] - Math.Pow(r, 2) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * (3 + 2 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[0] - Math.Pow(r, 4) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[0] + 6 * Math.Pow(r, 5) * Math.Cos(theta) * bpfOrde6[0] + Math.Pow(r, 6) * bpfOrde6[0] + signal[i] - 3 * signal[i - 2] + 3 * signal[0] - 1 * signal[0];
                        break;
                    case 3:
                        bpfOrde6[i] = 6 * r * Math.Cos(theta) * bpfOrde6[i - 1] - Math.Pow(r, 2) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * (3 + 2 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 3] - Math.Pow(r, 4) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[0] + 6 * Math.Pow(r, 5) * Math.Cos(theta) * bpfOrde6[0] + Math.Pow(r, 6) * bpfOrde6[0] + signal[i] - 3 * signal[i - 2] + 3 * signal[0] - 1 * signal[0];
                        break;
                    case 4:
                        bpfOrde6[i] = 6 * r * Math.Cos(theta) * bpfOrde6[i - 1] - Math.Pow(r, 2) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * (3 + 2 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 3] - Math.Pow(r, 4) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 4] + 6 * Math.Pow(r, 5) * Math.Cos(theta) * bpfOrde6[0] + Math.Pow(r, 6) * bpfOrde6[0] + signal[i] - 3 * signal[i - 2] + 3 * signal[i - 4] - 1 * signal[0];
                        break;
                    case 5:
                        bpfOrde6[i] = 6 * r * Math.Cos(theta) * bpfOrde6[i - 1] - Math.Pow(r, 2) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * (3 + 2 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 3] - Math.Pow(r, 4) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 4] + 6 * Math.Pow(r, 5) * Math.Cos(theta) * bpfOrde6[i - 5] + Math.Pow(r, 6) * bpfOrde6[0] + signal[i] - 3 * signal[i - 2] + 3 * signal[i - 4] - 1 * signal[0];
                        break;
                    default:
                        bpfOrde6[i] = 6 * r * Math.Cos(theta) * bpfOrde6[i - 1] - Math.Pow(r, 2) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 2] + 4 * Math.Pow(r, 3) * Math.Cos(theta) * (3 + 2 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 3] - Math.Pow(r, 4) * (3 + 12 * Math.Sqrt(Math.Cos(theta))) * bpfOrde6[i - 4] + 6 * Math.Pow(r, 5) * Math.Cos(theta) * bpfOrde6[i - 5] + Math.Pow(r, 6) * bpfOrde6[i - 6] + signal[i] - 3 * signal[i - 2] + 3 * signal[i - 4] - 1 * signal[i - 6];
                        break;
                }
            });
            return bpfOrde6;
        }

        private double[] filterBSF(double[] signal)//notch
        {
            double r = 0;
            double theta = 2 * Math.PI * _frequencyCutOff * _frekuensiSampling;
            double gain = (1 - 2 * r * Math.Cos(theta) + r * r) / (2 - 2 * Math.Cos(theta));
            double[] bsf = new double[_jumlahData];
            for (int i = 0; i < _jumlahData; i++)
            {
                switch (i)
                {
                    case 0:
                        bsf[i] = 2 * r * Math.Cos(theta) * 0 - (r * r * 0) + (signal[i] - (2 * Math.Cos(theta) * signal[0]) + signal[0]);
                        break;
                    case 1:
                        bsf[i] = 2 * r * Math.Cos(theta) * bsf[i - 1] - (r * r * bsf[0]) + (signal[i] - (2 * Math.Cos(theta) * signal[i - 1]) + signal[0]);
                        break;
                    default:
                        bsf[i] = 2 * r * Math.Cos(theta) * bsf[i - 1] - (r * r * bsf[i - 2]) + (signal[i] - (2 * Math.Cos(theta) * signal[i - 1]) + signal[i - 2]);
                        break;
                }
            }
            return bsf;
        }

    }
}
