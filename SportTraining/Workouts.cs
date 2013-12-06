using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace SportTraining
{
    public partial class Workouts : Form
    {
        public Workouts()
        {
            try
            {
                InitializeComponent();
                createFiles();
                setAllComboBox();
                firstSetPanelTraining();
                loadWorkouts(fileWorkout);
                setDate();
                switchSportsmen();
                setCurrentSportsmen();
                updateCMS(contextMenuStripWork, allWorks);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка при загрузке приложения");
            }
        }
        //загрузочные файл
        string fileAllSportsmens = Application.StartupPath + "\\" + "#sportsmens.txt";
        string fileWorkout = "";
        string fileNumTraining = "";
        string fileBackup = "";
        string fileInfo = "";
        string fileWorkoutDefault = Application.StartupPath + "\\" + "#workout.txt";
        string fileNumTrainingDefault = Application.StartupPath + "\\" + "#currentNumberOfWorkout.txt";
        string fileBackupDefault = Application.StartupPath + "\\" + "#backup.txt";
        string fileInfoDefault = Application.StartupPath + "\\" + "#info.txt";
        string fileXML = Application.StartupPath + "\\" + "xml.xml";
        Encoding myEncoding = Encoding.UTF8;
        //форма для добавления спортсмена
        SportsmenInfo addSp = new SportsmenInfo();
        //форма для редактирования личной информации спортсмена
        SportsmenInfo si = new SportsmenInfo();
        //текущий спортсмен
        Sportsmen sportsmen = new Sportsmen();
        List<Sportsmen> listSportsmens = new List<Sportsmen>();
        //все тренировки
        List<Workout> allWorkouts = new List<Workout>();
        //последний пункт списка
        Point lastPanelLocation = new Point();
        Size lastPanelSize = new Size();
        //отступ между панелями
        int indent = 6;
        int incVScrollBar = 4;
        int incHScrollBar = 10;
        //для чередования цветов пунктов
        bool chet = false;
        //для различия форм добавления и редактирования спортсмена
        bool isNewSportsmenAdd = false;
        Color color1 = Color.LightGoldenrodYellow;
        Color color2 = Color.Lavender;
        //для вставки на panelList
        Point textBoxDateLocation = new Point();
        Size textBoxDateSize = new Size();
        Point textBoxWarmUpLocation = new Point();
        Size textBoxWarmUpSize = new Size();
        Point textBoxWorkLocation = new Point();
        Size textBoxWorkSize = new Size();
        Point textBoxResultLocation = new Point();
        Size textBoxResultSize = new Size();
        Dictionary<string, string> theBestresult = new Dictionary<string, string>();
        void setAllComboBox()
        {
            string[] week = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };
            string[] timeOfDay = { "Утро", "День", "Вечер", "Ночь" };
            foreach (string s in week)
                comboBoxDayOfWeek.Items.Add(s);
            foreach (string s in timeOfDay)
                comboBoxTimeOfDay.Items.Add(s);
            string dw = DateTime.Today.DayOfWeek.ToString();
            switch (dw)
            {
                case "Monday": { comboBoxDayOfWeek.Text = "Понедельник"; }
                    break;
                case "Tuesday": { comboBoxDayOfWeek.Text = "Вторник"; }
                    break;
                case "Wednesday": { comboBoxDayOfWeek.Text = "Среда"; }
                    break;
                case "Thursday": { comboBoxDayOfWeek.Text = "Четверг"; }
                    break;
                case "Friday": { comboBoxDayOfWeek.Text = "Пятница"; }
                    break;
                case "Saturday": { comboBoxDayOfWeek.Text = "Суббота"; }
                    break;
                case "Sunday": { comboBoxDayOfWeek.Text = "Воскресенье"; }
                    break;

            }
            int hours = DateTime.Now.Hour;
            if (6 <= hours && hours < 12)
            {
                comboBoxTimeOfDay.Text = "Утро";
            }
            else if (12 <= hours && hours < 17)
            {
                comboBoxTimeOfDay.Text = "День";
            }
            else if (17 <= hours && hours <= 23)
            {
                comboBoxTimeOfDay.Text = "Вечер";
            }
            else if (0 <= hours && hours < 6)
            {
                comboBoxTimeOfDay.Text = "Ночь";
            }

            findSportsmens(comboBoxSportsmen);
        }

        public void findSportsmens(ComboBox comboBoxSportsmen)
        {
            comboBoxSportsmen.Items.Clear();
            string[] all = File.ReadAllLines(fileAllSportsmens, myEncoding);
            foreach (string s in all)
            {
                if (s.IndexOf("@info nameSportsmen:") != -1)
                {
                    comboBoxSportsmen.Text = s.Replace("@info nameSportsmen:", "");
                    comboBoxSportsmen.Text = comboBoxSportsmen.Text.Replace("_", " ");
                    comboBoxSportsmen.Items.Add(comboBoxSportsmen.Text);
                }
            }
        }

        private void comboBox_keyPress(object sender, KeyPressEventArgs e)
        {
            if (sender is ComboBox)
            {
                e.Handled = true;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            myButtonClick();
        }


        void myButtonClick()
        {
            clearPanelList();
            Workout workOut = new Workout();
            workOut.number = getNextNumOfWorkout().ToString();
            workOut.date = textBoxDate.Text;
            workOut.dayOfWeek = comboBoxDayOfWeek.Text;
            workOut.result = richTextBoxResult.Text;
            workOut.timeOfDay = comboBoxTimeOfDay.Text;
            workOut.warmUp = richTextBoxWarmUp.Text;
            workOut.work = richTextBoxWork.Text;
            workOut.sheff = comboBoxSheff.Text;
            workOut.sportsmen = comboBoxSportsmen.Text;
            
            //добавляем на панель
            chet = !chet;
            if (chet)
                addToPanelTraining(workOut, color1);
            else
                addToPanelTraining(workOut, color2);
            //сохраняем в файл
            setWorkout(workOut);
            loadWorkouts(allWorkouts);
            allWorkouts.Insert(0, workOut);
            EventArgs e = new EventArgs();
            resizePanels();
            updateCMS(contextMenuStripWork, allWorks);

        }

        //для первоначального задания panelTraining
        void firstSetPanelTraining()
        {
            //локация и размер панели
            lastPanelLocation = panelExsample.Location;
            lastPanelSize = panelExsample.Size;
            //локации и размеры составляющих
            textBoxDateLocation = textBoxDateExs.Location;
            textBoxDateSize = textBoxDateExs.Size;
            textBoxWarmUpLocation = textBoxWarmUpExs.Location;
            textBoxWarmUpSize = textBoxWarmUpExs.Size;
            textBoxWorkLocation = textBoxWorkExs.Location;
            textBoxWorkSize = textBoxWorkExs.Size;
            textBoxResultLocation = textBoxResultExs.Location;
            textBoxResultSize = textBoxResultExs.Size;

        }

        void addToPanelTraining(Workout w, Color c)
        {
            try
            {
                Panel p = new Panel();
                p.Name = "panelItem";
                p.Location = new Point(lastPanelLocation.X, lastPanelLocation.Y);
                
                p.BackColor = c;
                p.MouseEnter += new EventHandler(panelExsample_MouseHover);
                p.MouseLeave += new EventHandler(panelExsample_MouseLeave);
                p.ContextMenuStrip = contextMenuStripPanelList;

                Panel pDelete = new Panel();
                pDelete.Name = "panelDelete";
                pDelete.Location = new Point(panelDelete.Location.X, panelDelete.Location.Y);
                pDelete.Size = new Size(panelDelete.Width, panelDelete.Height);
                pDelete.BackColor = c;
                pDelete.MouseEnter += new EventHandler(panelExsample_MouseHover);
                pDelete.MouseLeave += new EventHandler(panelExsample_MouseLeave);
                p.Controls.Add(pDelete);

                TextBox tbDate = new TextBox();
                tbDate.Name = "date";
                tbDate.Location = textBoxDateLocation;
                tbDate.Size = new Size(textBoxDateSize.Width, textBoxDateSize.Height);
                tbDate.ReadOnly = true;
                tbDate.BackColor = c;
                tbDate.BorderStyle = BorderStyle.None;
                tbDate.MouseEnter += new EventHandler(panelExsample_MouseHover);
                tbDate.MouseLeave += new EventHandler(panelExsample_MouseLeave);
                tbDate.ContextMenuStrip = contextMenuStripPanelList;
                
                p.Controls.Add(tbDate);

                if (toolStripMenuItemDate.Checked && toolStripMenuItemDayOfWeek.Checked && toolStripMenuItemTimeOfDay.Checked && toolStripMenuItemNumber.Checked)
                    tbDate.Text = w.date + " (" + w.dayOfWeek + " " + w.timeOfDay + ")" + " №" + w.number;
                else if (toolStripMenuItemDate.Checked && toolStripMenuItemDayOfWeek.Checked && toolStripMenuItemNumber.Checked)
                    tbDate.Text = w.date + " (" + w.dayOfWeek + ")" + " №" + w.number;
                else if (toolStripMenuItemDate.Checked && toolStripMenuItemTimeOfDay.Checked && toolStripMenuItemNumber.Checked)
                    tbDate.Text = w.date + " (" + w.timeOfDay + ")" + " №" + w.number;
                else if (toolStripMenuItemDayOfWeek.Checked && toolStripMenuItemTimeOfDay.Checked && toolStripMenuItemNumber.Checked)
                    tbDate.Text = w.dayOfWeek + " (" + w.timeOfDay + ")" + " №" + w.number;
                else if (toolStripMenuItemDate.Checked && toolStripMenuItemNumber.Checked)
                    tbDate.Text = w.date + " №" + w.number;
                else if (toolStripMenuItemDayOfWeek.Checked && toolStripMenuItemNumber.Checked)
                    tbDate.Text = w.dayOfWeek + " №" + w.number;
                else if (toolStripMenuItemTimeOfDay.Checked && toolStripMenuItemNumber.Checked)
                    tbDate.Text = w.timeOfDay + " №" + w.number;
                else if (toolStripMenuItemNumber.Checked)
                    tbDate.Text = "№" + w.number;
                else if (toolStripMenuItemDate.Checked && toolStripMenuItemDayOfWeek.Checked && toolStripMenuItemTimeOfDay.Checked)
                    tbDate.Text = w.date + " (" + w.dayOfWeek + " " + w.timeOfDay + ")";
                else if (toolStripMenuItemDate.Checked && toolStripMenuItemDayOfWeek.Checked)
                    tbDate.Text = w.date + " (" + w.dayOfWeek + ")";
                else if (toolStripMenuItemDate.Checked && toolStripMenuItemTimeOfDay.Checked)
                    tbDate.Text = w.date + " (" + w.timeOfDay + ")";
                else if (toolStripMenuItemDayOfWeek.Checked && toolStripMenuItemTimeOfDay.Checked)
                    tbDate.Text = w.dayOfWeek + " (" + w.timeOfDay + ")";
                else if (toolStripMenuItemDate.Checked)
                    tbDate.Text = w.date;
                else if (toolStripMenuItemDayOfWeek.Checked)
                    tbDate.Text = w.dayOfWeek;
                else if (toolStripMenuItemTimeOfDay.Checked)
                    tbDate.Text = w.timeOfDay;
                else
                    tbDate.Visible = false;

                TextBox tbWarmUp = new TextBox();
                tbWarmUp.Name = "warmup";
                if (toolStripMenuItemDate.Checked || toolStripMenuItemDayOfWeek.Checked || toolStripMenuItemTimeOfDay.Checked || toolStripMenuItemNumber.Checked)
                    tbWarmUp.Location = textBoxWarmUpLocation;
                else
                    tbWarmUp.Location = textBoxDateLocation;
                tbWarmUp.Size = new Size(textBoxWarmUpSize.Width, textBoxWarmUpSize.Height);
                tbWarmUp.ReadOnly = true;
                tbWarmUp.BackColor = c;
                tbWarmUp.BorderStyle = BorderStyle.None;
                tbWarmUp.MouseEnter += new EventHandler(panelExsample_MouseHover);
                tbWarmUp.MouseLeave += new EventHandler(panelExsample_MouseLeave);
                tbWarmUp.MouseEnter += new EventHandler(textBoxResultExs_MouseEnter);
                tbWarmUp.ContextMenuStrip = contextMenuStripPanelList;
                p.Controls.Add(tbWarmUp);
                if (toolStripMenuItemWarmUp.Checked)
                    tbWarmUp.Text = "Разминка: " + w.warmUp;
                else
                    tbWarmUp.Visible = false;

                TextBox tbWork = new TextBox();
                tbWork.Name = "work";
                if (toolStripMenuItemDate.Checked || toolStripMenuItemDayOfWeek.Checked || toolStripMenuItemTimeOfDay.Checked || toolStripMenuItemNumber.Checked)
                {
                    if (toolStripMenuItemWarmUp.Checked)//есть хотя бы одна сост. даты и разминка
                        tbWork.Location = textBoxWorkLocation;
                    else
                        tbWork.Location = textBoxWarmUpLocation;//есть хотя бы одна сост. даты
                }
                else
                {
                    if (toolStripMenuItemWarmUp.Checked)//есть только разминка
                        tbWork.Location = textBoxWarmUpLocation;
                    else//ничего выше нет 
                        tbWork.Location = textBoxDateLocation;
                }
                tbWork.Size = new Size(textBoxWorkSize.Width, textBoxWorkSize.Height);
                tbWork.ReadOnly = true;
                tbWork.BackColor = c;
                tbWork.BorderStyle = BorderStyle.None;
                tbWork.MouseEnter += new EventHandler(panelExsample_MouseHover);
                tbWork.MouseLeave += new EventHandler(panelExsample_MouseLeave);
                tbWork.MouseEnter += new EventHandler(textBoxResultExs_MouseEnter);
                tbWork.ContextMenuStrip = contextMenuStripPanelList;
                p.Controls.Add(tbWork);
                if (toolStripMenuItemWork.Checked)
                    tbWork.Text = "Работа: " + w.work;
                else
                    tbWork.Visible = false;


                TextBox tbResult = new TextBox();
                tbResult.Name = "result";
                if (toolStripMenuItemDate.Checked || toolStripMenuItemDayOfWeek.Checked || toolStripMenuItemTimeOfDay.Checked || toolStripMenuItemNumber.Checked)
                {
                    if (toolStripMenuItemWarmUp.Checked)//есть разминка
                    {
                        if (toolStripMenuItemWork.Checked)//есть работа
                            tbResult.Location = textBoxResultLocation;
                        else//нет работы
                            tbResult.Location = textBoxWorkLocation;
                    }
                    else//нет разминки
                    {
                        if (toolStripMenuItemWork.Checked)//есть работа
                            tbResult.Location = textBoxWorkLocation;
                        else//нет работы
                            tbResult.Location = textBoxWarmUpLocation;
                    }
                }
                else//нет даты
                {
                    if (toolStripMenuItemWarmUp.Checked)//есть разминка
                    {
                        if (toolStripMenuItemWork.Checked)//есть работа
                            tbResult.Location = textBoxWorkLocation;
                        else//нет работы
                            tbResult.Location = textBoxWarmUpLocation;
                    }
                    else//нет разминки
                    {
                        if (toolStripMenuItemWork.Checked)//есть работа
                            tbResult.Location = textBoxWarmUpLocation;
                        else//нет работы
                            tbResult.Location = textBoxDateLocation;
                    }
                }
                
                tbResult.Size = new Size(textBoxResultSize.Width, textBoxResultSize.Height);
                tbResult.ReadOnly = true;
                tbResult.BackColor = c;
                tbResult.BorderStyle = BorderStyle.None;
                tbResult.MouseEnter += new EventHandler(panelExsample_MouseHover);
                tbResult.MouseLeave += new EventHandler(panelExsample_MouseLeave);
                tbResult.MouseEnter += new EventHandler(textBoxResultExs_MouseEnter);
                tbResult.ContextMenuStrip = contextMenuStripPanelList;
                p.Controls.Add(tbResult);
                if (toolStripMenuItemResult.Checked)
                    tbResult.Text = "Результат: " + w.result;
                else
                    tbResult.Visible = false;

                Label lSheff = new Label();
                lSheff.Name = "sheff";
                lSheff.Text = w.sheff;
                lSheff.Visible = false;
                p.Controls.Add(lSheff);

                Label lSportsmen = new Label();
                lSportsmen.Name = "sportsmen";
                lSportsmen.Text = w.sportsmen;
                lSportsmen.Visible = false;
                p.Controls.Add(lSportsmen);

                Label lDelete = new Label();
                lDelete.Name = "labelDelete";
                lDelete.Text = "X";
                //lDelete.Font = new System.Drawing.Font(Font.Name, (float)7, FontStyle.Regular);
                lDelete.Visible = true;
                lDelete.Location = new Point(labelDelete.Location.X, labelDelete.Location.Y);
                lDelete.MouseEnter += new EventHandler(textBoxResultExs_MouseEnter);
                lDelete.MouseLeave += new EventHandler(panelExsample_MouseLeave);
                lDelete.Click += new EventHandler(labelDelete_Click);

                pDelete.Controls.Add(lDelete);

                Label lNumber = new Label();
                lNumber.Name = "labelNumber";
                lNumber.Text = w.number;
                lNumber.Visible = false;
                p.Controls.Add(lNumber);
                //опускаем позицию следующей панели по списку
                int height = 0;
                int delta = textBoxWarmUpExs.Location.Y - (textBoxDateExs.Location.Y + textBoxDateExs.Height);
                if (tbDate.Visible)
                {
                    if (tbWarmUp.Visible)
                    {
                        if (tbWork.Visible)
                        {
                            if (tbResult.Visible)//есть всё
                            {
                                height = lastPanelSize.Height;
                            }
                            else//нет результата
                            {
                                height = lastPanelSize.Height - textBoxResultExs.Height - delta;
                            }
                        }
                        else//нет работы
                        {
                            if (tbResult.Visible)//есть результат
                            {
                                height = lastPanelSize.Height - textBoxWorkExs.Height - delta;
                            }
                            else//нет результата
                            {
                                height = lastPanelSize.Height - textBoxResultExs.Height - textBoxWorkExs.Height - 2 * delta;
                            }
                        }
                    }
                    else//нет разминки
                    {
                        if (tbWork.Visible)//есть работа
                        {
                            if (tbResult.Visible)//есть результат
                            {
                                height = lastPanelSize.Height - textBoxWarmUpExs.Height - delta;
                            }
                            else//нет результата
                            {
                                height = lastPanelSize.Height - textBoxResultExs.Height - textBoxWarmUpExs.Height - 2 * delta;
                            }
                        }
                        else//нет работы
                        {
                            if (tbResult.Visible)//есть результат
                            {
                                height = lastPanelSize.Height - textBoxWorkExs.Height - textBoxWarmUpExs.Height - 2 * delta;
                            }
                            else//нет результата
                            {
                                height = lastPanelSize.Height - textBoxResultExs.Height - textBoxWorkExs.Height - textBoxWorkExs.Height - 3 * delta;
                            }
                        }
                    }
                }
                else//нет даты
                {
                    if (tbWarmUp.Visible)
                    {
                        if (tbWork.Visible)
                        {
                            if (tbResult.Visible)
                            {
                                height = lastPanelSize.Height - textBoxDateExs.Height - delta;
                            }
                            else//нет результата
                            {
                                height = lastPanelSize.Height - textBoxResultExs.Height - textBoxDateExs.Height - 2*delta;
                            }
                        }
                        else//нет работы
                        {
                            if (tbResult.Visible)//есть результат
                            {
                                height = lastPanelSize.Height - textBoxWorkExs.Height - textBoxDateExs.Height - 2 * delta;
                            }
                            else//нет результата
                            {
                                height = lastPanelSize.Height - textBoxResultExs.Height - textBoxWorkExs.Height - textBoxDateExs.Height - 3 * delta;
                            }
                        }
                    }
                    else//нет разминки
                    {
                        if (tbWork.Visible)//есть работа
                        {
                            if (tbResult.Visible)//есть результат
                            {
                                height = lastPanelSize.Height - textBoxWarmUpExs.Height - textBoxDateExs.Height - 2 * delta;
                            }
                            else//нет результата
                            {
                                height = lastPanelSize.Height - textBoxResultExs.Height - textBoxWarmUpExs.Height - textBoxDateExs.Height - 3 * delta;
                            }
                        }
                        else//нет работы
                        {
                            if (tbResult.Visible)//есть результат
                            {
                                height = lastPanelSize.Height - textBoxWorkExs.Height - textBoxWarmUpExs.Height - textBoxDateExs.Height - 3 * delta;
                            }
                            else//нет результата
                            {
                                height = lastPanelSize.Height - textBoxResultExs.Height - textBoxWorkExs.Height - textBoxWorkExs.Height - textBoxDateExs.Height - 4 * delta;
                            }
                        }
                    }
                }
                height += indent;
                p.Size = new Size(hScrollBarPanelList.Width, height);
                panelList.Controls.Add(p);
                lastPanelLocation = new Point(lastPanelLocation.X, lastPanelLocation.Y + height);
                //увеличиваем максимум скроллбара
                vScrollBarPanelList.Maximum += incVScrollBar / 2;
                panelList.Update();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка добавления записи тренировки");
            }
        }

        //написать перезапись содержимого панели-пункта

        //создание файла (если есть надобность) для хранения всей информации
        void createFiles()
        {
            try
            {
                fileBackup = fileBackupDefault;
                fileWorkout = fileWorkoutDefault;
                fileNumTraining = fileNumTrainingDefault;
                fileInfo = fileInfoDefault;
                if (!File.Exists(fileBackup))
                    File.WriteAllText(fileBackup, "", myEncoding);
                if (!File.Exists(fileWorkout))
                    File.WriteAllText(fileWorkout, "", myEncoding);
                if (!File.Exists(fileNumTraining))
                    File.WriteAllText(fileNumTraining, "1", myEncoding);
                if (!File.Exists(fileInfo))
                    File.WriteAllText(fileInfo, "", myEncoding);
                if (!File.Exists(fileAllSportsmens))
                    File.WriteAllText(fileAllSportsmens, "", myEncoding);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка создания файла данных");
            }
        }

        //сохранает тренировку в файл
        void setWorkout(Workout w)
        {
            try
            {
                FileInfo fi = new FileInfo(fileWorkout);
                StreamWriter sw = fi.AppendText();
                sw.WriteLine("#begin");
                sw.WriteLine("#number:" + w.number);
                sw.WriteLine("#date:" + w.date);
                sw.WriteLine("#dayOfWeek:" + w.dayOfWeek);
                sw.WriteLine("#timeOfDay:" + w.timeOfDay);
                sw.WriteLine("#warmUp:" + w.warmUp);
                sw.WriteLine("#work:" + w.work);
                sw.WriteLine("#result:" + w.result);
                sw.WriteLine("#sportsmen:" + w.sportsmen);
                sw.WriteLine("#sheff:" + w.sheff);
                sw.WriteLine("#end");
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка записи тренировки");
            }
        }

        void setWorkout(Workout w, string filename)
        {
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (!fi.Exists)
                    fi.Create();
                StreamWriter sw = fi.AppendText();
                sw.WriteLine("#begin");
                sw.WriteLine("#number:" + w.number);
                sw.WriteLine("#date:" + w.date);
                sw.WriteLine("#dayOfWeek:" + w.dayOfWeek);
                sw.WriteLine("#timeOfDay:" + w.timeOfDay);
                sw.WriteLine("#warmUp:" + w.warmUp);
                sw.WriteLine("#work:" + w.work);
                sw.WriteLine("#result:" + w.result);
                sw.WriteLine("#sportsmen:" + w.sportsmen);
                sw.WriteLine("#sheff:" + w.sheff);
                sw.WriteLine("#end");
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка записи тренировки");
            }
        }

        //заменяя содержимое
        void replaseWorkouts(List<Workout> workouts)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fileWorkout);
                foreach (Workout w in workouts)
                {
                    sw.WriteLine("#begin");
                    sw.WriteLine("#number:" + w.number);
                    sw.WriteLine("#date:" + w.date);
                    sw.WriteLine("#dayOfWeek:" + w.dayOfWeek);
                    sw.WriteLine("#timeOfDay:" + w.timeOfDay);
                    sw.WriteLine("#warmUp:" + w.warmUp);
                    sw.WriteLine("#work:" + w.work);
                    sw.WriteLine("#result:" + w.result);
                    sw.WriteLine("#sportsmen:" + w.sportsmen);
                    sw.WriteLine("#sheff:" + w.sheff);
                    sw.WriteLine("#end");
                }
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка записи тренировки");
            }
        }

        // загружает из файла данные о тренировках
        void loadWorkouts(string filename)
        {
            try
            {
                StreamReader sr = new StreamReader(filename, myEncoding);
                string all = sr.ReadToEnd();
                sr.Close();
                string[] sep = { "\r\n" };
                string[] allLines = all.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                Workout workOut = null;
                for (int i = 0; i < allLines.Length; i++)
                {
                    if (allLines[i] == "#begin")
                    { workOut = new Workout(); }
                    else if (allLines[i].IndexOf("#number:") != -1)
                    { workOut.number = allLines[i].Replace("#number:", ""); }
                    else if (allLines[i].IndexOf("#date:") != -1)
                    { workOut.date = allLines[i].Replace("#date:", ""); }
                    else if (allLines[i].IndexOf("#dayOfWeek:") != -1)
                    { workOut.dayOfWeek = allLines[i].Replace("#dayOfWeek:", ""); }
                    else if (allLines[i].IndexOf("#timeOfDay:") != -1)
                    { workOut.timeOfDay = allLines[i].Replace("#timeOfDay:", ""); }
                    else if (allLines[i].IndexOf("#warmUp:") != -1)
                    { workOut.warmUp = allLines[i].Replace("#warmUp:", ""); }
                    else if (allLines[i].IndexOf("#work:") != -1)
                    { workOut.work = allLines[i].Replace("#work:", ""); }
                    else if (allLines[i].IndexOf("#result:") != -1)
                    { workOut.result = allLines[i].Replace("#result:", ""); }
                    else if (allLines[i].IndexOf("#sportsmen:") != -1)
                    {
                        workOut.sportsmen = allLines[i].Replace("#sportsmen:", "");
                        //if (!comboBoxSportsmen.Items.Contains(workOut.sportsmen))
                        //    comboBoxSportsmen.Items.Add(workOut.sportsmen);
                        //comboBoxSportsmen.Text = workOut.sportsmen;
                    }
                    else if (allLines[i].IndexOf("#sheff:") != -1)
                    {
                        workOut.sheff = allLines[i].Replace("#sheff:", "");
                        //if (!comboBoxSheff.Items.Contains(workOut.sheff))
                        //    comboBoxSheff.Items.Add(workOut.sheff);
                        //comboBoxSheff.Text = workOut.sheff;
                    }
                    else if (allLines[i].IndexOf("#end") != -1)
                    {
                        allWorkouts.Insert(0, workOut);
                    }
                }
                loadWorkouts(allWorkouts);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка загрузки тренировок");
            }
        }

        void loadWorkouts(List<Workout> list)
        {
            foreach (Workout w in list)
            {
                chet = !chet;
                if (chet)
                    addToPanelTraining(w, color1);
                else
                    addToPanelTraining(w, color2);
            }
        }

        //текущая дата и время суток 
        void setDate()
        {
            textBoxDate.Text = DateTime.Now.ToShortDateString();

        }

        private void vScrollBarPanelList_Scroll(object sender, ScrollEventArgs e)
        {
            //int kolItems = panelList.Controls.Count;
            //int height = kolItems * (panelExsample.Height + indent) - indent;
            //нужно определить количество экранов (видимой части панели), которые занимает панель
            //int kolScreens = height / vScrollBarPanelList.Height;
            //теперь нужно разделить счетчик скроллбара на количество экранов, чтобы в зависимости от нажатия на него показывать необходимый экран
            //richTextBoxWarmUp.Text = "Показатель счетчика скроллбара: " + e.NewValue.ToString() + "\n" +
            //    "Высота списка: " + height.ToString()  + "\n" +
            //    "Количество экранов: " + kolScreens.ToString();

            //если не принимать во внимание условие нажатие кнопки, то всё будет съезжать
            myScrollY(e.OldValue, e.NewValue);
        }

        private void hScrollBarPanelList_Scroll(object sender, ScrollEventArgs e)
        {
            myScrollX(e.OldValue, e.NewValue);
            resizePanelsWidth(e.OldValue, e.NewValue);
        }

        void myScrollY(int OldValue, int NewValue)
        {
            int step = ((panelExsample.Height + indent)) / incVScrollBar;
            foreach (object obj in panelList.Controls)
            {
                if (obj is Panel)
                {
                    if (((Panel)obj).Name == "panelItem")
                    {
                        int abs = Math.Abs(OldValue - NewValue);
                        if (OldValue > NewValue)
                        {
                            ((Panel)obj).Location = new Point(((Panel)obj).Location.X, ((Panel)obj).Location.Y + abs * step);
                            lastPanelLocation = new Point(lastPanelLocation.X, lastPanelLocation.Y + abs * step);
                        }
                        else if (OldValue < NewValue)
                        {
                            ((Panel)obj).Location = new Point(((Panel)obj).Location.X, ((Panel)obj).Location.Y - abs * step);
                            lastPanelLocation = new Point(lastPanelLocation.X, lastPanelLocation.Y - abs * step);
                        }
                        //richTextBoxWork.Text = ((Panel)obj).Location.X.ToString() + ":" + ((Panel)obj).Location.Y;
                    }
                }
            }
        }

        void myScrollX(int OldValue, int NewValue)
        {
            int step = incHScrollBar;
            foreach (object obj in panelList.Controls)
            {
                if (obj is Panel)
                {
                    if (((Panel)obj).Name == "panelItem")
                    {
                        int abs = Math.Abs(OldValue - NewValue);
                        if (OldValue > NewValue)
                        {
                            ((Panel)obj).Location = new Point(((Panel)obj).Location.X + abs * step, ((Panel)obj).Location.Y);
                            lastPanelLocation = new Point(lastPanelLocation.X + abs * step, lastPanelLocation.Y);
                        }
                        else if (OldValue < NewValue)
                        {
                            ((Panel)obj).Location = new Point(((Panel)obj).Location.X - abs * step, ((Panel)obj).Location.Y);
                            lastPanelLocation = new Point(lastPanelLocation.X - abs * step, lastPanelLocation.Y);
                        }
                        //richTextBoxWork.Text = ((Panel)obj).Location.X.ToString() + ":" + ((Panel)obj).Location.Y;
                    }
                }
            }
        }

        //это для ричтекстбоксов слева, чтобы изменяли высоту в зависимости от высоты формы
        void resizeRich()
        {
            int place = comboBoxSportsmen.Location.Y - richTextBoxWarmUp.Location.Y;
            richTextBoxWarmUp.Height = place / 7;
            richTextBoxWork.Height = place / 7;
            //richTextBoxResult.Height = place - place / 7 - place / 6 - 5;
            richTextBoxWork.Location = new Point(richTextBoxWork.Location.X, richTextBoxWarmUp.Location.Y + richTextBoxWarmUp.Height + 5);
            labelWork.Location = new Point(labelWork.Location.X, richTextBoxWork.Location.Y);
            richTextBoxResult.Location = new Point(richTextBoxResult.Location.X, richTextBoxWork.Location.Y + richTextBoxWork.Height + 5);
            labelResult.Location = new Point(labelResult.Location.X, richTextBoxResult.Location.Y);
        }
        //меняет ширину записей в зависимости от ширины формы
        void resizePanels()
        {
            foreach (object obj in panelList.Controls)
                if (obj is Panel)
                    if (((Panel)obj).Name == "panelItem")
                    {
                        ((Panel)obj).Width = hScrollBarPanelList.Width;
                        foreach (object ob in ((Panel)obj).Controls)
                            if (ob is TextBox)
                            {
                                ((TextBox)ob).Width = hScrollBarPanelList.Width;

                            }
                            else if (ob is Panel)
                            {
                                if (((Panel)ob).Name == "panelDelete")
                                {
                                    ((Panel)ob).Location = new Point(hScrollBarPanelList.Width - ((Panel)ob).Width, ((Panel)ob).Location.Y);
                                }
                            }

                    }
        }

        void resizePanelsWidth(int oldV, int newV)
        {
            int value = Math.Abs(oldV - newV)*hScrollBarPanelList.LargeChange;
            if (newV > oldV)
            {    
                foreach (object obj in panelList.Controls)
                    if (obj is Panel)
                        if (((Panel)obj).Name == "panelItem")
                        {
                            ((Panel)obj).Width += value;
                            foreach (object ob in ((Panel)obj).Controls)
                                if (ob is TextBox)
                                {
                                    ((TextBox)ob).Width += value;

                                }
                                else if (ob is Panel)
                                {
                                    if (((Panel)ob).Name == "panelDelete")
                                    {
                                        ((Panel)ob).Location = new Point(((Panel)ob).Parent.Width - ((Panel)ob).Width, ((Panel)ob).Location.Y);
                                    }
                                }

                        }
            }
            else if (newV < oldV)
            {
                foreach (object obj in panelList.Controls)
                    if (obj is Panel)
                        if (((Panel)obj).Name == "panelItem")
                        {
                            ((Panel)obj).Width -= value;
                            foreach (object ob in ((Panel)obj).Controls)
                                if (ob is TextBox)
                                {
                                    ((TextBox)ob).Width -= value;

                                }
                                else if (ob is Panel)
                                {
                                    if (((Panel)ob).Name == "panelDelete")
                                    {
                                        ((Panel)ob).Location = new Point(hScrollBarPanelList.Width - ((Panel)ob).Width, ((Panel)ob).Location.Y);
                                    }
                                }

                        }
            }
        }

        private void Workout_Resize(object sender, EventArgs e)
        {
            resizeRich();
            //для записей в списке
            resizePanels();
        }
        //выдает следующий порядковый номер записи, считывая текущий из файла
        int getNextNumOfWorkout()
        {
            try
            {
                StreamReader sr = new StreamReader(fileNumTraining);
                string num = sr.ReadToEnd();
                sr.Close();
                //StreamWriter sw = new StreamWriter(fileNumTraining, myEncoding);
                int number = 0;
                if (num != "")
                    number = Convert.ToInt32(num) + 1;
                else
                    number = 1;
                File.WriteAllText(fileNumTraining, number.ToString(), myEncoding);
                //sw.Write(number.ToString());
                //sw.Close();
                return number;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace, "Ошибка генерации номера тренировки");
                return -1;
            }
        }

        private void panelExsample_MouseHover(object sender, EventArgs e)
        {
            //richTextBoxResult.Text = sender.GetType().ToString();
            //if (sender is Panel)
            //{ 
            //    foreach (object ob in ((Panel)sender).Controls)
            //        if (ob is Panel)
            //        {
            //            if (((Panel)ob).Name == "panelDelete")
            //                foreach(object o in ((Panel)ob).Controls)
            //                    if (o is Label)
            //                    {
            //                        if (((Label)o).Name == "labelDelete")
            //                            ((Label)o).Visible = true;
            //                    } 
            //        }
            //}
            //else if (sender is TextBox)
            //{ 
            //    foreach (object ob in ((TextBox)sender).Parent.Controls)
            //        if (ob is Panel)
            //        {
            //            if (((Panel)ob).Name == "panelDelete")
            //                foreach (object o in ((Panel)ob).Controls)
            //                    if (o is Label)
            //                    {
            //                        if (((Label)o).Name == "labelDelete")
            //                            ((Label)o).Visible = true;
            //                    }
            //        }
            //}
            //else if (sender is Label)
            //{
            //    if (((Label)sender).Name == "labelDelete")
            //        ((Label)sender).Visible = true;
            //}
        }

        private void panelExsample_MouseLeave(object sender, EventArgs e)
        {
            //richTextBoxResult.Text = sender.GetType().ToString();
            //if (sender is Panel)
            //{
            //    foreach (object ob in ((Panel)sender).Controls)
            //        if (ob is Panel)
            //        {
            //            if (((Panel)ob).Name == "panelDelete")
            //                foreach(object o in ((Panel)ob).Controls)
            //                    if (o is Label)
            //                    {
            //                        if (((Label)o).Name == "labelDelete")
            //                            ((Label)o).Visible = false;
            //                    }
            //        } 
            //}
            //else if (sender is Label)
            //{
            //    if (((Label)sender).Name == "labelDelete")
            //        ((Label)sender).Visible = true;
            //}
        }

        private void labelDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is Label)
                {
                    string num = "";
                    if (((Label)sender).Name == "labelDelete")
                    {
                        foreach (object obj in ((Label)sender).Parent.Parent.Controls)
                        {
                            if (obj is Label)
                            {
                                if (((Label)obj).Name == "labelNumber")
                                {
                                    num = ((Label)obj).Text;
                                }
                            }
                        }
                    }
                    deleteItem(num);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка удаления");
            }
        }
        //удаляет одну запись из списка и эквивалентную ей из файла
        void deleteItem(string number)
        {
            for (int i = 0; i < allWorkouts.Count; i++)
            {
                if (allWorkouts[i].number == number)
                {
                    allWorkouts.RemoveAt(i);
                    i--;
                }
            }
            clearPanelList();
            replaseWorkouts(allWorkouts);
            loadWorkouts(fileWorkout);
        }

        //очищает только панель, на которой публикуются записи тренировок
        void clearPanelList()
        {
            for (int i = 0; i < panelList.Controls.Count; i++)
            {
                if (panelList.Controls[i].Name != "panelExsample")
                {
                    panelList.Controls.RemoveAt(i);
                    //vScrollBarPanelList.Maximum -= incVScrollBar / 2;
                    i--;
                }
            }
            firstSetPanelTraining();
            vScrollBarPanelList.Maximum = 0;
            oldValueX = 0;
            newValueX = 0;
            oldValueY = 0;
            newValueY = 0;
        }

        //полная очистка файлов и всех массивов (сомневаюсь, что её нужно предоставлять пользователю)
        void clearAll()
        {
            clearPanelList();
            //перезапись файла восстановления
            //StreamWriter stream = new StreamWriter(fileBackup);
            //stream.WriteLine("");
            //stream.Close();
            foreach (Workout w in allWorkouts)
                setWorkout(w, fileBackup);
            allWorkouts.Clear();
            StreamWriter sw = new StreamWriter(fileNumTraining);
            int number = 1;
            sw.Write(number.ToString());
            replaseWorkouts(allWorkouts);

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            clearAll();

        }

        int oldValueY = 0;
        int newValueY = 0;
        private void vScrollBarPanelList_ValueChanged(object sender, EventArgs e)
        {
            newValueY = vScrollBarPanelList.Value;
            myScrollY(oldValueY, newValueY);
            oldValueY = newValueY;
        }

        int oldValueX = 0;
        int newValueX = 0;
        private void hScrollBarPanelList_ValueChanged(object sender, EventArgs e)
        {
            newValueX = hScrollBarPanelList.Value;
            myScrollX(oldValueX, newValueX);
            resizePanelsWidth(oldValueX, newValueX);
            oldValueX = newValueX;
            
        }

        private void очисткаВсехРезультатовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Подтвердите удаление ВСЕХ результатов. Данные восстановить будет невозможно!", "Вы уверены, что нужно удалять?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                clearAll();
        }

        //ищет всех спортсменов и тренеров
        void findAllSportsmensAndSheffs()
        {

        }

        private void записатьНовогоСпортсменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isNewSportsmenAdd = true;
            createNewSportsmen();
        }

        void createNewSportsmen()
        {
            addSp.FormClosed += new FormClosedEventHandler(addSportsmen_FormClosed);
            addSp.ShowDialog();
        }

        //все изменения вступают в силу с закрытием формы добавления/редактирования личной информации спортсмена
        private void addSportsmen_FormClosed(object sender, FormClosedEventArgs e)
        {
                if (isNewSportsmenAdd)
                {
                    if (addSp.isButtonAddClick)
                    {
                        setNewSportsmen();
                        switchSportsmen();
                        setCurrentSportsmen();
                    }
                }
                else
                    changeInfo();
        }

        //добавление нового спортсмена
        void setNewSportsmen()
        {
            addSp.setSportsmen(out sportsmen);
            string name = sportsmen.name + "_" + sportsmen.firstName + "_" + sportsmen.fathersName;
            fileWorkout = Application.StartupPath + "\\#workout_" + name + ".txt";
            File.WriteAllText(fileWorkout, "");
            fileInfo = Application.StartupPath + "\\#info_" + name + ".txt";
            List<string> list = new List<string>();
            list.Add("@info name:" + sportsmen.name);
            list.Add("@info firstName:" + sportsmen.firstName);
            list.Add("@info fathersName:" + sportsmen.fathersName);
            list.Add("@info sportsLevel:" + sportsmen.sportslevel);
            list.Add("@info nickName:" + sportsmen.nickName);
            list.Add("@info birthsday:" + sportsmen.getBirthsday());
            list.Add("@info phone:" + sportsmen.phone);
            list.Add("@info from:" + sportsmen.from);
            list.Add("@info sheff:" + sportsmen.sheff);
            list.Add("@info phoneSheff:" + sportsmen.phoneSheff);
            File.WriteAllLines(fileInfo, list, myEncoding);

            fileNumTraining = Application.StartupPath + "\\" + "#currentNumberOfWorkout_" + name + ".txt";
            File.WriteAllText(fileNumTraining, "1");

            fileBackup = Application.StartupPath + "\\" + "#backup_" + name + ".txt";
            File.WriteAllText(fileBackup, "");

            clearPanelList();
            loadWorkouts(fileWorkout);

            comboBoxSportsmen.Text = name;
            comboBoxSportsmen.Items.Add(name);
            comboBoxSheff.Text = sportsmen.sheff;
            comboBoxSheff.Items.Add(sportsmen.sheff);

            List<string> list1 = new List<string>();
            list1.Add("@info nameSportsmen:" + name);
            list1.Add("@info pathWorkout:" + fileWorkout);
            list1.Add("@info pathBackup:" + fileBackup);
            list1.Add("@info pathNumTraining:" + fileNumTraining);
            list1.Add("@info pathInfo:" + fileInfo);
            list1.Add("@info end");
            File.AppendAllLines(fileAllSportsmens, list1, myEncoding);
            File.SetLastWriteTime(fileAllSportsmens, DateTime.Now);
        }

        //поиск по всем спортсменам
        void switchSportsmen()
        {
            string currentName = comboBoxSportsmen.Text.Replace(" ", "_").Trim();
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
                            //if (str.IndexOf("@info nameSportsmen:") != -1)
                            //{
                            //    comboBoxSportsmen.Text = s.Replace("@info nameSportsmen:", "");
                            //    comboBoxSportsmen.Text = comboBoxSportsmen.Text.Replace("_", " ");
                            //}
                            //else 
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
            clearPanelList();
            allWorkouts.Clear();
            loadWorkouts(fileWorkout);
        }

        private void comboBoxSportsmen_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxSportsmen_SelectedIndexChanged(object sender, EventArgs e)
        {
            switchSportsmen();
            setCurrentSportsmen();
            resizePanels();
            hScrollBarPanelList.Value = 0;
        }

        private void редактироватьИменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isNewSportsmenAdd = false;
            loadInfo();
        }

        //редактирование личной информации спортсмена
        void changeInfo()
        {
            foreach (object obj in si.Controls)
            {
                if (obj is TextBox)
                    if (((TextBox)obj).Name == "textBoxName")
                    {
                        sportsmen.name = ((TextBox)obj).Text;
                    }
                    else if (((TextBox)obj).Name == "textBoxFirstName")
                    { sportsmen.firstName = ((TextBox)obj).Text;}
                    else if (((TextBox)obj).Name == "textBoxFathersName")
                    { sportsmen.fathersName = ((TextBox)obj).Text;}
                    else if (((TextBox)obj).Name == "textBoxSportsLevel")
                    { sportsmen.sportslevel = ((TextBox)obj).Text;}
                    else if (((TextBox)obj).Name == "textBoxNickName")
                    { sportsmen.nickName = ((TextBox)obj).Text;}
                    else if (((TextBox)obj).Name == "textBoxBornDate")
                    { sportsmen.setBirthsday(((TextBox)obj).Text); }
                    else if (((TextBox)obj).Name == "textBoxPhone")
                    { sportsmen.phone = ((TextBox)obj).Text;}
                    else if (((TextBox)obj).Name == "textBoxFrom")
                    { sportsmen.from = ((TextBox)obj).Text;}
                    else if (((TextBox)obj).Name == "textBoxSheff")
                    { sportsmen.sheff = ((TextBox)obj).Text;}
                    else if (((TextBox)obj).Name == "textBoxPhoneSheff")
                    { sportsmen.phoneSheff = ((TextBox)obj).Text;}
            }
            List<string> list = new List<string>();
            list.Add("@info name:" + sportsmen.name);
            list.Add("@info firstName:" + sportsmen.firstName);
            list.Add("@info fathersName:" + sportsmen.fathersName);
            list.Add("@info sportsLevel:" + sportsmen.sportslevel);
            list.Add("@info nickName:" + sportsmen.nickName);
            list.Add("@info birthsday:" + sportsmen.getBirthsday());
            list.Add("@info phone:" + sportsmen.phone);
            list.Add("@info from:" + sportsmen.from);
            list.Add("@info sheff:" + sportsmen.sheff);
            list.Add("@info phoneSheff:" + sportsmen.phoneSheff);
            File.WriteAllLines(fileInfo,list);
        }
        //загружает личную информацию спортсмена на форму
        void loadInfo()
        {
            si = new SportsmenInfo();
            si.FormClosed += new FormClosedEventHandler(addSportsmen_FormClosed);
            si.Text = "Редактирование данных о спортсмене";
            foreach (object obj in si.Controls)
            {
                if (obj is TextBox)
                {
                    if (((TextBox)obj).Name == "textBoxName")
                    {
                        ((TextBox)obj).Text = sportsmen.name;
                        ((TextBox)obj).ReadOnly = true;
                    }
                    else if (((TextBox)obj).Name == "textBoxFirstName")
                    { 
                        ((TextBox)obj).Text = sportsmen.firstName;
                        ((TextBox)obj).ReadOnly = true;
                    }
                    else if (((TextBox)obj).Name == "textBoxFathersName")
                    { 
                        ((TextBox)obj).Text = sportsmen.fathersName; 
                        ((TextBox)obj).ReadOnly = true; 
                    }
                    else if (((TextBox)obj).Name == "textBoxSportsLevel")
                    { 
                        ((TextBox)obj).Text = sportsmen.sportslevel; 
                        ((TextBox)obj).ReadOnly = true; 
                    }
                    else if (((TextBox)obj).Name == "textBoxNickName")
                    { 
                        ((TextBox)obj).Text = sportsmen.nickName; 
                        ((TextBox)obj).ReadOnly = true; 
                    }
                    else if (((TextBox)obj).Name == "textBoxBornDate")
                    { 
                        ((TextBox)obj).Text = sportsmen.getBirthsday(); 
                        ((TextBox)obj).ReadOnly = true; 
                    }
                    else if (((TextBox)obj).Name == "textBoxPhone")
                    { 
                        ((TextBox)obj).Text = sportsmen.phone; 
                        ((TextBox)obj).ReadOnly = true; 
                    }
                    else if (((TextBox)obj).Name == "textBoxFrom")
                    { 
                        ((TextBox)obj).Text = sportsmen.from; 
                        ((TextBox)obj).ReadOnly = true; 
                    }
                    else if (((TextBox)obj).Name == "textBoxSheff")
                    { 
                        ((TextBox)obj).Text = sportsmen.sheff; 
                        ((TextBox)obj).ReadOnly = true; 
                    }
                    else if (((TextBox)obj).Name == "textBoxPhoneSheff")
                    { 
                        ((TextBox)obj).Text = sportsmen.phoneSheff; 
                        ((TextBox)obj).ReadOnly = true; 
                    }
                }
                else if (obj is Button)
                {
                    if (((Button)obj).Name == "buttonAdd")
                        ((Button)obj).Text = "Закрыть";
                    else if (((Button)obj).Name == "buttonChange")
                    {
                        ((Button)obj).Visible = true;
                    }
                    else if (((Button)obj).Name == "buttonUndo")
                    {
                        ((Button)obj).Visible = false;
                    }
                }
                

            }
            si.ShowDialog();
        }

        void setCurrentSportsmen()
        {
            string[] all = File.ReadAllLines(fileInfo, myEncoding);
            foreach (string s in all)
            {
                if (s.IndexOf("@info name:") != -1)
                {
                    sportsmen.name = s.Replace("@info name:", "");
                }
                else if (s.IndexOf("@info firstName:") != -1)
                {
                    sportsmen.firstName = s.Replace("@info firstName:", "");
                }
                else if (s.IndexOf("@info fathersName:") != -1)
                {
                    sportsmen.fathersName = s.Replace("@info fathersName:", "");
                }
                else if (s.IndexOf("@info sportsLevel:") != -1)
                {
                    sportsmen.sportslevel = s.Replace("@info sportsLevel:", "");
                }
                else if (s.IndexOf("@info nickName:") != -1)
                {
                    sportsmen.nickName = s.Replace("@info nickName:", "");
                }
                else if (s.IndexOf("@info birthsday:") != -1)
                {
                    sportsmen.setBirthsday(s.Replace("@info birthsday:", ""));
                }
                else if (s.IndexOf("@info phone:") != -1)
                {
                    sportsmen.phone = s.Replace("@info phone:", "");
                }
                else if (s.IndexOf("@info from:") != -1)
                {
                    sportsmen.from = s.Replace("@info from:", "");
                }
                else if (s.IndexOf("@info sheff:") != -1)
                {
                    sportsmen.sheff = s.Replace("@info sheff:", "");
                    comboBoxSheff.Text = sportsmen.sheff;

                }
                else if (s.IndexOf("@info phoneSheff:") != -1)
                {
                    sportsmen.phoneSheff = s.Replace("@info phoneSheff:", "");
                }
            }
        }

        private void расположениеФайловToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getAllPaths();
        }
        
        //возвращает в виде сообщения расположения всех используемых файлов
        void getAllPaths()
        { 
            MessageBox.Show("Информация о спортсмене: " + fileInfo + "\n" +
            "Записи тренировок: " + fileWorkout + "\n" +
            "Порядковый номер тренировки: " + fileNumTraining + "\n" +
            "Восстановление данных: " + fileBackup + "\n" + 
            "Хранилище всех спортсменов: " + fileAllSportsmens 
                );
        }

        private void checkItem(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
                if (((ToolStripMenuItem)sender).Name == "поПервойЗаписиToolStripMenuItem" || ((ToolStripMenuItem)sender).Name == "поПоследнейЗапиToolStripMenuItem")
                {
                    if (((ToolStripMenuItem)sender).Checked && ((ToolStripMenuItem)sender).Name == "поПервойЗаписиToolStripMenuItem")
                        поПоследнейЗапиToolStripMenuItem.Checked = false;
                    else if (((ToolStripMenuItem)sender).Checked && ((ToolStripMenuItem)sender).Name == "поПоследнейЗапиToolStripMenuItem")
                        поПервойЗаписиToolStripMenuItem.Checked = false;
                }
                
            }
            
        }

        private void toolStripMenuItemTimeOfDay_CheckedChanged(object sender, EventArgs e)
        {
            refreshPannelList();
            resizePanels();
        }

        //перерисовывает каждую тренировку, используя особый макет
        void refreshPannelList()
        {
            clearPanelList();
            if (поПоследнейЗапиToolStripMenuItem.Checked)
                foreach(Workout w in allWorkouts)
                {
                    chet = !chet;
                    if (chet)
                        addToPanelTraining(w, color1);
                    else
                        addToPanelTraining(w, color2);
                }
            else if (поПервойЗаписиToolStripMenuItem.Checked)
                for (int i = allWorkouts.Count - 1; i >= 0; i--)
                {
                    chet = !chet;
                    if (chet)
                        addToPanelTraining(allWorkouts[i], color1);
                    else
                        addToPanelTraining(allWorkouts[i], color2);
                }
        }

        private void поПервойЗаписиToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            refreshPannelList();
            resizePanels();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Приложение " + Text + " предназначено для ведения дневника тренировок одного или сразу нескольких спортсменов." + "\n" +
                "3.8 появился поиск по тренировкам" +
                "3.9 появился пункт меню \"Данные\" - \"Личные рекорды\"."
           );
        }

        private void разработчикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Developer d = new Developer();
            d.Text += " © Разработчик";
            
            d.Show();
        }

        private void удалитьСпортсменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSportsmen del = new DeleteSportsmen();
            del.FormClosed += new FormClosedEventHandler(del_FormClosed);
            del.Show();
        }

        private void toolStripMenuItemImport_Click(object sender, EventArgs e)
        {
            ImportData im = new ImportData();
            im.Show();
        }

        private void del_FormClosed(object sender, FormClosedEventArgs e)
        {
            findSportsmens(comboBoxSportsmen);
            switchSportsmen();
            setCurrentSportsmen();
        }

        private void textBoxResultExs_MouseEnter(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                toolTip.SetToolTip(((TextBox)sender), ((TextBox)sender).Text);
            }
            if (sender is Label)
            {
                if (((Label)sender).Name == "labelDelete")
                {
                    toolTip.SetToolTip(((Label)sender), "Удалить запись");
                }
            }
        }
        List<string> allWarmUp;
        List<string> allWorks;
        List<string> allResults;
        string[] exception = {"соревнования", "бег", "силовые", "силовые упражнения", "барьеры"};
        //находит все выполненные работы
        List<string> findAll(List<string> list)
        {
            try
            {
                list = new List<string>();
                char[] sep = { ',', ';', '.' };
                foreach (Workout wo in allWorkouts)
                {
                    string[] arr = wo.work.Split(sep);
                    if (arr.Length != 0)
                        if (arr[0] != "")
                            if (!list.Cast<string>().Contains(arr[0]))
                            {
                                if (!exception.Cast<string>().Contains(arr[0].ToLower()))
                                    if (arr[0].ToLower().IndexOf(exception[0]) == -1)
                                        list.Add(arr[0]);
                            }
                }
                return list;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        void setContextMenu(ContextMenuStrip cms, List<string> list)
        {
            try
            {
                if (list.Count != 0)
                    foreach (string s in list)
                    {
                        cms.Items.Add(s);
                        cms.Items[contextMenuStripWork.Items.Count - 1].Click += new EventHandler(ToolStripMenuWorkItem_Click);
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        }

        private void обновитьСписокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateCMS(contextMenuStripWork, allWorks);
        }
        //обновляет контекстное меню работ
        void updateCMS(ContextMenuStrip cms, List<string> list)
        {
            try
            {
                for (int i = 0; i < cms.Items.Count; i++)
                {
                    if (!cms.Items[i].Name.Equals("ToolStripMenuItemUpdate") && !cms.Items[i].Name.Equals("toolStripMenuItemSep"))
                    {
                        cms.Items.RemoveAt(i);
                        i--;
                    }
                }
                list = findAll(list);
                list.Sort();
                setContextMenu(cms, list);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        }

        private void ToolStripMenuWorkItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                richTextBoxWork.Text = richTextBoxWork.Text.Insert(richTextBoxWork.SelectionStart + richTextBoxWork.SelectionLength, ((ToolStripItem)sender).Text); 
            }
        }

        private void changeInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(fileWorkout);
        }

        private void обновитьИнформациюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearPanelList();
            loadWorkouts(fileWorkout);
        }

        private void времяСутокToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
                switch (((ToolStripMenuItem)sender).Name)
                {
                    case "времяСутокToolStripMenuItem": toolStripMenuItemTimeOfDay.Checked = ((ToolStripMenuItem)sender).Checked;
                        break;
                    case "датаToolStripMenuItem": toolStripMenuItemDate.Checked = ((ToolStripMenuItem)sender).Checked;
                        break;
                    case "деньНеделиToolStripMenuItem": toolStripMenuItemDayOfWeek.Checked = ((ToolStripMenuItem)sender).Checked;
                        break;
                    case "номерToolStripMenuItem": toolStripMenuItemNumber.Checked = ((ToolStripMenuItem)sender).Checked;
                        break;
                    case "разминкаToolStripMenuItem": toolStripMenuItemWarmUp.Checked = ((ToolStripMenuItem)sender).Checked;
                        break;
                    case "работаToolStripMenuItem": toolStripMenuItemWork.Checked = ((ToolStripMenuItem)sender).Checked;
                        break;
                    case "результатToolStripMenuItem": toolStripMenuItemResult.Checked = ((ToolStripMenuItem)sender).Checked;
                        break;

                }
        }

        private void показатьТолькоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selfRecords();
            string result = "";
            foreach (string s in theBestresult.Keys)
            {
                result += s + " - " + theBestresult[s];
            }
            MessageBox.Show(result);
        }

        void selfRecords()
        {
            try
            {
                Regex rxNums = new Regex(@"^\d+$");
                //разделитель работ 
                char[] splitWorks = { ';' };
                //разделитель для работ типа "1000/600/200"
                char[] folrtleg = { '/' };
                //разделитель для количества подходов
                char[] count = { 'х', 'x' };
                //разделитель подходов в работе
                char[] splitResults = { ',' };
                char[] elseWords = { ' ' };
                char[] num = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                char[] trash = { '.', ';', '(', ')' };
                //получить все дистанции спортсмена
                List<string> allDistance = new List<string>();
                foreach (Workout w in allWorkouts)
                {
                    string[] works = w.work.Split(splitResults);
                    foreach (string s in works)
                    {
                        string str = s.Trim();
                        if (str.IndexOfAny(num) != -1)
                        {
                            if (str.IndexOfAny(count) != -1)
                            {
                                string[] work = str.Split(count);
                                string wrk = work[1].Split(elseWords)[0].Split(trash)[0].Replace(")", "").Trim();
                                if (!allDistance.Contains(wrk))
                                    allDistance.Add(wrk);
                            }
                            else if (str.IndexOfAny(folrtleg) != -1)
                            {
                                string[] res = str.Split(folrtleg);
                                foreach (string r in res)
                                {
                                    string temp = r.Split(elseWords)[0].Split(trash)[0];
                                    if (!allDistance.Contains(temp))
                                        allDistance.Add(temp);
                                }
                            }
                            else if (rxNums.IsMatch(str))
                            {
                                string temp = str.Trim();
                                if (!allDistance.Contains(temp))
                                    allDistance.Add(temp);
                            }
                            else if (str.ToLower().IndexOf("соревнования") != -1)
                            {
                                string[] res = str.Split(trash);
                                string temp = res[0].Trim();
                                if (!allDistance.Contains(temp))
                                    allDistance.Add(temp);
                            }
                            else if (Char.IsDigit(str[0]))
                            {
                                string[] res = str.Split(trash);
                                string temp = res[0].Trim().Split(elseWords)[0];
                                if (!allDistance.Contains(temp))
                                    allDistance.Add(temp);
                            }

                        }
                    }
                }
                
                //получить лучшее время по каждой из дистанций
                foreach (string distance in allDistance)
                {
                    string result = "";
                    Workout[] all = findWorkout(distance);
                    //выделить время по данной дистанции у всех тренировок
                    List<string> times = new List<string>();
                    foreach (Workout w in all)
                    {
                        times.Add(w.getTimeOf(distance));
                    }
                    //взять минимальное
                    string min = "";
                    /*
                    List<string> allR = new List<string>();
                    foreach (Workout w in allWorkouts)
                    {
                        if (w.work.IndexOf(s) != -1 && w.work.IndexOf(s + "0") == -1 && w.work.IndexOf(s + " минут") == -1 
                            && w.work.Substring(0, w.work.IndexOf(s)).IndexOf('(') == -1)
                        {
                            string[] all = w.result.Split(';');
                            //ищем количество запятых до нашей дистанции в строке
                            
                            int countInStr = new Regex(",").Matches(w.work.Substring(0, w.work.IndexOf(s))).Count;
                            int countSlshInStr = new Regex("/").Matches(w.work.Substring(0, w.work.IndexOf(s))).Count;
                            if (countInStr >= 0)
                            {
                                string[] values = all[countInStr].Split(',');
                                if (w.work.Substring(0, w.work.IndexOf(s)).IndexOf('/') != -1 && countInStr == 0 || w.work.Split('/')[0].Equals(s))
                                    allR.Add(values[countSlshInStr]);
                                else
                                    foreach (string ss in values)
                                        allR.Add(ss);
                            }
                            else
                            {
                                string[] values = all[0].Split(',');
                                foreach (string ss in values)
                                    allR.Add(ss);
                            }
                        }
                    }
                    result = allR[0];
                    for (int i = 1; i < allR.Count; i++)
                        if (!S1IsBiggerS2(allR[i], result))
                            if (allR[i].IndexOf("х") == -1)
                                result = allR[i];

                    */
                    theBestresult.Add(distance, result);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        }

        Workout[] findWorkout(string work)
        {
            List<Workout> result = new List<Workout>();
            foreach (Workout w in allWorkouts)
            {
                if (w.work.IndexOf(work) != -1)
                    result.Add(w);
            }
            return result.ToArray();
        }

        //00.00.00 или 00'' или 00'  
        //если s1 больше s2 возвращает true
        bool S1IsBiggerS2(string s1, string s2)
        {

            int count1 = new Regex("'").Matches(s1).Count;
            int count2 = new Regex("'").Matches(s2).Count;
            if (count1 == 0 && count2 == 0)
            {
                string[] arr1 = s1.Split('.');
                string[] arr2 = s2.Split('.');
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i].IndexOf("х") != -1)
                        return false;
                    if (arr2[i].IndexOf("х") != -1)
                        return true;
                    if (Convert.ToInt16(arr1[i]) > Convert.ToInt16(arr2[i]))
                        return true;
                    else if (Convert.ToInt16(arr1[i]) < Convert.ToInt16(arr2[i]))
                        return false;
                }
            }
            //сравнить 1.04 и 67''
            else 
            {
                string[] arr1 = s1.Split('\'');
                string[] arr2 = s2.Split('\'');
                if (Convert.ToInt16(arr1[0]) > Convert.ToInt16(arr2[0]))
                    return true;
                return false;
            }
            return false;
        }

        private void создатьXMLфайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(fileXML);
                CreateXMLDocument(fileXML);
                WriteToXMLDocument(fileXML);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void CreateXMLDocument(string filepath)
        {
            XmlTextWriter xtw = new XmlTextWriter(filepath, Encoding.UTF8);
            xtw.WriteStartDocument();
            xtw.WriteStartElement("sportsmens");
            xtw.WriteEndDocument();
            xtw.Close();
        }

        private void WriteToXMLDocument(string filepath)  
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                FileStream fs = new FileStream(filepath, FileMode.Open);
                xd.Load(fs);
                string sportsmen_code = "1";//если будем в цикле, поставить счетчик
                long idwork = 100000; //соответствие между работой и результатом
                long idresult = 1;
                XmlElement xmlsportsmen = xd.CreateElement("sportsmen");
                xmlsportsmen.SetAttribute("id", sportsmen.name + sportsmen.firstName + sportsmen.fathersName);
                XmlElement code = xd.CreateElement("code");
                XmlElement name = xd.CreateElement("name");
                XmlElement firstname = xd.CreateElement("firstname");
                XmlElement fathersname = xd.CreateElement("fathersname");
                XmlElement sportslevel = xd.CreateElement("sportslevel");
                XmlElement nickname = xd.CreateElement("nickname");
                XmlElement birthday = xd.CreateElement("birthday");
                XmlElement phone = xd.CreateElement("phone");
                XmlElement from = xd.CreateElement("from");
                XmlElement sheff = xd.CreateElement("sheff");
                XmlElement phonesheff = xd.CreateElement("phonesheff");

                XmlText tcode = xd.CreateTextNode(sportsmen_code);
                XmlText tname = xd.CreateTextNode(sportsmen.name);
                XmlText tfirstname = xd.CreateTextNode(sportsmen.firstName);
                XmlText tfathersname = xd.CreateTextNode(sportsmen.fathersName);
                XmlText tsportslevel = xd.CreateTextNode(sportsmen.sportslevel);
                XmlText tnickname = xd.CreateTextNode(sportsmen.nickName);
                XmlText tbirthday = xd.CreateTextNode(sportsmen.setBirthsday(sportsmen.born_day + '.' + sportsmen.born_month + '.' + sportsmen.born_year));
                XmlText tphone = xd.CreateTextNode(sportsmen.phone);
                XmlText tfrom = xd.CreateTextNode(sportsmen.from);
                XmlText tsheff = xd.CreateTextNode(sportsmen.sheff);
                XmlText tphonesheff = xd.CreateTextNode(sportsmen.phoneSheff);

                code.AppendChild(tcode);
                name.AppendChild(tname);
                firstname.AppendChild(tfirstname);
                fathersname.AppendChild(tfathersname);
                sportslevel.AppendChild(tsportslevel);
                nickname.AppendChild(tnickname);
                birthday.AppendChild(tbirthday);
                phone.AppendChild(tphone);
                sheff.AppendChild(tsheff);
                phonesheff.AppendChild(tphonesheff);

                xmlsportsmen.AppendChild(code);
                xmlsportsmen.AppendChild(name);
                xmlsportsmen.AppendChild(firstname);
                xmlsportsmen.AppendChild(fathersname);
                xmlsportsmen.AppendChild(sportslevel);
                xmlsportsmen.AppendChild(nickname);
                xmlsportsmen.AppendChild(birthday);
                xmlsportsmen.AppendChild(phone);
                xmlsportsmen.AppendChild(sheff);
                xmlsportsmen.AppendChild(phonesheff);

                XmlElement xmlworkouts = xd.CreateElement("workouts");
                xmlworkouts.SetAttribute("id", "workouts");

                int i = 1;
                allWorkouts.Reverse();
                foreach (Workout w in allWorkouts)
                {
                    XmlElement xmlworkout = xd.CreateElement("workout");
                    xmlworkout.SetAttribute("id", i.ToString());

                    XmlElement scode = xd.CreateElement("sportsmencode");
                    XmlText tscode = xd.CreateTextNode(sportsmen_code);
                    scode.AppendChild(tscode);
                    xmlworkout.AppendChild(scode);

                    XmlElement num = xd.CreateElement("num");
                    XmlElement date = xd.CreateElement("date");
                    XmlElement dayofweek = xd.CreateElement("dayofweek");
                    XmlElement timeofday = xd.CreateElement("timeofday");
                    XmlElement warmup = xd.CreateElement("warmup");
                    XmlElement worksandresults = xd.CreateElement("worksandresults");

                    XmlText tNum = xd.CreateTextNode(w.number);
                    XmlText tDate = xd.CreateTextNode(w.date);
                    XmlText tDayofweek = xd.CreateTextNode(w.dayOfWeek);
                    XmlText tTimeofday = xd.CreateTextNode(w.timeOfDay);
                    XmlText tWarmup = xd.CreateTextNode(w.warmUp);

                    num.AppendChild(tNum);
                    date.AppendChild(tDate);
                    dayofweek.AppendChild(tDayofweek);
                    timeofday.AppendChild(tTimeofday);
                    warmup.AppendChild(tWarmup);
                    
                    string[] allworks = mySplit(w.work, ',', '(', ')');
                    string[] allresults = w.result.Split(';');
                    richTextBoxResult.Text += w.number + "\n";
                    for (int ind = 0; ind < allworks.Length; ind++ )
                    {
                        XmlElement works = xd.CreateElement("works");
                        XmlElement work = xd.CreateElement("work");
                        XmlElement workid = xd.CreateElement("id");

                        XmlElement ids = xd.CreateElement("ids");
                        XmlElement id = xd.CreateElement("id");

                        XmlText tWork = xd.CreateTextNode(allworks[ind].Trim());
                        XmlText tIdWork = xd.CreateTextNode(idwork.ToString());
                        workid.AppendChild(tIdWork);
                        work.AppendChild(tWork);
                        works.AppendChild(workid);
                        works.AppendChild(work);

                        XmlElement idWork = xd.CreateElement("idWork");
                        XmlText tIdW = xd.CreateTextNode(idwork.ToString());
                        idWork.AppendChild(tIdW);
                        id.AppendChild(idWork);

                        XmlElement result = xd.CreateElement("result");
                        XmlElement resultid = xd.CreateElement("id");
                        XmlText tIdResult = xd.CreateTextNode(idresult.ToString());
                        resultid.AppendChild(tIdResult);
                        result.AppendChild(resultid);

                        XmlElement idResult = xd.CreateElement("idResult");
                        XmlText tIdR = xd.CreateTextNode(idresult.ToString());
                        idResult.AppendChild(tIdR);
                        id.AppendChild(idResult);

                        string[] tasks = mySplit(allresults[ind], ',', '(', ')');
                        foreach (string str in tasks)
                        {
                            XmlElement task = xd.CreateElement("task");
                            XmlText tTask = xd.CreateTextNode(str.Trim());
                            task.AppendChild(tTask);
                            result.AppendChild(task);
                        }
                        XmlElement numWorkout = xd.CreateElement("numWorkout");
                        XmlText tnumWorkout = xd.CreateTextNode(w.number);
                        numWorkout.AppendChild(tnumWorkout);
                        result.AppendChild(numWorkout);
                        XmlElement numW = xd.CreateElement("numWorkout");
                        XmlText tnumW = xd.CreateTextNode(w.number);
                        numW.AppendChild(tnumW);
                        works.AppendChild(numW);

                        ids.AppendChild(id);
                        ids.AppendChild(works);
                        ids.AppendChild(result);
                        worksandresults.AppendChild(ids);
                        idwork++;
                        idresult++;
                    }
                    
                    xmlworkout.AppendChild(num);
                    xmlworkout.AppendChild(date);
                    xmlworkout.AppendChild(dayofweek);
                    xmlworkout.AppendChild(timeofday);
                    xmlworkout.AppendChild(warmup);
                    xmlworkout.AppendChild(worksandresults);

                    xmlworkouts.AppendChild(xmlworkout);

                    i++;
                }
                allWorkouts.Reverse();

                xmlsportsmen.AppendChild(xmlworkouts);

                xd.DocumentElement.AppendChild(xmlsportsmen);

                fs.Close();         // Закрываем поток  
                xd.Save(filepath); // Сохраняем файл  
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        }
        //тоже, что и обычный split, только есть возможность задавать интервал между символами begin и end, который сплитить не надо
        string[] mySplit(string str, char c, char begin, char end)
        {
            List<string> l = new List<string>();
            string s = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Equals(c))
                {
                    l.Add(s);
                    s = "";
                }
                else if (str[i].Equals(begin))
                {
                    while (!str[i].Equals(end) && i < str.Length)
                    {
                        s += str[i];
                        i++;
                    }
                    if (s.IndexOf('(') != -1)
                        s += ')';
                }
                else
                {
                    s += str[i];
                }

            }
            if (!s.Equals(""))
                l.Add(s);
            return l.ToArray();
        }

        private void splitTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] arr = mySplit(richTextBoxWork.Text, ',', '(', ')');
            foreach (string s in arr)
                richTextBoxResult.Text += s + "\n";
        }

        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('\r'))
                search(textBoxSearch.Text);
        }

        void search(string s)
        {
            List<Workout> searchResult = new List<Workout>();
            foreach (Workout w in allWorkouts)
            {
                if (w.date.IndexOf(s) != -1 || w.dayOfWeek.IndexOf(s) != -1 || w.result.IndexOf(s) != -1 || w.timeOfDay.IndexOf(s) != -1 || w.work.IndexOf(s) != -1 || w.warmUp.IndexOf(s) != -1)
                    searchResult.Add(w);
            }

            clearPanelList();
            loadWorkouts(searchResult);
        }
    }
}
