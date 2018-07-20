namespace EmotivController
{
    partial class SignalForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignalForm));
            this.lightningChartUltimate1 = new Arction.WinForms.Charting.LightningChartUltimate();
            this.SuspendLayout();
            // 
            // lightningChartUltimate1
            // 
            this.lightningChartUltimate1.BackColor = System.Drawing.Color.Gray;
            this.lightningChartUltimate1.Background = ((Arction.WinForms.Charting.Fill)(resources.GetObject("lightningChartUltimate1.Background")));
            this.lightningChartUltimate1.ChartManager = null;
            this.lightningChartUltimate1.Location = new System.Drawing.Point(12, 12);
            this.lightningChartUltimate1.MinimumSize = new System.Drawing.Size(110, 90);
            this.lightningChartUltimate1.Name = "lightningChartUltimate1";
            this.lightningChartUltimate1.Options = ((Arction.WinForms.Charting.ChartOptions)(resources.GetObject("lightningChartUltimate1.Options")));
            this.lightningChartUltimate1.OutputStream = null;
            this.lightningChartUltimate1.RenderOptions = ((Arction.WinForms.Charting.Views.RenderOptionsCommon)(resources.GetObject("lightningChartUltimate1.RenderOptions")));
            this.lightningChartUltimate1.Size = new System.Drawing.Size(1229, 596);
            this.lightningChartUltimate1.TabIndex = 0;
            this.lightningChartUltimate1.Title = ((Arction.WinForms.Charting.Titles.ChartTitle)(resources.GetObject("lightningChartUltimate1.Title")));
            this.lightningChartUltimate1.View3D = ((Arction.WinForms.Charting.Views.View3D.View3D)(resources.GetObject("lightningChartUltimate1.View3D")));
            this.lightningChartUltimate1.ViewPie3D = ((Arction.WinForms.Charting.Views.ViewPie3D.ViewPie3D)(resources.GetObject("lightningChartUltimate1.ViewPie3D")));
            this.lightningChartUltimate1.ViewPolar = ((Arction.WinForms.Charting.Views.ViewPolar.ViewPolar)(resources.GetObject("lightningChartUltimate1.ViewPolar")));
            this.lightningChartUltimate1.ViewSmith = ((Arction.WinForms.Charting.Views.ViewSmith.ViewSmith)(resources.GetObject("lightningChartUltimate1.ViewSmith")));
            this.lightningChartUltimate1.ViewXY = ((Arction.WinForms.Charting.Views.ViewXY.ViewXY)(resources.GetObject("lightningChartUltimate1.ViewXY")));
            // 
            // SignalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1253, 620);
            this.Controls.Add(this.lightningChartUltimate1);
            this.Name = "SignalForm";
            this.Text = "SignalForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Arction.WinForms.Charting.LightningChartUltimate lightningChartUltimate1;
    }
}