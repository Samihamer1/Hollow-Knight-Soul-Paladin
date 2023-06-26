using Modding;
using Modding.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Soul_Paladin
{
    public class Soul_Paladin : Mod
    {
        internal static Soul_Paladin Instance;

        new string GetName() => "Soul Paladin";
        public override string GetVersion() => "v0.0.0.1";
        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Mage_Knight","Mage Knight"),
                ("GG_Radiance","Boss Control/Absolute Radiance"),
                ("GG_Uumuu","Mega Jellyfish Multizaps")
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            ResourceLoader.LoadResource(preloadedObjects);
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += CheckScene;
            ModHooks.LanguageGetHook += ChangeText;
        }

        private string ChangeText(string key, string sheetTitle, string orig)
        {
            if (key == "NAME_MAGE_KNIGHT")
            {
                return "Soul Paladin";
            }
            if (key == "GG_S_MAGEKNIGHT")
            {
                return "Sorcerer god of combat";
            }
            if (key == "MAGE_KNIGHT_MAIN")
            {
                return "Paladin";
            }
            return orig;
        }

        private void CheckScene(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            if (arg1.name == "GG_Mage_Knight")
            {
                GameManager.instance.StartCoroutine(CheckChampion());
            }
        }

        private IEnumerator CheckChampion()
        {
            yield return new WaitWhile(() => GameObject.Find("Mage Knight") == null);
            GameObject.Find("Mage Knight").AddComponent<PaladinControl>();
        }

    }
}