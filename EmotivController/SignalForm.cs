using Arction.WinForms.Charting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmotivController
{
    public partial class SignalForm : Form
    {
        private delegate void ChartUpdateFromThreadHandler(double[][] samples);
        double _samplingFrequency = 1024;  // Sampling frequency (Hz).
        int _channelCount = 0;          // Channel count.
        double _xLength = 0;            // X axis length.
        double _previousX = 0;          // Latest X value on axis.
        long _now;                      // Latest time stamp. 
        long _startTicks;               // Controls timing.
        long _samplesOutput;            // Generated samples quantity.

        // Constants
        const double YMin = -100;       // Minimal y-value.
        const double YMax = 100;        // Maximal y-value.

        private volatile bool _stop;    // Stops thread work.
        private bool _IsRunning;        // Gives status of thread (running or not).

        /// <summary>
        /// Thread.
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// LightningChart component.
        /// </summary>
        private LightningChartUltimate _chart;

        /// <summary>
        /// Delegate reference.
        /// </summary>
        private ChartUpdateFromThreadHandler _chartUpdate;

        public SignalForm()
        {
            _stop = false;
            _IsRunning = false;
            _thread = null;
            InitializeComponent();
            
        }

    }
}
