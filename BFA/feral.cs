using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AddonWow.API; //needed to access Addon API

namespace AddonWow.Modules {
  /// <summary>
  //// Created by JS
  /// Check API-DOC for detailed documentation.
  /// </summary>
  public class JSFeralDruid: Rotation {

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
      Settings.Add(new Setting("Major Power", MajorAzeritePower, "Focused Azerite Beam"));

      List < string > Trinkets = new List < string > (new string[] {
        "Generic",
        "None"
      });
      Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
      Settings.Add(new Setting("Top Trinket AOE?", true));
      Settings.Add(new Setting("Bot Trinket", Trinkets, "Generic"));
      Settings.Add(new Setting("Bot Trinket AOE?", true));

      Settings.Add(new Setting("Use item: Case Sens", "None"));
      Settings.Add(new Setting("Use item @ HP%", 0, 100, 100));

      List < string > Race = new List < string > (new string[] {
        "Tauren",
        "Troll",
        "Night Elf",
        "None"
      });
      Settings.Add(new Setting("Racial Power", Race, "Night Elf"));
      Settings.Add(new Setting("Auto Mechanics"));
      Settings.Add(new Setting("Auto Heal @ HP%", 0, 100, 100));
      Settings.Add(new Setting("Auto Buff @ HP%", 0, 100, 80));
      Settings.Add(new Setting("Auto Thorns @ HP%", 0, 100, 70));
    }

    string MajorPower;
    string TopTrinket;
    string BotTrinket;
    string RacialPower;
    string usableitems;
    string[] dispelables = {
      "Hex",
      "Hex of Weakness",
      "Bane of Agony",
      "Bane of Doom",
      "Bane of Havoc",
      "Doom and Gloom",
      "Pandemic",
      "Agony",
      "Doom",
      "Havoc",
      "Curse of Weakness",
      "Curse of Tongues",
      "Curse of the Elements",
      "Curse of Exhaustion",
      "Curse of Shadow",
      "Curse of Recklessness",
      "Crippling Poison",
      "Mind-numbing Poison",
      "Leeching Poison",
      "Agonizing Poison",
      "Deadly Poison",
      "Wound Poison",
      "Hysteria"
    };

    string[] sootheables = {
      "Vengeance",
      "Enrage",
      "Berserker Rage",
      "Wrecking Crew",
      "Bloodbath",
      "Unholy Frenzy",
      "Owlkin Frenzy",
      "Savage Roar"
    };


    public override void Initialize() {
      //Addon.DebugMode();

      Addon.PrintMessage("JS Series: Feral Druid - v 2.5.2", Color.Yellow);
      Addon.PrintMessage("WarcraftLogs&Sims PVE / Skilledcap PVP", Color.Yellow);
      Addon.PrintMessage("PVE Recommended talents: 1313232", Color.Yellow);
      Addon.PrintMessage("PVP Recommended talents: 3331222", Color.Yellow);
      Addon.PrintMessage("Default mode: PVE, AOE ON, Auto Cat, USE CDs/Pots", Color.Yellow);
      Addon.PrintMessage("These macros can be used for manual control:", Color.Yellow);
      Addon.PrintMessage("/xxxxx savecd --Toggles the use of big cooldowns && potions on/off.", Color.Blue);
      Addon.PrintMessage("/xxxxx toggle --Toggles the entire rotation.", Color.Blue);
      Addon.PrintMessage("/xxxxx nocat --Toggles Auto Cat Form within melee on/off.", Color.Red);
      Addon.PrintMessage("/xxxxx noaoe --Toggles NO AOE mode on/off.", Color.Red);
      Addon.PrintMessage("/xxxxx pvp --Toggles PVP rotation mode on/off.", Color.Red);
      Addon.PrintMessage("/xxxxx burst --Toggles PVP Burst on/off.", Color.Red);
      Addon.PrintMessage("/xxxx ccc -- Toggles Cyclone interrupts in PVP.", Color.Red);
      Addon.PrintMessage("/xxxx savepp -- Toggles prepull buffs in rotation.", Color.Red);
      Addon.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Red);

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
      if (RacialPower == "Night Elf") {
        Spellbook.Add("Shadowmeld");
        Macros.Add("cleartar", "/cleartarget");
        Macros.Add("tar", "/targetenemyplayer");
      }

