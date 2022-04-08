using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    [SerializeField] bool mute_audio;
    [SerializeField] float song_end_buffer = 0.5f;

    [Header("Game Over Music")]
    [SerializeField] Music end_song;

    [Header("Game Music")]
    [SerializeField] Music[] background_songs;

    // For clarity
    const bool loop_end_music = true;
    const bool loop_bg_music = false;

    // Playback control resources
    AudioSource audio_ref;
    bool game_is_over;
    int[] shuffled_play_order;
    int curr_shuffle_idx;

    
    // ----------------------------------------------------------------------------------------------------------------
    // Events

    private void OnEnable() => FlockEventsManager.SubAbsorbEvent(PlayEndMusic);
    private void OnDisable() => FlockEventsManager.UnsubAbsorbEvent(PlayEndMusic);

    void PlayEndMusic() {

        // Signal end of game, so background music doesn't cycle
        game_is_over = true;

        // Stop playing background music
        StartCoroutine(FadeToEndMusic(1.5f));
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    void Start() {

        // Setup audio source
        audio_ref = this.gameObject.GetComponent<AudioSource>();

        // Set up play order
        shuffled_play_order = GetShuffledBGPlayOrder();
        curr_shuffle_idx = 0;
    }

    void FixedUpdate() {

        bool play_next_song = !this.audio_ref.isPlaying && !game_is_over;
        if (play_next_song) {

            // Select the next song, using shuffled indexing
            int next_song_idx = shuffled_play_order[curr_shuffle_idx];
            Music next_song = this.background_songs[next_song_idx];
            audio_ref = next_song.ConfigureAudioSource(audio_ref, this.mute_audio, loop_bg_music);
            audio_ref.PlayDelayed(song_end_buffer);

            // Update shuffle index, with wrap around so we keep looping over all songs
            curr_shuffle_idx = (curr_shuffle_idx + 1) % this.shuffled_play_order.Length;
        }
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    int[] GetShuffledBGPlayOrder() {

        // For clarity
        bool debug_output = false;

        // We want to create an array with shuffled indices for playing background music
        // -> Shuffling is done to make sequence different each playthru
        // -> However, want to fix first song (to zeroth entry), to give consistent start
        int[] shuffled_idxs = new int[this.background_songs.Length];
        shuffled_idxs[0] = 0;

        // Get original index counting sequence (e.g. 1, 2, 3, etc.), not including zeroth entry
        List<int> orig_indexes = new List<int>();
        for (int i = 1; i < this.background_songs.Length; i++) {
            orig_indexes.Add(i);
        }

        // See form of original indices
        if (debug_output) {
            Debug.Log("Original indices (excluding zeroth entry)");
            foreach (int item in orig_indexes) {
                Debug.Log(item.ToString());
            }
        }

        // Randomly pull entries from original set of indices & add to shuffled list
        for (int i = 1; i < shuffled_idxs.Length; i++) {
            int random_orig_idx = Random.Range(0, orig_indexes.Count);
            shuffled_idxs[i] = orig_indexes[random_orig_idx];
            orig_indexes.RemoveAt(random_orig_idx);
        }

        // For debugging
        if (debug_output) {
            Debug.Log("Shuffled indices");
            foreach (int item in shuffled_idxs) {
                Debug.Log(item.ToString());
            }
        }

        return shuffled_idxs;
    }

    IEnumerator FadeToEndMusic(float fade_time_sec) {

        float start_volume = audio_ref.volume;
        float start_time = Time.time;
        while (true) {

            // Calculate the current fraction of fade time
            float t = (Time.time - start_time) / fade_time_sec;
            if (t > 1) {
                break;
            }

            // Reduce volume over time to fade out
            audio_ref.volume = start_volume * (1f - t);
            yield return new WaitForEndOfFrame();
        }

        // Stop audio completely, just to be sure
        audio_ref.Stop();

        // Switch to playing end music
        audio_ref = end_song.ConfigureAudioSource(audio_ref, this.mute_audio, loop_end_music);
        audio_ref.PlayDelayed(0.5f);
    }

    [System.Serializable]
    private struct Music {

        [SerializeField] public AudioClip clip;
        [SerializeField, Range(0, 1f)] public float volume;

        public AudioSource ConfigureAudioSource(AudioSource source, bool mute, bool loop) {
            source.clip = this.clip;
            source.volume = this.volume;
            source.playOnAwake = false;
            source.mute = mute;
            source.loop = loop;
            return source;
        }

    }
}
