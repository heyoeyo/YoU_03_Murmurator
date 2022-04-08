using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEnder : MonoBehaviour
{

    [SerializeField] GameObject game_over_screen;
    [SerializeField] GameObject big_flock_screen;
    [SerializeField] int big_flock_threshold = 1000000;

    bool is_game_over;
    bool is_big_flock;
    bool big_flock_happened;

    private void OnEnable() {
        FlockEventsManager.SubAbsorbEvent(OnGameOver);
        FlockEventsManager.SubFlockSizeEvent(OnFlockGrow);
    }

    private void OnDisable() {
        FlockEventsManager.UnsubAbsorbEvent(OnGameOver);
        FlockEventsManager.UnsubFlockSizeEvent(OnFlockGrow);
    }

    void OnGameOver() {
        this.game_over_screen.SetActive(true);
        this.is_game_over = true;
    }

    void OnFlockGrow(int flock_size) {

        bool big_flock = (flock_size >= big_flock_threshold);
        if (big_flock && !big_flock_happened) {
            this.big_flock_screen.SetActive(true);
            this.is_big_flock = true;
            this.big_flock_happened = true;
        }
    }

    private void Awake() {
        this.is_game_over = false;
        this.is_big_flock = false;
        this.big_flock_happened = false;
    }


    private void Update() {
        
        if (is_game_over) {
            bool left_clicked = Input.GetMouseButton(0);
            if (left_clicked) {
                SceneState.ReloadScene();
            }
        }

        if (is_big_flock) {
            bool left_clicked = Input.GetMouseButton(0);
            bool space_pressed = Input.GetKeyDown(KeyCode.Space);
            if (left_clicked) {
                this.big_flock_screen.SetActive(false);

            } else if (space_pressed) {
                SceneState.ReloadScene();
            }
        }
    }
}
