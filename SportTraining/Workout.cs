using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SportTraining
{
    class Workout
    {
        public string number = "";
        public string sheff = "";
        public string sportsmen = "";
        public string date = "";
        public string dayOfWeek = "";
        public string timeOfDay = "";
        public string warmUp = "";
        public string work = "";
        public string result = "";

        public List<string> results = new List<string>();

        public void setResults()
        {
            char[] sep = { ';' };
            string[] s = result.Split(sep);
            foreach (string st in s)
                results.Add(st.Trim());
        }

        //возвращает время по указанной дистанции
        public string getTimeOf(string distance)
        {
            string time = "";
            string[] works = work.Split(',');
            string[] results = result.Split(';');
            for (int i = 0; i < works.Length; i++)
            {
                if (works[i].IndexOf(distance) != -1)
                    if (i < results.Length)
                    time = results[i];
            }
            return time;
        }
    }
}
