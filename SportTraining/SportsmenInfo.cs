using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SportTraining
{
    public partial class SportsmenInfo : Form
    {
        public SportsmenInfo()
        {
            InitializeComponent();
            textBoxBornDate.Text = DateTime.Now.ToShortDateString();
            textBoxName.Focus();
        }
        Sportsmen sportsmen = new Sportsmen();
        public bool isButtonAddClick = false;
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            sportsmen = new Sportsmen();
            sportsmen.name = textBoxName.Text.Replace(" ", "");
            sportsmen.firstName = textBoxFirstName.Text.Replace(" ", "");
            sportsmen.fathersName = textBoxFathersName.Text.Replace(" ", "");
            sportsmen.nickName = textBoxNickName.Text.Replace(" ", "");
            sportsmen.phone = textBoxPhone.Text.Replace(" ", "");
            sportsmen.sheff = textBoxSheff.Text;
            sportsmen.phoneSheff = textBoxPhoneSheff.Text.Replace(" ", "");
            sportsmen.sportslevel = textBoxSportsLevel.Text;
            sportsmen.from = textBoxFrom.Text;
            try
            {
                char[] sep = { '.' };
                string[] date = textBoxBornDate.Text.Split(sep);
                sportsmen.setBirthsday(date[0], date[1], date[2]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка формата даты");
            }
            isButtonAddClick = true;
            Close();
        }

        public void setSportsmen(out Sportsmen sp)
        {
            sp = new Sportsmen();
            sp.name = sportsmen.name;
            sp.firstName = sportsmen.firstName;
            sp.fathersName = sportsmen.fathersName;
            sp.nickName = sportsmen.nickName;
            sp.phone = sportsmen.phone;
            sp.sheff = sportsmen.sheff;
            sp.phoneSheff = sportsmen.phoneSheff;
            sp.sportslevel = sportsmen.sportslevel;
            sp.from = sportsmen.from;
            try
            {
                char[] sep = { '.' };
                string[] date = textBoxBornDate.Text.Split(sep);
                sp.setBirthsday(date[0], date[1], date[2]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка формата даты");
            }
        }



        private void textBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (sender is TextBox)
                {
                    switch (((TextBox)sender).Name)
                    {
                        case "textBoxName": textBoxFirstName.Focus();
                            break;
                        case "textBoxFirstName": textBoxFathersName.Focus();
                            break;
                        case "textBoxFathersName": textBoxSportsLevel.Focus();
                            break;
                        case "textBoxSportsLevel": textBoxNickName.Focus();
                            break;
                        case "textBoxNickName": textBoxBornDate.Focus();
                            break;
                        case "textBoxBornDate": textBoxPhone.Focus();
                            break;
                        case "textBoxPhone": textBoxFrom.Focus();
                            break;
                        case "textBoxFrom": textBoxSheff.Focus();
                            break;
                        case "textBoxSheff": textBoxPhoneSheff.Focus();
                            break;
                        case "textBoxPhoneSheff": buttonAdd.Focus();
                            break;

                    }
                }
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            if (sender is Button)
                foreach (object obj in ((Button)sender).Parent.Controls)
                    if (obj is TextBox)
                        ((TextBox)obj).ReadOnly = false;
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            isButtonAddClick = false;
            Close();
        }

    }
}
