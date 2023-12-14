using HarmonyLib;



[HarmonyPatch(typeof(V2), "Die")]
class Patch2 : DefaultPatch {
    static void Prefix(V2 __instance, bool ___dead, bool ___bossVersion) {
        // In case V2 already died, because for some fucking reason Die() is called twice when you spawn them with the spawner arm and kill them???
        if (___dead)
            return;
        
        // In case the player does the silly thing with the coin and kills V2 before they finish the intro
        if (__instance.inIntro)
            __instance.transform.Find("IntroVoice").gameObject.SetActive(false);

        ExtensionMethods.V2AdditionalData data = __instance.GetAdditionalData();

        data.timesChangedWeapon = 0;

        // Stop the AudioSource or V2 keeps talking when dead
        data.audioSource.Stop();

        if (!__instance.secondEncounter || !___bossVersion) {
            // Create a temporary new object because otherwise when V2 gets deleted it doesn't play the clip
            Utils.PlayClipAtPoint(data.deathAudioClip, __instance.transform.position);
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(data.deathSubtitle);
        }


        Plugin.LogDebug("v2 died");

        /* StackFrame[] stackFrames = new StackTrace().GetFrames();  // get method calls (frames)

        // write call stack method names
        foreach (StackFrame stackFrame in stackFrames)
        {
            Plugin.LogInfo(stackFrame.GetMethod().Name);   // write method name
        } */
    }
}