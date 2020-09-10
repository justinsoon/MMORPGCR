using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using AddonWow.API; //needed to access Addon API

namespace AddonWow.Modules {
  /// <summary>
  /// Created by JS
  /// Check API-DOC for detailed documentation.
  /// </summary>
  public class JSProtPally: Rotation {

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
        "Knot of Ancient Fury",
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
        "Bloodelf",
        "None"
      });
      Settings.Add(new Setting("Racial Power", Race, "None"));

      Settings.Add(new Setting("Mitigation"));
      Settings.Add(new Setting("DMG Reduction @ HP%", 0, 100, 80));
      Settings.Add(new Setting("Big DMG Reduction @ HP%", 0, 100, 70));
      Settings.Add(new Setting("Self Heal @ HP%", 0, 100, 100));
      Settings.Add(new Setting("Big Self Heal @ HP%", 0, 100, 45));
      Settings.Add(new Setting("Auto Bubble @ HP%", 0, 100, 25));
    }

    string MajorPower;
    string TopTrinket;
    string BotTrinket;
    string RacialPower;
    string usableitems;

    public override void Initialize() {
      // Addon.DebugMode();

      Addon.PrintMessage("JS: Prot Pally - v 1.1", Color.Yellow);
      Addon.PrintMessage("Recommended talents: 1233222", Color.Yellow);
      Addon.PrintMessage("These macros can be used for manual control:", Color.Blue);
      Addon.PrintMessage("/xxxxx Potions --Toggles using buff potions on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx SaveCooldowns --Toggles the use of big cooldowns on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx AOE --Toggles AOE heal mode on/off.", Color.Blue);
      Addon.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);

      Addon.Latency = 0;
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
      if (RacialPower == "Bloodelf") Spellbook.Add("Arcane Torrent");

      //Class spells
      Spellbook.Add("Flash of Light");
      Spellbook.Add("Judgment");
      Spellbook.Add("Lay on Hands");
      Spellbook.Add("Avenging Wrath");
      Spellbook.Add("Redemption");
      Spellbook.Add("Consecration");
      Spellbook.Add("Avenger's Shield");
      Spellbook.Add("Hammer of the Righteous");
      Spellbook.Add("Shield of the Righteous");
      Spellbook.Add("Light of the Protector");
      Spellbook.Add("Guardian of Ancient Kings");
      Spellbook.Add("Ardent Defender");
      Spellbook.Add("Divine Shield");
      Spellbook.Add("Final Stand");
      Spellbook.Add("Blessed Hammer");
      Spellbook.Add("Blinding Light");

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
      Buffs.Add("Consecration");

      //Class Buffs
      Buffs.Add("Beacon of Light");
      Buffs.Add("Judgment of Light");
      Buffs.Add("Shield of the Righteous");

      Debuffs.Add("Razor Coral");
      Debuffs.Add("Conductive Ink");
      Debuffs.Add("Shiver Venom");
      Debuffs.Add("Judgment");

      Items.Add(usableitems);

      Macros.Add("ItemUse", "/use " + usableitems);
      Macros.Add("TopTrink", "/use 13");
      Macros.Add("BotTrink", "/use 14");

      //Class Macros
      Macros.Add("fbop", "/cast [@focus] Blessing of Protection");

      CustomCommands.Add("Potions");
      CustomCommands.Add("SaveCooldowns");
      CustomCommands.Add("AOE");
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
      int FocusHealth = Addon.Health("focus");
      string LastCast = Addon.LastCast();
      bool IsChanneling = Addon.IsChanneling("player");
      int Mana = Addon.Power("player");
      int GCD = Addon.GCD();
      int GCDMAX = (int)(1500f / (Haste + 1f));
      bool Enemy = Addon.TargetIsEnemy();
      int EnemiesInMelee = Addon.EnemiesInMelee();

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
      int BuffRecklessForceRemains = Addon.BuffRemaining("Reckless Force") - GCD;
      bool BuffRecklessForceUp = BuffRecklessForceRemains > 0;
      int BuffRecklessForceStacks = Addon.BuffStacks("Reckless Force");

      //Settings
      bool AOEHeals = Addon.IsCustomCodeOn("AOE");
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");
      bool UsePotion = Addon.IsCustomCodeOn("Potions");

      // Buffs 
      bool BuffAvengingWrathUp = Addon.HasBuff("Avenging Wrath", "player");
      bool SotRUp = Addon.HasBuff("Shield of the Righteous", "player");
      bool ConsecrationUp = Addon.HasBuff("Consecration", "player");

      //Debuffs
      bool DebuffJudgmentUp = Addon.HasDebuff("Judgement", "target");

      if (IsChanneling || Addon.CastingID("player") == 295261 || Addon.CastingID("player") == 299338 || Addon.CastingID("player") == 295258 || Addon.CastingID("player") == 299336 || Addon.CastingID("player") == 295273 || Addon.CastingID("player") == 295264 || Addon.CastingID("player") == 295261 || Addon.CastingID("player") == 295263 || Addon.CastingID("player") == 295262 || Addon.CastingID("player") == 295272 || Addon.CastingID("player") == 299564) return false;

      if (!NoCooldowns) {
        if (Addon.CanUseItem(usableitems) && usableitems != "None" && !UsePotion) {
          if (EnemiesInMelee >= 1 && PlayerHealth <= GetSlider("Use item @ HP%")) {
            Addon.Cast("ItemUse", true);
            return true;
          }
        }
        if (RacialPower == "Lightforged Draenei" && Fighting) {
          if (Addon.CanCast("Light's Judgment", "player")) {
            if (true) //better to always use this
            {
              Addon.Cast("Light's Judgment", true);
              return true;
            }
          }
        }

        if (RacialPower == "Dark Iron Dwarf" && Fighting) {
          if (BuffAvengingWrathUp) {
            if (Addon.CanCast("Fireblood", "player")) {
              Addon.Cast("Fireblood", true);
              return true;
            }
          }
        }

        if (Addon.CanUseItem(TopTrinket)) {
          if (BuffAvengingWrathUp) {
            Addon.Cast(TopTrinket, true);
            return true;
          }
        }
        if (Addon.CanUseItem(BotTrinket)) {
          if (BuffAvengingWrathUp) {
            Addon.Cast(BotTrinket, true);
            return true;
          }
        }

        if (Addon.CanUseTrinket(0) && TopTrinket == "Generic") {
          if (BuffAvengingWrathUp) {
            Addon.Cast("TopTrink", true);
            return true;
          }
        }

        if (Addon.CanUseTrinket(1) && BotTrinket == "Generic") {
          if (BuffAvengingWrathUp) {
            Addon.Cast("BotTrink", true);
            return true;
          }
        }

        if (MajorPower == "Reaping Flames" && (TargetHealth > 80 || TargetHealth < 20)) {
          if (Addon.CanCast("Reaping Flames")) {
            Addon.Cast("Reaping Flames");
            return true;
          }
        }
        if (Addon.CanCast("Shield of Vengeance", "player") && Fighting) {
          if (!BuffMemoryOfLucidDreamsUp) {
            Addon.Cast("Shield of Vengeance");
            return true;
          }
        }

        if (Addon.CanUseItem("Ashvane's Razor Coral")) {
          if (!DebuffRazorCoralUp || MajorPower != "Guardian of Azeroth") {
            Addon.Cast("Ashvane's Razor Coral", true);
            return true;
          }
        }
        if (MajorPower == "The Unbound Force") {
          if (Addon.CanCast("The Unbound Force")) {
            if (Time <= 2000 || BuffRecklessForceUp) {
              Addon.Cast("The Unbound Force");
              return true;
            }
          }
        }
        if (MajorPower == "Blood of the Enemy" && EnemiesInMelee > 0) {
          if (Addon.CanCast("Blood of the Enemy", "player")) {
            if (BuffAvengingWrathUp) {
              Addon.Cast("Blood of the Enemy");
              return true;
            }
          }
        }
        if (MajorPower == "Guardian of Azeroth" && Fighting) {
          if (Addon.CanCast("Guardian of Azeroth", "player")) {
            Addon.Cast("Guardian of Azeroth");
            return true;
          }
        }
        if (MajorPower == "Worldvein Resonance" && Fighting) {
          if (Addon.CanCast("Worldvein Resonance", "player")) {
            Addon.Cast("Worldvein Resonance");
            return true;
          }
        }
        if (MajorPower == "Focused Azerite Beam" && Range < 15) {
          if (Addon.CanCast("Focused Azerite Beam", "player")) {
            if (!BuffAvengingWrathUp) {
              Addon.Cast("Focused Azerite Beam");
              return true;
            }
          }
        }
        if (MajorPower == "Memory of Lucid Dreams" && Fighting) {
          if (Addon.CanCast("Memory of Lucid Dreams", "player")) {
            if (BuffAvengingWrathUp) {
              Addon.Cast("Memory of Lucid Dreams");
              return true;
            }
          }
        }
        if (Addon.CanUseItem("Pocket-Sized Computation Device")) {
          if (!BuffAvengingWrathUp) {
            Addon.Cast("Pocket-Sized Computation Device", true);
            return true;
          }
        }
        if (Addon.CanCast("Avenging Wrath", "player") && Fighting && LastCast == "Consecration") {
          Addon.Cast("Avenging Wrath");
          return true;
        }

        if (Addon.CanCast("Aura Mastery", "player") && Fighting) {
          Addon.Cast("Aura Mastery");
          return true;
        }

        if (Addon.CanCast("Holy Avenger", "player") && Fighting) {
          Addon.Cast("Holy Avenger");
          return true;
        }
      }
      if (MajorPower == "Concentrated Flame") {
        if (Addon.CanCast("Concentrated Flame") && FlameFullRecharge < GCDMAX) {
          Addon.Cast("Concentrated Flame");
          return true;
        }
      }
      if (MajorPower == "Reaping Flames") {
        if (Addon.CanCast("Reaping Flames")) {
          Addon.Cast("Reaping Flames");
          return true;
        }
      }
      if (RacialPower == "Bloodelf" && Fighting) {
        if (Addon.CanCast("Arcane Torrent", "player")) {
          Addon.Cast("Arcane Torrent");
          return true;
        }
      }

      if (Enemy && Fighting) {
        if (PlayerHealth <= GetSlider("DMG Reduction @ HP%")) {
          if (Addon.CanCast("Guardian of Ancient Kings", "player") && !NoCooldowns) {
            Addon.Cast("Guardian of Ancient Kings");
            return true;
          }
        }
        if (PlayerHealth <= GetSlider("Big DMG Reduction @ HP%")) {
          if (Addon.CanCast("Ardent Defender", "player") && !NoCooldowns) {
            Addon.Cast("Ardent Defender");
            return true;
          }
        }
        if (Addon.CanCast("Shield of the Righteous", "player")) {
          if (!SotRUp) {
            Addon.Cast("Shield of the Righteous");
            return true;
          }
        }
        if (Addon.CanCast("Consecration", "player") && Range < 4) {
          if (!ConsecrationUp) {
            Addon.Cast("Consecration");
            return true;
          }
        }
        if (Addon.CanCast("Blessed Hammer", "player")) {
          Addon.Cast("Blessed Hammer");
          return true;
        }
        if (Addon.CanCast("Avenger's Shield")) {
          Addon.Cast("Avenger's Shield");
          return true;
        }
        if (Addon.CanCast("Hammer of the Righteous")) {
          Addon.Cast("Hammer of the Righteous");
          return true;
        }
        if (Addon.CanCast("Judgment")) {
          Addon.Cast("Judgment");
          return true;
        }
        if (EnemiesInMelee > 2 && !NoCooldowns) {
          if (Addon.CanCast("Blinding Light")) {
            Addon.Cast("Blinding Light");
            return true;
          }
        }
      }

      if (PlayerHealth <= GetSlider("Self Heal @ HP%")) {
        if (Addon.CanCast("Light of the Protector", "player")) {
          Addon.Cast("Light of the Protector");
          return true;
        }
      }

      if (PlayerHealth <= GetSlider("Big Self Heal @ HP%")) {
        if (Addon.CanCast("Lay on Hands", "player")) {
          Addon.Cast("Lay on Hands");
          return true;
        }
      }

      if (PlayerHealth <= GetSlider("Auto Bubble @ HP%")) {
        if (Addon.CanCast("Final Stand", "player")) {
          Addon.Cast("Final Stand");
          return true;
        } else {
          if (Addon.CanCast("Divine Shield", "player")) {
            Addon.Cast("Divine Shield");
            return true;
          }
        }
      }

      if (!Enemy && TargetHealth <= GetSlider("Auto Bubble @ HP%")) {
        if (Addon.CanCast("Blessing of Protection", "target")) {
          Addon.Cast("Blessing of Protection");
          return true;
        }

        if (Addon.CanCast("Blessing of Protection", "focus")) {
          Addon.Cast("fbop");
          return true;
        }
      }
      return false;
    }

    public override bool OutOfCombatTick() {
      return false;
    }

  }
}