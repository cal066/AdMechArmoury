﻿using System;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.TrailerProjectileExtension
    public class TrailerProjectileExtension : DefModExtension
    {
        public bool trailWhenLanded = false;
        public bool useGraphicColor = false;
        public bool useGraphicColorTwo = false;
        public string trailMoteDef = "Mote_Smoke";
        public float trailMoteSize = 0.5f;
        public int trailerMoteInterval = 30;
        public int trailInitalDelay = -1;
        public int motesThrown = 1;
    }

}
