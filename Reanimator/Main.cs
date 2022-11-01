using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using HarmonyLib;
using TaleWorlds.ObjectSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem;

namespace Reanimator {
    public class Main : MBSubModuleBase {
        //Needs to dynamically change w/ load order for compatibility purposes.
        public static int undeadRaceValue = 1;
        //The id of each tier of undead.
        public static string t1Undead = "undead_basic", t2Undead = "undead_basic2", t3Undead = "undead_footman", 
            t4Undead = "undead_infantry", t5Undead = "undead_veteran_infantry", t6Undead = "undead_elite_infantry";

        //Needs to change whenever the player loads the game or alters the number of troops in the party.
        public static int numOfUndeadInParty = 0;
        private Harmony _harmony = new Harmony("Reanimator");

        //The current valid resurrectable count. Gets set to 0 after battles and at game load.
        public static int[] resArray = new int[6];
        //An array used to hold enemy troops that die in simulation, for use with the player's battle contribution afterward.
        public static int[] simResArray = new int[6];
        //The troop to be used for resurrections.
        public static BasicCharacterObject resTroop;
        //Whether or not a battle was simulated, which will require battle contribution calculations.
        public static bool isSimulated = false;

        /// <summary>
        /// Prints an error message into the chatlog.
        /// </summary>
        /// <param name="ex">The exception to have its stacktrace printed.</param>
        public static void PrintError(Exception ex) {
            InformationManager.DisplayMessage(new InformationMessage("REANIMATOR ERROR: " + ex.Message + ex.StackTrace));
        }

        public static void PrintMessage(string msg) {
            InformationManager.DisplayMessage(new InformationMessage(msg, Colors.White));
        }
        /// <summary>
        /// Updates the number of undead in party variable.
        /// </summary>
        internal static void RecalculateUndeadCount() {
            if (MobileParty.MainParty == null) {
                PrintMessage("REANIMATOR ERROR: Undead recalculation called when main party did not exist.");
                return;
            }
            TroopRoster roster = MobileParty.MainParty.MemberRoster;
            int num = 0;
            for(int i = 0; i < roster.Count; i++) {
                if (roster.GetCharacterAtIndex(i).Race == undeadRaceValue) {
                    num+=roster.GetTroopCount(roster.GetCharacterAtIndex(i));
                }
            }
            numOfUndeadInParty = num;
        }

        //Overload for shorthand to not bother with the simulated array.
        internal static void IncrementUndeadTier(CharacterObject deadTroop) {
            IncrementUndeadTier(deadTroop, Main.resArray);
        }

        internal static void IncrementUndeadTier(CharacterObject deadTroop, int[] array) {
            int tier = Campaign.Current.Models.CharacterStatsModel.GetTier(deadTroop);
            //Ranged troops have inferior equipment and count as one tier less (to a minimum of 0) for reanimation purposes.
            if (deadTroop.IsRanged) {
                if (tier != 0) {
                    tier--;
                }
            }

            switch (tier) {
                case 0:
                    array[0]++;
                    break;
                case 1:
                    array[1]++;
                    break;
                case 2:
                    array[2]++;
                    break;
                case 3:
                    array[3]++;
                    break;
                case 4:
                    array[4]++;
                    break;
                case 5:
                    array[5]++;
                    break;
                case 6:
                    array[5]++;
                    break;
                default:
                    array[5]++;
                    break;
            }
        }
        /// <summary>
        /// Wipes the undead arrays, usually called after a battle or when a player exits to main menu.
        /// </summary>
        public static void ClearUndeadArrays() {
            Main.simResArray = new int[6];
            Main.resArray = new int[6];
        }

        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();

            _harmony.PatchAll();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot() {
            //Keeps the player from smuggling undead from one battle to another.
            ClearUndeadArrays();
        }
    }
}
