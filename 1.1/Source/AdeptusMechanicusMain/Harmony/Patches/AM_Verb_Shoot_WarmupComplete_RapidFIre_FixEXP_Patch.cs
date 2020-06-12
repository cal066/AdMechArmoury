﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_Shoot), "WarmupComplete")]
    public static class AM_Verb_Shoot_WarmupComplete_RapidFIre_FixEXP_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref Verb_Shoot __instance, ref float __state)
        {
            __state = __instance.verbProps.warmupTime;
            GunVerbEntry entry = __instance.SpecialRules();
            if (entry != null)
            {
                if (__instance.RapidFire(__state, out bool InRange, out float modified))
                {
                    if (InRange)
                    {
                        __instance.verbProps.warmupTime = modified;
                    }
                }
            //    Log.Message("Prefix original warmup " + __state  + " "+ __instance.verbProps.label + " fired by " + __instance.CasterPawn.LabelShortCap + "\nwarmup " + __instance.verbProps.warmupTime + " Cooldown " + __instance.verbProps.AdjustedCooldown(__instance, __instance.CasterPawn) + " Burst " + ((__instance.verbProps.burstShotCount - 1) * __instance.verbProps.ticksBetweenBurstShots).TicksToSeconds());
            }
        }

        [HarmonyPostfix]
        public static void Postfix(ref Verb_Shoot __instance, float __state)
        {
            GunVerbEntry entry = __instance.SpecialRules();
            if (entry != null)
            {
                __instance.verbProps.warmupTime = __state;
            //    Log.Message("Postfix original warmup " + __state + " " + __instance.verbProps.label + " fired by " + __instance.CasterPawn.LabelShortCap + "\nwarmup " + __instance.verbProps.warmupTime + " Cooldown " + __instance.verbProps.AdjustedCooldown(__instance, __instance.CasterPawn) + " Burst " + ((__instance.verbProps.burstShotCount - 1) * __instance.verbProps.ticksBetweenBurstShots).TicksToSeconds());
            }
        }

    }
    
    [HarmonyPatch(typeof(AbilitesExtended.Verb_ShootEquipment), "WarmupComplete")]
    public static class AM_Verb_ShootEquipment_WarmupComplete_RapidFIre_FixEXP_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref AbilitesExtended.Verb_ShootEquipment __instance, ref float __state)
        {
            __state = __instance.verbProps.warmupTime;
            GunVerbEntry entry = __instance.SpecialRules();
            if (entry != null)
            {
                if (__instance.RapidFire(__state, out bool InRange, out float modified))
                {
                    if (InRange)
                    {
                        __instance.verbProps.warmupTime = modified;
                    }
                }
                Log.Message("Prefix original warmup " + __state  + " "+ __instance.verbProps.label + " fired by " + __instance.CasterPawn.LabelShortCap + "\nwarmup " + __instance.verbProps.warmupTime + " Cooldown " + __instance.verbProps.AdjustedCooldown(__instance, __instance.CasterPawn) + " Ticks between shots " + __instance.verbProps.ticksBetweenBurstShots);
            }
        }

        [HarmonyPostfix]
        public static void Postfix(ref AbilitesExtended.Verb_ShootEquipment __instance, float __state)
        {
            GunVerbEntry entry = __instance.SpecialRules();
            if (entry != null)
            {
                __instance.verbProps.warmupTime = __state;
                Log.Message("Postfix original warmup " + __state + " " + __instance.verbProps.label + " fired by " + __instance.CasterPawn.LabelShortCap + "\nwarmup " + __instance.verbProps.warmupTime + " Cooldown " + __instance.verbProps.AdjustedCooldown(__instance, __instance.CasterPawn) + " Ticks between shots " + __instance.verbProps.ticksBetweenBurstShots);
            }
        }
    }
    
}
