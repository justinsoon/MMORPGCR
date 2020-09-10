using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using AddonWow.API; //needed to access Addon API

namespace AddonWow.Modules {
  /// <summary>
  /// Created by JS
  /// Check API-DOC for detailed documentation.
  /// </summary>
  public class JSHolyPally: Rotation {

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

      Settings.Add(new Setting("Heal Settings"));
      Settings.Add(new Setting("Holy Shock @ HP%", 0, 100, 85));
      Settings.Add(new Setting("Bestow Faith @ HP%", 0, 100, 90));
      Settings.Add(new Setting("Holy Light @ HP%", 0, 100, 95));
      Settings.Add(new Setting("Flash of Light @ HP%", 0, 100, 70));
      Settings.Add(new Setting("Light of Dawn @ HP%", 0, 100, 60));
      Settings.Add(new Setting("Lay on Hands @ HP%", 0, 100, 20));
      Settings.Add(new Setting("Blessing of Sacrifice @ HP%", 0, 100, 60));
      Settings.Add(new Setting("Divine Protection @ HP%", 0, 100, 30));
      Settings.Add(new Setting("Divine Sheild @ HP%", 0, 100, 10));
      Settings.Add(new Setting("Blessing of Protection @ HP%", 0, 100, 0));
    }

    string MajorPower;
    string TopTrinket;
    string BotTrinket;
    string RacialPower;
    string usableitems;

    public override void Initialize() {
      // Addon.DebugMode();

      Addon.PrintMessage("JS: Holy Pally - v 1.0", Color.Yellow);
      Addon.PrintMessage("Recommended talents: 3313213", Color.Yellow);
      Addon.PrintMessage("These macros can be used for manual control:", Color.Blue);
      Addon.PrintMessage("/xxxxx Potions", Color.Blue);
      Addon.PrintMessage("--Toggles using buff potions on/off.", Color.Blue);
      Addon.PrintMessage(" ");
      Addon.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
      Addon.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
      Addon.PrintMessage(" ");
      Addon.PrintMessage("/xxxxx AOE", Color.Blue);
      Addon.PrintMessage("--Toggles AOE heal mode on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx notarbol", Color.Red);
      Addon.PrintMessage("--Toggles Beacon of Light on target on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx notarprio", Color.Red);
      Addon.PrintMessage("--Toggles target mitigation priority. Will cast on focus instead. on/off.", Color.Blue);
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
      Spellbook.Add("Beacon of Light");
      Spellbook.Add("Holy Shock");
      Spellbook.Add("Bestow Faith");
      Spellbook.Add("Holy Light");
      Spellbook.Add("Flash of Light");
      Spellbook.Add("Judgment");
      Spellbook.Add("Light of Dawn");
      Spellbook.Add("Light of the Martyr");
      Spellbook.Add("Crusader Strike");
      Spellbook.Add("Lay on Hands");
      Spellbook.Add("Avenging Wrath");
      Spellbook.Add("Blessing of Sacrifice");
      Spellbook.Add("Divine Protection");
      Spellbook.Add("Divine Shield");
      Spellbook.Add("Aura Mastery");
      Spellbook.Add("Holy Avenger");
      Spellbook.Add("Blessing of Protection");
      Spellbook.Add("Blessing of Freedom");
      Spellbook.Add("Holy Prism");
      Spellbook.Add("Redemption");
      Spellbook.Add("Consecration");

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

      //Class Buffs
      Buffs.Add("Beacon of Light");
      Buffs.Add("Judgment of Light");
      Buffs.Add("Consecration");

      Debuffs.Add("Razor Coral");
      Debuffs.Add("Conductive Ink");
      Debuffs.Add("Shiver Venom");
      Debuffs.Add("Judgment");

      Items.Add(TopTrinket);
      Items.Add(BotTrinket);
      Items.Add(usableitems);

      Macros.Add("ItemUse", "/use " + usableitems);
      Macros.Add("TopTrink", "/use 13");
      Macros.Add("BotTrink", "/use 14");

      Macros.Add("floh", "/cast [@focus] Lay on Hands");
      Macros.Add("fbol", "/cast [@focus] Beacon of Light");
      Macros.Add("fdp", "/cast [@focus] Divine Protection");
      Macros.Add("fds", "/cast [@focus] Divine Shield");
      Macros.Add("fbop", "/cast [@focus] Blessing of Protection");

      CustomCommands.Add("Potions");
      CustomCommands.Add("SaveCooldowns");
      CustomCommands.Add("AOE");
      CustomCommands.Add("notarbol");
      CustomCommands.Add("notarprio");
    }

