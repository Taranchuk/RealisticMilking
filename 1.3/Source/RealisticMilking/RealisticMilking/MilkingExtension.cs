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
    public class MilkingExtension : DefModExtension
    {
        public bool excludeFromRealisticMilking;
        public float? firstLactatingPeriodLifestageOverride;
        public float? secondLactatingPeriodLifeStageOverride;
    }
}
