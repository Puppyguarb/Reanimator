using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.MapEvents;

namespace Reanimator {
    [HarmonyPatch(typeof(MapEventSide), "HandleMapEventEnd")]
    static class MapEventEndPatch {
        public static void Postfix(ref MapEventSide __instance) {
            if (!__instance.IsMainPartyAmongParties() && !__instance.OtherSide.IsMainPartyAmongParties())
                return;
            //Clears the saved undead arrays afterward, in the event the player loses.
            Main.ClearUndeadArrays();
        }
    }
}
