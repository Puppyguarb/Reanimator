using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Reanimator {

    [HarmonyPatch(typeof(Mission), "OnAgentRemoved")]
    static class BattlePatch {
        public static void Postfix(
      Mission __instance,
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow) {
            

            try {

                //Check if the agent that just got removed is actually dead, and human.
                if (affectedAgent == null || (MapEvent.PlayerMapEvent == null || affectedAgent.IsMainAgent || affectedAgent.IsMount || affectedAgent.State != AgentState.Killed)) {
                    return;
                }
                
                //Don't allow res of ally troops.
                if (affectedAgent.Team.IsPlayerAlly && !affectedAgent.Team.IsPlayerTeam) {
                    return;
                }

                //Don't allow troops that have been killed by allies. Death by player troops is ok, but not another lord's for example.
                if (affectorAgent != null) {
                    if (affectorAgent.Team.IsPlayerAlly && !affectorAgent.Team.IsPlayerTeam && !affectorAgent.IsMainAgent) {
                        return;
                    }
                }

                //Don't allow res of the undead. That wouldn't make any sense.
                if (affectedAgent.Monster.StringId.Equals("undead")) {
                    return;
                }

                Main.IncrementUndeadTier((CharacterObject)affectedAgent.Character);
                
            } catch (Exception ex) {
                //Prints the error into the general chat log.
                Main.PrintError(ex);
            }
        }
    }
}
