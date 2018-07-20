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
using Accord.Statistics;
using System.Threading;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Math.Optimization.Losses;
using Accord.MachineLearning.VectorMachines;
using System.IO.Ports;
using Accord.MachineLearning;
using Accord.Statistics.Analysis;
using EmotivController.NeuralNetwork;
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
        private bool _isTraining;
        //private List<double> _dataAF3 = new List<double>();
        //private List<double> _dataAF4 = new List<double>();
        private List<double> _dataF7 = new List<double>();
        private List<double> _dataF8 = new List<double>();
        private List<double> _dataF3 = new List<double>();
        private List<double> _dataF4 = new List<double>();
        private List<double> _dataFC5 = new List<double>();
        private List<double> _dataFC6 = new List<double>();
        private static int _frekuensiSampling = 128;
        private static int _second = 8;
        private static int _jumlahData = _frekuensiSampling * _second;
        private static int _secondTesting = 2;
        private static int _timeTesting = _frekuensiSampling * _secondTesting;
        private Stopwatch watch;
        private Stopwatch _watchTesting;
        private MulticlassSupportVectorMachine<Gaussian> machine;
        private SerialPort _myport;
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
                _isTraining = true;
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
                    if (_isTraining&&_selectedArrow!="testing")
                    {
                        // Place delegate on the Dispatcher.
                        if (_dataF3.Count() == _jumlahData)
                        {
                            string selectArrow = _selectedArrow;
                            string message = "Pengambilan Data sinyal otak selesai (" + selectArrow + ")";
                            string caption = "Setelah Klik OK, akan melakukan penyimpanan data!";
                            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                            DialogResult result;
                            result = MessageBox.Show(message, caption, buttons);
                            if (result == System.Windows.Forms.DialogResult.Yes)
                            {
                                preprocessingSignalTraining(_dataF3, _dataF4, _dataF7, _dataF8, _dataFC5, _dataFC6, selectArrow);
                                clearList();
                            }
                        }
                        else
                        {
                            _dataF3.Add(data.F3.value - mean);
                            _dataF4.Add(data.F4.value - mean);
                            _dataF7.Add(data.F7.value - mean);
                            _dataF8.Add(data.F8.value - mean);
                            _dataFC5.Add(data.FC5.value - mean);
                            _dataFC6.Add(data.FC6.value - mean);
                        }
                    }
                    else if(_isTraining==false&&_selectedArrow=="testing")
                    {
                        if (_dataF3.Count()== _timeTesting)
                        {
                            _watchTesting = new Stopwatch();
                            _watchTesting.Start();
                            var feature = preprocessingSignalTesting(_dataF3, _dataF4, _dataF7, _dataF8, _dataFC5, _dataFC6);
                            int dec = machine.Decide(feature)[0];
                            clearList();
                            sendCommand(dec);
                            watch.Stop();
                            MethodInvoker action = delegate{
                                tbElapsed.Text = "Respond Time"+_watchTesting.ElapsedMilliseconds+" ms" ;
                            };
                            tbElapsed.BeginInvoke(action);
                            Thread.Sleep(TimeSpan.FromSeconds(3));//rest 3 seconds
                        }
                        else
                        {
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
                    Console.WriteLine("Server Already exit"+e);
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

        private double _frequencyCutOffBPF = 30;
        private double _frequencyCutOffBSF = 10;

        private double[] filterBPFOrde2(double[] signal)
        {
            double r = 0.8;
            int jumlahData = signal.Length;
            double theta = 2 * Math.PI * _frequencyCutOffBPF / _frekuensiSampling;
            double gain= ((1 - r) * Math.Sqrt(1 - 2 * r * Math.Cos(2 * theta) + r * r)) / 2 * Math.Abs(Math.Sin(theta));
            double[] bpf = new double[jumlahData];
            Parallel.For(0, jumlahData, i =>
            {
                switch (i)
                {
                    case 0:
                        bpf[i] = 2 * r * Math.Cos(theta) * 0 - (Math.Pow(r, 2) * 0) + gain * (signal[i]-0);
                        break;
                    case 1:
                        bpf[i] = 2 * r * Math.Cos(theta) * bpf[i - 1] - (Math.Pow(r, 2) * 0) + gain * (signal[i] - 0);
                        break;
                    default:
                        bpf[i] = 2 * r * Math.Cos(theta) * bpf[i - 1] - (Math.Pow(r, 2) * bpf[i - 2]) + gain * (signal[i] - signal[i - 2]);
                        break;
                }
            });
            return bpf;
        }

        private double[] filterBPFOrde4(double[] signal, int jumlahData)
        {
            double r = 0.8;
            double theta = 2 * Math.PI * (_frequencyCutOffBPF / _frekuensiSampling);
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
            double r = 0.8;
            double theta = 2 * Math.PI * _frequencyCutOffBPF / _frekuensiSampling;
            int jumlahData = signal.Length;
            double[] bpfOrde6 = new double[jumlahData];
            //for (int i = 0; i <_jumlahData; i++)
            Parallel.For(0, jumlahData, i =>
            {
                //for (int i = 0; i < jumlahData; i++)
                //{
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
                //}
            });
            return bpfOrde6;
        }

        private double[] filterBSF(double[] signal)//notch
        {
            double r = 0.4;
            int jumlahData = signal.Length;
            double theta = 2 * Math.PI * _frequencyCutOffBSF * _frekuensiSampling;
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

        private double[] windowing(double[] signal)
        {
            int jumlahData = signal.Length;
            double[] window = new double[signal.Length];
            double[] windowFinal = new double[signal.Length];
            Parallel.For(0, jumlahData, i =>
            {
                window[i] = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (jumlahData - 1)));
            });
            Parallel.For(0, jumlahData, i =>
            {
                windowFinal[i] = signal[i] * window[i];
            });
            return windowFinal;
        }

        private double[] FFT(double[] signal)
        {
            int nPoint = (int)signal.Length;
            Complex[] fftComplex = new Complex[nPoint];
            double[] fft = new double[nPoint];
            Parallel.For(0, nPoint, i =>
            {
                fftComplex[i] = new Complex(signal[i], 0.0); // make it complex format
            });
            Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);
            Parallel.For(0, nPoint, i => {
                fft[i] = fftComplex[i].Magnitude; // back to double
            });
            return fft;
        }
        
        private double[] extractionFeature(double[] signal)
        {
            double[] feature = new double[6];
            feature[0] = mean(signal);
            feature[1] = modus(signal);
            feature[2] = min(signal);
            feature[3] = max(signal);
            feature[4] = standardDeviation(signal);
            feature[5] = median(signal);
            return feature;
        }

        private double mean(double[]signal)
        {
            double zigma = 0;
            var output = signal
                .Select(w => zigma += w);
            return (zigma / signal.Length);
        }
        private double modus(double[] signal)
        {
            return signal.GroupBy(v => v)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key;
        }
        private double min(double[] signal)
        {
            Array.Sort(signal);
            return signal[0];
        }
        private double max(double[] signal)
        {
            var desc = from s in signal
                       orderby s descending
                       select s;
            return desc.ToArray()[0];
        }
        private double standardDeviation(double[] signal)
        {
            double average = signal.Average();
            double sumOfSquaresOfDifferences = signal.Select(val => (val - average) * (val - average)).Sum();
            return Math.Sqrt(sumOfSquaresOfDifferences / signal.Length);
        }
        private double median(double[] signal)
        {
            int numberCount = signal.Count();
            int halfIndex = signal.Count() / 2;
            var sortedNumbers = signal.OrderBy(n => n);
            //double median;
            if ((numberCount % 2) == 0)
            {
                return ((sortedNumbers.ElementAt(halfIndex) +
                    sortedNumbers.ElementAt((halfIndex - 1))) / 2);
            }
            else
            {
                return sortedNumbers.ElementAt(halfIndex);
            }
        }

        //private double[] extractionFeature(double[] signal)
        //{

        //}

        private void preprocessingSignalTraining(
            List<double> dataF3, List<double> dataF4, List<double> dataF7, List<double> dataF8,
            List<double> dataFC5, List<double> dataFC6, string selectArrow)
        {
            double[][] BPFOrde2 = new double[6][];
            double[][] BSF = new double[6][];
            double[][] window = new double[6][];
            double[][] vFFT = new double[6][];
            double[][] feature = new double[6][];
                BPFOrde2[0] = filterBPFOrde2(dataF3.ToArray());
                savePerNewFile(dataF3.ToArray(), "RAW", "F3", selectArrow);
                savePerNewFile(BPFOrde2[0], "BPF", "AF4", selectArrow);
                BPFOrde2[1] = filterBPFOrde2(dataF4.ToArray());
                savePerNewFile(dataF4.ToArray(), "RAW", "F4", selectArrow);
                savePerNewFile(BPFOrde2[1], "BPF", "AF4", selectArrow);
                BPFOrde2[2] = filterBPFOrde2(dataF7.ToArray());
                savePerNewFile(dataF7.ToArray(), "RAW", "F7", selectArrow);
                savePerNewFile(BPFOrde2[2], "BPF", "F7", selectArrow);
                BPFOrde2[3] = filterBPFOrde2(dataF8.ToArray());
                savePerNewFile(dataF8.ToArray(), "RAW", "F8", selectArrow);
                savePerNewFile(BPFOrde2[3], "BPF", "F8", selectArrow);
                BPFOrde2[4] = filterBPFOrde2(dataFC5.ToArray());
                savePerNewFile(dataFC5.ToArray(), "RAW", "FC5", selectArrow);
                savePerNewFile(BPFOrde2[4], "BPF", "FC5", selectArrow);
                BPFOrde2[5] = filterBPFOrde2(dataFC6.ToArray());
                savePerNewFile(dataFC6.ToArray(), "RAW", "FC6", selectArrow);
                savePerNewFile(BPFOrde2[5], "BPF", "FC6", selectArrow);
            string[] signal = new string[6] { "F3", "F4", "F7", "F8", "FC5", "FC6" };
            Parallel.For(0, 6, i =>
            {
                BSF[i] = filterBSF(BPFOrde2[i]);
                savePerNewFile(BSF[i], "BSF", signal[i], selectArrow);
            });
            Parallel.For(0, 6, i =>
            {
                window[i] = windowing(BSF[i]);
                savePerNewFile(BSF[i], "Window", signal[i], selectArrow);
            });
            Parallel.For(0, 6, i =>
            {
                vFFT[i] = FFT(window[i]);
                savePerNewFile(vFFT[i], "FFT", signal[i], selectArrow);
            });
            Parallel.For(0, 6, i =>
            {
                feature[i] = extractionFeature(vFFT[i]);
            });
            saveOneFile(feature, selectArrow);
            disableEnableButton();
        }

        private double[][] preprocessingSignalTesting(
            List<double> dataF3, List<double> dataF4, List<double> dataF7, List<double> dataF8,
            List<double> dataFC5, List<double> dataFC6)
        {
            double[][] BPFOrde2 = new double[6][];
            double[][] BSF = new double[6][];
            double[][] window = new double[6][];
            double[][] vFFT = new double[6][];
            double[][] feature = new double[6][];
            BPFOrde2[0] = filterBPFOrde2(dataF3.ToArray());
            BPFOrde2[1] = filterBPFOrde2(dataF4.ToArray());
            BPFOrde2[2] = filterBPFOrde2(dataF7.ToArray());
            BPFOrde2[3] = filterBPFOrde2(dataF8.ToArray());
            BPFOrde2[4] = filterBPFOrde2(dataFC5.ToArray());
            BPFOrde2[5] = filterBPFOrde2(dataFC6.ToArray());
            string[] signal = new string[6] { "F3", "F4", "F7", "F8", "FC5", "FC6" };
            Parallel.For(0, 6, i =>
            {
                BSF[i] = filterBSF(BPFOrde2[i]);
            });
            Parallel.For(0, 6, i =>
            {
                window[i] = windowing(BSF[i]);
            });
            Parallel.For(0, 6, i =>
            {
                vFFT[i] = FFT(window[i]);
            });
            Parallel.For(0, 6, i =>
            {
                feature[i] = extractionFeature(vFFT[i]);
            });
            var ft = new double[1][];
            var perchannel = new List<double>();
            foreach(var f in feature)
            {
                foreach(var c in f)
                {
                    perchannel.Add(c);
                }
            }
            ft[0] = perchannel.ToArray();
            return ft;
        }

        private void disableEnableButton()
        {
            Console.WriteLine("Masuk disableenabled");
            MethodInvoker action;
            switch (_selectedArrow)
            {
                case "kiri":
                    action = delegate
                    { btKanan.Enabled = true; };
                    btKanan.BeginInvoke(action);
                    _selectedArrow = "";
                    break;
                case "kanan":
                    action = delegate
                    { btMaju.Enabled = true; };
                    btMaju.BeginInvoke(action);
                    _selectedArrow = "";
                    break;
                case "maju":
                    action = delegate
                    { btMundur.Enabled = true; };
                    btMundur.BeginInvoke(action);
                    _selectedArrow = "";
                    break;
                case "mundur":
                    action = delegate
                    { btBerhenti.Enabled = true; };
                    btBerhenti.BeginInvoke(action);
                    _selectedArrow = "";
                    break;
                case "berhenti":
                    action = delegate
                    { btfromZero.Enabled = true; };
                    btfromZero.BeginInvoke(action);
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

        private void savePerNewFile(double[] signal, string context, string signalName, string selectArrow)
        {
            string filename = "csv/" + context + "_" + signalName + "_" + selectArrow + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".csv";
            for (int i = 0; i < signal.Length; i++)
            {
                File.AppendAllText(filename, signal[i] + Environment.NewLine);
            }
        }

        private void saveOneFile(double[][] feature, string selectarrow)
        {
            string filename = "csv/feature.csv";
            string kelas = "csv/kelas.csv";
            for(int i = 0; i < feature.Length; i++)
            {
                for(int j = 0; j < feature[i].Length; j++)
                {
                    File.AppendAllText(filename, feature[i][j]+";");
                    if (i == feature.Length - 1 &&  j == feature[feature.Length - 1].Length - 1)
                    {
                        File.AppendAllText(filename, feature[i][j]  + "" + Environment.NewLine);
                    }
                }
            }

            //int arrow;
            switch (selectarrow)
            {
                case "kiri":
                    File.AppendAllText(kelas, 0 + "" + Environment.NewLine);
                    break;
                case "kanan":
                    File.AppendAllText(kelas, 1 + "" + Environment.NewLine);
                    break;
                case "maju":
                    File.AppendAllText(kelas, 2 + "" + Environment.NewLine);
                    break;
                case "mundur":
                    File.AppendAllText(kelas, 3 + "" + Environment.NewLine);
                    break;
                case "berhenti":
                    File.AppendAllText(kelas, 4 + "" + Environment.NewLine);
                    break;
            }
            MessageBox.Show("Save Done");
        }

        private Tuple<double[][], int[]> loadOneFile()
        {
            string dokumenTraining = "csv/feature.csv";
            var csv = File.ReadLines(dokumenTraining);
            int jumlahDataTraining = csv.Count();
            double[][] inputs = new double[jumlahDataTraining][];
            int[] outputs = new int[jumlahDataTraining];
            var lines = new List<int[]>();
            int index = 0;
            foreach (var rows in csv)
            {
                inputs[index] = rows.Split(';').Select(double.Parse).ToArray();
                index++;
            }
            string kelas = "csv/kelas.csv";
            index = 0;
            csv = File.ReadLines(kelas);
            foreach (var rows in csv)
            {
                outputs[index] = Convert.ToInt32(rows);
                index++;
            }
            return Tuple.Create(inputs, outputs);
        }

        private void btTesting_Click(object sender, EventArgs e)
        {
            initArduino();
            _isTraining = false;
            _selectedArrow = "testing";
            btTesting.Enabled = false;
            btCancelTest.Enabled = true;
        }

        private void btTraining_Click(object sender, EventArgs e)
        {
            svm();
        }
        private void knn()
        {
            var pair = loadOneFile();
            var feature = pair.Item1;
            var kelas = pair.Item2;
            var knn = new KNearestNeighbors(k: 4);
            knn.NumberOfInputs = 36;
            knn.Learn(feature, kelas);
            int[] predicted = knn.Decide(feature);
            accurate(kelas, predicted);
        }
        private void svm()
        {
            var pair = loadOneFile();
            var feature = pair.Item1;
            var kelas = pair.Item2;
            var teacher = new MulticlassSupportVectorLearning<Gaussian>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                {
                    // Estimate a suitable guess for the Gaussian kernel's parameters.
                    // This estimate can serve as a starting point for a grid search.
                    UseKernelEstimation = true
                }
            };

            // Learn a machine
            machine = teacher.Learn(feature, kelas);


            // Create the multi-class learning algorithm for the machine
            var calibration = new MulticlassSupportVectorLearning<Gaussian>()
            {
                Model = machine, // We will start with an existing machine

                // Configure the learning algorithm to use Platt's calibration
                Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
                {
                    Model = param.Model // Start with an existing machine
                }
            };


            // Configure parallel execution options
            calibration.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Learn a machine
            calibration.Learn(feature, kelas);

            // Obtain class predictions for each sample
            int[] predicted = machine.Decide(feature);

            // Get class scores for each sample
            double[] scores = machine.Score(feature);

            // Get log-likelihoods (should be same as scores)
            double[][] logl = machine.LogLikelihoods(feature);

            // Get probability for each sample
            double[][] prob = machine.Probabilities(feature);

            // Compute classification error
            double error = new ZeroOneLoss(kelas).Loss(predicted);
            double loss = new CategoryCrossEntropyLoss(kelas).Loss(prob);

            double temp = 0;
            for (int j = 0; j < predicted.Length; j++)
            {
                if (kelas[j] == predicted[j])
                {
                    temp++;
                }
            }
            double accurate = (temp / predicted.Length) * 100;
            tbAkurasi.Text = accurate + "%";
            MessageBox.Show("Learning Done, Thankyou!");
            btTesting.Enabled = true;
        }
        private void lda()
        {
            var pair = loadOneFile();
            var feature = pair.Item1;
            var kelas = pair.Item2;
            var lda = new LinearDiscriminantAnalysis(feature,kelas);
            lda.Compute();
            var predicted = lda.Classify(feature);
            accurate(kelas, predicted);
        }
        private void kda()
        {
            var pair = loadOneFile();
            var feature = pair.Item1;
            var kelas = pair.Item2;
            IKernel kernel = new Gaussian(3.6);
            var kda = new KernelDiscriminantAnalysis(feature, kelas, kernel);
            kda.Compute();
            int[] predicted = kda.Classify(feature);
            accurate(kelas, predicted);
        }
        private void bpnn()
        {
            var pair = loadOneFile();
            var feature = pair.Item1;
            var kelas = pair.Item2;
            int numInput = 36; // number features
            int numHidden = 5;
            int numOutput = 5; // number of classes for Y
            var bpnn = new Backpropagation(numInput, numHidden, numOutput);
            int maxEpochs = 1000;
            double learnRate = 0.05;
            double momentum = 0.01;
            double[] weights = bpnn.Train(feature, maxEpochs, learnRate, momentum);
        }
        private void accurate(int[] kelas, int[] predicted)
        {
            double temp = 0;
            for (int j = 0; j < predicted.Length; j++)
            {
                if (kelas[j] == predicted[j])
                {
                    temp++;
                }
            }
            foreach (var v in kelas)
            {
                Console.Write(v + " ");
            }
            Console.WriteLine();
            foreach (var v in predicted)
            {
                Console.Write(v + " ");
            }
            Console.WriteLine();
            double accurate = (temp / predicted.Length) * 100;
            tbAkurasi.Text = accurate + "%";
            MessageBox.Show("Learning Done, Thankyou!");
            btTesting.Enabled = true;
        }

        private void btAmbilDataLagi_Click(object sender, EventArgs e)
        {
            _isTraining = true;
            btKiri.Enabled = true;
        }

        private void btCancelTest_Click(object sender, EventArgs e)
        {
            _isTraining = true;
            btTesting.Enabled = true;
            btCancelTest.Enabled = false;
        }

        private void initArduino()
        {
            try
            {
                _myport = new SerialPort();
                _myport.BaudRate = 9600;//?
                _myport.PortName = "COM4";//?
                _myport.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("Can't find Wheelchair! " + e);
            }
        }
        private void sendCommand(int command)
        {
            _myport.Write(command + "");
            MethodInvoker action;
            switch (command)
            {
                case 0:
                    action = delegate
                    { tbArah.Text = "kiri"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 1:
                    action = delegate
                    { tbArah.Text = "kanan"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 2:
                    action = delegate
                    { tbArah.Text = "maju"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 3:
                    action = delegate
                    { tbArah.Text = "mundur"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 4:
                    action = delegate
                    { tbArah.Text = "berhenti"; };
                    tbArah.BeginInvoke(action);
                    break;
            }

        }
    }
}
