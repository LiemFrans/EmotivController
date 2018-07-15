namespace EmotivController
{
    partial class FormControl
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
            this.btLangsungTraining = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAkurasi = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.btMaju = new System.Windows.Forms.Button();
            this.btAmbilDataLagi = new System.Windows.Forms.Button();
            this.btMundur = new System.Windows.Forms.Button();
            this.transportSignal = new System.ComponentModel.BackgroundWorker();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbArah = new System.Windows.Forms.TextBox();
            this.btTesting = new System.Windows.Forms.Button();
            this.btCancelTest = new System.Windows.Forms.Button();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.btKiri = new System.Windows.Forms.Button();
            this.transportSignalForExecution = new System.ComponentModel.BackgroundWorker();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.btBerhenti = new System.Windows.Forms.Button();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.btKanan = new System.Windows.Forms.Button();
            this.btfromZero = new System.Windows.Forms.Button();
            this.pbCatch = new System.Windows.Forms.ProgressBar();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btAmbilData = new System.Windows.Forms.Button();
            this.btTraining = new System.Windows.Forms.Button();
            this.transportPythonSignal = new System.ComponentModel.BackgroundWorker();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btLangsungTraining
            // 
            this.btLangsungTraining.Enabled = false;
            this.btLangsungTraining.Location = new System.Drawing.Point(3, 32);
            this.btLangsungTraining.Name = "btLangsungTraining";
            this.btLangsungTraining.Size = new System.Drawing.Size(249, 23);
            this.btLangsungTraining.TabIndex = 7;
            this.btLangsungTraining.Text = "Langsung Training";
            this.btLangsungTraining.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Akurasi";
            // 
            // tbAkurasi
            // 
            this.tbAkurasi.Location = new System.Drawing.Point(51, 90);
            this.tbAkurasi.Name = "tbAkurasi";
            this.tbAkurasi.ReadOnly = true;
            this.tbAkurasi.Size = new System.Drawing.Size(196, 20);
            this.tbAkurasi.TabIndex = 4;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.btMaju);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(261, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(252, 135);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // btMaju
            // 
            this.btMaju.Enabled = false;
            this.btMaju.Location = new System.Drawing.Point(3, 3);
            this.btMaju.Name = "btMaju";
            this.btMaju.Size = new System.Drawing.Size(249, 132);
            this.btMaju.TabIndex = 0;
            this.btMaju.Text = "Maju";
            this.btMaju.UseVisualStyleBackColor = true;
            // 
            // btAmbilDataLagi
            // 
            this.btAmbilDataLagi.Enabled = false;
            this.btAmbilDataLagi.Location = new System.Drawing.Point(3, 116);
            this.btAmbilDataLagi.Name = "btAmbilDataLagi";
            this.btAmbilDataLagi.Size = new System.Drawing.Size(249, 23);
            this.btAmbilDataLagi.TabIndex = 4;
            this.btAmbilDataLagi.Text = "Ambil Data Lagi";
            this.btAmbilDataLagi.UseVisualStyleBackColor = true;
            // 
            // btMundur
            // 
            this.btMundur.Enabled = false;
            this.btMundur.Location = new System.Drawing.Point(3, 3);
            this.btMundur.Name = "btMundur";
            this.btMundur.Size = new System.Drawing.Size(249, 135);
            this.btMundur.TabIndex = 0;
            this.btMundur.Text = "Mundur";
            this.btMundur.UseVisualStyleBackColor = true;
            // 
            // transportSignal
            // 
            this.transportSignal.WorkerReportsProgress = true;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.tbArah);
            this.flowLayoutPanel4.Controls.Add(this.btTesting);
            this.flowLayoutPanel4.Controls.Add(this.btCancelTest);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(519, 3);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(252, 135);
            this.flowLayoutPanel4.TabIndex = 3;
            // 
            // tbArah
            // 
            this.tbArah.Location = new System.Drawing.Point(3, 3);
            this.tbArah.Name = "tbArah";
            this.tbArah.ReadOnly = true;
            this.tbArah.Size = new System.Drawing.Size(170, 20);
            this.tbArah.TabIndex = 0;
            // 
            // btTesting
            // 
            this.btTesting.Enabled = false;
            this.btTesting.Location = new System.Drawing.Point(3, 29);
            this.btTesting.Name = "btTesting";
            this.btTesting.Size = new System.Drawing.Size(75, 23);
            this.btTesting.TabIndex = 2;
            this.btTesting.Text = "Testing";
            this.btTesting.UseVisualStyleBackColor = true;
            // 
            // btCancelTest
            // 
            this.btCancelTest.Enabled = false;
            this.btCancelTest.Location = new System.Drawing.Point(84, 29);
            this.btCancelTest.Name = "btCancelTest";
            this.btCancelTest.Size = new System.Drawing.Size(75, 23);
            this.btCancelTest.TabIndex = 3;
            this.btCancelTest.Text = "Cancel Test";
            this.btCancelTest.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Controls.Add(this.btKiri);
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 145);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(252, 135);
            this.flowLayoutPanel5.TabIndex = 4;
            // 
            // btKiri
            // 
            this.btKiri.Enabled = false;
            this.btKiri.Location = new System.Drawing.Point(3, 3);
            this.btKiri.Name = "btKiri";
            this.btKiri.Size = new System.Drawing.Size(249, 132);
            this.btKiri.TabIndex = 0;
            this.btKiri.Text = "Kiri";
            this.btKiri.UseVisualStyleBackColor = true;
            // 
            // transportSignalForExecution
            // 
            this.transportSignalForExecution.WorkerReportsProgress = true;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Controls.Add(this.btBerhenti);
            this.flowLayoutPanel6.Location = new System.Drawing.Point(261, 145);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(252, 135);
            this.flowLayoutPanel6.TabIndex = 5;
            // 
            // btBerhenti
            // 
            this.btBerhenti.Enabled = false;
            this.btBerhenti.Location = new System.Drawing.Point(3, 3);
            this.btBerhenti.Name = "btBerhenti";
            this.btBerhenti.Size = new System.Drawing.Size(249, 132);
            this.btBerhenti.TabIndex = 0;
            this.btBerhenti.Text = "Berhenti";
            this.btBerhenti.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.Location = new System.Drawing.Point(519, 287);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            this.flowLayoutPanel9.Size = new System.Drawing.Size(252, 136);
            this.flowLayoutPanel9.TabIndex = 8;
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.Location = new System.Drawing.Point(3, 287);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(252, 136);
            this.flowLayoutPanel8.TabIndex = 7;
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.Controls.Add(this.btKanan);
            this.flowLayoutPanel7.Location = new System.Drawing.Point(519, 145);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(252, 135);
            this.flowLayoutPanel7.TabIndex = 6;
            // 
            // btKanan
            // 
            this.btKanan.Enabled = false;
            this.btKanan.Location = new System.Drawing.Point(3, 3);
            this.btKanan.Name = "btKanan";
            this.btKanan.Size = new System.Drawing.Size(249, 132);
            this.btKanan.TabIndex = 0;
            this.btKanan.Text = "Kanan";
            this.btKanan.UseVisualStyleBackColor = true;
            // 
            // btfromZero
            // 
            this.btfromZero.Enabled = false;
            this.btfromZero.Location = new System.Drawing.Point(3, 61);
            this.btfromZero.Name = "btfromZero";
            this.btfromZero.Size = new System.Drawing.Size(249, 23);
            this.btfromZero.TabIndex = 6;
            this.btfromZero.Text = "Ambil Data dan Training?";
            this.btfromZero.UseVisualStyleBackColor = true;
            // 
            // pbCatch
            // 
            this.pbCatch.Location = new System.Drawing.Point(12, 446);
            this.pbCatch.Name = "pbCatch";
            this.pbCatch.Size = new System.Drawing.Size(771, 23);
            this.pbCatch.TabIndex = 6;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.btAmbilData);
            this.flowLayoutPanel2.Controls.Add(this.btTraining);
            this.flowLayoutPanel2.Controls.Add(this.btLangsungTraining);
            this.flowLayoutPanel2.Controls.Add(this.btfromZero);
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Controls.Add(this.tbAkurasi);
            this.flowLayoutPanel2.Controls.Add(this.btAmbilDataLagi);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(252, 135);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // btAmbilData
            // 
            this.btAmbilData.Enabled = false;
            this.btAmbilData.Location = new System.Drawing.Point(3, 3);
            this.btAmbilData.Name = "btAmbilData";
            this.btAmbilData.Size = new System.Drawing.Size(75, 23);
            this.btAmbilData.TabIndex = 0;
            this.btAmbilData.Text = "Ambil Data";
            this.btAmbilData.UseVisualStyleBackColor = true;
            // 
            // btTraining
            // 
            this.btTraining.Enabled = false;
            this.btTraining.Location = new System.Drawing.Point(84, 3);
            this.btTraining.Name = "btTraining";
            this.btTraining.Size = new System.Drawing.Size(75, 23);
            this.btTraining.TabIndex = 1;
            this.btTraining.Text = "Training";
            this.btTraining.UseVisualStyleBackColor = true;
            // 
            // transportPythonSignal
            // 
            this.transportPythonSignal.WorkerReportsProgress = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btMundur);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(261, 287);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(252, 136);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel9, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel8, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel7, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 14);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 426);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // FormControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 512);
            this.Controls.Add(this.pbCatch);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormControl";
            this.Text = "FormControl";
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btLangsungTraining;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAkurasi;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button btMaju;
        private System.Windows.Forms.Button btAmbilDataLagi;
        private System.Windows.Forms.Button btMundur;
        private System.ComponentModel.BackgroundWorker transportSignal;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.TextBox tbArah;
        private System.Windows.Forms.Button btTesting;
        private System.Windows.Forms.Button btCancelTest;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Button btKiri;
        private System.ComponentModel.BackgroundWorker transportSignalForExecution;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.Button btBerhenti;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel9;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel8;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private System.Windows.Forms.Button btKanan;
        private System.Windows.Forms.Button btfromZero;
        private System.Windows.Forms.ProgressBar pbCatch;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btAmbilData;
        private System.Windows.Forms.Button btTraining;
        private System.ComponentModel.BackgroundWorker transportPythonSignal;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}