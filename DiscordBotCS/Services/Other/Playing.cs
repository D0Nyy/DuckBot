using System.Collections.Generic;
using System.Linq;

namespace DiscordBotCS.Services.Other
{
    public static class Playing
    {
        private static List<string> GameList { get; } = StringLists.GameList;
        private static int Pos { get; set; }

        public static string GetNextGame()
        {
            if (GameList[Pos].Equals(GameList.Last()))
            {
                Pos = 0;
                return GameList.Last();
            }

            return GameList[Pos];
        }
    }
}