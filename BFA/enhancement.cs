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
  public class JSEnShammy: Rotation {

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
        "Forbidden Obsidian Claw",
        "Manifesto of Madness",
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
        "None"
      });
      Settings.Add(new Setting("Racial Power", Race, "None"));

      Settings.Add(new Setting("Astral Shift @ HP%", 0, 100, 60));
    }

    string MajorPower;
    string TopTrinket;
    string BotTrinket;
    string RacialPower;
    string usableitems;

    public override void Initialize() {
      Addon.DebugMode();
      Addon.PrintMessage("JS Series: Enhancement Shaman - v 0.1", Color.Yellow);
      Addon.PrintMessage("Recommended Talents: 3212131", Color.Yellow);
      Addon.PrintMessage("These macros can be used for manual control:", Color.Blue);
      Addon.PrintMessage("/xxxxx Potions", Color.Blue);
      Addon.PrintMessage("--Toggles using buff potions on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
      Addon.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx AOE", Color.Blue);
      Addon.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx Prepull 10", Color.Blue);
      Addon.PrintMessage("--Starts the prepull actions.", Color.Blue);
      Addon.PrintMessage(" ");
      Addon.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);

      Addon.Latency = 150;
      Addon.QuickDelay = 100;
      Addon.SlowDelay = 350;

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

      //Class Spells
      Spellbook.Add("Flametongue");
      Spellbook.Add("Feral Spirit");
      Spellbook.Add("Frostbrand");
      Spellbook.Add("Stormstrike");
      Spellbook.Add("Sundering");
      Spellbook.Add("Rockbiter");
      Spellbook.Add("Lava Lash");
      Spellbook.Add("Lightning Bolt");
      Spellbook.Add("Earth Elemental");
      Spellbook.Add("Crash Lightning");
      Spellbook.Add("Astral Shift");
      Spellbook.Add("Ancestral Spirit");
      Spellbook.Add("Lightning Shield");
      Spellbook.Add("Totem Mastery");
      Spellbook.Add("Earthen Spike");
      Spellbook.Add("Fury of Air");
      Spellbook.Add("Ascendance");

      Buffs.Add("Bloodlust");
      Buffs.Add("Heroism");
      Buffs.Add("Time Warp");
      Buffs.Add("Ancient Hysteria");
      Buffs.Add("Netherwinds");
      Buffs.Add("Drums of Rage");
      Buffs.Add("Memory of Lucid Dreams");

      //Class Buffs
      Buffs.Add("Stormbringer");
      Buffs.Add("Hailstorm");
      Buffs.Add("Fury of Air");
      Buffs.Add("Primal Primer");
      Buffs.Add("Frostbrand");
      Buffs.Add("Flametongue");
      Buffs.Add("Ascendance");
      Buffs.Add("Resonance Totem");

      Debuffs.Add("Razor Coral");
      Debuffs.Add("Shiver Venom");

      Items.Add(TopTrinket);
      Items.Add(BotTrinket);
      Items.Add(usableitems);
      Items.Add("Neural Synapse Enhancer");

      Macros.Add("potion", "/use " + usableitems);
      Macros.Add("nse", "/use Neural Synapse Enhancer");
      Macros.Add("CapTotem", "/cast [@player] Capacitor Totem");
      Macros.Add("EarthTotem", "/cast [@player] Earthbind Totem");
      Macros.Add("TremorTotem", "/cast [@player] Tremor Totem");
      Macros.Add("TopTrink", "/use 13");
      Macros.Add("BotTrink", "/use 14");

      CustomCommands.Add("Potions");
      CustomCommands.Add("SaveCooldowns");
      CustomCommands.Add("AOE");
      CustomCommands.Add("Prepull");
    }

    // optional override for the CombatTick which executes while in combat
    public override bool CombatTick() {
      int ShiverVenomStacks = Addon.DebuffStacks("Shiver Venom");
      bool Fighting = Addon.Range("target") <= 45 && Addon.TargetIsEnemy();
      int Range = Addon.Range("target");
      bool Moving = Addon.PlayerIsMoving();
      float Haste = Addon.Haste() / 100f;
      int Time = Addon.CombatTime();
      bool IsChanneling = Addon.IsChanneling("player");
      int EnemiesInMelee = Addon.EnemiesInMelee();
      int EnemiesNearTarget = Addon.EnemiesNearTarget();
      bool Enemy = Addon.TargetIsEnemy();
      int GCDMAX = (int)(1500f / (Haste + 1f));
      int GCD = Addon.GCD();
      int Latency = Addon.Latency;
      int TargetTimeToDie = 1000000000;
      bool HasLust = Addon.HasBuff("Bloodlust", "player", false) || Addon.HasBuff("Heroism", "player", false) || Addon.HasBuff("Time Warp", "player", false) || Addon.HasBuff("Ancient Hysteria", "player", false) || Addon.HasBuff("Netherwinds", "player", false) || Addon.HasBuff("Drums of Rage", "player", false);
      int FlameFullRecharge = (int)(Addon.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Addon.SpellCharges("Concentrated Flame")));
      int GuardianCD = Addon.SpellCooldown("Guardian of Azeroth") - GCD;

      //Settings
      int PlayerHealth = Addon.Health("player");
      int TargetHealth = Addon.Health("Target");
      bool UsePotion = Addon.IsCustomCodeOn("Potions");
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");
      bool AOE = Addon.IsCustomCodeOn("AOE");

      //Class Power
      int Maelstrom = Addon.Power("player");
      int Mana = Addon.PlayerSecondaryPower();

      //Buffs
      bool BuffHailstormUp = Addon.HasBuff("Hailstorm", "player");
      bool BuffStormbringerUp = Addon.HasBuff("Stormbringer", "player");
      bool BuffFuryOfAirUp = Addon.HasBuff("Fury of Air", "player");
      bool TotemMasteryUp = Addon.HasBuff("Resonance Totem", "player");
      bool BuffFlametongueUp = Addon.HasBuff("Flametongue", "player");
      bool BuffFrostBrandUp = Addon.HasBuff("Frostbrand", "player");
      bool BuffAscendanceUp = Addon.HasBuff("Ascendance", "player");

      //Debuffs
      int DebuffPrimalStacks = Addon.DebuffStacks("Primal Primer");

      if (!AOE) {
        EnemiesNearTarget = 1;
        EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
      }

      if (IsChanneling || Addon.CastingID("player") == 299338) return false;

      if (UsePotion && Fighting) {
        if (Addon.CanUseItem(usableitems, false)) // don't check if equipped
        {
          if (MajorPower == "Guardian of Azeroth") {
            if (GuardianCD < 30000 || TargetTimeToDie < 60000) {
              Addon.Cast("potion", true);
              return true;
            }
          }
          else if (TargetTimeToDie < 60000) {
            Addon.Cast("potion", true);
            return true;
          }
        }
      }

      if (!NoCooldowns) {
        if (MajorPower == "Worldvein Resonance") {
          if (Addon.CanCast("Worldvein Resonance", "player")) {
            Addon.Cast("Worldvein Resonance");
            return true;
          }
        }

        if (MajorPower == "Blood of the Enemy" && ((AOE && EnemiesInMelee > 2) || (!AOE && Addon.Range("target") <= 10))) {
          if (Addon.CanCast("Blood of the Enemy", "player")) {
            Addon.Cast("Blood of the Enemy");
            return true;
          }
        }

        if (Addon.CanUseItem(usableitems, false) && usableitems != "None" && !UsePotion) {
          if (PlayerHealth <= GetSlider("Use item @ HP%")) {
            Addon.Cast("potion", true);
            return true;
          }
        }
      }

      // Big CDs
      if (Fighting) {
        if (Addon.CanCast("Totem Mastery", "player")) {
          if (!TotemMasteryUp || Addon.TotemTimer() < 2000) {
            Addon.Cast("Totem Mastery");
            return true;
          }
        }

        if (Addon.CanUseItem("Shiver Venom Relic")) {
          if (ShiverVenomStacks == 5) {
            Addon.Cast("Shiver Venom Relic", true);
            return true;
          }
        }

        if (MajorPower == "Guardian of Azeroth" && !NoCooldowns) {
          if (Addon.CanCast("Guardian of Azeroth", "player")) {
            if (TargetTimeToDie > 190000 || TargetTimeToDie < 32000) {
              Addon.Cast("Guardian of Azeroth");
              return true;
            }
          }
        }

        if (MajorPower == "Focused Azerite Beam" && !NoCooldowns) {
          if (Addon.CanCast("Focused Azerite Beam", "player")) {
            Addon.Cast("Focused Azerite Beam");
            return true;
          }
        }

        if (MajorPower == "The Unbound Force" && !NoCooldowns) {
          if (Addon.CanCast("The Unbound Force")) {
            Addon.Cast("The Unbound Force");
            return true;
          }
        }

        if (MajorPower == "Memory of Lucid Dreams" && !NoCooldowns) {
          if (Addon.CanCast("Memory of Lucid Dreams", "player")) {
            Addon.Cast("Memory of Lucid Dreams");
            return true;
          }
        }

        if (RacialPower == "Orc") {
          if (Addon.CanCast("Blood Fury", "player")) {
            Addon.Cast("Blood Fury", true);
            return true;
          }
        }

        if (RacialPower == "Troll") {
          if (Addon.CanCast("Berserking", "player")) {
            Addon.Cast("Berserking", true);
            return true;
          }
        }

        if (RacialPower == "Dark Iron Dwarf") {
          if (Addon.CanCast("Fireblood", "player")) {
            Addon.Cast("Fireblood", true);
            return true;
          }
        }

        if (RacialPower == "Mag'har Orc") {
          if (Addon.CanCast("Ancestral Call", "player")) {
            Addon.Cast("Ancestral Call", true);
            return true;
          }
        }

        if (Addon.CanUseItem("Neural Synapse Enhancer")) {
          Addon.Cast("nse", true);
          return true;
        }

        if (Addon.CanUseItem("Forbidden Obsidian Claw")) {
          Addon.Cast("Forbidden Obsidian Claw", true);
          return true;
        }

        if (Addon.CanUseItem("Manifesto of Madness")) {
          Addon.Cast("Manifesto of Madness", true);
          return true;
        }

        if (Addon.CanUseTrinket(0) && TopTrinket == "Generic") {
          Addon.Cast("TopTrink", true);
          return true;
        }

        if (Addon.CanUseTrinket(1) && BotTrinket == "Generic") {
          Addon.Cast("BotTrink", true);
          return true;
        }
      }

      //Mitigation
      if (Addon.CanCast("Astral Shift")); {
        if (PlayerHealth <= GetSlider("Astral Shift @ HP%")) {
          Addon.Cast("Astral Shift");
          return true;
        }
      }

      if (Addon.CanCast("Lightning Shield")) {
        if (!Addon.HasBuff("Lightning Shield", "player")) {
          Addon.Cast("Lightning Shield");
          return true;
        }
      }

      //Combat rotation
      if (Fighting) {
        //Preopener
        if (Addon.CanCast("Fury of Air", "player")) {
          if (!BuffFuryOfAirUp) {
            Addon.Cast("Fury of Air");
            return true;
          }
        }

        if (Addon.CanCast("Earth Elemental", "player") && !NoCooldowns) {
          if ((MajorPower != "Guardian of Azeroth" || GuardianCD > 150000 || TargetTimeToDie < 30000 || TargetTimeToDie > 155000 || !(GuardianCD + 30000 < TargetTimeToDie))) {
            Addon.Cast("Earth Elemental");
            return true;
          }
        }

        // 1 tar
        if (EnemiesInMelee < 2) {
          if (Addon.CanCast("Rockbiter")) {
            Addon.Cast("Rockbiter");
            return true;
          }

          if (Addon.CanCast("Flametongue")) {
            if (!BuffFlametongueUp) {
              Addon.Cast("Flametongue");
              return true;
            }
          }

          if (Addon.CanCast("Feral Spirit") && !NoCooldowns) {
            Addon.Cast("Feral Spirit");
            return true;
          }

          if (Addon.CanCast("Frostbrand")) {
            if (!BuffFrostBrandUp) {
              Addon.Cast("Frostbrand");
              return true;
            }
          }

          if (Addon.CanCast("Stormstrike")) {
            Addon.Cast("Stormstrike");
            return true;
          }

          if (Addon.CanCast("Sundering", "player") && Range < 4) {
            Addon.Cast("Sundering");
            return true;
          }

          if (Addon.CanCast("Earthen Spike")) {
            Addon.Cast("Earthen Spike");
            return true;
          }

          if (Addon.CanCast("Ascendance", "player") && !NoCooldowns) {
            Addon.Cast("Ascendance");
            return true;
          }

          if (Addon.CanCast("Windstrike")) {
            Addon.Cast("Windstrike");
            return true;
          }

          if (Addon.CanCast("Lava Lash")) {
            if (Maelstrom > 40) {
              Addon.Cast("Lava Lash");
              return true;
            }
          }
        }

        if (EnemiesInMelee > 1) {
          if (Addon.CanCast("Crash Lightning", "player")) {
            Addon.Cast("Crash Lightning");
            return true;
          }

          if (Addon.CanCast("Lava Lash")) {
            if (BuffFlametongueUp) {
              Addon.Cast("Lava Lash");
              return true;
            }
          }

          if (Addon.CanCast("Totem Mastery", "player")) {
            if (!TotemMasteryUp) {
              Addon.Cast("Totem Mastery");
              return true;
            }
          }

          if (Addon.CanCast("Sundering", "player") && Range < 4) {
            Addon.Cast("Sundering");
            return true;
          }

          if (Addon.CanCast("Flametongue")) {
            if (!BuffFlametongueUp) {
              Addon.Cast("Flametongue");
              return true;
            }
          }

          if (Addon.CanCast("Frostbrand")) {
            if (!BuffFrostBrandUp) {
              Addon.Cast("Frostbrand");
              return true;
            }
          }

          if (Addon.CanCast("Feral Spirit")) {
            Addon.Cast("Feral Spirit");
            return true;
          }

          if (Addon.CanCast("Stormstrike")) {
            if (BuffStormbringerUp) {
              Addon.Cast("Stormstrike");
              return true;
            }
          }

          if (Addon.CanCast("Stormstrike")) {
            if (BuffStormbringerUp) {
              Addon.Cast("Stormstrike");
              return true;
            }
          }

          if (Addon.CanCast("Rockbiter")) {
            Addon.Cast("Rockbiter");
            return true;
          }
        }

        if (Range > 4 && Range < 41) {
          if (Addon.CanCast("Lightning Bolt")) {
            Addon.Cast("Lightning Bolt");
            return true;
          }
        }

      }

      return false;
    }

    public override bool OutOfCombatTick() {

      bool Prepull = Addon.IsCustomCodeOn("Prepull");
      bool TotemMasteryUp = Addon.HasBuff("Resonance Totem");
      bool UsePotion = Addon.IsCustomCodeOn("Potions");
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");
      int TargetHealth = Addon.Health("target");
      int PlayerHealth = Addon.Health("player");
      bool Enemy = Addon.TargetIsEnemy();

      bool CastingLB = Addon.CastingID("player") == 51505 && Addon.CastingRemaining("player") < 500;

      if (Addon.CanCast("Ancestral Spirit", "target")) {
        if (TargetHealth < 1 && !Enemy) {
          Addon.Cast("Ancestral Spirit");
          return true;
        }
      }

      if (Addon.CanCast("Lightning Shield")) {
        if (!Addon.HasBuff("Lightning Shield", "player")) {
          Addon.Cast("Lightning Shield");
          return true;
        }
      }

      if (Prepull) {
        if (Addon.CanCast("Totem Mastery", "player")) {
          if (!TotemMasteryUp) {
            Addon.Cast("Totem Mastery");
            return true;
          }
        }

        if (Addon.CanUseItem("Azshara's Font of Power")) {
          Addon.Cast("Azshara's Font of Power", true);
          return true;
        }

        if (MajorPower == "Guardian of Azeroth" && !NoCooldowns) {
          if (Addon.CanCast("Guardian of Azeroth", "player")) {
            Addon.Cast("Guardian of Azeroth");
            return true;
          }
        }

        if (Addon.CanUseItem(usableitems, false) && usableitems != "None" && !UsePotion) {
          if (PlayerHealth <= GetSlider("Use item @ HP%")) {
            Addon.Cast("potion", true);
            return true;
          }
        }

        return false;
      }

      return false;
    }

  }
}