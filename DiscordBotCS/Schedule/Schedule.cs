using System;
using System.Collections.Generic;

namespace DiscordBotCS.Schedule
{
    public static class Schedule
    {
        public static List<Lesson> GetTodaysSchedule()
        {
            var Lessons = new List<Lesson>();

            switch (DateTime.Now.DayOfWeek.ToString())
            {
                case "Monday":
                    Lessons = Monday();
                    break;
                case "Tuesday":
                    Lessons = Tuesday();
                    break;
                case "Wednesday":
                    Lessons = Wednesday();
                    break;
                case "Thursday":
                    Lessons = Thursday();
                    break;
                case "Friday":
                    Lessons = Friday();
                    break;
            }

            return Lessons;
        }

        public static List<Lesson> GetWeeksSchedule()
        {
            var Lessons = new List<Lesson>();
            Lessons.AddRange(Monday());
            Lessons.AddRange(Tuesday());
            Lessons.AddRange(Wednesday());
            Lessons.AddRange(Thursday());
            Lessons.AddRange(Friday());
            return Lessons;
        }

        public static string GetDay()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }

        public static List<Lesson> Monday()
        {
            var Lessons = new List<Lesson>();
            Lessons.Add(new Lesson("08:15-10:00", "ΑΛΓΟΡΙΘΜΟΙ", "Monday"));
            Lessons.Add(new Lesson("10:15-12:00", "ΒΑΣΕΙΣ ΔΕΔΟΜΕΝΩΝ", "Monday"));
            Lessons.Add(new Lesson("12:15-14:00", "ΑΡΧΕΣ ΚΑΙ ΕΦΑΡΜΟΓΕΣ ΣΥΜΑΤΩΝ ΚΑΙ ΣΥΣΤΗΜΑΤΩΝ", "Monday"));
            return Lessons;
        }

        public static List<Lesson> Tuesday()
        {
            var Lessons = new List<Lesson>();
            Lessons.Add(new Lesson("10:15-12:00", "ΑΛΓΟΡΙΘΜΟΙ", "Tuesday"));
            Lessons.Add(new Lesson("12:15-14:00", "ΠΛΗΡΟΦΟΡΙΚΗ ΣΤΗΝ ΕΚΠΑΙΔΕΥΣΗ", "Tuesday"));
            Lessons.Add(new Lesson("16:15-18:00", "ΔΙΚΤΥΑ ΥΠΟΛΟΓΙΣΤΩΝ", "Tuesday"));
            return Lessons;
        }

        public static List<Lesson> Wednesday()
        {
            var Lessons = new List<Lesson>();
            Lessons.Add(new Lesson("08:15-10:00", "ΔΙΚΤΥΑ ΥΠΟΛΟΓΙΣΤΩΝ", "Wednesday"));
            Lessons.Add(new Lesson("10:15-12:00", "ΒΑΣΕΙΣ ΔΕΔΟΜΕΝΩΝ", "Wednesday"));
            Lessons.Add(new Lesson("12:15-14:00", "ΑΡΧΕΣ ΚΑΙ ΕΦΑΡΜΟΓΕΣ ΣΥΜΑΤΩΝ ΚΑΙ ΣΥΣΤΗΜΑΤΩΝ", "Wednesday"));
            return Lessons;
        }

        public static List<Lesson> Thursday()
        {
            var Lessons = new List<Lesson>();
            Lessons.Add(new Lesson(null, null, "Thursday"));
            return Lessons;
        }

        public static List<Lesson> Friday()
        {
            var Lessons = new List<Lesson>();
            Lessons.Add(new Lesson("08:15-12:00", "ΠΡΟΓΡΑΜΜΑΤΙΣΜΟΣ ΣΤΟ ΔΙΑΔΙΚΤΥΟ ΚΑΙ ΠΑΓΚΟΣΜΙΟ ΙΣΤΟ", "Friday"));
            Lessons.Add(new Lesson("12:15-14:00", "ΠΛΗΡΟΦΟΡΙΚΗ ΣΤΗΝ ΕΚΠΑΙΔΕΥΣΗ", "Friday"));
            return Lessons;
        }
    }
}