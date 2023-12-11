using HarmonyLib;
using UnityEngine;
using System.Collections;

[HarmonyPatch(typeof(V2), "Start")]
class Patch1 {
    static void Postfix(V2 __instance, Animator ___anim) {
        AudioSource audioSource;

        if (__instance.firstPhase) {
            audioSource = Utils.CreateVoiceObject("Voice", __instance.transform); 
        } else {
            // Start of second phase in 4-4, no need to create a new Voice GameObject
            audioSource = __instance.transform.Find("Voice").GetComponent<AudioSource>();

            Utils.CreateOneTimeVoiceObject(
                "FleeingVoice",
                Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_2/fleeing2.wav"),
                Utils.CreateSubtitleData(
                    Utils.MakeLine("Gotta get away, gotta get away, oh no..."),
                    Utils.MakeLine("YOUR FORM IS INCREDIBLE!", 3f)
                ),
                __instance.transform
            ).Play();
            
        }


        if (!__instance.secondEncounter) {
            // FIRST ENCOUNTER (1-4)

            // Setting additional data so that i can keep track about V2's audioclips and other stuff
            __instance.SetAdditionalData(
                new ExtensionMethods.V2AdditionalData(
                    audioSource,
                    [
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/taunt1.wav"),
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/taunt2.wav"),
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/taunt3.wav"),
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/taunt4.wav"),
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/taunt5.wav"), 
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/taunt6.wav"), 
                    ],
                    [
                        "You aren't the only one who's out for BLOOD, Brother!",
                        "Looks like you've gone a little RUSTY, Brother!",
                        "You think you can best ME?! AFTER ALL THAT I'VE BEEN THROUGH!",
                        "YOU'RE JUST A FUCKING NIKON!",
                        "I diagnose a skill issue, Brother!",
                        "[The essence of Comedy]"
                    ],
                    null,
                    null,
                    Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/death.wav"),
                    "FU-"
                )
            );
        }
        else {
            // SECOND ENCOUNTER (4-4)
            
            __instance.SetAdditionalData(
                new ExtensionMethods.V2AdditionalData(
                    audioSource,
                    [
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_2/taunt1.wav"),
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_2/taunt2.wav"),
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_2/taunt3.wav"),
                        Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_2/taunt4.wav"), 
                    ],
                    [
                        "This one will cost you an arm and a leg, Brother!",
                        "What's the matter, Brother? Does your arm hurt? BECAUSE I CAN FIX THAT!",
                        "I'LL TEAR YOU LIMB FROM LIMB!",
                        "I'LL FUCKING KILL YOU"
                    ],
                    Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_2/enrage.wav"),
                    "YOU MOTHER FUCKER",
                    Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/death.wav"),
                    "FU-"
                )
            );
            // If the player has died during the first phase of the second encounter
            // play a Taunt at the restart of the fight
            if (!__instance.longIntro && __instance.secondEncounter && __instance.firstPhase)
                __instance.Taunt();
        }

        // V2's 1-4 intro, when they come out from the window (fucking sick btw)
        if (__instance.intro) {
            __instance.PlayVoice(Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/windowbreak.wav"));
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle("Oh fuck here I come");
        }

        // V2's 1-4 long intro, when they bow (also fucking sick btw)
        if (__instance.longIntro) {
            __instance.StartCoroutine(LongIntro(__instance, ___anim));
        }


        Plugin.LogDebug("v2 started");
    }

    static IEnumerator LongIntro(V2 __instance, Animator ___anim) {
        // Wait until the Animator's Intro is set to true, so when anim.SetTrigger("Intro") is called inside the Update method
        yield return new WaitUntil(() => ___anim.GetBool("Intro"));

        AudioSource audioSource = Utils.CreateOneTimeVoiceObject(
            "IntroVoice", 
            Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/v2_1/intro.wav"),
            Utils.CreateSubtitleData(
                Utils.MakeLine("I thought it would be obvious, Brother", 1.2f),
                Utils.MakeLine("After all, I am you", 3.2f),
                Utils.MakeLine("But STRONGER", 5.2f)
            ),
            __instance.transform
        );

        audioSource.Play();
    }
}
