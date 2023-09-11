namespace KKManager.Data.Cards.HC
{
    public static class ChaFileDefine
    {
        public enum State
        {
            Blank,
            Favor,
            Enjoyment,
            Aversion,
            Slavery,
            Broken,
            Dependence
        }

        public enum Quirk : byte
        {
            None,
            Fastidious,
            Lazy,
            Frail,
            Tough,
            WeakBladder,
            Tenacious,
            GlassHeart,
            Indomitable,
            PentUp,
            IronWill,
            Capricious,
            Emotional
        }

        public enum Mood : byte
        {
            None,
            Interested,
            LikesYou,
            LoveAtFirstSight,
            Shy,
            Dislike,
            Disgust,
            WantsSuggestions,
            DominateMe,
            Obedient,
            FunLoving,
            HurtMe,
            ObeyMe
        }

        public enum Kink : byte
        {
            None,
            Lusty,
            S,
            M,
            SensitiveBreasts,
            SensitiveAss,
            SensitiveGroin,
            LikesKisses,
            CleanFreak,
            SexuallyCautious,
            Lonely
        }
    }
}
