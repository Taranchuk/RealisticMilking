using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RealisticMilking
{
	public class Hediff_Lactating : HediffWithComps
	{
        public override void PostRemoved()
        {
            base.PostRemoved();
            var hediff = HediffMaker.MakeHediff(RM_DefOf.RM_LactatingDryingUp, this.pawn);
            hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = (int)((this.pawn.GetStatValue(RM_DefOf.RM_LactationDryingUpPeriod) * GenDate.DaysPerYear) * GenDate.TicksPerDay);
            this.pawn.health.AddHediff(hediff);
        }
    }
}
