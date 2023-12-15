using HarmonyLib;
using UltrakULL.json;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UltrakULL.audio;

static class V2VoicelinesData {
    private static Dictionary<string, V2Subtitles> allSubtitles = new Dictionary<string, V2Subtitles>();

    public static void LoadSubtitles() {
        allSubtitles.Add("default", new V2Subtitles());
        if (Plugin.ultrakullLoaded) {
            string[] files = Directory.GetFiles(Path.Combine(BepInEx.Paths.ConfigPath,"ultrakull"),"*.json");
            foreach (string file in files) {
                try {
                    JObject jsonObject = JObject.Parse(File.ReadAllText(file));
                    string langName = (string)jsonObject["metadata"]["langName"];
                    if (!allSubtitles.ContainsKey(langName) && langName != "te-mp")
                    {
                        V2Subtitles subtitles = JsonConvert.DeserializeObject<V2Subtitles>(jsonObject["subtitles"].ToString());
                        allSubtitles.Add(langName, subtitles);
                    }

                } catch (Exception e) {
                    Plugin.LogError("something happened while trying to load a language: " + e);
                }
            }
        }
    }

    public static string GetSubtitle(string voiceLine) {
        string subtitle;
        if (Plugin.ultrakullLoaded) {
            string langName = LanguageManager.CurrentLanguage.metadata.langName;
            subtitle = Traverse.Create(allSubtitles[langName]).Field(voiceLine).GetValue<string>();
            if (subtitle != null)
                return subtitle;
        }
        subtitle = Traverse.Create(allSubtitles["default"]).Field(voiceLine).GetValue<string>();
        return subtitle;
    }

    public static AudioClip GetAudioClip(string audioFilePath) {
        AudioClip emptyAudioClip = new AudioClip();
        string fixedAudioFilePath = audioFilePath.Replace('/', Path.PathSeparator);
        AudioClip audioClip = AudioSwapper.SwapClipWithFile(emptyAudioClip, AudioSwapper.SpeechFolder + fixedAudioFilePath);
        if(audioClip != emptyAudioClip) {
            return audioClip;
        }
        return Plugin.WesV2AssetBundle.LoadAsset<AudioClip>("assets/" + audioFilePath);
        
    }
}

class V2Subtitles {
    public string subtitles_v2_jumpscare_1 = "Oh shit watch out";
    public string subtitles_v2_jumpscare_2 = "I'm comin' through";

    public string subtitles_v2_windowsbreak = "Oh fuck here I come";

    public string subtitles_v2_intro_1 = "I thought it would be obvious, Brother";
    public string subtitles_v2_intro_2 = "After all, I am you";
    public string subtitles_v2_intro_3 = "But STRONGER";

    public string subtitles_v2_taunt1 = "You aren't the only one who's out for BLOOD, Brother!";
    public string subtitles_v2_taunt2 = "Looks like you've gone a little RUSTY, Brother!";
    public string subtitles_v2_taunt3 = "You think you can best ME?! AFTER ALL THAT I'VE BEEN THROUGH!";
    public string subtitles_v2_taunt4 = "YOU'RE JUST A FUCKING NIKON!";
    public string subtitles_v2_taunt5 = "I diagnose a skill issue, Brother!";
    public string subtitles_v2_taunt6 = "[The essence of comedy]";

    public string subtitles_v2_death = "FUCK";

    public string subtitles_v2_outro1 = "Okay, you know what";
    public string subtitles_v2_outro2 = "I'm gonna call that one a draw";
    public string subtitles_v2_outro3 = "Team Rocket is pissing off again";

    public string subtitles_v2Second_intro1_1 = "Hello, Brother";
    public string subtitles_v2Second_intro1_2 = "I believe you have something of mine";
    public string subtitles_v2Second_intro1_3 = "Something VERY important";

    public string subtitles_v2Second_intro2_1 = "How's about this for a trade?";
    public string subtitles_v2Second_intro2_2 = "I beat you into a fucking pulp";
    public string subtitles_v2Second_intro2_3 = "And you give my arm back";

    public string subtitles_v2Second_taunt1 = "This one will cost you an arm and a leg, Brother!";
    public string subtitles_v2Second_taunt2 = "What's the matter, Brother? Does your arm hurt? BECAUSE I CAN FIX THAT!";
    public string subtitles_v2Second_taunt3 = "I'LL TEAR YOU LIMB FROM LIMB!";
    public string subtitles_v2Second_taunt4 = "I'LL FUCKING KILL YOU";

    public string subtitles_v2Second_enrage = "YOU MOTHERFUCKER";

    public string subtitles_v2Second_fleeing1 = "I WON'T GIVE YOU THE PLEASURE OF KILLING ME!";

    public string subtitles_v2Second_fleeing2_1 = "Gotta get away, gotta get away, oh no...";
    public string subtitles_v2Second_fleeing2_2 = "YOUR FORM IS INCREDIBLE!";

    public string subtitles_v2Second_death = "FUCKING NIKON";
}