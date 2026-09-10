using RimWorld;
using Verse;

namespace HeadPainBoost
{
    [DefOf]
    public static class HeadPainBoostDefOf
    {
        public static HeadPainSettingsDef HeadPainBoost_Settings;

        static HeadPainBoostDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HeadPainBoostDefOf));
        }
    }
}
