using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AddonWow.API; //needed to access Addon API

namespace AddonWow.Modules {
  /// <summary>
  /// Created by JS
  /// </summary>
  public class JSArmsWarrior: Rotation {

    public override void LoadSettings() {
      List < string > MajorAzeritePower = new List < string > (new string[] {
        "Guardian of Azeroth",
        "Focused Azerite Beam",
        "Concentrated Flame",
        "Worldvein Resonance",
        "Memory of Lucid Dreams",
        "Blood of the Enemy",
        "The Unbound Force",
        "Reaping Flames",
        "None"
      });
      Settings.Add(new Setting("Major Power", MajorAzeritePower, "None"));

      List < string > Trinkets = new List < string > (new string[] {
        "Azshara's Font of Power",
        "Ashvane's Razor Coral",
        "Pocket-Sized Computation Device",
        "Galecaller's Boon",
        "Shiver Venom Relic",
        "Lurker's Insidious Gift",
        "Notorious Gladiator's Badge",
        "Sinister Gladiator's Badge",
        "Sinister Gladiator's Medallion",
        "Notorious Gladiator's Medallion",
        "Vial of Animated Blood",
        "First Mate's Spyglass",
        "Jes' Howler",
        "Ashvane's Razor Coral",
        "Generic",
        "None"
      });
      Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
      Settings.Add(new Setting("Top Trinket AOE?", false));
      Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));
      Settings.Add(new Setting("Bot Trinket AOE?", false));
      Settings.Add(new Setting("Use item: Case Sens", "None"));
      Settings.Add(new Setting("Use item @ HP%", 0, 100, 100));
      List < string > Race = new List < string > (new string[] {
        "Orc",
        "Troll",
        "Dark Iron Dwarf",
        "Mag'har Orc",
        "Lightforged Draenei",
        "None"
      });
      Settings.Add(new Setting("Racial Power", Race, "Orc"));
      Settings.Add(new Setting("Test of Might Trait?", true));
      Settings.Add(new Setting("Mitigation"));
      Settings.Add(new Setting("Auto Victory Rush @ HP%", 0, 100, 100));
      Settings.Add(new Setting("Auto Shout @ HP%", 0, 100, 75));
      Settings.Add(new Setting("Auto Die by the Sword @ HP%", 0, 100, 50));
      Settings.Add(new Setting("Auto Stance @ HP%", 0, 100, 30));
      Settings.Add(new Setting("Unstance @ HP%", 0, 100, 80));
    }

    string MajorPower;
    string TopTrinket;
    string BotTrinket;
    string RacialPower;
    string usableitems;

    public override void Initialize() {
      //Addon.DebugMode();

      Addon.PrintMessage("JS Series: Arms Warrior - v 2.2", Color.Yellow);
      Addon.PrintMessage("Recommended PVE talents: 3323311", Color.Yellow);
      Addon.PrintMessage("Recommended PVP talents: 2333211", Color.Yellow);
      Addon.PrintMessage("Default mode: PVE, AOE ON, USE CDs/Pots", Color.Yellow);
      Addon.PrintMessage("These macros can be used for manual control:", Color.Yellow);
      Addon.PrintMessage("/xxxxx Potions --Toggles using buff potions on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx SaveCooldowns --Toggles the use of big cooldowns on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx noaoe --Toggles to turn off PVE AOE on/off.", Color.Orange);
      Addon.PrintMessage("/xxxxx savepp -- Toggles the use of prepull", Color.Orange);
      Addon.PrintMessage("/xxxxx pvp --Toggles the PVP rotation on/off.", Color.Red);

      Addon.Latency = 100;
      Addon.QuickDelay = 125;

      MajorPower = GetDropDown("Major Power");
      TopTrinket = GetDropDown("Top Trinket");
      BotTrinket = GetDropDown("Bot Trinket");
      RacialPower = GetDropDown("Racial Power");
      usableitems = GetString("Use item: Case Sens");

      Spellbook.Add(MajorPower);

      if (RacialPower == "Orc") Spellbook.Add("Blood Fury");
      if (RacialPower == "Troll") Spellbook.Add("Berserking");
      if (RacialPower == "Dark Iron Dwarf") Spellbook.Add("Fireblood");
      if (RacialPower == "Mag'har Orc") Spellbook.Add("Ancestral Call");
      if (RacialPower == "Lightforged Draenei") Spellbook.Add("Light's Judgment");

      Spellbook.Add("Charge");
      Spellbook.Add("Skullsplitter");
      Spellbook.Add("Colossus Smash");
      Spellbook.Add("Bladestorm");
      Spellbook.Add("Overpower");
      Spellbook.Add("Execute");
      Spellbook.Add("Mortal Strike");
      Spellbook.Add("Whirlwind");
      Spellbook.Add("Cleave");
      Spellbook.Add("Sweeping Strikes");
      Spellbook.Add("Overpower");
      Spellbook.Add("Victory Rush");
      Spellbook.Add("Die by the Sword");
      Spellbook.Add("Rend");
      Spellbook.Add("Avatar");
      Spellbook.Add("Warbreaker");
      Spellbook.Add("Battle Shout");
      Spellbook.Add("Sharpen Blade");
      Spellbook.Add("Slam");
      Spellbook.Add("Heroic Throw");
      Spellbook.Add("Hamstring");
      Spellbook.Add("Rallying Cry");
      Spellbook.Add("Defensive Stance");

      Buffs.Add("Bloodlust");
      Buffs.Add("Heroism");
      Buffs.Add("Time Warp");
      Buffs.Add("Ancient Hysteria");
      Buffs.Add("Netherwinds");
      Buffs.Add("Drums of Rage");
      Buffs.Add("Lifeblood");
      Buffs.Add("Memory of Lucid Dreams");
      Buffs.Add("Reckless Force");
      Buffs.Add("Guardian of Azeroth");

      Buffs.Add("Berserker Rage");
      Buffs.Add("Meat Cleaver");
      Buffs.Add("Enrage");
      Buffs.Add("Furious Slash");
      Buffs.Add("Whirlwind");
      Buffs.Add("Test of Might");
      Buffs.Add("Avatar");
      Buffs.Add("Sharpen Blade");
      Buffs.Add("Battle Shout");
      Buffs.Add("Overpower");
      Buffs.Add("Bladestorm");
      Buffs.Add("Defensive Stance");
      Buffs.Add("Sweeping Strikes");

      Debuffs.Add("Razor Coral");
      Debuffs.Add("Conductive Ink");
      Debuffs.Add("Shiver Venom");
      Debuffs.Add("Siegebreaker");
      Debuffs.Add("Colossus Smash");
      Debuffs.Add("Rend");
      Debuffs.Add("Hamstring");
      Debuffs.Add("Deep Wounds");

      Items.Add(TopTrinket);
      Items.Add(BotTrinket);
      Items.Add(usableitems);

      Macros.Add("ItemUse", "/use " + usableitems);
      Macros.Add("TopTrink", "/use 13");
      Macros.Add("BotTrink", "/use 14");

      CustomCommands.Add("Potions");
      CustomCommands.Add("SaveCooldowns");
      CustomCommands.Add("noaoe");
      CustomCommands.Add("pvp");
      CustomCommands.Add("savepp");
    }

    // optional override for the CombatTick which executes while in combat
    public override bool CombatTick() {

      bool Fighting = Addon.Range("target") <= 8 && Addon.TargetIsEnemy();
      bool Moving = Addon.PlayerIsMoving();
      float Haste = Addon.Haste() / 100f;
      int Time = Addon.CombatTime();
      int Range = Addon.Range("target");
      int TargetHealth = Addon.Health("target");
      int PlayerHealth = Addon.Health("player");
      string LastCast = Addon.LastCast();
      bool IsChanneling = Addon.IsChanneling("player");

      int EnemiesInMelee = Addon.EnemiesInMelee();
      int GCDMAX = (int)(1500f / (Haste + 1f));
      int GCD = Addon.GCD();
      int Latency = Addon.Latency;
      bool HasLust = Addon.HasBuff("Bloodlust", "player", false) || Addon.HasBuff("Heroism", "player", false) || Addon.HasBuff("Time Warp", "player", false) || Addon.HasBuff("Ancient Hysteria", "player", false) || Addon.HasBuff("Netherwinds", "player", false) || Addon.HasBuff("Drums of Rage", "player", false);
      int FlameFullRecharge = (int)(Addon.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Addon.SpellCharges("Concentrated Flame")));
      int ShiverVenomStacks = Addon.DebuffStacks("Shiver Venom");

      int CDGuardianOfAzerothRemains = Addon.SpellCooldown("Guardian of Azeroth") - GCD;
      bool BuffGuardianOfAzerothUp = Addon.HasBuff("Guardian of Azeroth");
      int CDBloodOfTheEnemyRemains = Addon.SpellCooldown("Blood of the Enemy") - GCD;
      int BuffMemoryOfLucidDreamsRemains = Addon.BuffRemaining("Memory of Lucid Dreams") - GCD;
      bool BuffMemoryOfLucidDreamsUp = BuffMemoryOfLucidDreamsRemains > 0;
      bool DebuffRazorCoralUp = Addon.HasDebuff("Razor Coral");
      bool DebuffConductiveInkUp = Addon.HasDebuff("Conductive Ink");

      int Rage = Addon.Power("player");

      // Commands
      bool UsePotion = Addon.IsCustomCodeOn("Potions");
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");
      bool OffAOE = Addon.IsCustomCodeOn("noaoe");
      bool PVPmode = Addon.IsCustomCodeOn("pvp");

      // CDS
      int ColossusSmashRemains = Addon.DebuffRemaining("Colossus Smash") - GCD;
      bool DeBuffColossusSmashUp = ColossusSmashRemains > 0;
      int RendRemains = Addon.DebuffRemaining("Rend") - GCD;
      bool DeBuffRendUp = RendRemains > 0;
      bool DebuffHamstringUp = Addon.DebuffRemaining("Hamstring") - GCD > 0;
      int BerserkerRageRemains = Addon.BuffRemaining("Berserker Rage");
      bool BuffBerserkerRageUp = BerserkerRageRemains > 0;

      int BuffWhirlwindRemains = Addon.BuffRemaining("Whirlwind") - GCD;
      bool BuffWhirlwindUp = BuffWhirlwindRemains > 0;

      //Talents
      bool TalentDoubleTime = Addon.Talent(2, 1);
      bool TalentStormbolt = Addon.Talent(2, 3);
      bool TalentMassacre = Addon.Talent(3, 1);
      bool TalentFervorOfBattle = Addon.Talent(3, 2);
      bool TalentCollateralDamage = Addon.Talent(5, 1);
      bool TalentWarbreaker = Addon.Talent(5, 2);
      bool TalentCleave = Addon.Talent(5, 3);
      bool TalentAvatar = Addon.Talent(6, 2);

      //Buffs
      bool TestOfMightUp = Addon.HasBuff("Test of Might", "player");
      bool BladestormUp = Addon.HasBuff("Bladestorm", "player");
      bool BattleShoutUp = Addon.HasBuff("Battle Shout", "player");
      bool BuffSharpenBladeUp = Addon.HasBuff("Sharpen Blade", "player");
      bool BuffOverpower = Addon.HasBuff("Overpower", "player");
      int BuffStacksOP = Addon.BuffStacks("Overpower", "player");
      bool BuffCrushingAssault = Addon.HasBuff("Crushing Assault", "player");

      int BuffTestOfMightRemains = Addon.BuffRemaining("Test of Might", "player") - GCD;
      bool BuffTestOfMightReady = BuffTestOfMightRemains <= 800 && BuffTestOfMightRemains >= 500;

      //Debuffs
      bool DebuffDeepWoundsUp = Addon.HasDebuff("Deep Wounds", "target");

      // Options
      bool TopTrinketAOE = GetCheckBox("Top Trinket AOE?");
      bool BotTrinketAOE = GetCheckBox("Bot Trinket AOE?");
      bool TestOfMightTrait = GetCheckBox("Test of Might Trait?");

      if (OffAOE) {
        EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
      }

      if (IsChanneling) return false;

      if (Addon.CanCast("Battle Shout", "player")) {
        if (!BattleShoutUp) {
          Addon.Cast("Battle Shout");
          return true;
        }
      }

      if (MajorPower == "Concentrated Flame") {
        if (Addon.CanCast("Concentrated Flame") && FlameFullRecharge < GCDMAX) {
          if (!BuffBerserkerRageUp) {
            Addon.Cast("Concentrated Flame");
            return true;
          }
        }
      }

      if (MajorPower == "Reaping Flames") {
        if (Addon.CanCast("Reaping Flames")) {
          if (!BuffBerserkerRageUp) {
            Addon.Cast("Reaping Flames");
            return true;
          }
        }
      }

      if (!NoCooldowns && Fighting) {
        if (Addon.CanUseTrinket(0) && TopTrinket == "Generic") {
          if (!TopTrinketAOE) {
            Addon.Cast("TopTrink", true);
            return true;
          } else if (EnemiesInMelee >= 1) {
            Addon.Cast("TopTrink", true);
            return true;
          }
        }

        if (Addon.CanUseTrinket(1) && BotTrinket == "Generic") {
          if (!BotTrinketAOE) {
            Addon.Cast("BotTrink", true);
            return true;
          } else if (EnemiesInMelee >= 1) {
            Addon.Cast("BotTrink", true);
            return true;
          }
        }

        if (Addon.CanUseItem(usableitems) && usableitems != "None" && !UsePotion) {
          if (EnemiesInMelee >= 1 && PlayerHealth <= GetSlider("Use item @ HP%")) {
            Addon.Cast("ItemUse", true);
            return true;
          }
        }

        //actions+=/blood_of_the_enemy,if=buff.recklessness.up
        if (MajorPower == "Blood of the Enemy" && EnemiesInMelee > 0) {
          if (Addon.CanCast("Blood of the Enemy", "player")) {
            Addon.Cast("Blood of the Enemy");
            return true;
          }
        }

        //actions+=/worldvein_resonance,if=!buff.recklessness.up&!buff.siegebreaker.up
        if (MajorPower == "Worldvein Resonance") {
          if (Addon.CanCast("Worldvein Resonance", "player")) {
            if (!BuffBerserkerRageUp) {
              Addon.Cast("Worldvein Resonance");
              return true;
            }
          }
        }

        //actions+=/focused_azerite_beam,if=!buff.recklessness.up&!buff.siegebreaker.up
        if (MajorPower == "Focused Azerite Beam") {
          if (Addon.CanCast("Focused Azerite Beam", "player")) {
            if (!BuffBerserkerRageUp || Rage < 70) {
              Addon.Cast("Focused Azerite Beam");
              return true;
            }
          }
        }

        //actions +=/ the_unbound_force,if= buff.reckless_force.up
        if (MajorPower == "The Unbound Force") {
          if (Addon.CanCast("The Unbound Force")) {
            Addon.Cast("The Unbound Force");
            return true;
          }
        }

        //actions+=/guardian_of_azeroth,if=!buff.recklessness.up
        if (MajorPower == "Guardian of Azeroth") {
          if (Addon.CanCast("Guardian of Azeroth", "player")) {
            if (!BuffBerserkerRageUp) {
              Addon.Cast("Guardian of Azeroth");
              return true;
            }
          }
        }

        //actions+=/memory_of_lucid_dreams,if=!buff.recklessness.up
        if (MajorPower == "Memory of Lucid Dreams") {
          if (Addon.CanCast("Memory of Lucid Dreams", "player")) {
            if (!BuffBerserkerRageUp && TestOfMightUp) {
              Addon.Cast("Memory of Lucid Dreams");
              return true;
            }
          }
        }

        //actions+=/recklessness,if=!essence.condensed_lifeforce.major&!essence.blood_of_the_enemy.major|cooldown.guardian_of_azeroth.remains>20|buff.guardian_of_azeroth.up|cooldown.blood_of_the_enemy.remains<gcd
        if (Addon.CanCast("Recklessness", "player")) {
          if (MajorPower != "Guardian of Azeroth" && MajorPower != "Blood of the Enemy" || CDGuardianOfAzerothRemains > 20000 || BuffGuardianOfAzerothUp || CDBloodOfTheEnemyRemains < GCDMAX) {
            Addon.Cast("Recklessness");
            return true;
          }
        }

        if (TalentAvatar) {
          if (Addon.CanCast("Avatar", "player")) {
            Addon.Cast("Avatar");
            return true;
          }
        }

        //actions +=/ use_item,name = ashvanes_razor_coral,if= !debuff.razor_coral_debuff.up | (target.health.pct < 30.1 & debuff.conductive_ink_debuff.up) | (!debuff.conductive_ink_debuff.up & buff.memory_of_lucid_dreams.up | prev_gcd.2.guardian_of_azeroth | prev_gcd.2.recklessness & (!essence.memory_of_lucid_dreams.major & !essence.condensed_lifeforce.major))
        if (Addon.CanUseItem("Ashvane's Razor Coral")) {
          if (!DebuffRazorCoralUp || (TargetHealth <= 30 && DebuffConductiveInkUp) || (!DebuffConductiveInkUp && BuffMemoryOfLucidDreamsUp || CDGuardianOfAzerothRemains > 180000 - GCDMAX * 2 || (MajorPower != "Memory of Lucid Dreams" && MajorPower != "Guardian of Azeroth"))) {
            Addon.Cast("Ashvane's Razor Coral", true);
            return true;
          }
        }

        //actions +=/ blood_fury
        if (RacialPower == "Orc") {
          if (Addon.CanCast("Blood Fury", "player")) {
            Addon.Cast("Blood Fury", true);
            return true;
          }
        }

        //actions+=/berserkin
        if (RacialPower == "Troll") {
          if (Addon.CanCast("Berserking", "player")) {
            Addon.Cast("Berserking", true);
            return true;
          }
        }

        //actions+=/lights_judgment,if=buff.recklessness.down
        if (RacialPower == "Lightforged Draenei") {
          if (Addon.CanCast("Light's Judgment", "player")) {
            Addon.Cast("Light's Judgment", true);
            return true;
          }
        }

        //actions+=/fireblood
        if (RacialPower == "Dark Iron Dwarf") {
          if (Addon.CanCast("Fireblood", "player")) {
            Addon.Cast("Fireblood", true);
            return true;
          }
        }

        //actions+=/ancestral_call
        if (RacialPower == "Mag'har Orc") {
          if (Addon.CanCast("Ancestral Call", "player")) {
            Addon.Cast("Ancestral Call", true);
            return true;
          }
        }
      }

      // Utility
      if (Fighting) {
        // Auto Victory Rush
        if (Addon.CanCast("Victory Rush")) {
          if (PlayerHealth <= GetSlider("Auto Victory Rush @ HP%")) {
            Addon.Cast("Victory Rush");
            return true;
          }
        }

        // Auto Commanding Shout
        if (Addon.CanCast("Rallying Cry", "player")) {
          if (PlayerHealth <= GetSlider("Auto Shout @ HP%")) {
            Addon.Cast("Rallying Cry");
            return true;
          }
        }

        // Auto Defensive Stance
        if (!Addon.HasBuff("Defensive Stance", "player") && Addon.CanCast("Defensive Stance", "player")) {
          if (PlayerHealth <= GetSlider("Auto Stance @ HP%")) {
            Addon.Cast("Defensive Stance");
            return true;
          }
        }

        if (Addon.HasBuff("Defensive Stance", "player") && Addon.CanCast("Defensive Stance", "player")) {
          if (PlayerHealth >= GetSlider("Unstance @ HP%")) {
            Addon.Cast("Defensive Stance");
            return true;
          }
        }

        if (Addon.CanCast("Die by the Sword", "player")) {
          if (PlayerHealth <= GetSlider("Auto Die by the Sword @ HP%")) {
            Addon.Cast("Die by the Sword");
            return true;
          }
        }
      }

      // PVE Rotation
      if (!PVPmode) {

        if (Fighting) {
          if (Addon.CanCast("Rend")) {
            if (!DeBuffRendUp && Rage >= 30) {
              Addon.Cast("Rend");
              return true;
            }
          }

          // 1 
          if (EnemiesInMelee <= 1) {
            if (!TalentWarbreaker) {
              if (Addon.CanCast("Colossus Smash") && !NoCooldowns && TargetHealth >= 5) {
                Addon.Cast("Colossus Smash");
                return true;
              }
            } else {
              if (Addon.CanCast("Warbreaker", "player")) {
                Addon.Cast("Warbreaker");
                return true;
              }
            }

            if (!TestOfMightUp) {
              if (Addon.CanCast("Overpower")) {
                Addon.Cast("Overpower");
                return true;
              }
            }

            if (Addon.CanCast("Mortal Strike")) {
              Addon.Cast("Mortal Strike");
              return true;
            }

            if (TestOfMightTrait) {
              if (Addon.CanCast("Bladestorm", "player")) {
                if (TestOfMightUp && Rage < 30 && !DeBuffColossusSmashUp) {
                  Addon.Cast("Bladestorm");
                  return true;
                }
              }
            } else {
              if (Addon.CanCast("Bladestorm", "player")) {
                if (DeBuffColossusSmashUp && DebuffDeepWoundsUp && Rage < 30 && !NoCooldowns) {
                  Addon.Cast("Bladestorm");
                  return true;
                }
              }
            }

            if (TalentFervorOfBattle) {
              if (Addon.CanCast("Whirlwind", "player")) {
                if (Rage >= 60) {
                  Addon.Cast("Whirlwind");
                  return true;
                }
              }
            }
            else {
              if (Addon.CanCast("Slam")) {
                if (Rage >= 60 || BuffCrushingAssault) {
                  Addon.Cast("Slam");
                  return true;
                }
              }
            }
          }

          if (EnemiesInMelee >= 2) {
            // 2-3 
            if (EnemiesInMelee >= 2 && EnemiesInMelee <= 3) {
              if (Addon.HasBuff("Sweeping Strikes", "player") && Addon.CanCast("Slam")) {
                if (!Addon.CanCast("Overpower") && !Addon.CanCast("Whirlwind") && !Addon.CanCast("Mortal Strike") && !Addon.CanCast("Bladestorm") && !Addon.CanCast("Colossus Smash") && !Addon.CanCast("Skullsplitter") && !Addon.CanCast("Execute") && BuffCrushingAssault && Rage >= 35) {
                  Addon.Cast("Slam");
                  return true;
                }
              }

              if (Addon.CanCast("Mortal Strike") && !Addon.CanCast("Overpower")) {
                Addon.Cast("Mortal Strike");
                return true;
              }
            }

            //4+
            if (Addon.HasBuff("Sweeping Strikes", "player") && Addon.CanCast("Execute")) {
              Addon.Cast("Execute");
              return true;
            }
            if (Addon.CanCast("Sweeping Strikes", "player")) {
              Addon.Cast("Sweeping Strikes");
              return true;
            }

            if (Addon.CanCast("Cleave", "player")) {
              if (TalentCleave) {
                Addon.Cast("Cleave");
                return true;
              }
            }

            if (!TestOfMightUp) {
              if (Addon.CanCast("Overpower") && BuffStacksOP < 3) {
                Addon.Cast("Overpower");
                return true;
              }
            }

            if (!TalentWarbreaker) {
              if (Addon.CanCast("Colossus Smash") && !NoCooldowns) {
                Addon.Cast("Colossus Smash");
                return true;
              }
            } else {
              if (Addon.CanCast("Warbreaker", "player")) {
                Addon.Cast("Warbreaker");
                return true;
              }
            }

            if (TestOfMightTrait) {
              if (Addon.CanCast("Bladestorm", "player")) {
                if (TestOfMightUp && Rage < 30 && !DeBuffColossusSmashUp) {
                  Addon.Cast("Bladestorm");
                  return true;
                }
              }
            } else {
              if (Addon.CanCast("Bladestorm", "player")) {
                if (DeBuffColossusSmashUp && DebuffDeepWoundsUp && Rage < 30 && !NoCooldowns) {
                  Addon.Cast("Bladestorm");
                  return true;
                }
              }
            }

            if (Addon.CanCast("Whirlwind", "player")) {
              if (DeBuffColossusSmashUp) {
                Addon.Cast("Whirlwind");
                return true;
              }
            }

            if (Addon.CanCast("Whirlwind", "player") && Rage > 35 && !(Addon.CanCast("Overpower")) && !Addon.CanCast("Mortal Strike")) {
              Addon.Cast("Whirlwind");
              return true;
            }

            if (Addon.CanCast("Mortal Strike")) {
              if (Addon.HasBuff("Sweeping Strikes", "player") && Addon.CanCast("Mortal Strike") && !Addon.CanCast("Overpower")) {
                Addon.Cast("Mortal Strike");
                return true;
              }
            }
          }

          if (Addon.CanCast("Skullsplitter")) {
            if (Rage < 60) {
              Addon.Cast("Skullsplitter");
              return true;
            }
          }

          if (Addon.CanCast("Heroic Throw")) {
            if (Rage <= 30 && Range <= 30) {
              Addon.Cast("Heroic Throw");
              return true;
            }
          }

          if (Addon.CanCast("Execute")) {
            Addon.Cast("Execute");
            return true;
          }
        }
      }

      //PVP Rotation
      if (PVPmode) {
        if (Addon.CanCast("Sharpen Blade")) {
          if (!BuffSharpenBladeUp && TargetHealth <= 50) {
            Addon.Cast("Sharpen Blade");
            return true;
          }
        }

        if (TalentAvatar) {
          if (Addon.CanCast("Avatar", "player")) {
            Addon.Cast("Avatar");
            return true;
          }
        }

        if (Addon.CanCast("Heroic Throw")) {
          if (Rage <= 30 && Range <= 30) {
            Addon.Cast("Heroic Throw");
            return true;
          }
        }

        if (Fighting) {
          if (Addon.CanCast("Hamstring")) {
            if (!DebuffHamstringUp) {
              Addon.Cast("Hamstring");
              return true;
            }
          }

          if (Addon.CanCast("Warbreaker", "player")) {
            Addon.Cast("Warbreaker");
            return true;
          }

          if (!TestOfMightUp) {
            if (Addon.CanCast("Overpower")) {
              Addon.Cast("Overpower");
              return true;
            }
          }

          if (Addon.CanCast("Mortal Strike")) {
            Addon.Cast("Mortal Strike");
            return true;
          }

          if (Addon.CanCast("Rend")) {
            if (!DeBuffRendUp) {
              Addon.Cast("Rend");
              return true;
            }
          }

          if (TestOfMightTrait) {
            if (Addon.CanCast("Bladestorm", "player")) {
              if (TestOfMightUp && Rage < 30 && !DeBuffColossusSmashUp) {
                Addon.Cast("Bladestorm");
                return true;
              }
            }
          } else {
            if (Addon.CanCast("Bladestorm", "player")) {
              if (DeBuffColossusSmashUp && DebuffDeepWoundsUp && Rage < 30 && !NoCooldowns) {
                Addon.Cast("Bladestorm");
                return true;
              }
            }
          }

          if (Addon.CanCast("Slam")) {
            if (Rage >= 60) {
              Addon.Cast("Slam");
              return true;
            }
          }

          if (Addon.CanCast("Execute")) {
            if (TargetHealth <= 20 || (TalentMassacre && TargetHealth <= 35)) Addon.Cast("Execute");
            return true;
          }
        }
      }

      return false;
    }

    public override bool OutOfCombatTick() {
      bool Prepull = Addon.IsCustomCodeOn("savepp");

      if (Addon.CanCast("Battle Shout", "player") && !Addon.HasBuff("Battle Shout", "player")) {
        Addon.Cast("Battle Shout");
        return true;
      }

      if (!Prepull) {
        if (MajorPower == "Memory of Lucid Dreams" && Addon.TargetIsEnemy() && Addon.Range("target") < 10 && Addon.Health("target") > 5) {
          if (Addon.CanCast("Memory of Lucid Dreams", "player")) {
            Addon.Cast("Memory of Lucid Dreams");
            return true;
          }
        }
      }
      return false;
    }
  }
}