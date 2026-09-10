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

    // Patched here instead of Hediff.PainOffset: Hediff_Injury (the class
    // used for ordinary wounds - cuts, gunshots, burns, etc.) computes its
    // own pain internally and doesn't route through the base Hediff
    // getter, so patching that getter never actually fired for real
    // injuries. HediffSet.PainTotal is the single point where every
    // hediff's pain contribution gets summed, regardless of which
    // subclass it is, so patching here catches all of them uniformly.
    [HarmonyPatch(typeof(HediffSet), nameof(HediffSet.PainTotal), MethodType.Getter)]
    public static class Patch_HediffSet_PainTotal_HeadBoost
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

        public static void Postfix(HediffSet __instance, ref float __result)
        {
            if (MultipliersByPart.Count == 0)
                return;

            float extra = 0f;
            List<Hediff> hediffs = __instance.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff hediff = hediffs[i];
                if (hediff.Part == null)
                    continue;

                if (MultipliersByPart.TryGetValue(hediff.Part.def.defName, out float multiplier))
                {
                    float basePain = hediff.PainOffset;
                    if (basePain > 0f)
                    {
                        extra += basePain * (multiplier - 1f);
                    }
                }
            }

            if (extra != 0f)
            {
                __result += extra;
            }
        }
    }
}
