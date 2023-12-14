using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(UltrakillEvent), "Invoke")]
class Patch8 : DefaultPatch {
    static void Prefix(UltrakillEvent __instance) {
        if (__instance.toDisActivateObjects != null) {
            foreach (GameObject gameObject in __instance.toDisActivateObjects) {
                if (gameObject?.name == "V2 Green Arm") {
                    gameObject.transform.Find("FleeingVoice").SetParent(null);
                }
            }
        }
    }
}