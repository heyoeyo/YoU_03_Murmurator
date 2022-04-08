using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuListener : MonoBehaviour {

    bool fadein_finished;
    bool game_has_started;
    AudioSource game_start_audio;

    private void OnEnable() => SceneState.SubLoadHasEnded(RecordFadeEnd);
    private void OnDisable() => SceneState.UnSubLoadHasEnded(RecordFadeEnd);
    void RecordFadeEnd() => this.fadein_finished = true;

    void Start() {
        this.fadein_finished = false;
        this.game_has_started = false;
        this.game_start_audio = this.GetComponent<AudioSource>();
    }

    void Update() {
        bool left_clicked = Input.GetMouseButton(0);
        if (!game_has_started && fadein_finished && left_clicked) {
            float audio_clip_length = this.game_start_audio.clip.length;
            this.game_has_started = true;
            this.game_start_audio.Play();
            SceneState.LoadNext(audio_clip_length);
        }
    }

}
