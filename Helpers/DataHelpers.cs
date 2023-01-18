namespace SolarisUnited.Warframe.Armory.Helpers
{
    public class DataHelpers
    {
        public bool TryParseMission(string text)
        {
            if (text.Contains("/") && text.Contains("(") && text.Contains(")"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseOddMission(string text)
        {
            if (!text.ToLowerInvariant().StartsWith("rotation "))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseRotation(string text)
        {
            if (text.ToLowerInvariant().StartsWith("rotation ") || text.ToLowerInvariant().Contains(" completion"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseRelic(string text)
        {
            if (text.ToLowerInvariant().Contains(" relic (") && text.EndsWith(")"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool TryParseReward(string text)
        {
            if (text.Contains("%)"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseBounty(string text)
        {
            if (text.ToLowerInvariant().StartsWith("level ") && text.Contains(" - "))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseStage(string text)
        {
            if (text.ToLowerInvariant().StartsWith("stage ") || text.ToLowerInvariant().EndsWith(" stage"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseSource(string text)
        {
            if (!text.ToLowerInvariant().Contains(" drop chance: ") && !text.ToLowerInvariant().EndsWith("%"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseChance(string text)
        {
            if (text.ToLowerInvariant().Contains(" drop chance: ") && text.ToLowerInvariant().EndsWith("%"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseItemByItem(string text)
        {
            if (!text.ToLowerInvariant().Equals("source") && !text.ToLowerInvariant().EndsWith(" drop chance") && !text.ToLowerInvariant().Equals("chance"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseDropSourceName(string text)
        {
            if (!text.ToLowerInvariant().Contains("%"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseDropSourceDropChance(string text)
        {
            if (text.ToLowerInvariant().Contains("%") && !text.ToLowerInvariant().Contains("(") && !text.ToLowerInvariant().EndsWith("%)"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryParseDropSourceChance(string text)
        {
            if (text.ToLowerInvariant().Contains("(") && text.ToLowerInvariant().EndsWith("%)"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}