using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlockCounterUI : MonoBehaviour
{
    [SerializeField, Range(0.05f, 1f)] float counter_scaling;

    Text display_text;

    const float minimum_counts_per_second = 3;

    int flock_count;
    float lagged_count;

    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    private void Awake() {
        this.display_text = this.GetComponent<Text>();
        this.lagged_count = 0;
        this.flock_count = 0;
    }

    private void Update() {

        // Only update display count if it is different from set flock count
        int count_diff = this.flock_count - Mathf.RoundToInt(this.lagged_count);
        if (count_diff > 0) {
            float amount_to_increase = Mathf.Max(minimum_counts_per_second, count_diff * counter_scaling) * Time.deltaTime;
            this.lagged_count += amount_to_increase;

            // Update displayed text
            this.display_text.text = Mathf.RoundToInt(this.lagged_count).ToString();
        }


    }

    // ----------------------------------------------------------------------------------------------------------------
    // Events

    private void OnEnable() => FlockEventsManager.SubFlockSizeEvent(SetFlockCount);
    private void OnDisable() => FlockEventsManager.UnsubFlockSizeEvent(SetFlockCount);

    void SetFlockCount(int new_flock_count) {
        this.flock_count = new_flock_count;
    }
}
