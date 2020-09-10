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
  public class JSGuardian: Rotation {

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
      Settings.Add(new Setting("Top Trinket AOE?", false));
      Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));
      Settings.Add(new Setting("Bot Trinket AOE?", false));

      Settings.Add(new Setting("Use item: Case Sens", "None"));
      Settings.Add(new Setting("Use item @ HP%", 0, 100, 100));

      List < string > Race = new List < string > (new string[] {
        "Tauren",
        "Troll",
        "None"
      });
      Settings.Add(new Setting("Racial Power", Race, "None"));

      Settings.Add(new Setting("Auto Heal?", true));
      Settings.Add(new Setting("Auto Heal @ HP%", 0, 100, 80));

      Settings.Add(new Setting("DMG Reduction Buff?", true));
      Settings.Add(new Setting("Auto Buff @ HP%", 0, 100, 90));
    }

    string MajorPower;
    string TopTrinket;
    string BotTrinket;
    string RacialPower;
    string usableitems;

    public override void Initialize() {
      //Addon.DebugMode();

      Addon.PrintMessage("JS: Guardian Druid - v 1.2", Color.Yellow);
      Addon.PrintMessage("Recommended talents: 2333311", Color.Yellow);
      Addon.PrintMessage("Tank PvE Edition", Color.Blue);
      Addon.PrintMessage("Enable AOE for Effectiveness", Color.Purple);
      Addon.PrintMessage("These macros can be used for manual control:", Color.Blue);
      Addon.PrintMessage("/xxxxx Potions", Color.Blue);
      Addon.PrintMessage("--Toggles using buff potions on/off.", Color.Blue);
      Addon.PrintMessage(" ");
      Addon.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
      Addon.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
      Addon.PrintMessage(" ");
      Addon.PrintMessage("/xxxxx AOE", Color.Blue);
      Addon.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
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
      Spellbook.Add("Thrash");
      Spellbook.Add("Mangle");
      Spellbook.Add("Moonfire");
      Spellbook.Add("Mangle");
      Spellbook.Add("Maul");
      Spellbook.Add("Swipe");
      Spellbook.Add("Ironfur");
      Spellbook.Add("Frenzied Regeneration");
      Spellbook.Add("Barkskin");
      Spellbook.Add("Survival Instincts");
      Spellbook.Add("Incarnation: Guardian of Ursoc");
      Spellbook.Add("Brambles");

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

      // Class Buffs
      Buffs.Add("Prowl");
      Buffs.Add("Cat Form");
      Buffs.Add("Bear Form");
      Buffs.Add("Incarnation: Guardian of Ursoc");
      Buffs.Add("Frenzied Regeneration");
      Buffs.Add("Barkskin");
      Buffs.Add("Survival Instincts");

      Debuffs.Add("Razor Coral");
      Debuffs.Add("Conductive Ink");
      Debuffs.Add("Shiver Venom");
      Debuffs.Add("Thrash");
      Debuffs.Add("Moonfire");

      Items.Add(TopTrinket);
      Items.Add(BotTrinket);
      Items.Add(usableitems);

      Macros.Add("TopTrink", "/use 13");
      Macros.Add("BotTrink", "/use 14");
      Macros.Add("ItemUse", "/use " + usableitems);

      CustomCommands.Add("Potions");
      CustomCommands.Add("SaveCooldowns");
      CustomCommands.Add("AOE");
    }

    Stopwatch CombatTimer = new Stopwatch();
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
      string LastCast = Addon.LastCast();
      bool IsChanneling = Addon.IsChanneling("player");
      bool UsePotion = Addon.IsCustomCodeOn("Potions");
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");
      bool AOE = Addon.IsCustomCodeOn("AOE");
      int EnemiesInMelee = Addon.EnemiesInMelee();
      int EnemiesNearTarget = Addon.EnemiesNearTarget();
      int GCDMAX = (int)(1500f / (Haste + 1f));
      int GCD = Addon.GCD();
      int Latency = Addon.Latency;
      int TargetTimeToDie = 1000000000;
      bool HasLust = Addon.HasBuff("Bloodlust", "player", false) || Addon.HasBuff("Heroism", "player", false) || Addon.HasBuff("Time Warp", "player", false) || Addon.HasBuff("Ancient Hysteria", "player", false) || Addon.HasBuff("Netherwinds", "player", false) || Addon.HasBuff("Drums of Rage", "player", false);
      int FlameFullRecharge = (int)(Addon.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Addon.SpellCharges("Concentrated Flame")));

      if (!AOE) {
        EnemiesNearTarget = 1;
        EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
      }

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

      int Rage = Addon.Power("player");
      int MaxRage = Addon.PlayerMaxPower();
      int RageDefecit = MaxRage - Rage;

      //Spells
      int BrutalSlashCharges = Addon.SpellCharges("Brutal Slash");
      int SurvivalInstinctsCharges = Addon.SpellCharges("Survival Instincts");

      //Talents
      bool TalentGalacticGuardian = Addon.Talent(5, 2);
      bool TalentIncarnation = Addon.Talent(5, 3);

      //Debuffs
      int DebuffThrashRemains = Addon.DebuffRemaining("Thrash") - GCD;
      bool DebuffThrashUp = DebuffThrashRemains > 0;
      int DebuffMoonfireRemains = Addon.DebuffRemaining("Moonfire") - GCD;
      bool DebuffMoonfireUp = DebuffMoonfireRemains > 0;
      int DebuffThrashStacks = Addon.DebuffStacks("Thrash", "target");

      //Buffs
      bool BuffStealthUp = Addon.HasBuff("Prowl");
      int BuffIronFurRemains = Addon.BuffRemaining("Iron Fur") - GCD;
      bool BuffIronFurUp = Addon.BuffRemaining("Iron Fur") > 0;
      int BuffFrenziedRegenerationRemains = Addon.BuffRemaining("Frenzied Regeneration") - GCD;
      bool BuffFrenziedRegenerationUp = Addon.BuffRemaining("Frenzied Regeneration") > 0;
      bool GalacticGuardianUp = Addon.HasBuff("Galactic Guardian", "player");
      bool BuffSurvivalInstinctsUp = Addon.HasBuff("Survival Instincts", "player");
      bool BuffFrenziedRegenUp = Addon.HasBuff("Frenzied Regeneration", "player");

      //Trinks
      bool TopTrinketAOE = GetCheckBox("Top Trinket AOE?");
      bool BotTrinketAOE = GetCheckBox("Bot Trinket AOE?");

      //Mitigation
      bool AutoHealEnabled = GetCheckBox("Auto Heal?");
      bool AutoDMGReduceEnabled = GetCheckBox("DMG Reduction Buff?");

      //CDBuffs
      int CDBarkskinRemains = Addon.SpellCooldown("Barkskin") - GCD;
      bool CDBarkskinReady = CDBarkskinRemains <= 10;

      //Combat Timer
      if (Fighting) {
        if (!CombatTimer.IsRunning) CombatTimer.Start();
      }
      if (TargetHealth < 1) CombatTimer.Reset();

      bool Stealthed = BuffStealthUp;

      if (IsChanneling) return false;

      if (!Stealthed && DebuffMoonfireUp && !NoCooldowns) {
        if (MajorPower == "Concentrated Flame") {
          if (Addon.CanCast("Concentrated Flame")) {
            if (FlameFullRecharge < GCDMAX) {
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
            if (EnemiesInMelee >= 2 || Rage < 70) {
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
            if (Rage < 50 && !CDBarkskinReady) {
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

        if (Addon.CanUseItem(usableitems) && usableitems != "None" && !UsePotion) {
          if (EnemiesInMelee >= 1 && PlayerHealth <= GetSlider("Use item @ HP%")) {
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

      //Heal
      if (AutoHealEnabled) {
        if (!BuffFrenziedRegenUp && Addon.CanCast("Frenzied Regeneration", "player")) {
          if (PlayerHealth <= GetSlider("Auto Heal @ HP%") && Rage >= 10) {
            Addon.Cast("Frenzied Regeneration");
            return true;
          }
        }

      }

      //DPS Reduction buff
      if (AutoDMGReduceEnabled) {
        if (!BuffSurvivalInstinctsUp && Addon.CanCast("Survival Instincts", "player")) {
          if ((PlayerHealth <= GetSlider("Auto Buff @ HP%"))) {
            Addon.Cast("Survival Instincts");
            return true;
          }
        }

        if (Addon.CanCast("Barkskin", "player")) {
          if ((PlayerHealth <= GetSlider("Auto Buff @ HP%"))) {
            Addon.Cast("Barkskin");
            return true;
          }
        }
      }

      // Single Target Rotation

      if (EnemiesInMelee <= 1 && Fighting) {
        if (Addon.CanCast("Moonfire")) {
          if (!DebuffMoonfireUp || GalacticGuardianUp && LastCast != "Moonfire") {
            Addon.Cast("Moonfire");
            return true;
          }
        }

        if (Addon.CanCast("Thrash", "player")) {
          Addon.Cast("Thrash");
          return true;
        }

        if (Addon.CanCast("Mangle")) {
          Addon.Cast("Mangle");
          return true;
        }

        if (Addon.CanCast("Maul")) {
          if (RageDefecit < 10) {
            Addon.Cast("Maul");
            return true;
          }
        }

        if (Addon.CanCast("Swipe", "player")) {
          Addon.Cast("Swipe");
          return true;
        }

        if (Addon.CanCast("Ironfur", "player")) {
          if (Rage >= 40) {
            Addon.Cast("Ironfur");
            return true;
          }
        }
        return false;
      }

      //AOE 2+ Targets
      if (EnemiesInMelee >= 2 && Fighting) {
        if (Addon.CanCast("Moonfire")) {
          if (!DebuffMoonfireUp || GalacticGuardianUp && LastCast != "Moonfire") {
            Addon.Cast("Moonfire");
            return true;
          }
        }

        if (Addon.CanCast("Incarnation: Guardian of Ursoc", "player")) {
          if (DebuffMoonfireUp && DebuffThrashUp && !NoCooldowns) {
            Addon.Cast("Incarnation: Guardian of Ursoc");
            return true;
          }
        }

        if (Addon.CanCast("Thrash", "player")) {
          Addon.Cast("Thrash");
          return true;
        }

        if (EnemiesInMelee >= 3) {
          if (Addon.CanCast("Maul")) {
            Addon.Cast("Maul");
            return true;
          }
          return false;
        }

        if (EnemiesInMelee >= 4) {
          if (Addon.CanCast("Mangle")) {
            Addon.Cast("Mangle");
            return true;
          }
          return false;
        }

        if (Addon.CanCast("Swipe", "player")) {
          Addon.Cast("Swipe");
          return true;
        }
        if (Addon.CanCast("Ironfur", "player")) {
          if (Rage >= 40) {
            Addon.Cast("Ironfur");
            return true;
          }
        }
        return false;
      }

      if (Addon.CanCast("Rebirth", "target") && !NoCooldowns && !Moving) {
        if (TargetHealth < 1) {
          Addon.Cast("Rebirth");
          return true;
        }
      }
      return false;
    }

    public override bool OutOfCombatTick() {
      int TargetHealth = Addon.Health("target");

      if (Addon.CanCast("Revive", "target")) {
        if (TargetHealth < 1) {
          Addon.Cast("Revive");
          return true;
        }
      }
      return false;
    }

  }
}