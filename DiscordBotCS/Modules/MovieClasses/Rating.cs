namespace DiscordBotCS.Modules.MovieClasses
{
    public class Rating
    {
        public string Source { get; set; }
        public string Value { get; set; }


        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Source) || string.IsNullOrWhiteSpace(Value))
            {
                return "-";
            }
            return $"{Source}: {Value}";
        }
    }
}