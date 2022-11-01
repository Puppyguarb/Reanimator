using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace Reanimator {
    [HarmonyPatch(typeof(MapEventSide), "OnTroopKilled")]
    static class SimulatedBattlePatch {
        public static void Postfix(
            ref MapEventSide __instance,
            ref CharacterObject ____selectedSimulationTroop,
            ref Dictionary<UniqueTroopDescriptor, MapEventParty> ____allocatedTroops,
            UniqueTroopDescriptor troopDesc1) {

            //Null check!
            if (PlayerEncounter.CurrentBattleSimulation == null || PartyBase.MainParty == null || 
                (____selectedSimulationTroop == null || ____allocatedTroops == null || 
                PlayerEncounter.CurrentBattleSimulation.MapEvent == null || 
                __instance.MapEvent != PlayerEncounter.CurrentBattleSimulation.MapEvent)) {
                return;
            }
            
            bool onPlayerSide = __instance.IsMainPartyAmongParties();
            bool onPlayerTeam = ____allocatedTroops[troopDesc1].Party == PartyBase.MainParty;
            if (!Main.isSimulated) {
                Main.isSimulated = true;
            }

            //Add player troops' deaths as normal, they get to res all of their own dead.
            if (onPlayerSide && onPlayerTeam) {
                Main.IncrementUndeadTier(____selectedSimulationTroop);
            }

            //Slain enemies are based on battle contribution.
            if (!onPlayerSide && !onPlayerTeam) {
                Main.IncrementUndeadTier(____selectedSimulationTroop,Main.simResArray);
            }
        }
    }
}
