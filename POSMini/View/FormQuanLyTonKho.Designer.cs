namespace POSMini
{
    partial class FormQuanLyTonKho
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartTonKho = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cmbChartMode = new System.Windows.Forms.ComboBox();
            this.dgvTonKho = new System.Windows.Forms.DataGridView();
            this.txtTimKiem = new System.Windows.Forms.TextBox();
            this.btnTimKiem = new System.Windows.Forms.Button();
            this.cmbSapXep = new System.Windows.Forms.ComboBox();
            this.btnLamMoi = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartTonKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTonKho)).BeginInit();
            this.SuspendLayout();
            // 
            // chartTonKho
            // 
            chartArea1.Name = "ChartArea1";
            this.chartTonKho.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartTonKho.Legends.Add(legend1);
            this.chartTonKho.Location = new System.Drawing.Point(257, 56);
            this.chartTonKho.Name = "chartTonKho";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartTonKho.Series.Add(series1);
            this.chartTonKho.Size = new System.Drawing.Size(300, 259);
            this.chartTonKho.TabIndex = 0;
            this.chartTonKho.Text = "chart1";
            // 
            // cmbChartMode
            // 
            this.cmbChartMode.FormattingEnabled = true;
            this.cmbChartMode.Location = new System.Drawing.Point(257, 12);
            this.cmbChartMode.Name = "cmbChartMode";
            this.cmbChartMode.Size = new System.Drawing.Size(147, 28);
            this.cmbChartMode.TabIndex = 1;
            // 
            // dgvTonKho
            // 
            this.dgvTonKho.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTonKho.Location = new System.Drawing.Point(2, 452);
            this.dgvTonKho.Name = "dgvTonKho";
            this.dgvTonKho.RowHeadersWidth = 62;
            this.dgvTonKho.RowTemplate.Height = 28;
            this.dgvTonKho.Size = new System.Drawing.Size(799, 199);
            this.dgvTonKho.TabIndex = 2;
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.Location = new System.Drawing.Point(528, 340);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.Size = new System.Drawing.Size(167, 26);
            this.txtTimKiem.TabIndex = 3;
            // 
            // btnTimKiem
            // 
            this.btnTimKiem.Location = new System.Drawing.Point(701, 334);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(87, 38);
            this.btnTimKiem.TabIndex = 4;
            this.btnTimKiem.Text = "Tìm kiếm";
            this.btnTimKiem.UseVisualStyleBackColor = true;
            // 
            // cmbSapXep
            // 
            this.cmbSapXep.FormattingEnabled = true;
            this.cmbSapXep.Location = new System.Drawing.Point(2, 388);
            this.cmbSapXep.Name = "cmbSapXep";
            this.cmbSapXep.Size = new System.Drawing.Size(164, 28);
            this.cmbSapXep.TabIndex = 5;
            // 
            // btnLamMoi
            // 
            this.btnLamMoi.Location = new System.Drawing.Point(695, 412);
            this.btnLamMoi.Name = "btnLamMoi";
            this.btnLamMoi.Size = new System.Drawing.Size(93, 34);
            this.btnLamMoi.TabIndex = 6;
            this.btnLamMoi.Text = "Làm mới";
            this.btnLamMoi.UseVisualStyleBackColor = true;
            // 
            // FormQuanLyTonKho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 652);
            this.Controls.Add(this.btnLamMoi);
            this.Controls.Add(this.cmbSapXep);
            this.Controls.Add(this.btnTimKiem);
            this.Controls.Add(this.txtTimKiem);
            this.Controls.Add(this.dgvTonKho);
            this.Controls.Add(this.cmbChartMode);
            this.Controls.Add(this.chartTonKho);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormQuanLyTonKho";
            this.Text = "FormQuanLyTonKho";
            ((System.ComponentModel.ISupportInitialize)(this.chartTonKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTonKho)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartTonKho;
        private System.Windows.Forms.ComboBox cmbChartMode;
        private System.Windows.Forms.DataGridView dgvTonKho;
        private System.Windows.Forms.TextBox txtTimKiem;
        private System.Windows.Forms.Button btnTimKiem;
        private System.Windows.Forms.ComboBox cmbSapXep;
        private System.Windows.Forms.Button btnLamMoi;
    }
}