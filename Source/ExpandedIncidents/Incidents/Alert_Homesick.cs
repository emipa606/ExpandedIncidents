using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Reflection;

namespace ExpandedIncidents
{
    public class Alert_Homesick : Alert_Thought
    {
        public Alert_Homesick()
        {
            this.explanationKey = "HomesickDesc".Translate();
        }
        
        public override string GetLabel()
        {
            if (this.GetReport().AllCulprits.Count() == 1)
            {
                return "ColonistHomesickAlert".Translate();
            }
            return "ColonistsHomesickAlert".Translate();
        }

        protected override ThoughtDef Thought
        {
            get
            {
                return ThoughtDefOfIncidents.Homesickness;
            }
        }

    }
}
