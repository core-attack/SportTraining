namespace SportTraining
{
    partial class ImportData
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
            this.comboBoxName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonChoise = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxFiles = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxImportFileName = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // comboBoxName
            // 
            this.comboBoxName.FormattingEnabled = true;
            this.comboBoxName.Location = new System.Drawing.Point(12, 33);
            this.comboBoxName.Name = "comboBoxName";
            this.comboBoxName.Size = new System.Drawing.Size(416, 29);
            this.comboBoxName.TabIndex = 1;
            this.comboBoxName.TextChanged += new System.EventHandler(this.comboBoxName_TextChanged);
            this.comboBoxName.MouseEnter += new System.EventHandler(this.comboBoxFiles_MouseEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(415, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "Выберите спортсмена, данные к которому желаете добавить.";
            // 
            // buttonChoise
            // 
            this.buttonChoise.Location = new System.Drawing.Point(12, 145);
            this.buttonChoise.Name = "buttonChoise";
            this.buttonChoise.Size = new System.Drawing.Size(108, 30);
            this.buttonChoise.TabIndex = 3;
            this.buttonChoise.Text = "Обзор";
            this.buttonChoise.UseVisualStyleBackColor = true;
            this.buttonChoise.Click += new System.EventHandler(this.buttonChoise_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 65);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(378, 21);
            this.label2.TabIndex = 6;
            this.label2.Text = "К какому из следующих файлов импортировать данные?";
            // 
            // comboBoxFiles
            // 
            this.comboBoxFiles.FormattingEnabled = true;
            this.comboBoxFiles.Location = new System.Drawing.Point(12, 89);
            this.comboBoxFiles.Name = "comboBoxFiles";
            this.comboBoxFiles.Size = new System.Drawing.Size(416, 29);
            this.comboBoxFiles.TabIndex = 2;
            this.comboBoxFiles.Text = "Выберите файл";
            this.comboBoxFiles.MouseEnter += new System.EventHandler(this.comboBoxFiles_MouseEnter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 121);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(289, 21);
            this.label3.TabIndex = 8;
            this.label3.Text = "Данные из какого файла следует добавить?";
            // 
            // textBoxImportFileName
            // 
            this.textBoxImportFileName.Location = new System.Drawing.Point(126, 146);
            this.textBoxImportFileName.Name = "textBoxImportFileName";
            this.textBoxImportFileName.Size = new System.Drawing.Size(302, 29);
            this.textBoxImportFileName.TabIndex = 4;
            this.textBoxImportFileName.MouseEnter += new System.EventHandler(this.textBoxImportFileName_MouseEnter);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(293, 181);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(135, 39);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "Импортировать";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(152, 181);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(135, 39);
            this.button1.TabIndex = 5;
            this.button1.Text = "Отмена";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // ImportData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 232);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxImportFileName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxFiles);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonChoise);
            this.Controls.Add(this.comboBoxName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe Print", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportData";
            this.ShowIcon = false;
            this.Text = "Импортирование данных спортсмена";
            this.Load += new System.EventHandler(this.ImportData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonChoise;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxFiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxImportFileName;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}