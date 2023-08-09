namespace DiscordBotCS.Schedule
{
    public class Lesson
    {
        public Lesson(string time, string name, string day)
        {
            Time = time;
            Name = name;
            Day = day;
        }

        public string Time { get; }
        public string Name { get; }
        public string Day { get; }
        public bool passed { get; set; } = false;

        public override string ToString()
        {
            var text = $"{Time} {Name}";
            if (string.IsNullOrWhiteSpace(text)) return "---";
            return $"`{Time}` {Name}";
        }
    }
}