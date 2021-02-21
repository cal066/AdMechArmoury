﻿using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace AdeptusMechanicus
{
    public static class AlienRaceUtility
    {
        [MayRequireAlienRaces]
        public static void AlienRaces()
        {
            AlienRace.ThingDef_AlienRace Human = DefDatabase<ThingDef>.GetNamedSilentFail("Human") as AlienRace.ThingDef_AlienRace;
            if (Human != null)
            {
                List<string> Tags = new List<string>() { "I", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ArmouryMain.ReseachImperial);
                projects.AddRange(ArmouryMain.ReseachChaos);
                DoRacialRestrictionsFor(Human, Tags, projects);
            }
            AlienRace.ThingDef_AlienRace Mechanicus = ArmouryMain.mechanicus as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Ogryn = ArmouryMain.ogryn as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Ratlin = ArmouryMain.ratlin as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Beastman = ArmouryMain.beastman as AlienRace.ThingDef_AlienRace;
            if (Mechanicus != null)
            {
                List<string> Tags = new List<string>() { "I", "AM", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ArmouryMain.ReseachImperial);
                projects.AddRange(ArmouryMain.ReseachMechanicus);
                projects.AddRange(ArmouryMain.ReseachChaos);
                DoRacialRestrictionsFor(Mechanicus, Tags, projects);
            }
            List<ThingDef> races = new List<ThingDef>();
            if (Ogryn != null) races.Add(Ogryn);
            if (Ratlin != null) races.Add(Ratlin);
            if (Beastman != null) races.Add(Beastman);

            if (AdeptusIntergrationUtility.enabled_GeneSeed)
            {
                AlienRace.ThingDef_AlienRace GeneseedAstartes = ArmouryMain.geneseedAstartes as AlienRace.ThingDef_AlienRace;
                AlienRace.ThingDef_AlienRace GeneseedCustodes = ArmouryMain.geneseedCustodes as AlienRace.ThingDef_AlienRace;
                if (GeneseedAstartes != null) races.Add(GeneseedAstartes);
                if (GeneseedCustodes != null) races.Add(GeneseedCustodes);
            }
            if (AdeptusIntergrationUtility.enabled_AstraServoSkulls)
            {
                races.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => (x.defName.Contains("IG_Serv_ServoSkull") && x.defName.Contains("_Race")) || (x.defName.Contains("IG_Serv_Servitor") && x.defName.Contains("_Race"))));
            }

            if (!races.NullOrEmpty())
            {
                List<string> Tags = new List<string>() { "I", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ArmouryMain.ReseachImperial);
                projects.AddRange(ArmouryMain.ReseachChaos);
                DoRacialRestrictionsFor(races, Tags, projects);
            }
        }

        [MayRequireAlienRaces]
        public static void DoRacialRestrictionsFor(ThingDef race, string Tag, List<ResearchProjectDef> researches = null, List<ThingDef> apparel = null, List<ThingDef> weapons = null, List<ThingDef> plants = null, List<ThingDef> animals = null)
        {
            List<ThingDef> races = new List<ThingDef>();
            races.Add(race);
            List<string> Tags = new List<string>();
            Tags.Add(Tag);
            DoRacialRestrictionsFor(races, Tags, researches, apparel, weapons, plants, animals);
        }

        [MayRequireAlienRaces]
        public static void DoRacialRestrictionsFor(ThingDef race, List<string> Tags, List<ResearchProjectDef> researches = null, List<ThingDef> apparel = null, List<ThingDef> weapons = null, List<ThingDef> plants = null, List<ThingDef> animals = null)
        {
            List<ThingDef> races = new List<ThingDef>();
            races.Add(race);
            DoRacialRestrictionsFor(races, Tags, researches, apparel, weapons, plants, animals);
        }

        [MayRequireAlienRaces]
        public static void DoRacialRestrictionsFor(List<ThingDef> races, string Tag, List<ResearchProjectDef> researches = null, List<ThingDef> apparel = null, List<ThingDef> weapons = null, List<ThingDef> plants = null, List<ThingDef> animals = null)
        {
            List<string> Tags = new List<string>();
            Tags.Add(Tag);
            DoRacialRestrictionsFor(races, Tags, researches, apparel, weapons, plants, animals);
        }

        [MayRequireAlienRaces]
        public static void DoRacialRestrictionsFor(List<ThingDef> races, List<string> Tags, List<ResearchProjectDef> researches = null, List<ThingDef> apparel = null, List<ThingDef> weapons = null, List<ThingDef> plants = null, List<ThingDef> animals = null)
        {
            foreach (ThingDef race in races)
            {
                AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    string msg = "Try Do Racial Restrictions For " + alien.LabelCap + " (" + alien.defName + ")";
                    foreach (string Tag in Tags)
                    {
                        List<RecipeDef> recipes = DefDatabase<RecipeDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_"));
                        if (!recipes.NullOrEmpty())
                        {
                            msg += "\n" + recipes.Count + " OG" + Tag + " Recipes";
                            RestrictRecipes(alien, recipes);
                        }
                        List<ThingDef> buildings = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_") && (x.building != null || x.IsBuildingArtificial));
                        if (!buildings.NullOrEmpty())
                        {
                            msg += "\n" + buildings.Count + " OG" + Tag + " Buildings";
                            RestrictBuildings(alien, buildings);
                        }
                    }
                    if (!researches.NullOrEmpty())
                    {
                        msg += "\n" + researches.Count + " Reseaches";
                        RestrictResearch(alien, researches);
                    }
                    if (!apparel.NullOrEmpty())
                    {
                        msg += "\n" + apparel.Count + " Apparel";
                        RestrictApparel(alien, apparel);
                    }
                    if (!weapons.NullOrEmpty())
                    {
                        msg += "\n" + weapons.Count + " Weapons";
                        RestrictWeapons(alien, weapons);
                    }
                    if (!plants.NullOrEmpty())
                    {
                        msg += "\n" + plants.Count + " Plants";
                        RestrictPlants(alien, plants);
                    }
                    if (!animals.NullOrEmpty())
                    {
                        msg += "\n" + animals.Count + " Animals";
                        RestrictAnimals(alien, animals);
                    }
                    //    Log.Message(msg);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictRecipes(ThingDef race, List<RecipeDef> things)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.recipeList.AddRange(things);
            foreach (RecipeDef def in things)
            {
                if (!AlienRace.RaceRestrictionSettings.recipeRestrictionDict.ContainsKey(key: def))
                {
                    AlienRace.RaceRestrictionSettings.recipeRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                }
                if (!AlienRace.RaceRestrictionSettings.recipeRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.recipeRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictBuildings(ThingDef race, List<ThingDef> things)
        {

            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.buildingList.AddRange(things);
            foreach (ThingDef def in things)
            {
                if (!AlienRace.RaceRestrictionSettings.buildingRestrictionDict.ContainsKey(key: def))
                {
                    AlienRace.RaceRestrictionSettings.buildingRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                }
                if (!AlienRace.RaceRestrictionSettings.buildingRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.buildingRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictResearch(ThingDef race, List<ResearchProjectDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            if (alien.alienRace.raceRestriction.researchList.NullOrEmpty())
            {
                alien.alienRace.raceRestriction.researchList = new List<AlienRace.ResearchProjectRestrictions>();
                alien.alienRace.raceRestriction.researchList.Add(new AlienRace.ResearchProjectRestrictions());
                alien.alienRace.raceRestriction.researchList[0].projects = new List<ResearchProjectDef>();
                alien.alienRace.raceRestriction.researchList[0].apparelList = new List<ThingDef>();
            }
            alien.alienRace.raceRestriction.researchList[0].projects.AddRange(list);
            foreach (ResearchProjectDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.researchRestrictionDict.ContainsKey(key: def))
                {
                    List<AlienRace.ThingDef_AlienRace> l2 = new List<AlienRace.ThingDef_AlienRace>();
                    l2.Add(alien);
                    AlienRace.RaceRestrictionSettings.researchRestrictionDict.Add(key: def, value: l2);
                }
                else
                {
                    if (!AlienRace.RaceRestrictionSettings.researchRestrictionDict[key: def].Contains(alien))
                    {
                        AlienRace.RaceRestrictionSettings.researchRestrictionDict[key: def].Add(item: alien);
                    }
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictWeapons(ThingDef race, List<ThingDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.weaponList.AddRange(list);
            foreach (ThingDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.weaponRestrictionDict.ContainsKey(key: def))
                    AlienRace.RaceRestrictionSettings.weaponRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                if (!AlienRace.RaceRestrictionSettings.weaponRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.weaponRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictPlants(ThingDef race, List<ThingDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.plantList.AddRange(list);
            foreach (ThingDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.plantRestrictionDict.ContainsKey(key: def))
                    AlienRace.RaceRestrictionSettings.plantRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                if (!AlienRace.RaceRestrictionSettings.plantRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.plantRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictAnimals(ThingDef race, List<ThingDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.petList.AddRange(list);
            foreach (ThingDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.tameRestrictionDict.ContainsKey(key: def))
                    AlienRace.RaceRestrictionSettings.tameRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                if (!AlienRace.RaceRestrictionSettings.tameRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.tameRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictApparel(ThingDef race, List<ThingDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.apparelList.AddRange(list);
            foreach (ThingDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.apparelRestrictionDict.ContainsKey(key: def))

                    AlienRace.RaceRestrictionSettings.apparelRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                if (!AlienRace.RaceRestrictionSettings.apparelRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.apparelRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }
    }
}