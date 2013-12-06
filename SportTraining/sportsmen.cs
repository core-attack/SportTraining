using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SportTraining
{
    public class Sportsmen
    {
        //id
        public int id = 0;
        //фамилия спортсмена
        public string name = "";
        //имя
        public string firstName = "";
        //отчество
        public string fathersName = "";
        //кличка
        public string nickName = "";
        //его тренер
        public string sheff = "";
        //телефон тренера
        public string phoneSheff = "";
        //телефон спортсмена
        public string phone = "";
        //год рождения
        public string born_year = "";
        //месяц рождения
        public string born_month = "";
        //день рождения
        public string born_day = "";

        public void setBirthsday(string day, string month, string year)
        {
            born_day = day;
            born_month = month;
            born_year = year;
        }

        public string setBirthsday(string date)
        {
            char[] sep = { '.' };
            string[] dat = date.Split(sep);
            born_day = dat[0];
            born_month = dat[1];
            born_year = dat[2];
            return born_day + "." + born_month + "." + born_year;
        }

        public string getBirthsday()
        {
            return born_day + "." + born_month + "." + born_year;
        }

        //спортивный разряд
        public string sportslevel = "";
        //адрес проживания
        public string from = "";
    }
}
