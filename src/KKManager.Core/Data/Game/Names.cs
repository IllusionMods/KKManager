using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KKManager.Data.Game
{
    public class GameName
    {
        public string LongName { get; }
        public string ShortName { get; }

        public static readonly Dictionary<string, string> LongToShort = new Dictionary<string, string>
        {
            {"HoneySelect2", "HS2"},
            {"AI-Syoujyo", "AI"},
            {"Koikatu", "KK"},
            {"KoikatsuSunshine", "KKS"},
            {"EmotionCreations", "EC"},
            {"RoomGirl", "RG"},
            {"PlayHome", "PH"}
        };

        public static readonly Dictionary<string, GameName> ShortToLong = LongToShort
            .ToDictionary(kvp => NormalizeName(kvp.Value), kvp => new GameName(kvp.Key), StringComparer.OrdinalIgnoreCase);

        public static readonly List<KeyValuePair<string, string>> GameNamesList = LongToShort
            .Concat(ShortToLong.Select(kvp => new KeyValuePair<string, string>(kvp.Value.ShortName, kvp.Value.LongName)))
            .Select(kvp => new KeyValuePair<string, string>(NormalizeName(kvp.Key), NormalizeName(kvp.Value)))
            .ToList();

        public static readonly Dictionary<string, string> GameNamesDictionary = GameNamesList
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(g => g.Key, g => g.First().Value, StringComparer.OrdinalIgnoreCase);

        public GameName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                LongName = null;
                ShortName = null;
                return;
            }

            name = NormalizeName(name);

            if (!LongToShort.TryGetValue(name, out string shortName))
            {
                if (!ShortToLong.TryGetValue(name, out GameName gameName))
                {
                    LongName = null;
                    ShortName = null;
                    return;
                }

                LongName = gameName.LongName;
                ShortName = gameName.ShortName;
            }
            else
            {
                LongName = name;
                ShortName = shortName;
            }
        }

        public static implicit operator GameName(string name) => new GameName(name);

        public static explicit operator string(GameName gameName) => gameName.LongName;

        public override int GetHashCode() => LongName.GetHashCode() ^ ShortName.GetHashCode();

        private static string NormalizeName(string name)
        {
            name = name.Trim();
            name = Regex.Replace(name, @"^AISyoujo$", "AI-Syoujyo");
            return name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is string)
            {
                return this.Equals(new GameName((string)obj));
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            GameName other = (GameName)obj;
            return LongName == other.LongName && ShortName == other.ShortName;
        }


        public override string ToString() => LongName;
    }

}
