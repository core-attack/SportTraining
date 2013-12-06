using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SportTraining
{
    public partial class ImportData : Form
    {
        public ImportData()
        {
            InitializeComponent();
        }

        string fileAllSportsmens = Application.StartupPath + "\\" + "#sportsmens.txt";
        string fileWorkout = "";
        string fileNumTraining = "";
        string fileBackup = "";
        string fileInfo = "";
        Encoding myEncoding = Encoding.UTF8;
        string importFile = "";
        string importTo = "";
        private void buttonChoise_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = openFileDialog1;
            dialog.Filter = "Текстовые файлы(*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                importFile = dialog.FileName;
                textBoxImportFileName.Text = importFile;
                textBoxImportFileName.ScrollToCaret();
            }
        }

        private void ImportData_Load(object sender, EventArgs e)
        {
            Workouts w = new Workouts();
            w.findSportsmens(comboBoxName);
            switchSportsmen();
            comboBoxFiles.Items.Clear();
            comboBoxFiles.Items.Add(fileWorkout);
            comboBoxFiles.Items.Add(fileInfo);
            comboBoxFiles.Items.Add(fileBackup);
            comboBoxFiles.Items.Add(fileNumTraining);
            comboBoxFiles.Items.Add(fileAllSportsmens);

        }

        void switchSportsmen()
        {
            string currentName = comboBoxName.Text.Replace(" ", "_").Trim();
            string[] all = File.ReadAllLines(fileAllSportsmens, myEncoding);
            int i = 0;
            foreach (string s in all)
            {
                if (s.IndexOf("@info nameSportsmen:") != -1)
                {
                    string n = s.Replace("@info nameSportsmen:", "").Trim();
                    if (n == currentName)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            string str = all[i + k];
                            if (str.IndexOf("@info pathInfo:") != -1)
                            {
                                fileInfo = str.Replace("@info pathInfo:", "").Trim();
                            }
                            else if (str.IndexOf("@info pathWorkout:") != -1)
                            {
                                fileWorkout = str.Replace("@info pathWorkout:", "").Trim();
                            }
                            else if (str.IndexOf("@info pathBackup:") != -1)
                            {
                                fileBackup = str.Replace("@info pathBackup:", "").Trim();
                            }
                            else if (str.IndexOf("@info pathNumTraining:") != -1)
                            {
                                fileNumTraining = str.Replace("@info pathNumTraining:", "").Trim();
                            }
                        }
                    }
                }
                i++;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            importTo = comboBoxFiles.Text;
            string[] all = File.ReadAllLines(importFile, myEncoding);
            File.AppendAllLines(importTo, all, myEncoding);
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBoxFiles_MouseEnter(object sender, EventArgs e)
        {
            if (sender is ComboBox)
            {
                toolTip1.Active = true;
                toolTip1.Show(((ComboBox)sender).Text, ((ComboBox)sender));
            }
        }

        private void textBoxImportFileName_MouseEnter(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                toolTip1.Active = true;
                toolTip1.Show(((TextBox)sender).Text, ((TextBox)sender));
            }
        }

        private void comboBoxName_TextChanged(object sender, EventArgs e)
        {
            switchSportsmen();
            comboBoxFiles.Items.Clear();
            comboBoxFiles.Items.Add(fileWorkout);
            comboBoxFiles.Items.Add(fileInfo);
            comboBoxFiles.Items.Add(fileBackup);
            comboBoxFiles.Items.Add(fileNumTraining);
            comboBoxFiles.Items.Add(fileAllSportsmens);
        }
    }
}
