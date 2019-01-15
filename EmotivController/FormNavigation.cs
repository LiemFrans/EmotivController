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
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Math;
using Accord.Controls;
using Accord.Neuro.Networks;
using MathNet.Filtering.IIR;
using Accord.MachineLearning.DecisionTrees;
using Accord.DataSets;
using Accord.MachineLearning.Bayes;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Distributions.Fitting;
using Accord.Math.Distances;

namespace EmotivController
{
    public partial class FormNavigation : Form
    {
        private Process cmdProcess;
        private bool isOK = false;
        //private bool isRunning = false;
        //private bool isConnect = false;
        private WebSocket ws;
        private string old_data = "";
        private string ipAddress = "127.0.0.1";
        private string _selectedArrow = "";
        private bool _isTraining;
        private List<double> _dataF3 = new List<double>();
        private List<double> _dataF4 = new List<double>();
        private List<double> _dataF7 = new List<double>();
        private List<double> _dataF8 = new List<double>();
        private List<double> _dataFC5 = new List<double>();
        private List<double> _dataFC6 = new List<double>();
        private static int _frekuensiSampling = 128;
        private static int _second = 4;
        private static int _jumlahData = _frekuensiSampling * _second;
        private static int _secondTesting = 4;
        private static int _timeTesting = _frekuensiSampling * _secondTesting;
        private int _cuttOffLowBPF = 8;
        private int _cutOffHighBPF = 30;
        private Stopwatch watch;
        private Stopwatch _watchTesting;
        private MulticlassSupportVectorMachine<Gaussian> machine;
        private KNearestNeighbors knn;
        private SerialPort _myport;
        private Focus focus;
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
        int times = 0;
        int akusisike = 0;
        public void OnMessageResult(object sender, MessageEventArgs e)
        {
            
            if (_isDecide == false && !old_data.Equals(e.Data))
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
                if (isParsed && _selectedArrow != "")
                {
                    if (_isTraining && _selectedArrow != "testing")
                    {
                        if (_dataF3.Count()== _jumlahData)
                        {
                            string selectArrow = _selectedArrow;
                            MethodInvoker action = delegate
                            {
                                focus.Hide();
                            };
                            focus.BeginInvoke(action);
                            Console.WriteLine("masuk");
                            string message = "Pengambilan Data sinyal otak selesai (" + selectArrow + ")";
                            string caption = "Setelah Klik OK, akan melakukan penyimpanan data!";
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            DialogResult result;
                            result = MessageBox.Show(message, caption, buttons);
                            if (result == System.Windows.Forms.DialogResult.OK)
                            {
                                preprocessingSignalTraining(_dataF3, _dataF4, _dataF7, _dataF8, _dataFC5, _dataFC6, selectArrow);
                                MessageBox.Show("Save Done");
                                clearList();
                            }
                            times = 0;
                            akusisike++;
                        }
                        else if(_dataF3.Count()<_jumlahData)
                        {
                            Console.WriteLine("masuk akusisi "+akusisike+", " + times);
                            _dataF3.Add(data.F3.value - mean);
                            _dataF4.Add(data.F4.value - mean);
                            _dataF7.Add(data.F7.value - mean);
                            _dataF8.Add(data.F8.value - mean);
                            _dataFC5.Add(data.FC5.value - mean);
                            _dataFC6.Add(data.FC6.value - mean);
                            times++;
                        }
                    }
                    else if (_isTraining == false && _selectedArrow == "testing")
                    {
                        if (_dataF3.Count()== _jumlahData)
                        {
                            _isDecide = true;
                            {
                                /*
                                //MethodInvoker action;
                                //_watchTesting = new Stopwatch();
                                //_watchTesting.Start();
                                //var feature = preprocessingSignalTesting(_dataF3, _dataF4, _dataF7, _dataF8, _dataFC5, _dataFC6);
                                //int dec = machine.Decide(feature[0]);

                                //int decknn = knn.Decide(feature[0]);
                                //sendCommand(dec, decknn);
                                //Console.WriteLine(dec + ", " + decknn);
                                //_watchTesting.Stop();

                                //action = delegate
                                //{
                                //    tbElapsed.Text = "Respond Time" + _watchTesting.ElapsedMilliseconds + " ms";
                                //};
                                //tbElapsed.BeginInvoke(action);
                                //action = delegate
                                //{ tbJalan.Text = "JALAN"; };
                                //tbJalan.BeginInvoke(action);
                                //if (sleep(2))
                                //{
                                //    action = delegate
                                //    { tbJalan.Text = "ISTIRAHAT"; };
                                //    tbJalan.BeginInvoke(action);
                                //    sendCommand(4, 4);
                                //    if (sleep(4))
                                //    {
                                //        action = delegate
                                //        { tbJalan.Text = "BERPIKIR"; };
                                //        tbJalan.BeginInvoke(action);
                                //        times = 0;
                                //        clearList();
                                //        sleep(4);
                                //        akusisike++;
                                //    }
                                //}*/
                            }
                        }
                        else if (_dataF3.Count() < _timeTesting)
                        {
                            MethodInvoker action = delegate
                            { tbTime.Text = ""+times; };
                            tbJalan.BeginInvoke(action);
                            _dataF3.Add(data.F3.value - mean);
                            _dataF4.Add(data.F4.value - mean);
                            _dataF7.Add(data.F7.value - mean);
                            _dataF8.Add(data.F8.value - mean);
                            _dataFC5.Add(data.FC5.value - mean);
                            _dataFC6.Add(data.FC6.value - mean);
                            times++;
                        }
                    }
                }
            }
            else if (_isDecide && !old_data.Equals(e.Data))
            {
                MethodInvoker action;
                _watchTesting = new Stopwatch();
                _watchTesting.Start();
                var feature = preprocessingSignalTesting(_dataF3, _dataF4, _dataF7, _dataF8, _dataFC5, _dataFC6);
                int dec = machine.Decide(feature[0]);

                int decknn = knn.Decide(feature[0]);
                sendCommand(dec, decknn);
                Console.WriteLine(dec + ", " + decknn);
                _watchTesting.Stop();

                action = delegate
                {
                    tbElapsed.Text = "Respond Time" + _watchTesting.ElapsedMilliseconds + " ms";
                };
                tbElapsed.BeginInvoke(action);
                action = delegate
                { tbJalan.Text = "JALAN"; };
                tbJalan.BeginInvoke(action);
                if (sleep(2))
                {
                    action = delegate
                    { tbJalan.Text = "ISTIRAHAT"; };
                    tbJalan.BeginInvoke(action);
                    sendCommand(4, 4);
                    if (sleep(4))
                    {
                        action = delegate
                        { tbJalan.Text = "BERPIKIR"; };
                        tbJalan.BeginInvoke(action);
                        if (clearList())
                        {
                            times = 0;
                            akusisike++;
                            _isDecide = false;
                        }
                    }
                }
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
                    Console.WriteLine("Server Already exit" + e);
                }
            }
        }

        private bool clearList()
        {
            _dataF3.Clear();
            _dataF4.Clear();
            _dataF7.Clear();
            _dataF8.Clear();
            _dataFC5.Clear();
            _dataFC6.Clear();
            return true;
        }

        private void btfromZero_Click(object sender, EventArgs e)
        {
            btKiri.Enabled = true;
        }

        private void btLangsungTraining_Click(object sender, EventArgs e)
        {
            btTraining.Enabled = true;
        }

        private void serverStart_DoWork(object sender, DoWorkEventArgs e)
        {
            var psi = new ProcessStartInfo("CMD.exe", "/K python C:\\Users\\Frans\\Documents\\GitHub\\EmoGuy\\socketEmoGuy.py");
            cmdProcess = Process.Start(psi);
        }

        private double[] filterBPF(double[] signal, int cutOffLow, int cutOffHigh)
        {
            var bpfFilter = OnlineIirFilter.CreateBandpass(MathNet.Filtering.ImpulseResponse.Finite, _frekuensiSampling, cutOffLow, cutOffHigh);
            var bpf = bpfFilter.ProcessSamples(signal);
            return bpf;
        }

        private double[] Hanning(double[] signal)
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

        private double[] Hamming(double[] signal)
        {
            int jumlahData = signal.Length;
            double[] window = new double[signal.Length];
            double[] windowFinal = new double[signal.Length];
            Parallel.For(0, jumlahData, i =>
            {
                window[i] = 0.54 - 0.46 * (Math.Cos(2 * Math.PI * i / (jumlahData - 1)));
            });
            Parallel.For(0, jumlahData, i =>
            {
                windowFinal[i] = signal[i] * window[i];
            });
            return windowFinal;
        }

        private double[] FFT(double[] signal)
        {
            int nPoint = signal.Length;
            Complex[] fftComplex = new Complex[nPoint];
            double[] fft = new double[nPoint];
            Parallel.For(0, nPoint, i =>
            {
                fftComplex[i] = new Complex(signal[i], 0.0); // make it complex format
            });
            Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);
            Parallel.For(0, nPoint, i =>
            {
                fft[i] = fftComplex[i].Magnitude; // back to double
            });
            return fft.Take(fft.Length / 2).ToArray();
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

        private double mean(double[] signal)
        {
            return signal.Average();
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
            return signal.Min();
        }
        private double max(double[] signal)
        {
            var desc = from s in signal
                       orderby s descending
                       select s;
            return desc.First();
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
        private void preprocessingSignalTraining(
            List<double> dataF3, List<double> dataF4, List<double> dataF7, List<double> dataF8,
            List<double> dataFC5, List<double> dataFC6, string selectArrow)
        {
            double[][] RAW = new double[6][] { dataF3.ToArray(), dataF4.ToArray(), dataF7.ToArray(), dataF8.ToArray(), dataFC5.ToArray(), dataFC6.ToArray() };
            double[][] BPF= new double[6][];
            double[][] windowHanning = new double[6][];
            double[][] windowHamming = new double[6][];
            double[][] vFFTHanning = new double[6][];
            double[][] vFFTHamming = new double[6][];
            double[][] featureHanning = new double[6][];
            double[][] featureHamming = new double[6][];
            string[] signal = new string[6] { "F3", "F4", "F7", "F8", "FC5", "FC6" };
            Parallel.For(0, 6, i =>
            {
                BPF[i] = filterBPF(RAW[i], _cuttOffLowBPF, _cutOffHighBPF);
                savePerNewFile(dataF3.ToArray(), "RAW", signal[i], selectArrow);
                savePerNewFile(BPF[0], "BPF", signal[i], selectArrow);
            });

            Parallel.For(0, 6, i =>
            {
                windowHanning[i] = Hanning(BPF[i]);
                windowHamming[i] = Hamming(BPF[i]);
                savePerNewFile(windowHanning[i], "Hanning", signal[i], selectArrow);
                savePerNewFile(windowHamming[i], "Hamming", signal[i], selectArrow);
            });
            Parallel.For(0, 6, i =>
            {
                vFFTHanning[i] = FFT(windowHanning[i]);
                vFFTHamming[i] = FFT(windowHamming[i]);
                savePerNewFile(vFFTHanning[i], "FFT_Hanning", signal[i], selectArrow);
                savePerNewFile(vFFTHamming[i], "FFT_Hamming", signal[i], selectArrow);
            });
            Parallel.For(0, 6, i =>
            {
                featureHanning[i] = extractionFeature(vFFTHanning[i].Skip(64).Take(176).ToArray());
                featureHamming[i] = extractionFeature(vFFTHamming[i].Skip(64).Take(176).ToArray());
            });
            saveOneFile(featureHanning, selectArrow, "hanning");
            saveOneFile(featureHamming, selectArrow, "hamming");
            disableEnableButton();
        }

        private double[][] preprocessingSignalTesting(
            List<double> dataF3, List<double> dataF4, List<double> dataF7, List<double> dataF8,
            List<double> dataFC5, List<double> dataFC6)
        {
            double[][] RAW = new double[6][] { dataF3.ToArray(), dataF4.ToArray(), dataF7.ToArray(), dataF8.ToArray(), dataFC5.ToArray(), dataFC6.ToArray() };
            double[][] BPF = new double[6][];
            double[][] window = new double[6][];
            double[][] vFFT = new double[6][];
            double[][] feature = new double[6][];
            string[] signal = new string[6] { "F3", "F4", "F7", "F8", "FC5", "FC6" };
            Parallel.For(0, 6, i =>
            {
                BPF[i] = filterBPF(RAW[i], _cuttOffLowBPF, _cutOffHighBPF);
                //savePerNewFile(dataF3.ToArray(), "RAW_TEST", signal[i], "UNKNOWN");
            });
            Parallel.For(0, 6, i =>
            {
                window[i] = Hanning(BPF[i]);
            });
            Parallel.For(0, 6, i =>
            {
                vFFT[i] = FFT(window[i]);
            });
            //var mean = meanWave(vFFT);
            //var feature = extractionFeature(mean.Skip(64).Take(176).ToArray());
            Parallel.For(0, 6, i =>
            {
                feature[i] = extractionFeature(vFFT[i].Skip(64).Take(176).ToArray());
            });
            var ft = new double[1][];
            var perchannel = new List<double>();
            foreach (var f in feature)
            {
                foreach (var c in f)
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
        private Stopwatch waiting;
        private void btKiri_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke kiri";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                focus = new Focus();
                focus.Show();
                if (sleep(3))
                {
                    _selectedArrow = "kiri";
                    btKiri.Enabled = false;
                    Console.WriteLine("masuk");
                }
            }

        }

        private void btKanan_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke kanan";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                focus = new Focus();
                focus.Show();
                if (sleep(3))
                {
                    _selectedArrow = "kanan";
                    btKanan.Enabled = false;
                }
            }
        }

        private void btMaju_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke maju";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                focus = new Focus();
                focus.Show();

                if (sleep(3))
                {
                    _selectedArrow = "maju";
                    btMaju.Enabled = false;
                }
            }
        }

        private void btMundur_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke mundur";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                focus = new Focus();
                focus.Show();
                if (sleep(3))
                {
                    _selectedArrow = "mundur";
                    btMundur.Enabled = false;
                }
            }
        }

        private void btBerhenti_Click(object sender, EventArgs e)
        {
            string message = "Pengambilan Data sinyal otak ke berhenti";
            string caption = "Setelah Klik OK, akan melakukan pengambilan data!";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                focus = new Focus();
                focus.Show();
                if (sleep(3))
                {
                    _selectedArrow = "berhenti";
                    btBerhenti.Enabled = false;
                }
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

        private void saveOneFile(double[][] feature, string selectarrow, string window)
        {
            string filename = "csv/feature_" + window + ".csv";
            string kelas = "csv/kelas_" + window + ".csv";
            for (int i = 0; i < feature.Length; i++)
            {
                for (int j = 0; j < feature[i].Length; j++)
                {
                    if (i == feature.Length - 1 && j == feature[feature.Length - 1].Length - 1)
                    {
                        File.AppendAllText(filename, feature[i][j] + "" + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(filename, feature[i][j] + ";");
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
        }

        private Tuple<double[][], int[]> loadOneFile()
        {
            string dokumenTraining = "csv/feature_hanning.csv";
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
            string kelas = "csv/kelas_hanning.csv";
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
            MethodInvoker action = delegate
            { tbJalan.Text = "BERPIKIR"; };
            tbJalan.BeginInvoke(action);
        }

        private void btTraining_Click(object sender, EventArgs e)
        {
            knns();
            svm();
        }
        private void knns()
        {
            var pair = loadOneFile();
            var feature = pair.Item1;
            var kelas = pair.Item2;
            var featureTest = pair.Item1;
            var kelasTest = pair.Item2;
            knn = new KNearestNeighbors(k: 6);
            knn.NumberOfInputs = 36;
            knn.Learn(feature, kelas);
            int[] predicted = knn.Decide(featureTest);
            accurate(kelasTest, predicted);
        }

        private void svm()
        {
        var pair = loadOneFile();
            var inputs = pair.Item1;
            var outputs = pair.Item2;
            var teacher = new MulticlassSupportVectorLearning<Gaussian>()
            {
                Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                {
                    Complexity =1000,
                    Tolerance=1e-10
                }
            };

            machine = teacher.Learn(inputs, outputs);
            int[] predicted = machine.Decide(inputs);
            double[] scores = machine.Score(inputs);
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            accurate(outputs, predicted);
            btTesting.Enabled = true;
        }
        private bool sleep(double seconds)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for(int i = 0; ; i++)
            {
               if (i % 100000 == 0)
                {
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > seconds * 1000)
                    {
                        Console.WriteLine("Done " + sw.Elapsed.Seconds + " s");
                        return true;
                    }
                    else
                    {
                        sw.Start();
                    }
                }
            }
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
            //tbAkurasi.Text = accurate + "%";
            MessageBox.Show("Learning Done, Thankyou!");
            btTesting.Enabled = true;
        }
        bool _isDecide = false;
        private void btAmbilDataLagi_Click(object sender, EventArgs e)
        {
            _isTraining = true;
            _isDecide = false;
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
        private void sendCommand(int command, int command1)
        {
            //_myport.Write(command + "");
            MethodInvoker action;
            switch (command)
            {
                case 0:
                    action = delegate
                    { tbArah.Text = "left"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 1:
                    action = delegate
                    { tbArah.Text = "right"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 2:
                    action = delegate
                    { tbArah.Text = "forward"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 3:
                    action = delegate
                    { tbArah.Text = "backward"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 4:
                    action = delegate
                    { tbArah.Text = "stop"; };
                    tbArah.BeginInvoke(action);
                    break;
            }
            switch (command1)
            {
                case 0:
                    action = delegate
                    { knnTb.Text = "left"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 1:
                    action = delegate
                    { knnTb.Text = "right"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 2:
                    action = delegate
                    { knnTb.Text = "forward"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 3:
                    action = delegate
                    { knnTb.Text = "backward"; };
                    tbArah.BeginInvoke(action);
                    break;
                case 4:
                    action = delegate
                    { knnTb.Text = "stop"; };
                    tbArah.BeginInvoke(action);
                    break;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendCommand(4,4);
        }
    }
}
