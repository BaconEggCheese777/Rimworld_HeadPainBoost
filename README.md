# Head Pain Boost

Small RimWorld mod: injuries on a chosen body part (default: **Head**) contribute more to overall pain than the same injury elsewhere. The multiplier is set in XML, not hardcoded.

## How it works
A single Harmony postfix on `Hediff.PainOffset` checks whether the hediff's body part matches `targetBodyPart` (from `Defs/HeadPainSettings.xml`) and, if so, multiplies its pain contribution by `painMultiplier`. This applies to *any* injury/disease/hediff on that part — cuts, burns, gunshots, infections, everything — not just a specific hediff type.

## Configuring
Edit `Defs/HeadPainSettings.xml` — add or remove `<li>` entries freely, one per body part:
```xml
<partMultipliers>
  <li>
    <bodyPart>Head</bodyPart>
    <multiplier>1.5</multiplier>
  </li>
  <li>
    <bodyPart>Skull</bodyPart>
    <multiplier>1.5</multiplier>
  </li>
</partMultipliers>
```
- `bodyPart`: the `defName` of any `BodyPartDef` — `Head` (outer head), `Skull` (if you're running a mod that adds one), `Brain`, or anything else. It's just a plain string match, so it's safe to list a part that doesn't exist in your current mod list (it simply never matches, no error).
- `multiplier`: `1.0` = no change, `1.5` = +50% pain from injuries on that part, `2.0` = double, etc. Each part gets its own independent multiplier.

Changes take effect on next game launch (Defs are read once at startup).

## Build via GitHub Actions
1. Push this whole folder to the root of a new GitHub repo.
2. Go to **Actions** → **Build Mod** → **Run workflow** (or just push to `main`).
3. Download the `HeadPainBoost` artifact zip from the finished run — it contains `About/`, `Defs/`, and `Assemblies/`.
4. Copy those three folders into a `HeadPainBoost` folder in your RimWorld `Mods` directory:
   ```
   HeadPainBoost/
     About/About.xml
     Defs/HeadPainSettings.xml
     Assemblies/HeadPainBoost.dll
   ```
5. Enable it in-game, **loaded after Harmony**.

## Notes
- Requires the **Harmony** mod (pardeike) loaded first — same as almost every C# RimWorld mod.
- Only affects pain calculation. It doesn't change capacities, HP, or bleed rate — just how much a given injury on that part hurts.
