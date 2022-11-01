using HarmonyLib;
using SandBox.GameComponents;
using StoryMode.GameComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Reanimator {
    [HarmonyPatch(typeof(SandboxAgentStatCalculateModel), "GetEffectiveMaxHealth")]
    static class UndeadHealthPatch {
        
        public static void Postfix(ref float __result,
      Agent agent) {
            try {
                //Don't patch the health for horses and stuff, only undead.
                if (!agent.Monster.StringId.Equals("undead")) return;

                //Base hp is 100 for humans. So just take off 100, and add in the monster's written HP.
                //For undead this is 200, so this just sets them to 200 hp, plus any small changes from perks.
                float fixedHealth = __result + (agent.Monster.HitPoints - 100);
                if (fixedHealth <= 0) {
                    Main.PrintMessage("REANIMATOR ERROR: Check monster's health values, was less than or equal to zero.");
                    return;
                }
                __result = fixedHealth;
                return;
            } catch (Exception ex) {
                Main.PrintError(ex);
            }
        }
    }
}
