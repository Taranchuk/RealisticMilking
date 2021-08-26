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
    [DefOf]
    public static class RM_DefOf
    {
        public static StatDef RM_MilkingYieldFactor;
        public static StatDef RM_LactationPeriod;
        public static StatDef RM_LactationDryingUpPeriod;
        public static HediffDef RM_Lactating;
        public static HediffDef RM_LactatingDryingUp;
    }
}
