using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEditor;

[RequireComponent(typeof(CapsuleCollider))]
public class BoidVFXControl : MonoBehaviour {

    [SerializeField] LayerMask terrain_layer;
    [SerializeField] Transform target_transform;
    [SerializeField] FlockScale flock_scale;

    [SerializeField] AnimationCurve wander_amplitude;
    [SerializeField] AnimationCurve wander_speed;
    [SerializeField] AnimationCurve wander_frequency;
    [SerializeField] AnimationCurve collision_radius;

    VisualEffect vfx;
    CapsuleCollider collider_ref;
    int num_boids;
    bool is_collected_debug = false;

    // VFX property references
    int target_position_id;
    int num_boids_id;
    int wander_amplitude_id;
    int wander_speed_id;
    int wander_frequency_id;
    int collision_radius_id;


    // ----------------------------------------------------------------------------------------------------------------
    // Public

    public int GetFlockSize() => this.num_boids;
    public FlockScale GetFlockScale() => this.flock_scale;

    public int JoinFlock(Transform new_target, int destination_flock_size) {

        // Prevent future joins, by removing collisions
        DisableCollisions();

        // Make sure this flock now follows the new target
        this.target_transform = new_target;

        // Update flock parameters to be consistent with larger flock being joined
        int new_flock_size = destination_flock_size + GetFlockSize();
        SetVFXProperties(new_flock_size);

        return new_flock_size;
    }



    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    void Start() {

        this.collider_ref = this.GetComponent<CapsuleCollider>();

        // Setup vfx
        this.vfx = this.GetComponent<VisualEffect>();
        GetPropertyIds();
        IntializeVFX();
    }

    void FixedUpdate() {
        this.vfx.SetVector3(target_position_id, target_transform.position);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // VFX Graph-specific

    void GetPropertyIds() {
        target_position_id = Shader.PropertyToID("TargetPosition");
        num_boids_id = Shader.PropertyToID("NumBoids");
        wander_amplitude_id = Shader.PropertyToID("WanderAmplitude");
        wander_speed_id = Shader.PropertyToID("WanderSpeed");
        wander_frequency_id = Shader.PropertyToID("WanderFrequency");
        collision_radius_id = Shader.PropertyToID("CollisionRadius");
    }

    void IntializeVFX() {

        // Make sure we have a target for the boids above ground
        SetInitialFlockTarget();

        // Generate a boid count on startup
        this.num_boids = SpawnSizer.GetSpawnCount(this.flock_scale);
        this.vfx.SetInt(this.num_boids_id, num_boids);

        // Scale collider to 'match' (very approximately) flock size
        this.collider_ref.radius = ScaleFlockRadius(this.num_boids);

        // Set initial properties based on initial flock size
        SetVFXProperties(this.num_boids);
    }

    void SetVFXProperties(int num_total_boids) {

        // The total number of boids determines the flock behavior, through relative sizing
        float boid_size_log = Mathf.Log10(num_total_boids);

        // Get wander properties
        float r_amplitude = wander_amplitude.Evaluate(boid_size_log) * RandomScale();
        float r_speed = wander_speed.Evaluate(boid_size_log) * RandomScale();
        float r_freq = wander_frequency.Evaluate(boid_size_log) * RandomScale(0.9f);

        // Shrink collision radius down when dealing with smaller flocks
        float r_collision_radius = collision_radius.Evaluate(boid_size_log) * RandomScale();

        // Set properties on vfx graph
        this.vfx.SetInt(num_boids_id, num_boids);
        this.vfx.SetFloat(wander_amplitude_id, r_amplitude);
        this.vfx.SetFloat(wander_speed_id, r_speed);
        this.vfx.SetFloat(wander_frequency_id, r_freq);
        this.vfx.SetFloat(collision_radius_id, r_collision_radius);
    }

    // ----------------------------------------------------------------------------------------------------------------
    // Helpers
    float RandomScale() => Random.Range(0.75f, 1f);
    float RandomScale(float min_scale) => Random.Range(min_scale, 1f);

    void SetInitialFlockTarget() {
        RaycastHit hit_info = Utils.GetTerrainHit(this.transform.position, terrain_layer);
        this.transform.position = hit_info.point + Vector3.up * 4;
        this.target_transform = this.transform;
    }

    float ScaleFlockRadius(int flock_size) {
        float log_size = Mathf.Log10(1 + flock_size);
        float new_radius = Mathf.Lerp(25, 115, log_size / 7);
        return new_radius;
    }

    void DisableCollisions() {

        // Disable the collider
        this.collider_ref.enabled = false;
        this.is_collected_debug = true;

        // Remove from collectible layer
        this.gameObject.layer = LayerMask.NameToLayer("Default");
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Debug

    void OnDrawGizmos() {

        // Stop indicating info if we're collected
        if (this.is_collected_debug) {
            return;
        }

        Vector3 line_top = this.target_transform.position + 200 * Vector3.up;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.target_transform.position, line_top);

        #if UNITY_EDITOR
            Handles.Label(line_top, " " + this.flock_scale.ToString());
        #endif
    }
}
