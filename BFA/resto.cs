using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AddonWow.API; //needed to access Addon API

namespace AddonWow.Modules {
  /// <summary>
  /// Created by JS
  /// Check API-DOC for detailed documentation.
  /// </summary>
  public class JSRestoDruid: Rotation {

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
        "Generic",
        "None"
      });
      Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
      Settings.Add(new Setting("Top Trinket AOE?", true));
      Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));
      Settings.Add(new Setting("Bot Trinket AOE?", true));

      Settings.Add(new Setting("Use item: Case Sens", "None"));
      Settings.Add(new Setting("Use item @ HP%", 0, 100, 100));

      List < string > Race = new List < string > (new string[] {
        "Tauren",
        "Troll",
        "None"
      });
      Settings.Add(new Setting("Racial Power", Race, "None"));

      Settings.Add(new Setting("Auto Self Buff @ HP%", 0, 100, 80));
      Settings.Add(new Setting("BUBBLE @ HP%", 0, 100, 60));
      Settings.Add(new Setting("REDUCE TARGET DMG BUFF @ HP%", 0, 100, 90));
      Settings.Add(new Setting("LIFEBLOOM @ HP%", 0, 100, 99));
      Settings.Add(new Setting("REJUVENATION @ HP%", 0, 100, 95));
      Settings.Add(new Setting("SWIFTMEND @ HP%", 0, 100, 85));
      Settings.Add(new Setting("REGROWTH @ HP%", 0, 100, 95));
      Settings.Add(new Setting("WILD GROWTH @ HP%", 0, 100, 80));
      Settings.Add(new Setting("TRANQUILITY @ HP%", 0, 100, 5));
    }

    string MajorPower;
    string TopTrinket;
    string BotTrinket;
    string RacialPower;
    string usableitems;

    public override void Initialize() {
      //Addon.DebugMode();

      Addon.PrintMessage("JS Series: Resto Druid - v1.2", Color.Yellow);
      Addon.PrintMessage("Recommended talents: 3313131", Color.Yellow);
      Addon.PrintMessage("Default mode: OOC Heal, AOE ON, Auto Cat, USE CDs/Pots, Lifebloom ON", Color.Yellow);
      Addon.PrintMessage("These macros can be used for manual control:", Color.Blue);
      Addon.PrintMessage("/xxxxx SaveCooldowns -- Toggles the use of big cooldowns & potions on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx toggle -- Toggles the rotation.", Color.Blue);
      Addon.PrintMessage("/xxxxx saveitems -- Toggles the use of items have to have use cooldowns on.", Color.Orange);
      Addon.PrintMessage("/xxxxx nocat -- Toggles Auto Cat Form in DPS combat on/off", Color.Orange);
      Addon.PrintMessage("/xxxxx nooocheal -- Toggles healing outside of combat on/off.", Color.Orange);
      Addon.PrintMessage("/xxxxx spamrej -- Toggles spam rev to preheal damage on/off in out of combat.", Color.Orange);
      Addon.PrintMessage("/xxxxx noregrospam -- Toggles spamming of regrowth as main heal on/off. Will cast on proc.", Color.Orange);
      Addon.PrintMessage("/xxxxx notarlb -- Toggles the use of target lifebloom heal on/off.", Color.Orange);
      Addon.PrintMessage("/xxxxx notarib --Toggles the use of target ironbark on/off.", Color.Orange);
      Addon.PrintMessage("/xxxxx notarcw --Toggles the use of target cenarion ward on/off.", Color.Orange);
      Addon.PrintMessage("/xxxxx aoe --Toggles the use of AOE heal on/off.", Color.Orange);
      Addon.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);

      Addon.Latency = 50;
      Addon.QuickDelay = 125;
      Addon.SlowDelay = 250;

      MajorPower = GetDropDown("Major Power");
      TopTrinket = GetDropDown("Top Trinket");
      BotTrinket = GetDropDown("Bot Trinket");
      RacialPower = GetDropDown("Racial Power");
      usableitems = GetString("Use item: Case Sens");

      Spellbook.Add(MajorPower);

      if (RacialPower == "Tauren") Spellbook.Add("War Stomp");
      if (RacialPower == "Troll") Spellbook.Add("Berserking");

      // Class Spells
      Spellbook.Add("Efflorescence"); //AOE Heal Instant HOT
      Spellbook.Add("Lifebloom"); //Instant HOT 
      Spellbook.Add("Wild Growth"); //  Cast HOT
      Spellbook.Add("Rejuvenation"); // Instant HOT
      Spellbook.Add("Regrowth"); // Cast HOT
      Spellbook.Add("Swiftmend"); // Instant HOT
      Spellbook.Add("Barkskin");
      Spellbook.Add("Rebirth");
      Spellbook.Add("Revive");
      Spellbook.Add("Cenarion Ward");
      Spellbook.Add("Tranquility");
      Spellbook.Add("Moonfire");
      Spellbook.Add("Solar Wrath");
      Spellbook.Add("Sunfire");
      Spellbook.Add("Ironbark");
      Spellbook.Add("Cat Form");
      Spellbook.Add("Moonkin Form");
      Spellbook.Add("Rip");
      Spellbook.Add("Rake");
      Spellbook.Add("Shred");
      Spellbook.Add("Swipe");
      Spellbook.Add("Starsurge");
      Spellbook.Add("Lunar Strike");
      Spellbook.Add("Ferocious Bite");

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
      Buffs.Add("Spring Blossoms");

      // Class Buffs
      Buffs.Add("Prowl");
      Buffs.Add("Cat Form");
      Buffs.Add("Moonkin Form");
      Buffs.Add("Bear Form");
      Buffs.Add("Spring Blossums"); // Eff Effect
      Buffs.Add("Lifebloom"); //Instant HOT 
      Buffs.Add("Wild Growth"); //  Cast HOT
      Buffs.Add("Rejuvenation"); // Instant HOT
      Buffs.Add("Regrowth"); // Cast HOT
      Buffs.Add("Swiftmend"); // Instant HOT
      Buffs.Add("Clearcasting");
      Buffs.Add("Solar Empowerment");
      Buffs.Add("Lunar Empowerment");
      Buffs.Add("Soul of the Forest");

      //Debuffs
      Debuffs.Add("Razor Coral");
      Debuffs.Add("Conductive Ink");
      Debuffs.Add("Shiver Venom");
      Debuffs.Add("Thrash");
      Debuffs.Add("Moonfire");
      Debuffs.Add("Sunfire");
      Debuffs.Add("Rip");
      Debuffs.Add("Rake");

      //Talent
      bool TalentFeral = Addon.Talent(3, 2);
      bool TalentBalance = Addon.Talent(3, 1);

      Items.Add(TopTrinket);
      Items.Add(BotTrinket);
      Items.Add(usableitems);

      Macros.Add("ItemUse", "/use " + usableitems);
      Macros.Add("TopTrink", "/use 13");
      Macros.Add("BotTrink", "/use 14");
      Macros.Add("selfheal", "/cast [@player] Regrowth");
      Macros.Add("healform", "/cancelform");
      Macros.Add("cursoreff", "/cast [@cursor] Efflorescence");
      Macros.Add("flb", "/cast [@focus] Lifebloom");
      Macros.Add("fib", "/cast [@focus] Ironbark");
      Macros.Add("frej", "/cast [@focus] Rejuvenation");
      Macros.Add("fcw", "/cast [@focus] Cenarion Ward");

      CustomCommands.Add("SaveCooldowns");
      CustomCommands.Add("spamrej");
      CustomCommands.Add("nooocheal");
      CustomCommands.Add("noregrospam");
      CustomCommands.Add("nocat");
      CustomCommands.Add("notarlb");
      CustomCommands.Add("notarib");
      CustomCommands.Add("notarcw");
      CustomCommands.Add("aoe");
      CustomCommands.Add("saveitems");
    }

    // optional override for the CombatTick which executes while in combat
    public override bool CombatTick() {

      bool Fighting = Addon.Range("target") <= 8 && Addon.TargetIsEnemy();
      bool Moving = Addon.PlayerIsMoving();
      float Haste = Addon.Haste() / 100f;
      float SpellHaste = 1f / (1f + Haste);
      int Time = Addon.CombatTime();
      int Range = Addon.Range("target");
      int TargetHealth = Addon.Health("target");
      int PlayerHealth = Addon.Health("player");
      int FocusHealth = Addon.Health("focus");
      bool Enemy = Addon.TargetIsEnemy();
      string LastCast = Addon.LastCast();
      bool IsChanneling = Addon.IsChanneling("player");
      int EnemiesInMelee = Addon.EnemiesInMelee();
      int EnemiesNearTarget = Addon.EnemiesNearTarget();
      int GCDMAX = (int)(1500f / (Haste + 1f));
      int GCD = Addon.GCD();
      int Latency = Addon.Latency;
      int TargetTimeToDie = 1000000000;
      bool HasLust = Addon.HasBuff("Bloodlust", "player", false) || Addon.HasBuff("Heroism", "player", false) || Addon.HasBuff("Time Warp", "player", false) || Addon.HasBuff("Ancient Hysteria", "player", false) || Addon.HasBuff("Netherwinds", "player", false) || Addon.HasBuff("Drums of Rage", "player", false);
      int FlameFullRecharge = (int)(Addon.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Addon.SpellCharges("Concentrated Flame")));

      int CDGuardianOfAzerothRemains = Addon.SpellCooldown("Guardian of Azeroth") - GCD;
      bool BuffGuardianOfAzerothUp = Addon.HasBuff("Guardian of Azeroth");
      int CDBloodOfTheEnemyRemains = Addon.SpellCooldown("Blood of the Enemy") - GCD;
      int BuffMemoryOfLucidDreamsRemains = Addon.BuffRemaining("Memory of Lucid Dreams") - GCD;
      bool BuffMemoryOfLucidDreamsUp = BuffMemoryOfLucidDreamsRemains > 0;
      bool DebuffRazorCoralUp = Addon.HasDebuff("Razor Coral");
      bool DebuffConductiveInkUp = Addon.HasDebuff("Conductive Ink");
      int BuffRecklessForceRemains = Addon.BuffRemaining("Reckless Force") - GCD;
      bool BuffRecklessForceUp = BuffRecklessForceRemains > 0;
      int BuffRecklessForceStacks = Addon.BuffStacks("Reckless Force");
      int CDRazorCoral = Addon.ItemCooldown("Razor Coral");

      int Mana = Addon.Power("player");
      int MaxMana = Addon.PlayerMaxPower();
      int ManaDefecit = MaxMana - Mana;
      int Energy = Addon.Power("player");
      int ComboPoints = Addon.PlayerSecondaryPower();
      int MaxComboPoints = 5;
      int ComboPointsDefecit = MaxComboPoints - ComboPoints;

      //Commands
      bool NoHeal = Addon.IsCustomCodeOn("nooocheal");
      bool SpamRejuvenation = Addon.IsCustomCodeOn("spamrej");
      bool NoRegrowthAlways = Addon.IsCustomCodeOn("noregrospam");
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");
      bool AutoCatForm = Addon.IsCustomCodeOn("nocat");
      bool DontUseLifebloom = Addon.IsCustomCodeOn("notarlb");
      bool DontUseIronbark = Addon.IsCustomCodeOn("notarib");
      bool AOEHeal = Addon.IsCustomCodeOn("aoe");
      bool SaveitemUse = Addon.IsCustomCodeOn("saveitems");
      bool TargetCW = Addon.IsCustomCodeOn("notarcw");

      //Settings
      bool TopTrinketAOE = GetCheckBox("Top Trinket AOE?");
      bool BotTrinketAOE = GetCheckBox("Bot Trinket AOE?");

      //Debuffs
      bool DebuffMoonfireUp = Addon.DebuffRemaining("Moonfire") - GCD > 0;
      bool DebuffSunfireUp = Addon.DebuffRemaining("Sunfire") - GCD > 0;
      bool DebuffRipUp = Addon.DebuffRemaining("Rip") - GCD > 0;
      bool DebuffRakeUp = Addon.DebuffRemaining("Rake") - GCD > 0;

      //Buffs
      bool BuffStealthUp = Addon.HasBuff("Prowl");
      bool BuffLifebloomUp = Addon.HasBuff("Lifebloom", "target");
      bool BuffFLifebloomUp = Addon.HasBuff("Lifebloom", "focus");
      bool BuffWildGrowthUp = Addon.HasBuff("Wild Growth", "target");
      bool BuffRejuvenationUp = Addon.HasBuff("Rejuvenation", "target");
      bool BuffFRejuvenationUp = Addon.HasBuff("Rejuvenation", "focus");
      bool BuffRegrowthUp = Addon.HasBuff("Regrowth", "target");
      bool BuffSwiftmendUp = Addon.HasBuff("Swiftmend", "target");
      bool ClearcastingUp = Addon.HasBuff("Clearcasting", "player");
      bool LunarEmpowermentUp = Addon.HasBuff("Lunar Empowerment", "player");
      bool SolarEmpowermentUp = Addon.HasBuff("Solar Empowerment", "player");
      bool SouloftheForestUp = Addon.HasBuff("Soul of the Forest", "player");
      bool BuffEffUp = Addon.HasBuff("Spring Blossoms", "target");
      bool Stealthed = BuffStealthUp;

      if (IsChanneling) return false;

      if (!Stealthed && !NoCooldowns) {
        if (MajorPower == "Concentrated Flame") {
          if (Addon.CanCast("Concentrated Flame")) {
            if (FlameFullRecharge < GCDMAX && TargetHealth <= 80) {
              Addon.Cast("Concentrated Flame");
              return true;
            }
          }
        }

        if (MajorPower == "Blood of the Enemy" && EnemiesInMelee > 0) {
          if (Addon.CanCast("Blood of the Enemy", "player")) {
            if (TargetTimeToDie <= 10000) {
              Addon.Cast("Blood of the Enemy");
              return true;
            }
          }
        }

        if (MajorPower == "Guardian of Azeroth" && Fighting) {
          if (Addon.CanCast("Guardian of Azeroth", "player")) {
            if (TargetTimeToDie < 30000) {
              Addon.Cast("Guardian of Azeroth");
              return true;
            }
          }
        }

        if (MajorPower == "Focused Azerite Beam" && Range < 15) {
          if (Addon.CanCast("Focused Azerite Beam", "player")) {
            if (EnemiesInMelee >= 2 || Mana < 70) {
              Addon.Cast("Focused Azerite Beam");
              return true;
            }
          }
        }

        if (MajorPower == "The Unbound Force") {
          if (Addon.CanCast("The Unbound Force")) {
            if (BuffRecklessForceUp || BuffRecklessForceStacks < 10) {
              Addon.Cast("The Unbound Force");
              return true;
            }
          }
        }

        if (MajorPower == "Worldvein Resonance" && Fighting) {
          if (Addon.CanCast("Worldvein Resonance", "player")) {
            Addon.Cast("Worldvein Resonance");
            return true;
          }
        }

        if (MajorPower == "Memory of Lucid Dreams" && Fighting) {
          if (Addon.CanCast("Memory of Lucid Dreams", "player")) {
            if (Mana < 50) {
              Addon.Cast("Memory of Lucid Dreams");
              return true;
            }
          }
        }
      }

      //generic trinket usage

      if (!NoCooldowns) {
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

        if (Addon.CanUseItem(usableitems, false) && usableitems != "None" && !SaveitemUse) {
          if (PlayerHealth <= GetSlider("Use item @ HP%")) {
            Addon.Cast("ItemUse", true);
            return true;
          }
        }

        if (RacialPower == "Troll" && Fighting) {
          if (Addon.CanCast("Berserking", "player")) {
            Addon.Cast("Berserking", true);
            return true;
          }
        }

        if (RacialPower == "Tauren" && Fighting) {
          if (Addon.CanCast("War Stomp", "player")) {
            Addon.Cast("War Stomp", true);
            return true;
          }
        }
      }

      if (!Addon.HasBuff("Moonkin Form") && Addon.CanCast("Moonkin Form", "player") && Addon.TargetIsEnemy()) {
        Addon.Cast("Moonkin Form");
        return true;
      }

      if (!AutoCatForm) {
        if (!Addon.HasBuff("Cat Form") && Addon.CanCast("Cat Form", "player") && Fighting && Range < 3) {
          Addon.Cast("Cat Form");
          return true;
        }
      }

      //DPS Reduction buff
      if (Addon.CanCast("Barkskin", "player")) {
        if ((PlayerHealth <= GetSlider("Auto Self Buff @ HP%"))) {
          Addon.Cast("Barkskin");
          return true;
        }
      }

      if (Addon.CanCast("Lifebloom", "focus")) {
        if (!BuffFLifebloomUp && FocusHealth <= GetSlider("LIFEBLOOM @ HP%")) {
          Addon.Cast("flb");
          return true;
        }
      }

      if (Addon.CanCast("Rejuvenation", "focus")) {
        if (!BuffFRejuvenationUp && FocusHealth <= GetSlider("REJUVENATION @ HP%")) {
          Addon.Cast("frej");
          return true;
        }
      }

      if (Addon.CanCast("Cenarion Ward", "focus")) {
        if (FocusHealth <= GetSlider("BUBBLE @ HP%")) {
          Addon.Cast("fcw");
          return true;
        }
      }

      if (Addon.CanCast("Ironbark", "focus")) {
        if (FocusHealth <= GetSlider("REDUCE TARGET DMG BUFF @ HP%")) {
          Addon.Cast("fib");
          return true;
        }
      }

      if (Enemy) {
        if (!Addon.HasBuff("Cat Form") && !Addon.HasBuff("Moonkin Form")) {
          if (Addon.CanCast("Moonfire") && Addon.SpellInRange("Moonfire", "target")) {
            if (!DebuffMoonfireUp) {
              Addon.Cast("Moonfire");
              return true;
            }
          }
          if (Addon.CanCast("Sunfire") && Addon.SpellInRange("Sunfire", "target")) {
            if (!DebuffSunfireUp) {
              Addon.Cast("Sunfire");
              return true;
            }
          }
          if (Addon.CanCast("Solar Wrath") && Addon.SpellInRange("Solar Wrath", "target")) {
            Addon.Cast("Solar Wrath");
            return true;
          }
        }

        if (Addon.HasBuff("Moonkin Form")) {
          if (Addon.CanCast("Moonfire")) {
            if (!DebuffMoonfireUp) {
              Addon.Cast("Moonfire");
              return true;
            }
            if (!DebuffSunfireUp) {
              Addon.Cast("Sunfire");
              return true;
            }
          }
          if (DebuffMoonfireUp && DebuffSunfireUp && !Moving) {
            if (!LunarEmpowermentUp && !SolarEmpowermentUp) {
              if (Addon.CanCast("Starsurge")) {
                Addon.Cast("Starsurge");
                return true;
              }
            }

            if (EnemiesNearTarget >= 2) {
              if (Addon.CanCast("Lunar Strike")) {
                if (!SolarEmpowermentUp) {
                  Addon.Cast("Lunar Strike");
                  return true;
                }
              }

              if (Addon.CanCast("Solar Wrath")) {
                if (SolarEmpowermentUp && LastCast != "Solar Wrath") {
                  Addon.Cast("Solar Wrath");
                  return true;
                }
              }
            } else {
              if (Addon.CanCast("Solar Wrath")) {
                if (!LunarEmpowermentUp) {
                  Addon.Cast("Solar Wrath");
                  return true;
                }
              }

              if (Addon.CanCast("Lunar Strike")) {
                if (LunarEmpowermentUp && LastCast != "Lunar Strike") {
                  Addon.Cast("Lunar Strike");
                  return true;
                }
              }
            }
          }
        }

        if (Addon.HasBuff("Cat Form")) {
          if (Addon.CanCast("Rake") && Energy >= 35 && ComboPoints < 5) {
            if (!DebuffRakeUp) {
              Addon.Cast("Rake");
              return true;
            }
          }

          if (Addon.CanCast("Rip") && Energy >= 20 && TargetHealth >= 5) {
            if (!DebuffRipUp && ComboPoints > 4) {
              Addon.Cast("Rip");
              return true;
            }
          }

          if (Addon.CanCast("Ferocious Bite") && Energy >= 25) {
            if (ComboPointsDefecit <= 1 && DebuffRipUp) {
              Addon.Cast("Ferocious Bite");
              return true;
            }
          }

          if (Addon.CanCast("Shred") && Energy >= 40 && ComboPoints < 4) {
            Addon.Cast("Shred");
            return true;
          }

          if (EnemiesInMelee >= 2) {
            if (Addon.CanCast("Swipe", "player") && Energy >= 35) {
              if (ComboPoints >= 3 && ComboPoints < 5) {
                Addon.Cast("Swipe");
                return true;
              }
            }
          }
        }
      }

      if (!Enemy) {
        if (Addon.CanCast("Lifebloom", "target") && !DontUseLifebloom) {
          if (!BuffLifebloomUp && TargetHealth <= GetSlider("LIFEBLOOM @ HP%")) {
            Addon.Cast("Lifebloom");
            return true;
          }
        }

        if (Addon.CanCast("Efflorescence", "target")) {
          if (!BuffEffUp) {
            Addon.Cast("cursoreff");
            return true;
          }
        }

        if (Addon.CanCast("Wild Growth", "target") && AOEHeal) {
          if (TargetHealth <= GetSlider("WILD GROWTH @ HP%") && !Moving && BuffRegrowthUp && BuffRejuvenationUp) {
            Addon.Cast("Wild Growth");
            return true;
          }
        }

        if (Addon.CanCast("Tranquility", "player") && !NoCooldowns && AOEHeal) {
          if (TargetHealth <= GetSlider("TRANQUILITY @ HP%") && !Moving) {
            Addon.Cast("Tranquility");
            return true;
          }
        }

        if (Addon.CanCast("Rejuvenation", "target")) {
          if (!BuffRejuvenationUp && TargetHealth <= GetSlider("REJUVENATION @ HP%")) {
            Addon.Cast("Rejuvenation");
            return true;
          }
        }

        if (NoRegrowthAlways) {
          if (Addon.CanCast("Regrowth", "target")) {
            if (TargetHealth < 100 && !Moving && !BuffRegrowthUp && LastCast != "Regrowth" || ClearcastingUp || SouloftheForestUp) {
              Addon.Cast("Regrowth");
              return true;
            }
          }
        } else {
          if (Addon.CanCast("Regrowth", "target")) {
            if (TargetHealth <= GetSlider("REGROWTH @ HP%") && !Moving) {
              Addon.Cast("Regrowth");
              return true;
            }
          }
        }

        if (Addon.CanCast("Swiftmend", "target")) {
          if (TargetHealth <= GetSlider("SWIFTMEND @ HP%")) {
            Addon.Cast("Swiftmend");
            return true;
          }
        }

        if (Addon.CanCast("Ironbark", "target")) {
          if (TargetHealth <= GetSlider("REDUCE TARGET DMG BUFF @ HP%") && !DontUseIronbark) {
            Addon.Cast("Ironbark");
            return true;
          }
        }

        if (Addon.CanCast("Cenarion Ward", "target") && !TargetCW) {
          if (TargetHealth < GetSlider("BUBBLE @ HP%")) {
            Addon.Cast("Cenarion Ward");
            return true;
          }
        }

        if (Addon.CanCast("Rebirth", "target") && !NoCooldowns && !Moving) {
          if (TargetHealth < 1) {
            Addon.Cast("Rebirth");
            return true;
          }
        }
      }

      return false;
    }

    public override bool OutOfCombatTick() {
      bool Enemy = Addon.TargetIsEnemy();
      bool Moving = Addon.PlayerIsMoving();
      int TargetHealth = Addon.Health("target");

      //Commands
      bool NoHeal = Addon.IsCustomCodeOn("nooocheal");
      bool SpamRejuvenation = Addon.IsCustomCodeOn("spamrej");
      bool NoRegrowthAlways = Addon.IsCustomCodeOn("noregrospam");
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");
      bool DontUseLifebloom = Addon.IsCustomCodeOn("notarlb");
      bool AOEHeal = Addon.IsCustomCodeOn("aoe");

      //Buffs
      bool BuffStealthUp = Addon.HasBuff("Prowl");
      bool BuffLifebloomUp = Addon.HasBuff("Lifebloom", "target");
      bool BuffWildGrowthUp = Addon.HasBuff("Wild Growth", "target");
      bool BuffRejuvenationUp = Addon.HasBuff("Rejuvenation", "target");
      bool BuffRegrowthUp = Addon.HasBuff("Regrowth", "target");
      bool BuffSwiftmendUp = Addon.HasBuff("Swiftmend", "target");

      if (!NoHeal) {
        if (!Enemy) {
          if (Addon.CanCast("Lifebloom", "target") && !DontUseLifebloom) {
            if (!BuffLifebloomUp && TargetHealth <= GetSlider("LIFEBLOOM @ HP%")) {
              Addon.Cast("Lifebloom");
              return true;
            }
          }

          if (Addon.CanCast("Wild Growth") && AOEHeal) {
            if (TargetHealth <= GetSlider("WILD GROWTH @ HP%") && !Moving) {
              Addon.Cast("Wild Growth");
              return true;
            }
          }

          if (SpamRejuvenation) {
            if (Addon.CanCast("Rejuvenation", "target")) {
              if (!BuffRejuvenationUp && TargetHealth <= 100) {
                Addon.Cast("Rejuvenation");
                return true;
              }
            }
          } else {
            if (Addon.CanCast("Rejuvenation", "target")) {
              if (!BuffRejuvenationUp && TargetHealth <= GetSlider("REJUVENATION @ HP%")) {
                Addon.Cast("Rejuvenation");
                return true;
              }
            }
          }

          if (NoRegrowthAlways) {
            if (Addon.CanCast("Regrowth", "target")) {
              if (TargetHealth < 100 && !Moving && !BuffRegrowthUp) {
                Addon.Cast("Regrowth");
                return true;
              }
            }
          } else {
            if (Addon.CanCast("Regrowth", "target")) {
              if (TargetHealth <= GetSlider("REGROWTH @ HP%") && !Moving) {
                Addon.Cast("Regrowth");
                return true;
              }
            }
          }

          if (Addon.CanCast("Swiftmend", "target")) {
            if (TargetHealth <= GetSlider("SWIFTMEND @ HP%")) {
              Addon.Cast("Swiftmend");
              return true;
            }
          }

          if (Addon.CanCast("Revive", "target")) {
            if (TargetHealth < 1) {
              Addon.Cast("Revive");
              return true;
            }
          }
        }
      }
      return false;
    }

  }
}