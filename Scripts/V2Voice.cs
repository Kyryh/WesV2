using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WesV2.Scripts {
    public class V2Voice : MonoBehaviour {
        AudioSource audioSource;
        V2 v2;

        AudioClip[] tauntAudioClips;
        string[] tauntSubtitles;

        AudioClip enrageAudioClip;
        string enrageSubtitle;

        AudioClip deathAudioClip;
        string deathSubtitle;

        int[] tauntWeights;

        int timesChangedWeapon = 0;


        protected void Awake() {
            v2 = GetComponent<V2>();
            audioSource = transform.Find("Voice").GetComponent<AudioSource>();
            if (!v2.secondEncounter) {
                // FIRST ENCOUNTER (1-4)

                tauntAudioClips = [
                    V2VoicelinesData.GetAudioClip("v2_1/taunt1"),
                    V2VoicelinesData.GetAudioClip("v2_1/taunt2"),
                    V2VoicelinesData.GetAudioClip("v2_1/taunt3"),
                    V2VoicelinesData.GetAudioClip("v2_1/taunt4"),
                    V2VoicelinesData.GetAudioClip("v2_1/taunt5"),
                    V2VoicelinesData.GetAudioClip("v2_1/taunt6"),
                ];
                tauntSubtitles = [
                    V2VoicelinesData.GetSubtitle("subtitles_v2_taunt1"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2_taunt2"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2_taunt3"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2_taunt4"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2_taunt5"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2_taunt6")
                ];

                deathAudioClip = V2VoicelinesData.GetAudioClip("v2_1/death");
                deathSubtitle = V2VoicelinesData.GetSubtitle("subtitles_v2_death");

            } else {
                // SECOND ENCOUNTER (4-4)

                tauntAudioClips = [
                    V2VoicelinesData.GetAudioClip("v2_2/taunt1"),
                    V2VoicelinesData.GetAudioClip("v2_2/taunt2"),
                    V2VoicelinesData.GetAudioClip("v2_2/taunt3"),
                    V2VoicelinesData.GetAudioClip("v2_2/taunt4"),
                ];
                tauntSubtitles = [
                    V2VoicelinesData.GetSubtitle("subtitles_v2Second_taunt1"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2Second_taunt2"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2Second_taunt3"),
                    V2VoicelinesData.GetSubtitle("subtitles_v2Second_taunt4")
                ];

                enrageAudioClip = V2VoicelinesData.GetAudioClip("v2_2/enrage");
                enrageSubtitle = V2VoicelinesData.GetSubtitle("subtitles_v2Second_enrage");

                deathAudioClip = V2VoicelinesData.GetAudioClip("v2_1/death");
                deathSubtitle = V2VoicelinesData.GetSubtitle("subtitles_v2_death");
            }

            tauntWeights = Enumerable.Repeat(1, tauntAudioClips.Length).ToArray();
        }

        protected void Start() {
            if (v2.secondEncounter && v2.firstPhase && !Plugin.secondEncounterIntro) {
                Taunt();
            }


            // V2's 1-4 intro, when they come out from the window (fucking sick btw)
            if (v2.intro) {
                PlayVoice(V2VoicelinesData.GetAudioClip("v2_1/windowbreak"));
                MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(V2VoicelinesData.GetSubtitle("subtitles_v2_windowsbreak"));
            }

            // V2's 1-4 long intro, when they bow (also fucking sick btw)
            if (v2.longIntro) {
                StartCoroutine(LongIntro());
            }

        }

        // Gets a random taunt based on each taunt's weight
        // Weights are set to 1 to every taunt initially and
        // a taunt's weight is set to 0 whenever it's used
        // (so that no taunt can get used twice in a row)
        // and the rest of the weights are increased by 1
        // so that a taunt that hasn't been used for a long time
        // has a higher chance to get used next

        public int WeightedRandomIndex() {
            int currentSum = 0;
            int index = -1;
            int randomNum = Random.Range(0, tauntWeights.Sum());

            while (randomNum >= currentSum) {
                index++;
                currentSum += tauntWeights[index];
            }

            return index;
        }

        // Plays a random taunt
        public void Taunt() {

            int randomTaunt = WeightedRandomIndex();

            // Increases each weight by 1
            for (int i = 0; i < tauntWeights.Length; i++) {
                tauntWeights[i]++;
            }
            // Sets the chosen taunt's weight to 0

            tauntWeights[randomTaunt] = 0;

            PlayVoice(tauntAudioClips[randomTaunt]);
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(tauntSubtitles[randomTaunt]);
        }

        // Play a voice clip
        void PlayVoice(AudioClip clip) {
            audioSource.pitch = Random.Range(0.95f, 1f);
            audioSource.clip = clip;
            audioSource.Play();
        }


        IEnumerator LongIntro() {
            // Wait until the Animator's Intro is set to true, so when anim.SetTrigger("Intro") is called inside the Update method
            yield return new WaitUntil(() => v2.anim.GetBool("Intro"));

            AudioSource audioSource = Utils.CreateOneTimeVoiceObject(
                "IntroVoice",
                V2VoicelinesData.GetAudioClip("v2_1/intro"),
                Utils.CreateSubtitleData(
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_intro_1"), 1.2f),
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_intro_2"), 3.2f),
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_intro_3"), 5.2f)
                ),
                v2.transform
            );

            audioSource.Play();
        }

        public void SwitchWeapons() {

            timesChangedWeapon++;

            // Play the taunt after that V2 changes weapon 7 times
            if (timesChangedWeapon >= 7) {
                timesChangedWeapon = 0;
                Taunt();
            }

        }

        internal void Enrage(string enrageName) {
            if (enrageName == "STOP HITTING YOURSELF" && !v2.enraged) {
                timesChangedWeapon = 0;
                PlayVoice(enrageAudioClip);
                MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(enrageSubtitle);
            }
        }

        public void Die() {
            // In case V2 already died, because for some fucking reason Die() is called twice when you spawn them with the spawner arm and kill them???
            if (v2.dead) {
                return;
            }

            // In case the player does the silly thing with the coin and kills V2 before they finish the intro
            if (v2.inIntro) {
                transform.Find("IntroVoice").gameObject.SetActive(false);
            }


            timesChangedWeapon = 0;

            // Stop the AudioSource or V2 keeps talking when dead
            audioSource.Stop();

            if (!v2.secondEncounter || !v2.bossVersion) {
                // Create a temporary new object because otherwise when V2 gets deleted it doesn't play the clip
                Utils.PlayClipAtPoint(deathAudioClip, transform.position);
                MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(deathSubtitle);
            }

        }

        public void KnockedOut(string triggerName) {
            if (triggerName == "KnockedDown") {
                if (!v2.secondEncounter)
                    v2.StartCoroutine(Outro());
                else
                    v2.StartCoroutine(Fleeing());
            } else if (triggerName == "Flailing") {
                // the V2 script gets destroyed when transitioning to the
                // final room, so i have to start the coroutine from
                // the plugin itself
                Plugin.Instance.StartCoroutine(Falling());
            } else
                Plugin.LogError("what");

        }


        IEnumerator Outro() {
            yield return new WaitForSeconds(1f);

            AudioSource audioSource = Utils.CreateOneTimeVoiceObject(
                "OutroVoice",
                V2VoicelinesData.GetAudioClip("v2_1/outro"),
                Utils.CreateSubtitleData(
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_outro1")),
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_outro2"), 0.9f),
                    Utils.MakeLine(V2VoicelinesData.GetSubtitle("subtitles_v2_outro3"), 2f)
                ),
                transform
            );

            audioSource.Play();
        }

        IEnumerator Fleeing() {
            // Play the first fight's death clip, just because
            PlayVoice(deathAudioClip);
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(deathSubtitle);

            AudioSource fleeingVoice = Utils.CreateOneTimeVoiceObject(
                "FleeingVoice",
                V2VoicelinesData.GetAudioClip("v2_2/fleeing1"),
                Utils.CreateSubtitleData(),
                transform
            );

            yield return new WaitForSeconds(2.5f);

            fleeingVoice.Play();
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(V2VoicelinesData.GetSubtitle("subtitles_v2Second_fleeing1"));
        }

        static IEnumerator Falling() {
            GameObject fallingV2 = Utils.FindGameObjectByName("8 Stuff(Clone)(Clone)").transform.Find("v2_GreenArm").gameObject;

            AudioSource audioSource = Utils.CreateVoiceObject("FallingVoice", fallingV2.transform);
            audioSource.clip = V2VoicelinesData.GetAudioClip("v2_2/death");
            // Wait until V2 starts falling
            yield return new WaitUntil(() => fallingV2.activeInHierarchy);
            yield return new WaitForSeconds(0.3f);

            audioSource.Play();
            MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(V2VoicelinesData.GetSubtitle("subtitles_v2Second_death"));

        }

        protected void OnDestroy() {
            Plugin.Log.LogInfo("destroyed");
        }

        protected void OnDisable() {
            Plugin.Log.LogInfo("disabled");
        }


    }
}
