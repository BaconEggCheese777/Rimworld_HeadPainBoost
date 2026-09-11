using System;
using System.Collections.Generic;
using System.Reflection;
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

            var postfix = new HarmonyMethod(
                typeof(PainOffsetPostfix),
                nameof(PainOffsetPostfix.Postfix));

            int patchedCount = 0;
            var seenMethods = new HashSet<MethodInfo>();

            // Scan every loaded type derived from Hediff and patch every
            // getter that specific type declares for "PainOffset" - not
            // just the one on the base Hediff class. Different hediff
            // subclasses (e.g. the class used for ordinary wounds) can
            // each provide their own override, and Harmony only
            // intercepts calls that actually dispatch to a patched
            // method, so we patch all of them rather than guess which
            // one is in play at runtime.
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = Array.FindAll(ex.Types, t => t != null);
                }

                foreach (Type type in types)
                {
                    if (!typeof(Hediff).IsAssignableFrom(type))
                        continue;

                    PropertyInfo prop = type.GetProperty(
                        "PainOffset",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    if (prop == null)
                        continue;

                    MethodInfo getter = prop.GetGetMethod(true);
                    if (getter == null || !seenMethods.Add(getter))
                        continue;

                    harmony.Patch(getter, postfix: postfix);
                    patchedCount++;
                }
            }

            var settings = HeadPainBoostDefOf.HeadPainBoost_Settings;
            if (settings?.partMultipliers != null)
            {
                foreach (var entry in settings.partMultipliers)
                {
                    Log.Message("[Head Pain Boost] " + entry.bodyPart + " pain x" + entry.multiplier);
                }
            }
            Log.Message("[Head Pain Boost] Patched " + patchedCount + " PainOffset getter(s).");
        }
    }

    public static class PainOffsetPostfix
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
