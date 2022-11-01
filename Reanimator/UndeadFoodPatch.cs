using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace Reanimator {
    [HarmonyPatch(typeof(DefaultMobilePartyFoodConsumptionModel), "CalculateDailyBaseFoodConsumptionf")]
    static class UndeadFoodPatch {
        public static ExplainedNumber Postfix(ExplainedNumber originalResult, MobileParty party,
      bool includeDescription = false) {
            //This method is called constantly for every party in the game.
            //Who cares if the AI has to pay food costs for undead, not our problem!
            if (!party.IsMainParty) {
                return originalResult;
            }
            
            try {
                Main.RecalculateUndeadCount();

                if (Main.numOfUndeadInParty == 0) {
                    return originalResult;
                } else {
                    int numOfMenForOneFood = Campaign.Current.Models.MobilePartyFoodConsumptionModel.NumberOfMenOnMapToEatOneFood;
                    originalResult.Add(((float)Main.numOfUndeadInParty/ numOfMenForOneFood),new TaleWorlds.Localization.TextObject("Undead don't eat"));
                }
            } catch (Exception ex) {
                Main.PrintError(ex);
            }

            return originalResult;

        }
    }
}
