using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RealisticMilking
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("RealisticMilking.Mod");
            harmony.PatchAll();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.race != null && thingDef.HasComp(typeof(CompMilkable)))
                {
                    var extension = thingDef.GetModExtension<MilkingExtension>();
                    if (extension != null && extension.excludeFromRealisticMilking)
                    {
                        continue;
                    }

                    var lactationPeriod = thingDef.race.lifeStageAges.Count > 1 ? thingDef.race.lifeStageAges[2].minAge : thingDef.race.lifeExpectancy;
                    if (!thingDef.StatBaseDefined(RM_DefOf.RM_LactationPeriod))
                    {
                        if (extension != null && extension.firstLactatingPeriodLifestageOverride.HasValue)
                        {
                            thingDef.SetStatBaseValue(RM_DefOf.RM_LactationPeriod, lactationPeriod * extension.firstLactatingPeriodLifestageOverride.Value);
                        }
                        else
                        {
                            thingDef.SetStatBaseValue(RM_DefOf.RM_LactationPeriod, lactationPeriod);
                        }
                    }

                    if (!thingDef.StatBaseDefined(RM_DefOf.RM_LactationDryingUpPeriod))
                    {
                        if (extension != null && extension.secondLactatingPeriodLifeStageOverride.HasValue)
                        {
                            thingDef.SetStatBaseValue(RM_DefOf.RM_LactationDryingUpPeriod, lactationPeriod * extension.secondLactatingPeriodLifeStageOverride.Value * 60);
                        }
                        else
                        {
                            thingDef.SetStatBaseValue(RM_DefOf.RM_LactationDryingUpPeriod, lactationPeriod * 0.33f);
                        }
                    }

                }
            }
        }

        public static Hediff GetLactatingHediff(this Pawn pawn)
        {
            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(RM_DefOf.RM_Lactating);
            if (hediff is null)
            {
                hediff = pawn.health.hediffSet.GetFirstHediffOfDef(RM_DefOf.RM_LactatingDryingUp);
            }
            return hediff;
        }
    }

    [HarmonyPatch(typeof(AnimalProductionUtility), "AnimalProductionStats")]
    public class AnimalProductionStats_Patch
    {
        private static IEnumerable<StatDrawEntry> Postfix(ThingDef d, IEnumerable<StatDrawEntry> __result)
        {
            var extension = d.GetModExtension<MilkingExtension>();
            if (extension != null && extension.excludeFromRealisticMilking)
            {
                foreach (var r in __result)
                {
                    yield return r;
                }
                yield break;
            }
            else
            {
                foreach (var stat in __result)
                {
                    var labelInt = Traverse.Create(stat).Field("labelInt").GetValue<string>();
                    if (labelInt != "Stat_Animal_MilkValuePerYear".Translate() && labelInt != "Stat_Animal_MilkPerYear".Translate())
                    {
                        yield return stat;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Hediff_Pregnant), "DoBirthSpawn")]
    public class DoBirthSpawn_Patch
    {
        private static void Postfix(Pawn mother, Pawn father)
        {
            var extension = mother.def.GetModExtension<MilkingExtension>();
            if (extension != null && extension.excludeFromRealisticMilking)
            {
                return;
            }
            var hediff = mother.GetLactatingHediff();
            if (hediff != null)
            {
                mother.health.RemoveHediff(hediff);
            }
            hediff = HediffMaker.MakeHediff(RM_DefOf.RM_Lactating, mother);
            hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = (int)((mother.GetStatValue(RM_DefOf.RM_LactationPeriod) * GenDate.DaysPerYear) * GenDate.TicksPerDay);
            mother.health.AddHediff(hediff);
        }
    }

    [HarmonyPatch(typeof(CompMilkable), "Active", MethodType.Getter)]
    public class Active_Patch
    {
        private static void Postfix(CompMilkable __instance, ref bool __result)
        {
            var extension = __instance.parent.def.GetModExtension<MilkingExtension>();
            if (extension != null && extension.excludeFromRealisticMilking)
            {
                return;
            }
            var pawn = __instance.parent as Pawn;
            __result = pawn.GetLactatingHediff() != null;
        }
    }

    [HarmonyPatch(typeof(CompMilkable), "ResourceAmount", MethodType.Getter)]
    public class ResourceAmount_Patch
    {
        private static void Postfix(CompMilkable __instance, ref int __result)
        {
            __result = (int)(__result *__instance.parent.GetStatValue(RM_DefOf.RM_MilkingYieldFactor));
        }
    }
}