    // optional override for the CombatTick which executes while in combat
    public override bool CombatTick() {

      bool Fighting = Addon.Range("target") <= 8 && Addon.TargetIsEnemy();
      bool Moving = Addon.PlayerIsMoving();
      float Haste = Addon.Haste() / 100f;
      int Time = Addon.CombatTime();
      int Range = Addon.Range("target");
      int TargetHealth = Addon.Health("target");
      int FocusHealth = Addon.Health("focus");
      int PlayerHealth = Addon.Health("player");
      string LastCast = Addon.LastCast();
      bool IsChanneling = Addon.IsChanneling("player");
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
      bool TargetBOL = Addon.IsCustomCodeOn("notarbol");
      bool AOEHeals = Addon.IsCustomCodeOn("AOE");
      bool NoCooldowns = Addon.IsCustomCodeOn("SaveCooldowns");
      bool UsePotion = Addon.IsCustomCodeOn("Potions");
      bool TargetPrio = Addon.IsCustomCodeOn("notarprio");

      // Buffs 
      bool BuffBOLUp = Addon.HasBuff("Beacon of Light", "target");
      bool BuffFBOLUp = Addon.HasBuff("Beacon of Light", "focus");
      bool BuffAvengingWrathUp = Addon.HasBuff("Avenging Wrath", "player");
      bool ConUp = Addon.HasBuff("Consecration", "player");

      //Debuffs
      bool DebuffJudgmentUp = Addon.HasDebuff("Judgement", "target");

      //CD
      int CDHolyShock = Addon.SpellCooldown("Holy Shock") - GCD;
      bool HolyShockDown = CDHolyShock > 10;

      int Mana = Addon.Power("player");

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
        if (Addon.CanCast("Avenging Wrath", "player") && Fighting) {
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
        if (Addon.CanCast("Consecration", "player") && Range < 4) {
          if (!ConUp) {
            Addon.Cast("Consecration");
            return true;
          }
        }
        if (Addon.CanCast("Judgment")) {
          Addon.Cast("Judgment");
          return true;
        }
        if (Addon.CanCast("Crusader Strike")) {
          Addon.Cast("Crusader Strike");
          return true;
        }
        if (Addon.CanCast("Holy Shock", "target")) {
          Addon.Cast("Holy Shock");
          return true;
        }
      } else {
        if (Addon.CanCast("Beacon of Light", "target")) {
          if (!BuffBOLUp && !TargetBOL) {
            Addon.Cast("Beacon of Light");
            return true;
          }
        }
        if (Addon.CanCast("Beacon of Light", "focus")) {
          if (!BuffFBOLUp) {
            Addon.Cast("fbol");
            return true;
          }
        }
        if (Addon.CanCast("Holy Shock", "target")) {
          if (TargetHealth <= GetSlider("Holy Shock @ HP%")) {
            Addon.Cast("Holy Shock");
            return true;
          }
        }
        if (Addon.CanCast("Light of the Martyr", "target")) {
          if (HolyShockDown && TargetHealth <= GetSlider("Holy Shock @ HP%")) {
            Addon.Cast("Light of the Martyr");
            return true;
          }
        }
        if (Addon.CanCast("Bestow Faith", "target")) {
          if (TargetHealth <= GetSlider("Bestow Faith @ HP%")) {
            Addon.Cast("Bestow Faith");
            return true;
          }
        }
        if (Addon.CanCast("Holy Light", "target") && !Moving) {
          if (TargetHealth <= GetSlider("Holy Light @ HP%")) {
            Addon.Cast("Holy Light");
            return true;
          }
        }
        if (Addon.CanCast("Flash of Light", "target")) {
          if (TargetHealth <= GetSlider("Flash of Light @ HP%") && !Moving) {
            Addon.Cast("Flash of Light");
            return true;
          }
        }
        if (Addon.CanCast("Lay on Hands", "target")) {
          if (TargetHealth <= GetSlider("Lay on Hands @ HP%") && !TargetPrio) {
            Addon.Cast("Lay on Hands");
            return true;
          }
        }
        if (Addon.CanCast("Lay on Hands", "focus")) {
          if (FocusHealth <= GetSlider("Lay on Hands @ HP%")) {
            Addon.Cast("floh");
            return true;
          }
        }
        if (Addon.CanCast("Blessing of Sacrifice", "target")) {
          if (TargetHealth <= GetSlider("Blessing of Sacrifice @ HP%")) {
            Addon.Cast("Blessing of Sacrifice");
            return true;
          }
        }
        if (Addon.CanCast("Divine Protection", "target")) {
          if (TargetHealth <= GetSlider("Divine Protection @ HP%") && !TargetPrio) {
            Addon.Cast("Divine Protection");
            return true;
          }
        }
        if (Addon.CanCast("Divine Protection", "focus")) {
          if (FocusHealth <= GetSlider("Divine Protection @ HP%")) {
            Addon.Cast("fdp");
            return true;
          }
        }
        if (Addon.CanCast("Divine Shield", "target")) {
          if (TargetHealth <= GetSlider("Divine Sheild @ HP%") && !TargetPrio) {
            Addon.Cast("Divine Shield");
            return true;
          }
        }
        if (Addon.CanCast("Divine Shield", "focus")) {
          if (FocusHealth <= GetSlider("Divine Shield @ HP%")) {
            Addon.Cast("fds");
            return true;
          }
        }
        if (Addon.CanCast("Blessing of Protection", "target")) {
          if (TargetHealth <= GetSlider("Blessing of Protection @ HP%") && !TargetPrio) {
            Addon.Cast("Blessing of Protection");
            return true;
          }
        }
        if (Addon.CanCast("Blessing of Protection", "focus")) {
          if (FocusHealth <= GetSlider("Blessing of Protection @ HP%")) {
            Addon.Cast("fbop");
            return true;
          }
        }

        if (AOEHeals) {
          if (Addon.CanCast("Light of Dawn", "player")) {
            if (Range < 4) {
              Addon.Cast("Light of Dawn");
              return true;
            }
          }
          if (Addon.CanCast("Holy Prism", "player")) {
            Addon.Cast("Holy Prism");
            return true;
          }
        }
      }
      return false;
    }

