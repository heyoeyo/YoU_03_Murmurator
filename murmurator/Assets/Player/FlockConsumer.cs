using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FlockConsumer : MonoBehaviour
{
    [SerializeField] BoidVFXControl start_flock;
    [SerializeField] LayerMask target_layer;
    [SerializeField] float size_check_buffer_factor = 1.5f;
    [SerializeField] int minimum_flock_size_padding = 50;

    SphereCollider collider_ref;
    int flock_size;


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    private void Awake() {

        flock_size = 0;

        // Get collider reference so we can scale collision radius as flock grows bigger
        this.collider_ref = this.GetComponent<SphereCollider>();

        // Make sure we have the target layer set... since this seems to come undone easily
        bool no_layermask_set = (target_layer.value == 0);
        if (no_layermask_set) {
            Debug.LogErrorFormat("Missing target layermask!  ({0})", this.name);
        }
    }

    private void Start() {
        CombineWithFlock(start_flock);
    }

    private void OnTriggerEnter(Collider other) {

        // Check for overlap with collectible layermask
        // -> This is done using binary-encoded integers
        int collider_layermask = 1 << other.gameObject.layer;
        bool is_target = (collider_layermask & target_layer.value) > 0;
        if (is_target) {

            // Get info about other flock
            BoidVFXControl flock_ref = other.GetComponent<BoidVFXControl>();
            int other_flock_size = flock_ref.GetFlockSize();
            bool other_flock_is_bigger = CheckOtherFlockIsBigger(other_flock_size);

            // Signal that a flock collision has happened & combine with it
            FlockEventsManager.TriggerCollideEvent(other_flock_size);
            CombineWithFlock(flock_ref);

            // Trigger absorb event to signal game end if we are a lot smaller than the other flock
            if (other_flock_is_bigger) {
                FlockEventsManager.TriggerAbsorbedByFlockEvent();
            }
        }
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    bool CheckOtherFlockIsBigger(int other_flock_size) {

        int curr_flock_size_with_buffer = Mathf.RoundToInt(this.flock_size * size_check_buffer_factor);
        curr_flock_size_with_buffer = Mathf.Max(curr_flock_size_with_buffer, minimum_flock_size_padding);
        bool other_is_bigger = (other_flock_size > curr_flock_size_with_buffer);

        return other_is_bigger;
    }

    void CombineWithFlock(BoidVFXControl other_flock) {

        // Combine other flock into this flock
        this.flock_size = other_flock.JoinFlock(this.transform, this.flock_size);
        FlockEventsManager.TriggerFlockSizeEvent(this.flock_size);

        // Update collider radius, since our flock is getting bigger!
        this.collider_ref.radius = ScaleFlockRadius();
    }

    float ScaleFlockRadius() {
        float log_size = Mathf.Log10(1 + this.flock_size);
        float new_radius = Mathf.Lerp(1, 15, log_size / 7);
        return new_radius;
    }
}
