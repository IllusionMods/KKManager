﻿namespace KKManager
{
    public static class Constants
    {
        public const string Version = "1.5.0"
#if DEBUG
            + ".*"
#endif
            ;

        public const string RepoLink = "https://github.com/IllusionMods/KKManager";
        public const string LatestReleaseLink = "https://github.com/IllusionMods/KKManager/releases/latest";
    }
}
