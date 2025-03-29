using HarmonyLib;
using UnityEngine;

namespace WesV2.Patches;

[HarmonyPatch(typeof(UltrakillEvent))]
class UltrakillEventPatches : DefaultPatch {

    [HarmonyPatch(nameof(UltrakillEvent.Invoke))]
    [HarmonyPostfix]
    static void InvokePostfix(UltrakillEvent __instance) {
        if (__instance.toDisActivateObjects != null) {
            foreach (GameObject gameObject in __instance.toDisActivateObjects) {
                if (gameObject?.name == "V2 Green Arm") {
                    gameObject.transform.Find("FleeingVoice").SetParent(null);
                }
            }
        }
    }
}
