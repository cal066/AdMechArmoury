﻿using System;
using System.Linq;
using Verse;
using HarmonyLib;
using UnityEngine;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) })]
    public static class AM_PawnRenderer_RenderPawnInternal_DrawExtras_Patch
    {
        [HarmonyPostfix]
        public static void PawnRenderer_RenderPawnInternal_Postfix(ref PawnRenderer __instance, Pawn ___pawn, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        {
            if (!__instance.graphics.AllResolved)
            {
                __instance.graphics.ResolveAllGraphics();
            }
            Mesh mesh = null;
            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);
            if (AdeptusIntergrationUtil.enabled_AlienRaces)
            {
                AM_PawnRenderer_RenderPawnInternal_DrawExtras_Patch.AlienRacesPatch(ref __instance, ___pawn, rootLoc, angle, renderBody, bodyFacing, headFacing, out mesh, bodyDrawType, portrait, headStump, invisible);
            }
            else
            {
                if (___pawn.RaceProps.Humanlike)
                {
                    mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                }
                else
                {
                    mesh = __instance.graphics.nakedGraphic.MeshAt(bodyFacing);
                }
            }
            if (renderBody)
            {
                Vector3 vector = rootLoc;
                /*
                if (bodyFacing != Rot4.North)
                {
                    vector.y += 0.0227272734f;
                }
                else
                {
                    vector.y += 0.0265151523f;
                }
                */
                if (___pawn.apparel!=null && ___pawn.apparel.WornApparelCount>0)
                {
                    for (int k = 0; k < ___pawn.apparel.WornApparel.Count; k++)
                    {
                        if (___pawn.apparel.WornApparel[k].TryGetComp<CompPauldronDrawer>() != null)
                        {
                            foreach (CompPauldronDrawer Pauldron in ___pawn.apparel.WornApparel[k].AllComps.Where(x => x.GetType() == typeof(CompPauldronDrawer)))
                            {
                                Vector3 drawAt = vector;
                                if (!Pauldron.Props.PauldronEntries.NullOrEmpty())
                                {
                                    if (Pauldron.Props.drawAll)
                                    {
                                        foreach (ShoulderPadEntry item in Pauldron.Props.PauldronEntries)
                                        {
                                            Pauldron.shoulderPadEntry = item;
                                            Pauldron.pauldronGraphicPath = Pauldron.shoulderPadEntry.padTexPath;
                                            Pauldron.useSecondaryColor = Pauldron.shoulderPadEntry.UseSecondaryColor;
                                            Pauldron.padType = Pauldron.shoulderPadEntry.shoulderPadType;
                                            Vector3 v = drawAt;
                                            if (Pauldron.ShouldDrawPauldron(bodyFacing, out Material pauldronMat, item))
                                            {
                                                v += quat * Pauldron.GetAltitudeOffset(bodyFacing, item);
                                                GenDraw.DrawMeshNowOrLater(mesh, v, quat, pauldronMat, portrait);
                                                //    vector.y += CompPauldronDrawer.MinClippingDistance;
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (!Pauldron.pauldronInitialized)
                                        {
                                            if (Rand.Chance(Pauldron.Props.PauldronEntryChance))
                                            {
                                                Pauldron.shoulderPadEntry = Pauldron.Props.PauldronEntries.RandomElementByWeight((ShoulderPadEntry x) => x.commonality);
                                                Pauldron.pauldronGraphicPath = Pauldron.shoulderPadEntry.padTexPath;
                                                Pauldron.useSecondaryColor = Pauldron.shoulderPadEntry.UseSecondaryColor;
                                                Pauldron.padType = Pauldron.shoulderPadEntry.shoulderPadType;
                                            }
                                            Pauldron.pauldronInitialized = true;
                                        }
                                        if (Pauldron.ShouldDrawPauldron(bodyFacing, out Material pauldronMat))
                                        {
                                            drawAt += quat * Pauldron.GetAltitudeOffset(bodyFacing);
                                            GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, pauldronMat, portrait);
                                            //    vector.y += CompPauldronDrawer.MinClippingDistance;
                                        }
                                    }
                                }
                            }
                        }
                        if (___pawn.apparel.WornApparel[k].TryGetComp<CompApparelExtraDrawer>() != null)
                        {
                            foreach (CompApparelExtraDrawer Extas in ___pawn.apparel.WornApparel[k].AllComps.Where(x => x.GetType() == typeof(CompApparelExtraDrawer)))
                            {
                                Vector3 drawAt = vector;
                                if (!Extas.Props.ExtrasEntries.NullOrEmpty())
                                {
                                    if (Extas.ShouldDrawExtra(___pawn, ___pawn.apparel.WornApparel[k], bodyFacing, out Material extraMat))
                                    {
                                        if (Extas.onHead)
                                        {
                                            drawAt = vector + quat * __instance.BaseHeadOffsetAt(headFacing);
                                        }
                                        drawAt += quat * Extas.GetAltitudeOffset(bodyFacing, Extas.ExtraPartEntry);
                                        GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, extraMat, portrait);
                                        //    vector.y += CompApparelExtaDrawer.MinClippingDistance;
                                    }
                                }
                            }
                        }
                        Apparel_VisibleAccessory VisibleAccessory;
                        if (___pawn.apparel.WornApparel[k].GetType() == typeof(Apparel_VisibleAccessory))
                        {
                            VisibleAccessory = (Apparel_VisibleAccessory)___pawn.apparel.WornApparel[k];
                        }
                        //ApparelGraphicRecord apparelGraphicRecord = __instance.graphics.apparelGraphics[k];
                        //if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayer.Shell)
                        //{
                        //           Material material2 = apparelGraphicRecord.graphic.MatAt(bodyFacing, null);
                        //           material2 = __instance.graphics.flasher.GetDamagedMat(material2);
                        //           GenDraw.DrawMeshNowOrLater(mesh, vector, quat, material2, portrait);

                        //}
                    }
                }
                if (!___pawn.Dead)
                {
                    for (int l = 0; l < ___pawn.health.hediffSet.hediffs.Count; l++)
                    {
                        Vector3 drawAt = vector;
                        HediffComp_DrawImplant_AdMech drawer = ___pawn.health.hediffSet.hediffs[l].TryGetComp<HediffComp_DrawImplant_AdMech>();
                        if (drawer != null)
                        {
                            Material material = null;
                            if (drawer.implantDrawProps.implantDrawerType != ImplantDrawerType.Head)
                            {
                                drawAt.y += 0.005f;
                                if (bodyFacing == Rot4.South && drawer.implantDrawProps.implantDrawerType == ImplantDrawerType.Backpack)
                                {
                                    drawAt.y -= 0.3f;
                                }
                                material = drawer.ImplantMaterial(___pawn, bodyFacing);
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, material, portrait);
                            }
                            else
                            {
                                if (!___pawn.Downed && !___pawn.Dead && drawer.implantDrawProps.useHeadOffset)
                                {
                                    drawAt = vector + __instance.BaseHeadOffsetAt(headFacing);
                                }
                                else
                                {
                                    if (___pawn.Downed || ___pawn.Dead && drawer.implantDrawProps.useHeadOffset)
                                    {
                                        drawAt.y = vector.y + __instance.BaseHeadOffsetAt(headFacing).y;
                                    }
                                }
                                drawAt.y += 0.005f;
                                material = drawer.ImplantMaterial(___pawn, headFacing);
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, material, portrait);
                            }

                            if (material != null)
                            {
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt , quat, material, portrait);

                                //                                                                                        Angle calculation to not pick the shortest, taken from Quaternion.Angle and modified
                                GenDraw.DrawMeshNowOrLater(mesh: mesh, loc: drawAt + drawer.offsetVector().RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: quat)) * 2f * 57.29578f),
                                    quat: quat, mat: material, drawNow: portrait);

                                drawAt.y += HediffComp_DrawImplant_AdMech.MinClippingDistance;
                            }
                        }
                        HediffComp_Shield _Shield;
                        if ((_Shield = ___pawn.health.hediffSet.hediffs[l].TryGetComp<HediffComp_Shield>()) != null)
                        {
                            _Shield.DrawWornExtras();
                        }
                    }
                }
            }
        }

        static void AlienRacesPatch(ref PawnRenderer __instance, Pawn pawn, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, out Mesh mesh, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        {
            mesh = null;
            AlienRace.ThingDef_AlienRace alienDef = pawn.def as AlienRace.ThingDef_AlienRace;
            if (alienDef != null)
            {
                Mesh mesh2;
                if (bodyDrawType == RotDrawMode.Rotting)
                {
                    if (__instance.graphics.dessicatedGraphic.ShouldDrawRotated)
                    {
                        mesh2 = MeshPool.GridPlane(portrait ? alienDef.alienRace.generalSettings.alienPartGenerator.customPortraitDrawSize : alienDef.alienRace.generalSettings.alienPartGenerator.customDrawSize);
                    }
                    else
                    {
                        Vector2 size = portrait ? alienDef.alienRace.generalSettings.alienPartGenerator.customPortraitDrawSize : alienDef.alienRace.generalSettings.alienPartGenerator.customDrawSize;
                        if (bodyFacing.IsHorizontal)
                        {
                            size = size.Rotated();
                        }
                        if (bodyFacing == Rot4.West && (__instance.graphics.dessicatedGraphic.data == null || __instance.graphics.dessicatedGraphic.data.allowFlip))
                        {
                            mesh = MeshPool.GridPlaneFlip(size);
                        }
                        mesh = MeshPool.GridPlane(size);
                    }
                }
                else
                {
                    AlienRace.AlienPartGenerator.AlienComp comp = pawn.TryGetComp<AlienRace.AlienPartGenerator.AlienComp>();
                    if (comp != null)
                    {
                        mesh = (portrait ? comp.alienPortraitGraphics.bodySet.MeshAt(bodyFacing) : comp.alienGraphics.bodySet.MeshAt(bodyFacing));
                    }
                }
            }
            else
            {

                if (pawn.RaceProps.Humanlike)
                {
                    mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                }
                else
                {
                    mesh = __instance.graphics.nakedGraphic.MeshAt(bodyFacing);
                }
            }
        }
    }

}
