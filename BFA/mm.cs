using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AddonWow.API; //needed to access Addon API

namespace AddonWow.Modules {
  /// <summary>
  // Created by JS
  /// Check API-DOC for detailed documentation.
  /// </summary>
  public class JSMMHunt: Rotation {
    public override void LoadSettings() {

      Addon.Latency = 50;
      Addon.QuickDelay = 125;

      List < string > MajorAzeritePower = new List < string > (new string[] {
        "Guardian of Azeroth",
        "Focused Azerite Beam",
        "Concentrated Flame",
        "Worldvein Resonance",
        "Memory of Lucid Dreams",
        "Blood of the Enemy",
        "Reaping Flames",
        "None"
      });
      Settings.Add(new Setting("Major Power", MajorAzeritePower, "Blood of the Enemy"));

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
      Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));

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
      Settings.Add(new Setting("Racial Power", Race, "None"));
    }

    string MajorPower;
    string TopTrinket;
    string BotTrinket;
    string RacialPower;
    string usableitems;

    public override void Initialize() {
      Addon.DebugMode();
      Addon.PrintMessage("JS Series: MM Hunter - v 1.0", Color.Yellow);
      Addon.PrintMessage("Recommended talents: 1113211", Color.Yellow);
      Addon.PrintMessage("These macros can be used for manual control:", Color.Blue);
      Addon.PrintMessage("/xxxxx AOE --Toggles AOE mode on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx Potions --Toggles using buff potions on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx SaveCooldowns --Toggles the use of big cooldowns on/off.", Color.Blue);
      Addon.PrintMessage(" ");
      Addon.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);

      MajorPower = GetDropDown("Major Power");
      TopTrinket = GetDropDown("Top Trinket");
      BotTrinket = GetDropDown("Bot Trinket");
      RacialPower = GetDropDown("Racial Power");
      usableitems = GetString("Use item: Case Sens");

      if (RacialPower == "Orc") Spellbook.Add("Blood Fury");
      if (RacialPower == "Troll") Spellbook.Add("Berserking");
      if (RacialPower == "Dark Iron Dwarf") Spellbook.Add("Fireblood");
      if (RacialPower == "Mag'har Orc") Spellbook.Add("Ancestral Call");
      if (RacialPower == "Lightforged Draenei") Spellbook.Add("Light's Judgment");

      Spellbook.Add(MajorPower);
      Spellbook.Add("Rapid Fire");
      Spellbook.Add("Hunter's Mark");
      Spellbook.Add("Aimed Shot");
      Spellbook.Add("Steady Shot");
      Spellbook.Add("Multi-Shot");
      Spellbook.Add("Trueshot");
      Spellbook.Add("Barrage");
      Spellbook.Add("Explosive Shot");
      Spellbook.Add("Arcane Shot");

      Buffs.Add("Lifeblood");
      Buffs.Add("Precise Shots");

      Debuffs.Add("Razor Coral");
      Debuffs.Add("Hunter's Mark");

      Items.Add(TopTrinket);
      Items.Add(BotTrinket);
      Items.Add(usableitems);

      Macros.Add("ItemUse", "/use " + usableitems);
      Macros.Add("TopTrink", "/use 13");
      Macros.Add("BotTrink", "/use 14");

      CustomCommands.Add("AOE");
      CustomCommands.Add("Potions");
      CustomCommands.Add("SaveCooldowns");
    }

    // optional override for the CombatTick which executes while in combat
    public override bool CombatTick() {
      int PetHealth = Addon.Health("pet");
      int GCD = Addon.GCD();
      int Latency = Addon.Latency;
      bool Moving = Addon.PlayerIsMoving();
      bool IsChanneling = Addon.IsChanneling("player");
      bool Fighting = Addon.Range("target") <= 45 && Addon.TargetIsEnemy();
      bool FontEquipped = Addon.IsEquipped("Azshara's Font of Power");
      bool CanUseFont = Addon.CanUseItem("Azshara's Font of Power");
      bool CoralEquipped = Addon.IsEquipped("Ashvane's Razor Coral");
      bool CanUseCoral = Addon.CanUseItem("Ashvane's Razor Coral");
      bool CycloEquipped = Addon.IsEquipped("Pocket-Sized Computation Device");
      bool CanUseCyclo = Addon.CanUseItem("Pocket-Sized Computation Device");
      bool CoralDebuffUp = Addon.HasDebuff("Razor Coral", "target");
      string PrevGCD = Addon.LastCast();
      string LastCast = Addon.LastCast();
      int TargetHealth = Addon.Health("target");
      int PlayerHealth = Addon.Health("player");
      int TargetTimeToDie = 1000000000;
      int CycloCD = Addon.ItemCooldown("Cyclotronic Blast") - GCD;
      float Haste = Addon.Haste() / 100f;
      int GCDMAX = (int)((1500f / (Haste + 1f)));
      bool UsePotion = Addon.IsCustomCodeOn("Potions");
      bool AOE = Addon.IsCustomCodeOn("AOE");
      int EnemiesNearTarget = Addon.EnemiesNearTarget();
      int EnemiesInMelee = Addon.EnemiesInMelee();
      int Focus = Addon.Power("player");
      float FocusRegen = 10f * (1f + Haste);
      int BarbedShotBuffCount = Addon.BuffStacks("Barbed Shot");

      bool DebuffHunterMark = Addon.HasDebuff("Hunter's Mark", "target");
      bool BuffPreciseShots = Addon.HasBuff("Precise Shots", "player");

      int FocusMax = Addon.PlayerMaxPower();
      float CritPercent = Addon.Crit() / 100f;
      float FocusTimeToMax = (FocusMax - Focus) * 1000f / FocusRegen;
      int FlameFullRecharge = (int)(Addon.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Addon.SpellCharges("Concentrated Flame")));
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");

      if (!AOE) {
        EnemiesNearTarget = 1;
        EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
      }

      if (IsChanneling || Addon.CastingID("player") == 295261 || Addon.CastingID("player") == 299338 || Addon.CastingID("player") == 295258 || Addon.CastingID("player") == 299336 || Addon.CastingID("player") == 295273 || Addon.CastingID("player") == 295264 || Addon.CastingID("player") == 295261 || Addon.CastingID("player") == 295263 || Addon.CastingID("player") == 295262 || Addon.CastingID("player") == 295272 || Addon.CastingID("player") == 299564) return false;

      if (!NoCooldowns) {
        if (FontEquipped) {
          if (CanUseFont && TargetTimeToDie > 10000 && !Moving && Fighting) {
            Addon.Cast("Azshara's Font of Power");
            return true;
          }
        }

        if (CoralEquipped) {
          if (CanUseCoral && Fighting) {
            if (MajorPower != "Guardian of Azeroth" || !CoralDebuffUp || (TargetTimeToDie < 26000 && TargetTimeToDie > 24000)) {
              Addon.Cast("Ashvane's Razor Coral");
              return true;
            }
          }
        }

        if (CycloEquipped) {
          if (CanUseCyclo && Fighting) {
            if (TargetTimeToDie < 5000) {
              Addon.Cast("Pocket-Sized Computation Device");
              return true;
            }
          }
        }

        if (Addon.CanCast("Ancestral Call", "player") && Fighting) {
          Addon.Cast("Ancestral Call", true);
          return true;
        }

        if (Addon.CanCast("Fireblood", "player") && Fighting) {
          Addon.Cast("Fireblood", true);
          return true;
        }

        if (Addon.CanCast("Berserking", "player") && Fighting) {
          if (TargetTimeToDie > 192000 || (TargetHealth < 35) || TargetTimeToDie < 13000) {
            Addon.Cast("Berserking", true);
            return true;
          }
        }

        if (Addon.CanCast("Blood Fury", "player") && Fighting) {
          if (TargetTimeToDie > 192000 || (TargetHealth < 35 || TargetTimeToDie < 16000)) {
            Addon.Cast("Blood Fury", true);
            return true;
          }
        }

        if (Addon.CanUseTrinket(0) && TopTrinket == "Generic" && Fighting) {
          if (TargetTimeToDie > 192000 || (TargetHealth < 35) || TargetTimeToDie < 16000) {
            Addon.Cast("TopTrink", true);
            return true;
          }
        }

        if (Addon.CanUseTrinket(1) && BotTrinket == "Generic" && Fighting) {
          if (TargetTimeToDie > 192000 || TargetHealth < 35 || TargetTimeToDie < 16000) {
            Addon.Cast("BotTrink", true);
            return true;
          }
        }

        if (Addon.CanCast("Light's Judgment", "player") && Fighting) {
          Addon.Cast("Light's Judgment", true);
          return true;
        }

        if (Addon.CanUseItem(usableitems) && usableitems != "None" && !UsePotion) {
          if (EnemiesInMelee >= 1 && PlayerHealth <= GetSlider("Use item @ HP%")) {
            Addon.Cast("ItemUse", true);
            return true;
          }
        }

        if (MajorPower == "Worldvein Resonance" && Fighting) {
          if (Addon.CanCast("Worldvein Resonance", "player")) {
            if (Addon.BuffStacks("Lifeblood", "player", false) < 4) {
              Addon.Cast("Worldvein Resonance");
              return true;
            }

          }
        }

        if (MajorPower == "Blood of the Enemy" && Fighting) {
          if (Addon.CanCast("Blood of the Enemy", "player")) {
            if (EnemiesInMelee > 1) {
              Addon.Cast("Blood of the Enemy");
              return true;
            }
          }
        }

        if (MajorPower == "Guardian of Azeroth" && Fighting) {
          if (Addon.CanCast("Guardian of Azeroth", "player")) {
            if (TargetTimeToDie > 210000 || TargetTimeToDie < 30000) {
              Addon.Cast("Guardian of Azeroth");
              return true;
            }
          }
        }
      }

      if (EnemiesNearTarget <= 1) {
        if (Addon.CanCast("Hunter's Mark")) {
          if (!DebuffHunterMark) {
            Addon.Cast("Hunter's Mark");
            return true;
          }
        }
        if (Addon.CanCast("Rapid Fire")) {
          Addon.Cast("Rapid Fire");
          return true;
        }
        if (Addon.CanCast("Aimed Shot")) {
          if (LastCast != "Aimed Shot" && !Moving) {
            Addon.Cast("Aimed Shot");
            return true;
          }
        }
        if (Addon.CanCast("Arcane Shot")) {
          if (LastCast != "Arcane Shot" || BuffPreciseShots) {
            Addon.Cast("Arcane Shot");
            return true;
          }
        }
        if (Addon.CanCast("Trueshot", "player")) {
          Addon.Cast("Trueshot");
          return true;
        }
        if (Addon.CanCast("Steady Shot")) {
          if (Focus < 31) {
            Addon.Cast("Steady Shot");
            return true;
          }
        }
      }

      if (EnemiesNearTarget > 1) {
        if (Addon.CanCast("Hunter's Mark")) {
          if (!DebuffHunterMark) {
            Addon.Cast("Hunter's Mark");
            return true;
          }
        }
        if (Addon.CanCast("Aimed Shot")) {
          if (!Moving) {
            Addon.Cast("Aimed Shot");
            return true;
          }
        }
        if (Addon.CanCast("Multi-Shot")) {
          if (LastCast != "Multi-Shot" || BuffPreciseShots) {
            Addon.Cast("Multi-Shot");
            return true;
          }
        }
        if (Addon.CanCast("Barrage")) {
          Addon.Cast("Barrage");
          return true;
        }
        if (Addon.CanCast("Rapid Fire")) {
          Addon.Cast("Rapid Fire");
          return true;
        }
        if (Addon.CanCast("Trueshot", "player")) {
          Addon.Cast("Trueshot");
          return true;
        }
        if (Addon.CanCast("Steady Shot")) {
          if (Focus < 31) {
            Addon.Cast("Steady Shot");
            return true;
          }
        }
      }

      if (MajorPower == "Concentrated Flame") {
        if (Addon.CanCast("Concentrated Flame")) {
          if (((float) Focus + FocusRegen * (GCDMAX + GCD) < FocusMax) || (FlameFullRecharge < GCDMAX) || TargetTimeToDie < 5000) {
            Addon.Cast("Concentrated Flame");
            return true;
          }
        }
      }

      if (MajorPower == "Reaping Flames") {
        if (Addon.CanCast("Reaping Flames")) {
          Addon.Cast("Reaping Flames");
          return true;
        }
      }

      if (MajorPower == "Focused Azerite Beam" && Fighting) {
        if (Addon.CanCast("Focused Azerite Beam", "player")) {
          if (TargetTimeToDie < 5000) {
            Addon.Cast("Focused Azerite Beam");
            return true;
          }
        }
      }
      return false;
    }

    public override bool OutOfCombatTick() {
      return false;
    }

  }
}