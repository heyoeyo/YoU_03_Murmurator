using System;
using UnityEngine.SceneManagement;

public static class SceneState {

    public static Action LoadHasEnded;
    public static Action<int, float> LoadStage;

    public static void SubLoadStage(Action<int, float> func) => SceneState.LoadStage += func;
    public static void UnSubLoadStage(Action<int, float> func) => SceneState.LoadStage -= func;
    public static void Load(int stage_index, float fade_time_sec) {
        if (LoadStage != null) {
            LoadStage(stage_index, fade_time_sec);
        }
    }
    public static void LoadNext(float fade_time_sec) {
        int curr_scene_idx = SceneManager.GetActiveScene().buildIndex;
        Load(curr_scene_idx + 1, fade_time_sec);
    }

    public static void ReloadScene() {
        float fade_time_sec = 0.5f;
        int curr_scene_idx = SceneManager.GetActiveScene().buildIndex;
        Load(curr_scene_idx, fade_time_sec);
    }

    public static void SubLoadHasEnded(Action func) => SceneState.LoadHasEnded += func;
    public static void UnSubLoadHasEnded(Action func) => SceneState.LoadHasEnded -= func;
    public static void SignalLoadHasEnded() {
        if (LoadHasEnded != null) {
            LoadHasEnded();
        }
    }
}