using HarmonyLib;
using UnityEngine;
using System.Collections;

[HarmonyPatch(typeof(V2), "KnockedOut")]
class Patch3 : DefaultPatch {
    static void Postfix(V2 __instance, string triggerName) {
        if (triggerName == "KnockedDown") {
            if (!__instance.secondEncounter)
                __instance.StartCoroutine(Outro(__instance));
            else 
                __instance.StartCoroutine(Fleeing(__instance));
        } else if (triggerName == "Flailing") {
            // the V2 script gets destroyed when transitioning to the
            // final room, so i have to start the coroutine from
            // the plugin itself
            Plugin.Instance.StartCoroutine(Falling());
        } else Plugin.LogError("what");

        Plugin.LogDebug("v2 knocked out: " + triggerName);
    }

    static IEnumerator Outro(V2 __instance) {
        yield return new WaitForSeconds(1f);

        AudioSource audioSource = Utils.CreateOneTimeVoiceObject(
            "OutroVoice", 
            Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/outro.wav"),
            Utils.CreateSubtitleData(
                Utils.MakeLine("Okay, you know what"),
                Utils.MakeLine("I'm gonna call that one a draw", 0.9f),
                Utils.MakeLine("Team Rocket is pissing off again", 2f)
            ),
            __instance.transform
        );

        audioSource.Play();
    }

    static IEnumerator Fleeing(V2 __instance) {
        ExtensionMethods.V2AdditionalData data = __instance.GetAdditionalData();

        // Play the first fight's death clip, just because
        __instance.PlayVoice(data.deathAudioClip);
        MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(data.deathSubtitle);

        AudioSource fleeingVoice = Utils.CreateOneTimeVoiceObject(
            "FleeingVoice",
            Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_2/fleeing1.wav"),
            Utils.CreateSubtitleData(
                Utils.MakeLine("I WON'T GIVE YOU THE PLEASURE OF KILLING ME!")
            ),
            __instance.transform
        );

        yield return new WaitForSeconds(2.5f);
        
        fleeingVoice.Play();
    }

    static IEnumerator Falling() {
        GameObject fallingV2 = Utils.FindGameObjectByName("8 Stuff(Clone)(Clone)").transform.Find("v2_GreenArm").gameObject;
        
        AudioSource audioSource = Utils.CreateVoiceObject("FallingVoice", fallingV2.transform);
        audioSource.clip = Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_2/death.wav");
        // Wait until V2 starts falling
        yield return new WaitUntil(() => fallingV2.activeInHierarchy);
        yield return new WaitForSeconds(0.3f);
        
        audioSource.Play();
        MonoSingleton<SubtitleController>.Instance.DisplaySubtitle("FUCKING NIKON");

    }
} 