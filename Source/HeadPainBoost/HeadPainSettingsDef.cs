using System.Collections.Generic;
using Verse;

namespace HeadPainBoost
{
    /// <summary>
    /// A plain Def (not a Hediff/Thing/etc.) whose only job is to hold
    /// tunable numbers for this mod. Editing Defs/HeadPainSettings.xml
    /// changes behavior with no recompile - just restart the game.
    /// </summary>
    public class HeadPainSettingsDef : Def
    {
        public List<BodyPartPainMultiplier> partMultipliers;
    }

    public class BodyPartPainMultiplier
    {
        // defName of the BodyPartDef this multiplier applies to,
        // e.g. "Head", "Skull", "Brain".
        public string bodyPart;

        // 1.0 = no change, 1.5 = +50% pain from injuries on this part, etc.
        public float multiplier = 1f;
    }
}
