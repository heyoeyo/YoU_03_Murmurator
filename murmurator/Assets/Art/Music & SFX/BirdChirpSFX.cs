using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdChirpSFX : MonoBehaviour
{

    [SerializeField] string sfx_folder_path;
    [SerializeField, Min(1)] int num_audio_sources = 5;
    [SerializeField, Range(0, 1f)] float max_volume = 1f;
    [SerializeField] bool mute_sfx;

    AudioSource[] audio_sources;
    AudioClip[] sfx_clips;


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    void OnEnable() => FlockEventsManager.SubCollideEvent(PlaySounds);
    void OnDisable() => FlockEventsManager.UnsubCollideEvent(PlaySounds);


    void PlaySounds(int flock_size) {

        // Figure out how many sfx clips to play, based on size of flock
        int num_sounds = Mathf.RoundToInt(Mathf.Log10(flock_size * Random.Range(0.75f, 1.5f)));
        num_sounds = Mathf.Min(num_sounds, this.audio_sources.Length);

        // First figure out all the sound effects we'll play
        int next_sfx_idx = Random.Range(0, this.sfx_clips.Length);
        int max_offset = this.sfx_clips.Length / num_sounds;
        AudioClip[] sfx_to_play = new AudioClip[num_sounds];
        for (int i = 0; i < num_sounds; i++) {
            sfx_to_play[i] = this.sfx_clips[next_sfx_idx];
            int next_offset = Random.Range(1, max_offset);
            next_sfx_idx = (next_sfx_idx + next_offset) % this.sfx_clips.Length;
        }

        // Attach each sound effect to it's own source, so they can play in parallel
        float delay = 0f;
        for (int i = 0; i < sfx_to_play.Length; i++) {
            this.audio_sources[i].clip = sfx_to_play[i];
            this.audio_sources[i].volume = Random.Range(0.3f, 1f) * this.max_volume;
            this.audio_sources[i].PlayDelayed(delay);
            delay += Random.Range(0.1f, 0.25f);
        }

    }

    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    void Awake() {

        // Create audio sources
        audio_sources = new AudioSource[num_audio_sources];
        for (int i = 0; i < audio_sources.Length; i++) {
            AudioSource new_source = this.gameObject.AddComponent<AudioSource>();
            new_source.playOnAwake = false;
            new_source.loop = false;
            new_source.volume = 1;
            new_source.mute = this.mute_sfx;
            audio_sources[i] = new_source;
        }

        // Load chirp sound effects
        this.sfx_clips = Resources.LoadAll<AudioClip>(sfx_folder_path);
        if (this.sfx_clips.Length == 0) {
            Debug.LogError("No chirp sound effects loaded!");
        }
    }
}
