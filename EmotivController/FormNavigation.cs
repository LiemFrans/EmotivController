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
using System.IO;
using System.Numerics;
namespace EmotivController
{
    public partial class FormNavigation : Form
    {
        private Process cmdProcess;
        private bool isOK = false;
        private bool isRunning = false;
        //private bool isConnect = false;
        private WebSocket ws;
        private string old_data = "";
        private string ipAddress = "127.0.0.1";
        private string _selectedArrow = "";
        private List<double> _dataAF3 = new List<double>();
        private List<double> _dataAF4 = new List<double>();
        private List<double> _dataF7 = new List<double>();
        private List<double> _dataF8 = new List<double>();
        private List<double> _dataF3 = new List<double>();
        private List<double> _dataF4 = new List<double>();
        private List<double> _dataFC5 = new List<double>();
        private List<double> _dataFC6 = new List<double>();
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
                Console.WriteLine(ws.IsAlive);
                btConnect.Enabled = false;
                btfromZero.Enabled = true;
                btLangsungTraining.Enabled = true;
                ws.Connect();
                transportSignal.RunWorkerAsync();
                
            }
        }



        private void transportSignal_DoWork(object sender, DoWorkEventArgs e)
        {
            ws.OnMessage += OnMessageResult;
            ws.Send("START");
        }

        public void OnMessageResult(object sender, MessageEventArgs e)
        {
            if (!old_data.Equals(e.Data))
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
                    Console.WriteLine("error : " + ex.Message);
                }
                if (isParsed&&_selectedArrow!="")
                {
                    // Place delegate on the Dispatcher.
                    if (_dataAF3.Count() == _jumlahData)
                    {
                        string selectArrow = _selectedArrow;
                        disableEnableButton();
                        string message = "Pengambilan Data sinyal otak selesai ("+selectArrow+")";
                        string caption = "Setelah Klik OK, akan melakukan penyimpanan data!";
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;
                        result = MessageBox.Show(message, caption, buttons);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            preprocessingSignalTraining(_dataAF3,_dataAF4,_dataF3,_dataF4,_dataF7,_dataF8,_dataFC5,_dataFC6, selectArrow);
                        }
                        clearList();
                    }
                    else
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
            }
        }

        //public void OnMessageResult(object sender, MessageEventArgs e)
        //{
        //    if (isRunning == false)
        //    {
        //        isRunning = true;
        //        Console.WriteLine(isRunning);
        //        watch = new Stopwatch();
        //        Console.WriteLine("start watch");
        //        watch.Start();
        //    }
        //    else
        //    {
        //        Console.WriteLine("Masuk");
        //        if (watch.Elapsed.Seconds != 5)
        //        {
        //            old_data = e.Data;
        //            DataModel data = new DataModel();
        //            bool isParsed = false;
        //            float mean = (float)4167.9950522878;
        //            try
        //            {
        //                data = new JavaScriptSerializer().Deserialize<DataModel>(e.Data);
        //                isParsed = true;
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("Error " + ex);
        //                Console.WriteLine("error : " + ex.Message);
        //            }
        //            if (isParsed)
        //            {
        //                _dataAF3.Add(data.AF3.value - mean);
        //                _dataAF4.Add(data.AF3.value - mean);
        //                _dataF3.Add(data.F3.value - mean);
        //                _dataF4.Add(data.F4.value - mean);
        //                _dataF7.Add(data.F7.value - mean);
        //                _dataF8.Add(data.F8.value - mean);
        //                _dataFC5.Add(data.FC5.value - mean);
        //                _dataFC6.Add(data.FC6.value - mean);
        //            }
        //        }
        //        else if (watch.Elapsed.Seconds >= 5)
        //        {
        //            if (_dataAF3.Count==_jumlahData)
        //            {
        //                Console.WriteLine("Stop");
        //                //ws.Close();
        //                ws.Send("STOP");
        //                watch.Stop();
        //                isRunning = false;
        //                string message = "Done";
        //                string caption = "Done Capture Signal";
        //                MessageBoxButtons buttons = MessageBoxButtons.OK;
        //                DialogResult result;
        //                result = MessageBox.Show(message, caption, buttons);
        //                ws.Close();
        //                if (result == System.Windows.Forms.DialogResult.Yes)
        //                {

        //                }
        //            }

        //        }
        //    }

        //}

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
        
        private void btfromZero_Click(object sender, EventArgs e)
        {
            btKiri.Enabled = true;
        }

        private void btLangsungTraining_Click(object sender, EventArgs e)
        {
            btTraining.Enabled = true;
        }

        //serverStart DO WORK
        private void serverStart_DoWork(object sender, DoWorkEventArgs e)
        {
            var psi = new ProcessStartInfo("CMD.exe", "/K python C:\\Users\\Frans\\Documents\\GitHub\\EmoGuy\\socketEmoGuy.py");
            cmdProcess = Process.Start(psi);
        }
        
        //transport Signal DO WORK


        //Preprocessing

        private double[] DFT(double[] signal)
        {
            int jumlahData = signal.Length;
            double[] re = new double[jumlahData];
            double[] im = new double[jumlahData];
            double[] hasilKuadrat = new double[jumlahData];
            double[] dft = new double[jumlahData];
            Parallel.For(0, jumlahData, i =>
            {
                re[i] = 0;
                im[i] = 0;
                for(int j = 0; j < jumlahData; j++)
                {
                    re[i] = re[i] + signal[j] * Math.Cos(2 * Math.PI * j * i) / jumlahData;
                    im[i] = im[i] - signal[j] * Math.Sin(2 * Math.PI * j * i) / jumlahData;
                }
                hasilKuadrat[i] = Math.Sqrt(re[i]) + Math.Sqrt(im[i]);
                dft[i] = Math.Sqrt(hasilKuadrat[i]) / (jumlahData / 2);
            });
            return dft;
        }

        private double[] FFT(double[] signal)
        {
            double[] fft = new double[signal.Length];
            Complex[] fftComplex = new Complex[signal.Length];
            //Parallel.For(0, signal.Length, i =>
            //{
            for (int i = 0; i < signal.Length; i++)
            {
                fftComplex[i] = new Complex(signal[i], 0.0);
            }
            //});
            Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);
            //Parallel.For(0, signal.Length, i =>
            //{
            for (int i = 0; i < signal.Length; i++)
            {
                fft[i] = fftComplex[i].Magnitude;
            }
            //});
            return fft;
        }

        double _frequencyCutOff = 0.5;

        private double[] filterBPFOrde4(double[] signal, int jumlahData)
        {
            double r = 0;
            double theta = 2 * Math.PI * (_frequencyCutOff / _frekuensiSampling);
            double[] bpfOrde4 = new double[jumlahData];
            double gain = ((1 - r) * Math.Sqrt(1 - 2 * r * Math.Cos(2 * theta) + r * r)) / 4 * Math.Abs(Math.Sin(theta));
            //for (int i = 0; i < _jumlahData; i++)
            Parallel.For(0, jumlahData, i =>
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
            double theta = 2 * Math.PI * _frequencyCutOff / _frekuensiSampling;
            int jumlahData = signal.Length;
            double[] bpfOrde6 = new double[jumlahData];
            //for (int i = 0; i <_jumlahData; i++)
            Parallel.For(0, jumlahData, i =>
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
            int jumlahData = signal.Length;
            double theta = 2 * Math.PI * _frequencyCutOff * _frekuensiSampling;
            double gain = (1 - 2 * r * Math.Cos(theta) + r * r) / (2 - 2 * Math.Cos(theta));
            double[] bsf = new double[jumlahData];
            //for (int i = 0; i < _jumlahData; i++)
            Parallel.For(0, jumlahData, i =>
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
            });
            return bsf;
        }

        private void preprocessingSignalTraining(List<double>dataAF3, List<double>dataAF4,
            List<double>dataF3, List<double>dataF4, List<double>dataF7, List<double>dataF8,
            List<double>dataFC5, List<double>dataFC6, string selectArrow)
        {
            double[][] BPFOrde6 = new double[8][];
            double[][] BSF = new double[8][];
            double[][] vFFT = new double[8][];

            BPFOrde6[0] = filterBPFOrde6(dataAF3.ToArray());
            savePerNewFile(dataAF3.ToArray(), "RAW", "AF3");
            savePerNewFile(BPFOrde6[0], "BPF", "AF3");
            BPFOrde6[1] = filterBPFOrde6(dataAF4.ToArray());
            savePerNewFile(dataAF4.ToArray(), "RAW", "AF4");
            savePerNewFile(BPFOrde6[1], "BPF", "AF4");
            BPFOrde6[2] = filterBPFOrde6(dataF3.ToArray());
            savePerNewFile(dataF3.ToArray(), "RAW", "F3");
            savePerNewFile(BPFOrde6[2], "BPF", "AF4");
            BPFOrde6[3] = filterBPFOrde6(dataF4.ToArray());
            savePerNewFile(dataF4.ToArray(), "RAW", "F4");
            savePerNewFile(BPFOrde6[3], "BPF", "AF4");
            BPFOrde6[4] = filterBPFOrde6(dataF7.ToArray());
            savePerNewFile(dataF7.ToArray(), "RAW", "F7");
            savePerNewFile(BPFOrde6[4], "BPF", "F7");
            BPFOrde6[5] = filterBPFOrde6(dataF8.ToArray());
            savePerNewFile(dataF8.ToArray(), "RAW", "F8");
            savePerNewFile(BPFOrde6[5], "BPF", "F8");
            BPFOrde6[6] = filterBPFOrde6(dataFC5.ToArray());
            savePerNewFile(dataFC5.ToArray(), "RAW", "FC5");
            savePerNewFile(BPFOrde6[6], "BPF", "FC5");
            BPFOrde6[7] = filterBPFOrde6(dataFC6.ToArray());
            savePerNewFile(dataFC6.ToArray(), "RAW", "FC6");
            savePerNewFile(BPFOrde6[7], "BPF", "FC6");
            string[] signal = new string[8] { "AF3", "AF4", "F3", "F4", "F7", "F8", "FC5", "FC6" };
            Parallel.For(0, 8, i =>
            {
                BSF[i] = filterBSF(BPFOrde6[i]);
                savePerNewFile(BSF[i], "BSF", signal[i]);
            });
            Parallel.For(0, 8, i =>
            {
                vFFT[i] = FFT(BSF[i]);
                savePerNewFile(vFFT[i], "FFT", signal[i]);
            });
        }

        private void preprocessingSignalTesting()
        {
            double[][] BPFOrde6 = new double[8][];
            double[][] BSF = new double[8][];
            double[][] vDFT = new double[8][];
        }

        private void disableEnableButton()
        {
            switch (_selectedArrow)
            {
                case "kiri":
                    btKanan.Invoke(new MethodInvoker(delegate { Enabled = true; }));
                    _selectedArrow = "";
                    break;
                case "kanan":
                    btMundur.Invoke(new MethodInvoker(delegate { Enabled = true; }));
                    _selectedArrow = "";
                    break;
                case "maju":
                    btMundur.Invoke(new MethodInvoker(delegate { Enabled = true; }));
                    _selectedArrow = "";
                    break;
                case "mundur":
                    btBerhenti.Invoke(new MethodInvoker(delegate { Enabled = true; }));
                    _selectedArrow = "";
                    break;
                case "berhenti":
                    btfromZero.Invoke(new MethodInvoker(delegate { Enabled = true; }));
                    _selectedArrow = "";
                    break;
            }
        }

        private void btKiri_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke kiri";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _selectedArrow = "kiri";
                btKiri.Enabled = false;
            }

        }

        private void btKanan_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke kanan";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _selectedArrow = "kanan";
                btKanan.Enabled = false;
            }
        }

        private void btMaju_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke maju";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _selectedArrow = "maju";
                btMaju.Enabled = false;
            }
        }

        private void btMundur_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke mundur";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _selectedArrow = "mundur";
                btMundur.Enabled = false;
            }
        }

        private void btBerhenti_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke berhenti";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _selectedArrow = "berhenti";
                btBerhenti.Enabled = false;
            }
        }

        private void savePerNewFile(double[] signal, string context, string signalName)
        {
            string filename = "csv/"+context+"_"+signalName+"_"+ DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")+".csv";
            for (int i = 0; i < signal.Length; i++)
            {
                File.AppendAllText(filename, signal[i] + Environment.NewLine);
            }
        }
    }
}