      Spellbook.Add("Tiger's Fury");
      Spellbook.Add("Berserk");
      Spellbook.Add("Rip");
      Spellbook.Add("Rake");
      Spellbook.Add("Thrash");
      Spellbook.Add("Ferocious Bite");
      Spellbook.Add("Shred");
      Spellbook.Add("Swipe");
      Spellbook.Add("Regrowth");
      Spellbook.Add("Entangling Roots");
      Spellbook.Add("Moonfire");
      Spellbook.Add("Survival Instincts");
      Spellbook.Add("Primal Wrath");
      Spellbook.Add("Savage Roar");
      Spellbook.Add("Cat Form");
      Spellbook.Add("Prowl");
      Spellbook.Add("Feral Frenzy");
      Spellbook.Add("Moonkin Form");
      Spellbook.Add("Starsurge");
      Spellbook.Add("Sunfire");
      Spellbook.Add("Solar Wrath");
      Spellbook.Add("Lunar Strike");
      Spellbook.Add("Remove Corruption");
      Spellbook.Add("Rebirth");
      Spellbook.Add("Thorns");
      Spellbook.Add("Cyclone");
      Spellbook.Add("Revive");
      Spellbook.Add("Brutal Slash");
      Spellbook.Add("Skull Bash");
      Spellbook.Add("Wild Charge");
      Spellbook.Add("Maim");
      Spellbook.Add("Soothe");

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

      Buffs.Add("Prowl");
      Buffs.Add("Cat Form");
      Buffs.Add("Moonkin Form");
      Buffs.Add("Predatory Swiftness");
      Buffs.Add("Survival Instincts");
      Buffs.Add("Savage Roar");
      Buffs.Add("Berserk");
      Buffs.Add("Solar Empowerment");
      Buffs.Add("Lunar Empowerment");
      Buffs.Add("Shadowmeld");

      Debuffs.Add("Razor Coral");
      Debuffs.Add("Conductive Ink");
      Debuffs.Add("Shiver Venom");
      Debuffs.Add("Rip");
      Debuffs.Add("Rake");
      Debuffs.Add("Thrash");
      Debuffs.Add("Moonfire");
      Debuffs.Add("Mighty Bash");
      Debuffs.Add("Maim");
      Debuffs.Add("Sunfire");

      foreach(string dispelable in dispelables) {
        Debuffs.Add(dispelable);
      }

      foreach(string sootheable in sootheables) {
        Buffs.Add(sootheable);
      }

      Items.Add(TopTrinket);
      Items.Add(BotTrinket);
      Items.Add(usableitems);

      Macros.Add("ItemUse", "/use " + usableitems);
      Macros.Add("TopTrink", "/use 13");
      Macros.Add("BotTrink", "/use 14");
      Macros.Add("selfheal", "/cast [@player] Regrowth");
      Macros.Add("sunfirem", "/cast Sunfire(Solar)");

