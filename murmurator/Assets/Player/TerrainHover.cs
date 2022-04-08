using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TerrainHover : MonoBehaviour
{
    [SerializeField] LayerMask terrain_layer;
    [SerializeField, Range(1f, 10f)] float hover_height = 5f;
    [SerializeField] float corrective_strength = 10f;
    [SerializeField] float drag_strength = 0.5f;

    // Downward ray-cast resources
    Ray down_ray;
    Vector3 d_ray_origin;
    Vector3 d_ray_direction;
    const float d_ray_y_offset = 1000f;

    Rigidbody rb;

    private void Awake() {
        this.rb = this.GetComponent<Rigidbody>();

        // Allocate storage for 'downward' ray, which is fired from way up in the sky
        this.d_ray_origin = Vector3.up * d_ray_y_offset;
        this.d_ray_direction = Vector3.down;
        this.down_ray = new Ray(d_ray_origin, d_ray_direction);
    }

    void FixedUpdate() {

        Vector3 player_shadow_point = GetTerrainShadowPoint(this.transform.position);
        float curr_hover_height = this.transform.position.y - player_shadow_point.y;
        float target_height_delta = hover_height - curr_hover_height;

        Vector3 corrective_force = Vector3.up * target_height_delta * target_height_delta * corrective_strength * Time.fixedDeltaTime * Mathf.Sign(target_height_delta);
        Vector3 drag_force = Vector3.down * this.rb.velocity.y * drag_strength * Time.fixedDeltaTime;
        this.rb.AddForce(corrective_force + drag_force);
    }

    RaycastHit GetTerrainHit(Vector3 birds_eye_position) {

        // Initialize output
        RaycastHit hit_info;

        // Cast ray straight down at the birds eye position
        d_ray_origin.x = birds_eye_position.x;
        d_ray_origin.z = birds_eye_position.z;
        down_ray.origin = d_ray_origin;
        bool hit_terrain = Physics.Raycast(down_ray, out hit_info, Mathf.Infinity, terrain_layer);
        if (hit_terrain) {
            return hit_info;
        }

        // If we didn't hit the terrain, something is probably wrong...
        Debug.LogWarning("Error raycasting to terrain: No intersection found! Are we off the map?");
        return hit_info;
    }

    Vector3 GetTerrainShadowPoint(Vector3 birds_eye_position) {
        return GetTerrainHit(birds_eye_position).point;
    }

}
