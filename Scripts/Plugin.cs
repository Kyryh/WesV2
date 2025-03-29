using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using System.Collections;
using System;


[BepInPlugin("kyryh.wesv2", WesV2.PluginInfo.PLUGIN_NAME, WesV2.PluginInfo.PLUGIN_VERSION)]
[BepInDependency("clearwater.ultrakill.ultrakull", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin {
    public static AssetBundle WesV2AssetBundle;
    public static ManualLogSource Log;
    public static Plugin Instance;
    public static bool secondEncounterIntro = false;
    public static bool ultrakullLoaded = false;

    public void Awake() {

        Instance = this;

        // static attribute to log outside of the Plugin class
        Log = Logger;

        Assembly assembly = Assembly.GetExecutingAssembly();

        // Load the asset bundle from the mod's assembly
        using (Stream stream = assembly.GetManifestResourceStream("WesV2.Resources.wesv2assetbundle")) {
            WesV2AssetBundle = AssetBundle.LoadFromStream(stream);
        }

        SceneManager.sceneLoaded += (_, _) => OnSceneLoaded();


        Harmony harmony = new Harmony("kyryh.wesv2");

        foreach (Type type in assembly.GetPatchesOfType(typeof(DefaultPatch))) {
            harmony.PatchAll(type);
        }

        ultrakullLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("clearwater.ultrakill.ultrakull");


        // Only apply the UltrakULL patch if UltrakULL is present and loaded
        if (ultrakullLoaded) {
            foreach (Type type in assembly.GetPatchesOfType(typeof(UltrakULLPatch))) {
                harmony.PatchAll(type);
            }
        }

        V2VoicelinesData.LoadSubtitles();

        Logger.LogInfo("Plugin Wes V2 is loaded!");

    }






    public void OnSceneLoaded() {
        // In case the V2_2Intro coroutine is still playing
        StopAllCoroutines();
        string levelName = SceneHelper.CurrentScene;

        if (levelName == "Level 1-4") {

            // Find the fake V2 object and add the AudioSource and AudioClip to it
            GameObject fakeV2 = Utils.FindGameObjectByName("Fake V2");

            Utils.CreateOneTimeVoiceObject(
                "Voice",
                V2VoicelinesData.GetAudioClip("v2_1/jumpscare"),
                Utils.CreateSubtitleData(
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_jumpscare_1")),
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_jumpscare_2"), .8f)
                ),
                fakeV2.transform
            );

        } else if (levelName == "Level 4-4") {
            StartCoroutine(V2_2Intro());
        }
    }



    private static IEnumerator V2_2Intro() {

        secondEncounterIntro = true;

        // Waits one frame before finding the objects, so that the game has time to clone the rooms,
        // otherwise Utils.FindGameObjectByName("7 Stuff(Clone)") would return null
        yield return null;

        GameObject bossArena = Utils.FindGameObjectByName("7 Stuff(Clone)");

        if (!bossArena.GetComponentInChildren<V2>(true).longIntro)
            yield break;
        GameObject bossIntro = bossArena.transform.Find("BossIntro").gameObject;

        GameObject versusIntro = bossIntro.transform.Find("VersusIntro").gameObject;

        GameObject introVoice = new GameObject("IntroVoice");
        introVoice.transform.position = bossIntro.transform.Find("v2_GreenArm").position;

        AudioSource audioSource1 = Utils.CreateOneTimeVoiceObject(
            "IntroVoice1",
            V2VoicelinesData.GetAudioClip("v2_2/intro1"),
            Utils.CreateSubtitleData(
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2Second_intro1_1")),
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2Second_intro1_2"), 1.6f),
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2Second_intro1_3"), 3.2f)
            ),
            introVoice.transform
        );

        AudioSource audioSource2 = Utils.CreateOneTimeVoiceObject(
            "IntroVoice2",
            V2VoicelinesData.GetAudioClip("v2_2/intro2"),
            Utils.CreateSubtitleData(
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2Second_intro2_1"), 1.5f),
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2Second_intro2_2"), 3.1f),
                Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2Second_intro2_3"), 5.2f)
            ),
            introVoice.transform
        );


        // Waits until the boss arena loads
        yield return new WaitUntil(() => {
            // i need to do this shit because for some fucking reason
            // using the teleport menu deletes all the references to the
            // objects???? i need to continuously check if they're null
            // and if they are i find them again
            //
            // technically it breaks if you start the intro and then
            // use the teleport menu again but that's such a intricate
            // thing to do and it's 2:30 AM so fuck you just don't do
            // that thanks
            if (bossArena == null) {
                bossArena = Utils.FindGameObjectByName("7 Stuff(Clone)");
            }
            return bossArena.activeInHierarchy;
        });
        yield return new WaitForSeconds(1.5f);
        audioSource1.Play();
        LogDebug("INTRO 1");

        // Waits until the Versus Intro starts
        yield return new WaitUntil(() => {
            if (versusIntro == null) {
                versusIntro = Utils.FindGameObjectByName("7 Stuff(Clone)").transform.Find("BossIntro/VersusIntro").gameObject;
            }
            return versusIntro.activeInHierarchy;
        });
        audioSource1.Stop();
        yield return new WaitForSeconds(1f);
        audioSource2.Play();
        LogDebug("INTRO 2");

        // Wait until the Versus Intro finishes
        yield return new WaitWhile(() => {
            if (versusIntro == null) {
                versusIntro = Utils.FindGameObjectByName("7 Stuff(Clone)").transform.Find("BossIntro/VersusIntro").gameObject;
            }
            return versusIntro.activeInHierarchy;
        });
        audioSource2.Stop();

        yield return null;
        secondEncounterIntro = false;

    }

    public static void LogDebug(object data) {
        Log.LogDebug(data);
    }

    public static void LogError(object data) {
        Log.LogError(data);
    }



}
