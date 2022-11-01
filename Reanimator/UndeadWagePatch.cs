using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Reanimator {
    [HarmonyPatch(typeof(DefaultPartyWageModel), "GetCharacterWage")]
    static class UndeadWagePatch {
        public static int Postfix(int originalwage, CharacterObject character) {
            //If the character in question is undead, they don't cost any wages. Otherwise, run it as normal.
            if (character.Race != Main.undeadRaceValue) {
                return originalwage;
            } else {
                return 0;
            }
        }
    }
}
