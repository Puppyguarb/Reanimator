using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace Reanimator {

    /// <summary>
    /// Intercepts the method that counts the enemy party's prisoners, and adds undead.
    /// </summary>
    [HarmonyPatch(typeof(MapEvent), "GetMemberRosterReceivingLootShare")]
    static class GiveUndeadPatch {
        public static void Postfix(ref TroopRoster __result) {
            try {
                for (int i = 0; i < Main.resArray.Length; i++) {
                    string troopToGive;
                    switch (i) {
                        case 0:
                            troopToGive = Main.t1Undead;
                            break;
                        case 1:
                            troopToGive = Main.t1Undead;
                            break;
                        case 2:
                            troopToGive = Main.t2Undead;
                            break;
                        case 3:
                            troopToGive = Main.t3Undead;
                            break;
                        case 4:
                            troopToGive = Main.t4Undead;
                            break;
                        case 5:
                            troopToGive = Main.t5Undead;
                            break;
                        case 6:
                            troopToGive = Main.t5Undead;
                            break;
                        case 7:
                            troopToGive = Main.t5Undead;
                            break;
                        default:
                            troopToGive = Main.t1Undead;
                            break;
                    }

                    int count = 0;
                    if (!Main.isSimulated) {
                        count = Main.resArray[i];
                    } else {
                        float contributedNum = (float)Main.simResArray[i] * MobileParty.MainParty.MapEventSide.GetPlayerPartyContributionRate();

                        //If the player would get less than a single troop, they get at least one.
                        //Technically, the player always has a very very very very small contribution, so make sure that
                        //they have to do at least a little bit to get troops.
                        if (contributedNum > 0.05f && contributedNum < 1.0f) {
                            contributedNum = 1.0f;
                        }
                        
                        count = Main.resArray[i] + MathF.Round(contributedNum);
                        Main.simResArray[i] = 0;
                    }

                    //If the total count is zero, don't bother trying to add the troop.
                    if (count != 0) {
                        CharacterObject troop = (CharacterObject)MBObjectManager.Instance.GetObject<BasicCharacterObject>(troopToGive);

                        TroopRosterElement rosterElement = new TroopRosterElement(troop);
                        rosterElement.Number = count;

                        __result.Add(rosterElement);
                    }
                    Main.resArray[i] = 0;

                }
                if (Main.isSimulated) {
                    Main.isSimulated = false;
                }
            } catch (Exception ex) {
                Main.PrintError(ex);
            }
        }
    }

    /*
     * Didn't always fire off, when a party was defeated with no wounded or prisoners, no undead were given to the player.
    [HarmonyPatch(typeof(PartyScreenManager), "OpenScreenAsLoot")]
    internal static class LootTroopsPatch {
        
        public static void Prefix(TroopRoster leftMemberRoster,
            TroopRoster leftPrisonerRoster,
            TextObject leftPartyName,
            int leftPartySizeLimit,
            PartyScreenClosedDelegate partyScreenClosedDelegate = null) {
            
            try {
                if (Main.resCounter == 0) {
                    Main.PrintMessage("There were no bodies available to resurrect.");
                    return;
                } else if (Main.resCounter == 1) {
                    Main.PrintMessage("Resurrected a single corpse.");
                } else {
                    Main.PrintMessage("Resurrected " + Main.resCounter + " corpses.");
                }



                CharacterObject troop = (CharacterObject)MBObjectManager.Instance.GetObject<BasicCharacterObject>("undead_basic");//Main.resTroop;
                
                TroopRosterElement rosterElement = new TroopRosterElement(troop);

                rosterElement.Number = Main.resCounter;
                Main.resCounter = 0;

                leftMemberRoster.Add(rosterElement);
                
                
            } catch (Exception ex) {
                Main.PrintError(ex);
            }
        }
    }*/
}

