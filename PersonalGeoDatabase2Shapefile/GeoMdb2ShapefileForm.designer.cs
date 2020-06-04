namespace CRSDsys
{
    partial class GeoMdb2ShapefileForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbx_mdb = new System.Windows.Forms.TextBox();
            this.btn_SelectMDB = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbx_Dir = new System.Windows.Forms.TextBox();
            this.btn_SelectDir = new System.Windows.Forms.Button();
            this.btn_Cacnel = new System.Windows.Forms.Button();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.IsOutput = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.layerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GeomType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库文件:";
            // 
            // tbx_mdb
            // 
            this.tbx_mdb.Location = new System.Drawing.Point(89, 21);
            this.tbx_mdb.Name = "tbx_mdb";
            this.tbx_mdb.Size = new System.Drawing.Size(369, 21);
            this.tbx_mdb.TabIndex = 1;
            // 
            // btn_SelectMDB
            // 
            this.btn_SelectMDB.Location = new System.Drawing.Point(464, 19);
            this.btn_SelectMDB.Name = "btn_SelectMDB";
            this.btn_SelectMDB.Size = new System.Drawing.Size(75, 23);
            this.btn_SelectMDB.TabIndex = 2;
            this.btn_SelectMDB.Text = "选择";
            this.btn_SelectMDB.UseVisualStyleBackColor = true;
            this.btn_SelectMDB.Click += new System.EventHandler(this.btn_SelectMDB_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 542);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "保存文件夹:";
            // 
            // tbx_Dir
            // 
            this.tbx_Dir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbx_Dir.Location = new System.Drawing.Point(89, 539);
            this.tbx_Dir.Name = "tbx_Dir";
            this.tbx_Dir.Size = new System.Drawing.Size(369, 21);
            this.tbx_Dir.TabIndex = 1;
            // 
            // btn_SelectDir
            // 
            this.btn_SelectDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_SelectDir.Location = new System.Drawing.Point(464, 537);
            this.btn_SelectDir.Name = "btn_SelectDir";
            this.btn_SelectDir.Size = new System.Drawing.Size(75, 23);
            this.btn_SelectDir.TabIndex = 2;
            this.btn_SelectDir.Text = "选择";
            this.btn_SelectDir.UseVisualStyleBackColor = true;
            this.btn_SelectDir.Click += new System.EventHandler(this.btn_SelectDir_Click);
            // 
            // btn_Cacnel
            // 
            this.btn_Cacnel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Cacnel.Location = new System.Drawing.Point(330, 585);
            this.btn_Cacnel.Name = "btn_Cacnel";
            this.btn_Cacnel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cacnel.TabIndex = 3;
            this.btn_Cacnel.Text = "取消";
            this.btn_Cacnel.UseVisualStyleBackColor = true;
            this.btn_Cacnel.Click += new System.EventHandler(this.btn_Cacnel_Click);
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Ok.Location = new System.Drawing.Point(125, 585);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.btn_Ok.TabIndex = 3;
            this.btn_Ok.Text = "确定";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsOutput,
            this.layerName,
            this.GeomType});
            this.dataGridView1.Location = new System.Drawing.Point(12, 64);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(527, 453);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // IsOutput
            // 
            this.IsOutput.HeaderText = "是否转换";
            this.IsOutput.Name = "IsOutput";
            this.IsOutput.ReadOnly = true;
            this.IsOutput.Width = 80;
            // 
            // layerName
            // 
            this.layerName.HeaderText = "图层名";
            this.layerName.Name = "layerName";
            this.layerName.ReadOnly = true;
            this.layerName.Width = 200;
            // 
            // GeomType
            // 
            this.GeomType.HeaderText = "几何类型";
            this.GeomType.Name = "GeomType";
            this.GeomType.ReadOnly = true;
            // 
            // GeoMdb2ShapefileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 621);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.btn_Cacnel);
            this.Controls.Add(this.btn_SelectDir);
            this.Controls.Add(this.btn_SelectMDB);
            this.Controls.Add(this.tbx_Dir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbx_mdb);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GeoMdb2ShapefileForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "个人地理数据量转Shapefile";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbx_mdb;
        private System.Windows.Forms.Button btn_SelectMDB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbx_Dir;
        private System.Windows.Forms.Button btn_SelectDir;
        private System.Windows.Forms.Button btn_Cacnel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsOutput;
        private System.Windows.Forms.DataGridViewTextBoxColumn layerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GeomType;
    }
}