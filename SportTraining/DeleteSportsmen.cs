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
    public partial class DeleteSportsmen : Form
    {
        public DeleteSportsmen()
        {
            InitializeComponent();
        }

        string fileAllSportsmens = Application.StartupPath + "\\" + "#sportsmens.txt";
        string fileWorkout = "";
        string fileNumTraining = "";
        string fileBackup = "";
        string fileInfo = "";
        Encoding myEncoding = Encoding.UTF8;
        private void DeleteSportsmen_Load(object sender, EventArgs e)
        {
            Workouts w = new Workouts();
            w.findSportsmens(comboBox1);
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Подтвердите удаление спортсмена со всеми записями его тренировок. \nДанные восстановить будет невозможно!", "Вы хотите удалить спортсмена?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                {
                    switchSportsmen();
                    File.Delete(fileInfo);
                    File.Delete(fileBackup);
                    File.Delete(fileNumTraining);
                    File.Delete(fileWorkout);
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка удаления данных");
            }
        }

        void switchSportsmen()
        {
            string currentName = comboBox1.Text.Replace(" ", "_").Trim();
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
            for (int j = 0; j < all.Length; j++ )
            {
                if (all[j].IndexOf("@info nameSportsmen:") != -1)
                {
                    string n = all[j].Replace("@info nameSportsmen:", "").Trim();
                    if (currentName == n)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            all[j + k] = "";
                        }
                    }
                }
            }
            File.WriteAllLines(fileAllSportsmens, all, myEncoding);
        }
    }
}
