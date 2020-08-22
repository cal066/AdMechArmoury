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
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Projectile), "Draw")]
    public static class Projectile_Draw_ProjectileGlow_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Projectile __instance)
        {
            if (__instance != null)
            {
                if (__instance.def.HasModExtension<GlowerProjectileExtension>())
                {

                    GlowerProjectileExtension glower = __instance.def.GetModExtension<GlowerProjectileExtension>();
                    if (glower != null)
                    {
                        Mesh mesh2 = MeshPool.GridPlane(DefDatabase<ThingDef>.GetNamed(glower.GlowMoteDef).graphicData.drawSize * glower.GlowMoteSize);
                        Graphics.DrawMesh(mesh2, __instance.DrawPos, __instance.ExactRotation, DefDatabase<ThingDef>.GetNamed(glower.GlowMoteDef).graphic.MatSingle, 0);
                    }
                }
            }
        }
    }
}