    public override bool OutOfCombatTick() {
      int TargetHealth = Addon.Health("target");
      int FocusHealth = Addon.Health("focus");
      bool Moving = Addon.PlayerIsMoving();

      //Settings
      bool TargetBOL = Addon.IsCustomCodeOn("notarbol");
      bool AOEHeals = Addon.IsCustomCodeOn("AOE");

      // Buffs 
      bool BuffBOLUp = Addon.HasBuff("Beacon of Light", "target");
      bool BuffFBOLUp = Addon.HasBuff("Beacon of Light", "focus");

      if (Addon.CanCast("Beacon of Light", "target")) {
        if (!BuffBOLUp && !TargetBOL) {
          Addon.Cast("Beacon of Light");
          return true;
        }
      }
      if (Addon.CanCast("Holy Shock", "target")) {
        if (TargetHealth <= GetSlider("Holy Shock @ HP%")) {
          Addon.Cast("Holy Shock");
          return true;
        }
      }
      if (Addon.CanCast("Bestow Faith", "target")) {
        if (TargetHealth <= GetSlider("Bestow Faith @ HP%")) {
          Addon.Cast("Bestow Faith");
          return true;
        }
      }
      if (Addon.CanCast("Holy Light", "target") && !Moving) {
        if (TargetHealth <= GetSlider("Holy Light @ HP%")) {
          Addon.Cast("Holy Light");
          return true;
        }
      }
      if (Addon.CanCast("Flash of Light", "target")) {
        if (TargetHealth <= GetSlider("Flash of Light @ HP%") && !Moving) {
          Addon.Cast("Flash of Light");
          return true;
        }
      }
      if (Addon.CanCast("Redemption", "target")) {
        if (TargetHealth < 1) {
          Addon.Cast("Redemption");
          return true;
        }
      }

      return false;
    }

  }
}