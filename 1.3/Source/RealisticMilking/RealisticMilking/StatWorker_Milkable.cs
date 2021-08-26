using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RealisticMilking
{
	public class StatWorker_Milkable : StatWorker
	{
        public override bool ShouldShowFor(StatRequest req)
        {
            if (req.Thing is Pawn pawn && pawn.TryGetComp<CompMilkable>() != null)
            {
                var extension = pawn.def.GetModExtension<MilkingExtension>();
                if (extension != null && extension.excludeFromRealisticMilking)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
	}

    public class StatWorker_LactationPeriod : StatWorker_Milkable
    {
        public override string ValueToString(float val, bool finalized, ToStringNumberSense numberSense = ToStringNumberSense.Absolute)
        {
            return "PeriodDays".Translate((val * 60f).ToStringDecimalIfSmall());
        }
    }
}
