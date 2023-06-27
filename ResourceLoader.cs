using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections.Generic;
using UnityEngine;
using Vasi;

namespace Soul_Paladin
{
    public class ResourceLoader
    {
        public static GameObject radiancelaser;
        public static FsmObject laserpreparesound;
        public static FsmObject laserburstsound;
        public static GameObject multizap;

        public static void LoadResource(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            laserpreparesound = preloadedObjects["GG_Radiance"]["Boss Control/Absolute Radiance"].LocateMyFSM("Attack Commands").GetAction<AudioPlaySimple>("EB 1", 1).oneShotClip;
            laserburstsound = preloadedObjects["GG_Radiance"]["Boss Control/Absolute Radiance"].LocateMyFSM("Attack Commands").GetAction<AudioPlayerOneShotSingle>("EB 1", 2).audioClip;
            radiancelaser = preloadedObjects["GG_Radiance"]["Boss Control/Absolute Radiance"].LocateMyFSM("Attack Commands").GetAction<ActivateGameObject>("AB Start",0).gameObject.GameObject.Value.Child("Burst 1").Child("Radiant Beam (3)");
            multizap = preloadedObjects["GG_Uumuu"]["Mega Jellyfish Multizaps"].Child("Pattern 1").Child("Mega Jelly Multizap");
        }
    }
}