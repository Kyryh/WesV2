using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace WesV2.Patches;

[HarmonyPatch(typeof(V2))]
partial class SwitchWeaponPatch : DefaultPatch {
    [HarmonyPatch(nameof(V2.KnockedOut))]
    [HarmonyPostfix]
    static void KnockedOutPostfix(V2 __instance, string triggerName) {
        __instance.GetVoice()?.KnockedOut(triggerName);
        Plugin.LogDebug("v2 knocked out: " + triggerName);
    }

}
