using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderMovement : MonoBehaviour
{
    [SerializeField, Min(0.5f)] float pattern_radius;
    [SerializeField, Min(2f)] float pattern_period_sec;

    // For convenience
    const float twopi = 2f * Mathf.PI;

    // Variables used to reference relative positioning & orientation
    Vector3 starting_point;
    Vector3 prev_point;

    // Variables used to handle wander pattern
    float pattern_period_multiplier;
    RandomSinusoid h_scaler;
    RandomSinusoid r_base;
    RandomSinusoid r_offset;
    RandomSinusoid r_scaler;

    void Start()
    {
        // Get starting point, since we'll wander 'around' this position over time
        starting_point = this.transform.position;
        prev_point = starting_point - this.transform.forward;


        // Pre-generate random quantities for pattern creation
        h_scaler = new RandomSinusoid(2, 8).SetRange(0, 3);
        r_base = new RandomSinusoid(3, 8);
        r_offset = new RandomSinusoid(1, 5);
        r_scaler = new RandomSinusoid(1,5);
        
        // Pre-generate a random timing offset, to prevent synchronization among flocks
        float pattern_direction = Random.Range(0, 2) > 0 ? 1 : -1;
        float randomized_period = Random.Range(1f, 3f) * pattern_period_sec;
        pattern_period_multiplier = pattern_direction * twopi / randomized_period;
    }

    void Update()
    {
        // Calculate 2D x-z planar movement
        float pattern_time = Time.time * pattern_period_multiplier;
        float r = CalculateRadialPosition(pattern_time);
        Vector3 delta_x = Vector3.right * r * Mathf.Cos(pattern_time);
        Vector3 delta_z = Vector3.forward * r * Mathf.Sin(pattern_time);

        // Calculate up-down movement along z-axis only
        float h = h_scaler.Update(pattern_time);
        Vector3 delta_y = Vector3.up * h;

        // Update positioning and orientation
        Vector3 new_point = this.starting_point + delta_x + delta_y + delta_z;
        this.transform.position = new_point;
        this.transform.rotation = Quaternion.LookRotation((new_point - prev_point), this.transform.up);
        prev_point = new_point;
    }

    float CalculateRadialPosition(float time) {
        return pattern_radius * r_scaler.Update(time) * (r_base.Update(time) + r_offset.Update(time));
    }
}
