using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitions : MonoBehaviour
{
    [SerializeReference] float fade_in_time = 0.5f;

    bool in_transition;
    Image black_cover;


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    private void OnEnable() => SceneState.SubLoadStage(FadeOut);
    private void OnDisable() => SceneState.UnSubLoadStage(FadeOut);


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    private void Awake() {
        this.in_transition = false;
        this.black_cover = GetComponentInChildren<Image>();
    }

    private void Start() {
        FadeIn(fade_in_time);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    void FadeIn(float fade_time_sec) {
        if (!in_transition) {
            StartCoroutine(_FadeInCR(fade_time_sec));
        }
    }

    void FadeOut(int scene_index, float fade_time_sec) {
        if (!in_transition) {
            StartCoroutine(_FadeOutCR(scene_index, fade_time_sec));
        }
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Behaviors (Co-routines)

    IEnumerator _FadeInCR(float fade_time_sec) {

        // Signal transisiton so we don't repeat calls
        in_transition = true;

        // Be sure to enable the frame cover
        Color frame_color = new Color(0, 0, 0, 1);
        this.black_cover.color = frame_color;
        this.black_cover.enabled = true;

        // Loop while making the cover more transparent
        float start_time = Time.time;
        float t = 0;
        while (true) {

            // Calculate the current fraction of fade time
            t = (Time.time - start_time) / fade_time_sec;
            if (t > 1) {
                break;
            }

            // Update the frame alpha change to become more transparent
            frame_color.a = 1 - t;
            this.black_cover.color = frame_color;
            yield return new WaitForEndOfFrame();
        }

        // Disable the frame at the end, since it's invisible anyways
        this.black_cover.enabled = false;

        // Signal end of transition
        in_transition = false;
        SceneState.SignalLoadHasEnded();
    }

    IEnumerator _FadeOutCR(int scene_index, float fade_time_sec) {

        // Signal transisiton so we don't repeat calls
        in_transition = true;

        // Be sure to enable the frame cover
        Color frame_color = new Color(0, 0, 0, 0);
        this.black_cover.color = frame_color;
        this.black_cover.enabled = true;

        // Loop while making the cover more opaque
        float start_time = Time.time;
        float t = 0;
        while (true) {

            // Calculate the current fraction of fade time
            t = (Time.time - start_time) / fade_time_sec;
            if (t > 1) {
                break;
            }

            // Update the frame alpha change to become more opaque
            frame_color.a = t;
            this.black_cover.color = frame_color;
            yield return new WaitForEndOfFrame();
        }

        // Signal end of transition
        in_transition = false;
        SceneManager.LoadScene(scene_index);
    }
}