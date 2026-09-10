using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace HeadPainBoost
{
    [StaticConstructorOnStartup]
    public static class HeadPainBoostInit
    {
        static HeadPainBoostInit()
        {
            var harmony = new Harmony("yourname.headpainboost");
            harmony.PatchAll();

            var settings = HeadPainBoostDefOf.HeadPainBoost_Settings;
            if (settings?.partMultipliers != null)
            {
                foreach (var entry in settings.partMultipliers)
                {
                    Log.Message("[Head Pain Boost] " + entry.bodyPart + " pain x" + entry.multiplier);
                }
            }
        }
    }

    // Patching PainOffset directly (rather than the HediffSet.PainTotal
    // aggregate) means BOTH the per-injury tooltip and the pawn's total
    // pain read the same boosted number, since PainTotal is just a sum
    // of each hediff's PainOffset.
    [HarmonyPatch(typeof(Hediff), nameof(Hediff.PainOffset), MethodType.Getter)]
    public static class Patch_Hediff_PainOffset_HeadBoost
    {
        private static Dictionary<string, float> multipliersByPart;

        private static Dictionary<string, float> MultipliersByPart
        {
            get
            {
                if (multipliersByPart == null)
                {
                    multipliersByPart = new Dictionary<string, float>();
                    var settings = HeadPainBoostDefOf.HeadPainBoost_Settings;
                    if (settings?.partMultipliers != null)
                    {
                        foreach (var entry in settings.partMultipliers)
                        {
                            if (!string.IsNullOrEmpty(entry.bodyPart))
                                multipliersByPart[entry.bodyPart] = entry.multiplier;
                        }
                    }
                }
                return multipliersByPart;
            }
        }

        public static void Postfix(Hediff __instance, ref float __result)
        {
            if (__result <= 0f)
                return;

            if (__instance.Part == null)
                return;

            if (MultipliersByPart.TryGetValue(__instance.Part.def.defName, out float multiplier))
            {
                __result *= multiplier;
            }
        }
    }
}