      CustomCommands.Add("savecd");
      CustomCommands.Add("noaoe");
      CustomCommands.Add("pvp");
      CustomCommands.Add("nocat");
      CustomCommands.Add("burst");
      CustomCommands.Add("ccc");
      CustomCommands.Add("savepp");
    }

    Stopwatch CombatTimer = new Stopwatch();
    // optional override for the CombatTick which executes while in combat
    public override bool CombatTick() {
      //init
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
      int EnemiesInMelee = Addon.EnemiesInMelee();
      int EnemiesNearTarget = Addon.EnemiesNearTarget();
      bool Enemy = Addon.TargetIsEnemy();
      int GCDMAX = (int)(1500f / (Haste + 1f));
      int GCD = Addon.GCD();
      int Latency = Addon.Latency;
      int TargetTimeToDie = 1000000000;
      bool HasLust = Addon.HasBuff("Bloodlust", "player", false) || Addon.HasBuff("Heroism", "player", false) || Addon.HasBuff("Time Warp", "player", false) || Addon.HasBuff("Ancient Hysteria", "player", false) || Addon.HasBuff("Netherwinds", "player", false) || Addon.HasBuff("Drums of Rage", "player", false);
      int FlameFullRecharge = (int)(Addon.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Addon.SpellCharges("Concentrated Flame")));

      // Settings
      bool TopTrinketAOE = GetCheckBox("Top Trinket AOE?");
      bool BotTrinketAOE = GetCheckBox("Bot Trinket AOE?");

      //Commands
      bool PVPMode = Addon.IsCustomCodeOn("pvp");
      bool noautocat = Addon.IsCustomCodeOn("nocat");
      bool PVPBurstMode = Addon.IsCustomCodeOn("burst");
      bool NoAOEMode = Addon.IsCustomCodeOn("noaoe");
      bool NoCooldowns = Addon.IsCustomCodeOn("savecd");
      bool CycloneCC = Addon.IsCustomCodeOn("ccc");

      //Traits
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

      int Energy = Addon.Power("player");
      int ComboPoints = Addon.PlayerSecondaryPower();
      int MaxEnergy = Addon.PlayerMaxPower();
      int EnergyDefecit = MaxEnergy - Energy;
      int MaxComboPoints = 5;
      int ComboPointsDefecit = MaxComboPoints - ComboPoints;

      //Spells
      int SurvivalInstinctsCharges = Addon.SpellCharges("Survival Instincts");

      //Talents
      bool TalentPrimalWrath = Addon.Talent(6, 3);
      bool TalentBloodTalons = Addon.Talent(7, 2);
      bool TalentSabertooth = Addon.Talent(1, 2);
      bool TalentLunarInspiration = Addon.Talent(1, 3);
      bool TalentSavageRoar = Addon.Talent(5, 2);
      bool TalentFeralFrenzy = Addon.Talent(7, 3);
      bool TalentBrutalSlash = Addon.Talent(6, 2);
      bool TalentRange = Addon.Talent(3, 1);

      //Debuffs
      bool DebuffRipUp = Addon.DebuffRemaining("Rip") - GCD > 0;
      bool DebuffRakeUp = Addon.DebuffRemaining("Rake") - GCD > 0;
      bool DebuffThrashUp = Addon.DebuffRemaining("Thrash") - GCD > 0;
      bool DebuffMoonfireUp = Addon.DebuffRemaining("Moonfire") - GCD > 0;
      bool DebuffMaimUp = Addon.DebuffRemaining("Maim") - GCD > 0;
      bool DebuffMightyBashUp = Addon.DebuffRemaining("Mighty Bash") - GCD > 0;
      bool DebuffSunfireUp = Addon.DebuffRemaining("Sunfire") - GCD > 0;

      //Buffs
      bool BuffStealthUp = Addon.HasBuff("Prowl");
      bool BuffTigerFuryUp = Addon.HasBuff("Tiger's Fury", "player");
      bool BuffBerserkUp = Addon.HasBuff("Berserk", "player");
      bool BuffSurvivalInstinctsUp = Addon.HasBuff("Survival Instincts", "player");
      bool PredatorySwiftnessUp = Addon.HasBuff("Predatory Swiftness", "player");
      bool SavageRoarUp = Addon.HasBuff("Savage Roar", "player");
      bool LunarEmpowermentUp = Addon.HasBuff("Lunar Empowerment", "player");
      bool SolarEmpowermentUp = Addon.HasBuff("Solar Empowerment", "player");

      //CDBuffs
      int CDTigerFuryRemains = Addon.SpellCooldown("Tiger's Fury") - GCD;
      bool CDTigerFuryReady = CDTigerFuryRemains <= 10;
      int CDBerserkRemains = Addon.SpellCooldown("Berserk") - GCD;
      bool CDBerserkReady = CDBerserkRemains <= 10;
      int BeamReamins = Addon.SpellCooldown("Focused Azerite Beam") - GCD;
      bool CDBeamReady = BeamReamins <= 10;

      if (NoAOEMode) {
        EnemiesNearTarget = 1;
        EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
      }

      //Combat Timer
      if (Fighting) {
        if (!CombatTimer.IsRunning) CombatTimer.Start();
      }
      if (TargetHealth < 1) CombatTimer.Reset();

      float EnergyRegen = 10f * (1f + Haste) * (1f);
      int TimeUntilMaxEnergy = (int)((EnergyDefecit * 1000f) / EnergyRegen);

      bool Stealthed = BuffStealthUp;
      bool Bleeds = DebuffRipUp || DebuffThrashUp || DebuffRakeUp;
      int DotBleeds = ((DebuffRipUp ? 1 : 0) + (DebuffThrashUp ? 1 : 0)) * (Bleeds ? 1 : 0);
      float VariableEnergyRegenCombined = EnergyRegen + DotBleeds * 7 % (2 * SpellHaste);

      if (IsChanneling || Addon.CastingID("player") == 295261 || Addon.CastingID("player") == 299338 || Addon.CastingID("player") == 295258 || Addon.CastingID("player") == 299336 || Addon.CastingID("player") == 295273 || Addon.CastingID("player") == 295264 || Addon.CastingID("player") == 295261 || Addon.CastingID("player") == 295263 || Addon.CastingID("player") == 295262 || Addon.CastingID("player") == 295272 || Addon.CastingID("player") == 299564) return false;

      if (!Addon.HasBuff("Moonkin Form") && Addon.CanCast("Moonkin Form", "player") && Addon.TargetIsEnemy() && Range > 20) {
        Addon.Cast("Moonkin Form");
        return true;
      }

      if (!noautocat) {
        if (!Addon.HasBuff("Cat Form") && Addon.CanCast("Cat Form", "player") && Fighting && Range < 3) {
          Addon.Cast("Cat Form");
          return true;
        }
      }

      if (!Stealthed && DebuffRakeUp && !NoCooldowns && Fighting) {
        if (MajorPower == "Concentrated Flame") {
          if (Addon.CanCast("Concentrated Flame")) {
            if (TimeUntilMaxEnergy > 1000 && (FlameFullRecharge < GCDMAX)) {
              Addon.Cast("Concentrated Flame");
              return true;
            }
          }
        }

        if (MajorPower == "Blood of the Enemy" && EnemiesInMelee > 0) {
          if (Addon.CanCast("Blood of the Enemy", "player")) {
            if (ComboPointsDefecit <= 1 || TargetTimeToDie <= 10000) {
              Addon.Cast("Blood of the Enemy");
              return true;
            }
          }
        }

        if (MajorPower == "Guardian of Azeroth") {
          if (Addon.CanCast("Guardian of Azeroth", "player")) {
            if (TargetTimeToDie < 30000) {
              Addon.Cast("Guardian of Azeroth");
              return true;
            }
          }
        }

        if (MajorPower == "Reaping Flames") {
          if (Addon.CanCast("Reaping Flames")) {
            if (!BuffBerserkUp && TimeUntilMaxEnergy > 1000) {
              Addon.Cast("Reaping Flames");
              return true;
            }
          }
        }

        if (MajorPower == "Focused Azerite Beam" && Range < 15 && !Moving && LastCast != "Shadowmeld") {
          if (Addon.CanCast("Focused Azerite Beam", "player")) {
            if (Fighting && Energy < 31 && ComboPoints < 4 && TargetHealth >= 10) {
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

        if (MajorPower == "Worldvein Resonance") {
          if (Addon.CanCast("Worldvein Resonance", "player")) {
            Addon.Cast("Worldvein Resonance");
            return true;
          }
        }

        if (MajorPower == "Memory of Lucid Dreams") {
          if (Addon.CanCast("Memory of Lucid Dreams", "player")) {
            if (Energy < 50 && !CDBerserkReady) {
              Addon.Cast("Memory of Lucid Dreams");
              return true;
            }
          }
        }
      }

      //generic trinket usage
      if (!NoCooldowns && !Stealthed && Fighting) {
        if (Addon.CanUseTrinket(0) && TopTrinket == "Generic") {
          if (!TopTrinketAOE) {
            Addon.Cast("TopTrink", true);
            return true;
          }
          if (TopTrinketAOE && EnemiesInMelee >= 1 && (LastCast == "Rip" || LastCast == "Primal Wrath")) {
            Addon.Cast("TopTrink", true);
            return true;
          }
        }

        if (Addon.CanUseTrinket(1) && BotTrinket == "Generic") {
          if (!BotTrinketAOE) {
            Addon.Cast("BotTrink", true);
            return true;
          } 
          if (BotTrinketAOE && EnemiesInMelee >= 1 && (LastCast == "Rip" || LastCast == "Primal Wrath")) {
            Addon.Cast("BotTrink", true);
            return true;
          }
        }

        if (Addon.CanUseItem(usableitems, false) && usableitems != "None" && !NoCooldowns) {
          if (EnemiesNearTarget >= 1 && PlayerHealth <= GetSlider("Use item @ HP%")) {
            Addon.Cast("ItemUse");
            return true;
          }
        }

        if (RacialPower == "Troll") {
          if (Addon.CanCast("Berserking", "player")) {
            Addon.Cast("Berserking", true);
            return true;
          }
        }

        if (RacialPower == "Tauren") {
          if (Addon.IsInterruptable("target") && Addon.CanCast("War Stomp", "player")) {
            Addon.Cast("War Stomp", true);
            return true;
          }
        }

        if (RacialPower == "Night Elf") {
          if (MajorPower == "Focused Azerite Beam") {
            if (!(Addon.CastingID("player") == 299338) && !IsChanneling && !CDBeamReady 
              && Fighting && Energy < 31 && ComboPoints < 4 && TargetHealth >= 10 && !Moving) {
              if (Addon.CanCast("Shadowmeld", "player")) {
                Addon.Cast("Shadowmeld");
                Addon.Cast("cleartar");
                Addon.Cast("tar");
                return true;
              }
            }
          } else {
            if (!(Addon.CastingID("player") == 299338) && !IsChanneling && Fighting 
            && Energy < 31 && ComboPoints < 4 && TargetHealth >= 10 && !Moving) {
              if (Addon.CanCast("Shadowmeld", "player")) {
                Addon.Cast("Shadowmeld");
                Addon.Cast("cleartar");
                Addon.Cast("tar");
                return true;
              }
            }
          }

          if (LastCast == "Shadowmeld" && Addon.CanCast("Prowl", "player")) {
            Addon.Cast("Prowl");
            return true;
          }

          if (!Addon.CanCast("Prowl") && Addon.CanCast("Rake") && (Addon.HasBuff("Prowl", "player") || Addon.HasBuff("Shadowmeld", "player"))) {
            Addon.Cast("Rake");
            return true;
          }
        }
      }

      //Preopener buff
      if (!Addon.TargetIsBoss()) {
        if (Addon.CanCast("Berserk", "player") && Addon.HasBuff("Cat Form") && !NoCooldowns) {
          if (TargetHealth > 5 && Fighting && EnemiesInMelee > 0) {
            Addon.Cast("Berserk");
            return true;
          }
        }
      } else {
        if (Addon.CanCast("Berserk", "player") && Addon.HasBuff("Cat Form") && LastCast == "Tiger's Fury") {
          if (TargetHealth > 5 && Fighting && EnemiesInMelee > 0) {
            Addon.Cast("Berserk");
            return true;
          }
        }
      }

      if (!Addon.TargetIsBoss()) {
        if (TalentRange) {
          if (Addon.CanCast("Tiger's Fury", "player") && Fighting && Range < 8) {
            Addon.Cast("Tiger's Fury");
            return true;
          }
        } else {
          if (Addon.CanCast("Tiger's Fury", "player") && Fighting && EnemiesInMelee > 0) {
            Addon.Cast("Tiger's Fury");
            return true;
          }
        }
      } else {
        if (Addon.CanCast("Tiger's Fury", "player") && Fighting && Energy <= 30) {
          Addon.Cast("Tiger's Fury");
          return true;
        }
      }

      //Rake opener
      if (Stealthed && Addon.HasBuff("Cat Form")) {
        if (Addon.CanCast("Rake") && !DebuffRakeUp) {
          Addon.Cast("Rake");
          return true;
        }
      }

      if (!Stealthed) {
        //DPS Buff
        if (PredatorySwiftnessUp && Addon.CanCast("Regrowth", "player")) {
          if (PlayerHealth <= GetSlider("Auto Heal @ HP%") && ComboPoints > 3) {
            Addon.Cast("selfheal");
            return true;
          }
        }

        if (PredatorySwiftnessUp && TargetHealth < 8 && PVPMode && Addon.CanCast("Entangling Roots", "target") && !Addon.HasDebuff("Maim", "target")) {
          Addon.Cast("Entangling Roots");
          return true;
        }

        //DPS Reduction buff
        if (!BuffSurvivalInstinctsUp && Addon.CanCast("Survival Instincts", "player")) {
          if ((PlayerHealth <= GetSlider("Auto Buff @ HP%"))) {
            Addon.Cast("Survival Instincts");
            return true;
          }
        }

        if (TalentSavageRoar && Addon.HasBuff("Cat Form")) {
          if (Addon.CanCast("Savage Roar", "player")) {
            if (!SavageRoarUp && Range < 4 && Fighting && ComboPoints >= 2 && ComboPoints <= 4) {
              Addon.Cast("Savage Roar");
              return true;
            }
          }
        }

        //Dispel
        foreach(string dispelable in dispelables) {
          if (Addon.HasDebuff(dispelable, "player")) {
            if (Addon.CanCast("Remove Corruption", "player") && LastCast != "Remove Corruption") {
              Addon.Cast("Remove Corruption");
              return true;
            }
          }
        }

        //soothe
        foreach(string sootheable in sootheables) {
          if (Addon.HasBuff(sootheable, "target")) {
            if (Addon.CanCast("Soothe", "target") && LastCast != "Soothe") {
              Addon.Cast("Soothe");
              return true;
            }
          }
        }

        if (Addon.CanCast("Rebirth", "target") && !NoCooldowns && !Moving && !Enemy) {
          if (TargetHealth < 1) {
            Addon.Cast("Rebirth");
            return true;
          }
        }
      }

      if (!PVPMode && !Stealthed && !(Addon.CastingID("player") == 299338) && !IsChanneling && LastCast != "Shadowmeld") {
        //Guardian Affinity
        if (Addon.HasBuff("Moonkin Form") && Addon.TargetIsEnemy()) {
          if (Addon.CanCast("Moonfire")) {
            if (!DebuffSunfireUp) {
              Addon.Cast("sunfirem");
              return true;
            }
            if (!DebuffMoonfireUp) {
              Addon.Cast("Moonfire");
              return true;
            }
            if (DebuffMoonfireUp && DebuffSunfireUp && Moving) {
              Addon.Cast("sunfirem");
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
          // Single Target Rotation
          if (EnemiesInMelee <= 1 && Fighting) {
            if (TalentLunarInspiration) {
              if (Addon.CanCast("Moonfire") && Energy >= 30 && ComboPoints < 5) {
                if (!DebuffMoonfireUp) {
                  Addon.Cast("Moonfire");
                  return true;
                }
              }
            }
            if (Addon.CanCast("Rake") && Energy >= 35 && ComboPoints < 5) {
              if (!DebuffRakeUp) {
                Addon.Cast("Rake");
                return true;
              }
            }
            if (TalentSabertooth) {
              if (Addon.CanCast("Rip") && Energy >= 20 && TargetHealth >= 5) {
                if (!DebuffRipUp && ComboPoints < 3) {
                  Addon.Cast("Rip");
                  return true;
                }
              }
            } else {
              if (Addon.CanCast("Rip") && Energy >= 20 && TargetHealth >= 5) {
                if (!DebuffRipUp && ComboPoints > 4) {
                  Addon.Cast("Rip");
                  return true;
                }
              }
            }

            if (Addon.CanCast("Thrash", "player") && Fighting && Energy >= 45 && ComboPoints < 5) {
              if (!DebuffThrashUp && DebuffRipUp && DebuffRakeUp && LastCast != "Thrash") {
                Addon.Cast("Thrash");
                return true;
              }
            }

            if (Addon.CanCast("Ferocious Bite") && Energy >= 25) {
              if (ComboPoints > 3) {
                Addon.Cast("Ferocious Bite");
                return true;
              }
            }

            if (TalentBrutalSlash) {
              if (Addon.CanCast("Brutal Slash", "player") && Energy >= 25 && ComboPoints < 5) {
                Addon.Cast("Brutal Slash");
                return true;
              }
              if (Addon.CanCast("Shred")) {
                if (!Addon.CanCast("Brutal Slash", "player") && Energy >= 40 && ComboPoints < 5) {
                  Addon.Cast("Shred");
                  return true;
                }
              }
            } else {
              if (Addon.CanCast("Shred") && Energy >= 40 && ComboPoints < 5) {
                Addon.Cast("Shred");
                return true;
              }
            }

            return false;
          }

          //AOE 2-3 Targets
          if ((EnemiesInMelee > 1) && (EnemiesInMelee < 4) && Fighting) {
            if (CombatTimer.ElapsedMilliseconds < 2500) {
              if (Addon.CanCast("Thrash", "player") && Fighting && Energy >= 40 && ComboPoints < 5) {
                if (!DebuffThrashUp) {
                  Addon.Cast("Thrash");
                  return true;
                }
              }
              if (Addon.CanCast("Rake") && Fighting && Energy >= 35 && ComboPoints < 5) {
                if (!DebuffRakeUp && Fighting) {
                  Addon.Cast("Rake");
                  return true;
                }
              }

              if (Addon.CanCast("Ferocious Bite") && Fighting && Energy >= 25) {
                if (ComboPointsDefecit <= 1) {
                  Addon.Cast("Ferocious Bite");
                  return true;
                }
              }
              return false;
            } else {
              if (Addon.CanCast("Thrash", "player") && Energy >= 40 && ComboPoints < 5) {
                if (!DebuffThrashUp) {
                  Addon.Cast("Thrash");
                  return true;
                }
              }
              if (Addon.CanCast("Rake") && Energy >= 35 && ComboPoints < 5) {
                if (!DebuffRakeUp) {
                  Addon.Cast("Rake");
                  return true;
                }
              }

              if (Addon.CanCast("Swipe", "player") && Energy >= 35) {
                if (ComboPoints >= 3 && ComboPoints < 5) {
                  Addon.Cast("Swipe");
                  return true;
                }
              }

              if (TalentPrimalWrath) {
                if (Addon.CanCast("Primal Wrath", "player") && Fighting && Energy >= 20) {
                  if (ComboPointsDefecit <= 1) {
                    Addon.Cast("Primal Wrath");
                    return true;
                  }
                }
              } else {
                if (Addon.CanCast("Rip") && Energy >= 20 && TargetHealth >= 5) {
                  if (ComboPointsDefecit <= 1 && !DebuffRipUp) {
                    Addon.Cast("Rip");
                    return true;
                  }
                }

                if (Addon.CanCast("Ferocious Bite") && Energy >= 25) {
                  if (ComboPointsDefecit <= 1) {
                    Addon.Cast("Ferocious Bite");
                    return true;
                  }
                }
              }

              if (Addon.CanCast("Shred") && Energy >= 40) {
                if (ComboPoints <= 3) {
                  Addon.Cast("Shred");
                  return true;
                }
              }
            }
            return false;
          }

          // AOE 4+ Targets
          if (EnemiesInMelee >= 4 && Range < 4 && Fighting && !IsChanneling) {
            if (CombatTimer.ElapsedMilliseconds < 2500) {
              if (Addon.CanCast("Ferocious Bite") && Energy >= 25) {
                if (ComboPointsDefecit <= 1) {
                  Addon.Cast("Ferocious Bite");
                  return true;
                }
              }
              if (Addon.CanCast("Thrash", "player") && Energy >= 40) {

                if (!DebuffThrashUp && ComboPoints < 5) {
                  Addon.Cast("Thrash");
                  return true;
                }
              }

              if (Addon.CanCast("Swipe", "player") && Energy >= 35) {
                if (ComboPoints < 5) {
                  Addon.Cast("Swipe");
                  return true;
                }
              }
              return false;
            }
            else {
              if (Addon.CanCast("Thrash", "player") && Energy >= 40) {
                if (!DebuffThrashUp && ComboPoints < 5) {
                  Addon.Cast("Thrash");
                  return true;
                }
              }
              if (Addon.CanCast("Rake") && Energy >= 35) {
                if (!DebuffRakeUp && ComboPoints < 5) {
                  Addon.Cast("Rake");
                  return true;
                }
              }
              if (TalentPrimalWrath) {
                if (Addon.CanCast("Primal Wrath", "player") && Energy >= 20) {
                  if (ComboPointsDefecit <= 1) {
                    Addon.Cast("Primal Wrath");
                    return true;
                  }
                }
              } else {
                if (Addon.CanCast("Rip") && Energy >= 20 && TargetHealth >= 5) {
                  if (ComboPointsDefecit <= 1 && !DebuffRipUp) {
                    Addon.Cast("Rip");
                    return true;
                  }
                }

                if (Addon.CanCast("Ferocious Bite") && Energy >= 25) {
                  if (ComboPointsDefecit <= 1) {
                    Addon.Cast("Ferocious Bite");
                    return true;
                  }
                }
              }

              if (Addon.CanCast("Swipe", "player") && Energy >= 35) {
                if (ComboPoints < 5) {
                  Addon.Cast("Swipe");
                  return true;
                }
              }
            }
          }
        }

        return false;
      }

      if (PVPMode) {
        if (Fighting && !(Addon.CastingID("player") == 299338) && !IsChanneling && LastCast != "Shadowmeld") {
          if (Stealthed || !Stealthed) {
            if (Addon.CanCast("Rake") && Energy >= 35 && ComboPoints < 5) {
              if (!DebuffRakeUp) {
                Addon.Cast("Rake");
                return true;
              }
            }
          }
          if (!Stealthed) {
            if ((PlayerHealth <= GetSlider("Auto Thorns @ HP%"))) {
              if (Addon.CanCast("Thorns", "player")) {
                Addon.Cast("Thorns");
                return true;
              }
            }

            if (CycloneCC) {
              if (Addon.IsInterruptable("target")) {
                if (Addon.CanCast("Cyclone") && !Addon.CanCast("Skull Bash") && !Addon.CanCast("Mighty Bash") && !Moving && Addon.CastingRemaining("target") < 1500) {
                  Addon.Cast("Cyclone");
                  return true;
                }
              }
            }

            if (Addon.HasBuff("Cat Form")) {
              if (TalentSabertooth) {
                if (Addon.CanCast("Rip") && Energy >= 20 && TargetHealth >= 5) {
                  if (!DebuffRipUp && ComboPoints < 3) {
                    Addon.Cast("Rip");
                    return true;
                  }
                }
              } else {
                if (Addon.CanCast("Rip") && Energy >= 20 && TargetHealth >= 5) {
                  if (!DebuffRipUp && ComboPoints > 4) {
                    Addon.Cast("Rip");
                    return true;
                  }
                }
              }

              if (TalentLunarInspiration) {
                if (Addon.CanCast("Moonfire") && Energy >= 30 && ComboPoints < 5) {
                  if (!DebuffMoonfireUp) {
                    Addon.Cast("Moonfire");
                    return true;
                  }
                }
              }

              if (Addon.CanCast("Thrash", "player") && Energy >= 45 && ComboPoints < 5) {
                if (!DebuffThrashUp && DebuffRipUp && DebuffRakeUp) {
                  Addon.Cast("Thrash");
                  return true;
                }
              }

              if (DebuffRakeUp && DebuffRipUp) {
                if (TalentFeralFrenzy) {
                  if (Addon.CanCast("Feral Frenzy")) {
                    if (ComboPoints < 1) {
                      Addon.Cast("Feral Frenzy");
                      return true;
                    }
                    if (DebuffMaimUp || DebuffMightyBashUp && ComboPoints < 1) {
                      Addon.Cast("Feral Frenzy");
                      return true;
                    }
                  }
                }
              }

              if (Addon.CanCast("Ferocious Bite") && Energy >= 25) {
                if (ComboPoints > 4 || TargetHealth <= 25 && !Addon.CanCast("Maim", "target")) {
                  Addon.Cast("Ferocious Bite");
                  return true;
                }
              }
              if (Addon.CanCast("Maim") && ComboPoints > 3 && Energy > 29 && TargetHealth >= 35 && !Addon.HasDebuff("Maim", "target") && DebuffRipUp) {
                Addon.Cast("Maim");
                return true;
              } 
              if (Addon.CanCast("Ferocious Bite") && Energy >= 25) {
                if (PVPBurstMode) {
                  Addon.Cast("Ferocious Bite");
                  return true;
                }
              }
              if (EnemiesInMelee <= 1) {
                if (TalentBrutalSlash) {
                  if (Addon.CanCast("Brutal Slash", "player") && Energy >= 25 && ComboPoints < 5) {
                    Addon.Cast("Brutal Slash");
                    return true;
                  }
                  if (Addon.CanCast("Shred")) {
                    if (!Addon.CanCast("Brutal Slash", "player") && Energy >= 40 && ComboPoints < 5) {
                      Addon.Cast("Shred");
                      return true;
                    }
                  }
                } else {
                  if (Addon.CanCast("Shred") && Energy >= 40 && ComboPoints < 5) {
                    Addon.Cast("Shred");
                    return true;
                  }
                }
              } else {
                if (Addon.CanCast("Swipe", "player") && Energy >= 35 && ComboPoints < 5) {
                  Addon.Cast("Swipe");
                  return true;
                }
              }
            }
          }
        }
      }
      return false;
    }

    public override bool OutOfCombatTick() {
      bool BuffStealthUp = Addon.HasBuff("Prowl") || Addon.HasBuff("Shadowmeld") && Addon.HasBuff("Cat Form");
      bool prepull = Addon.IsCustomCodeOn("savepp");
      bool NoCooldowns = Addon.IsCustomCodeOn("savecd");
      bool TalentRange = Addon.Talent(3, 1);

      if (BuffStealthUp && Addon.CanCast("Rake") && Addon.TargetIsEnemy() && !Addon.CanCast("Tiger's Fury") && !Addon.CanCast("Berserk")) {
        Addon.Cast("Rake");
        return true;
      }

      if (Addon.Health("target") < 1 && !Addon.TargetIsEnemy() && Addon.CanCast("Revive", "target")) {
        Addon.Cast("Revive");
        return true;
      }

      if (!prepull) {
        if (!Addon.TargetIsBoss() && Addon.TargetIsEnemy() && Addon.Health("target") >= 5 && Addon.HasBuff("Cat Form") && Addon.HasBuff("Prowl")) {
          if (TalentRange) {
            if (Addon.Range("target") < 15) {
              if (Addon.CanCast("Berserk", "player") && !NoCooldowns || Addon.LastCast() == "Wild Charge") {
                  Addon.Cast("Berserk");
                  return true;
              }
              if (Addon.CanCast("Tiger's Fury", "player") || Addon.LastCast() == "Wild Charge") {
                Addon.Cast("Tiger's Fury");
                return true;
              }
            }
          } else {
            if (Addon.Range("target") < 10) {
              if (Addon.CanCast("Berserk", "player") && !NoCooldowns || Addon.LastCast() == "Wild Charge") {
                  Addon.Cast("Berserk");
                  return true;
              }
              if (Addon.CanCast("Tiger's Fury", "player") || Addon.LastCast() == "Wild Charge") {
                Addon.Cast("Tiger's Fury");
                return true;
              }
            }
          }
        }
      }
      return false;
    }
  }
}