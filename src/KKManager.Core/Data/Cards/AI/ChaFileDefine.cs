namespace KKManager.Data.Cards.AI
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

        public enum Trait : byte
        {
            None,
            Fastidious,
            Lazy,
            Frailty,
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

        public enum Mentality : byte
        {
            None,
            Curious,
            Affectionate,
            Lovestruck,
            Awkward,
            Reluctant,
            Loathing,
            Cooperative,
            Obedient,
            Submissive,
            Interested,
            Charmed,
            Aroused
        }

        public enum SexTrait : byte
        {
            None,
            Horny,
            Sadist,
            Masochist,
            SensitiveBreasts,
            SensitiveAss,
            SensitivePussy,
            LoveKisses,
            CleanFreak,
            SexHater,
            Lonely
        }
    }
}
