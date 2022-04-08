using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientToTerrain
{
    // Constructor args
    float forward_distance;
    LayerMask terrain_layer;

    // Forward ray-cast resources
    Ray forward_ray;
    Vector3 f_ray_origin;
    Vector3 f_ray_direction;

    // Downward ray-cast resources
    Ray down_ray;
    Vector3 d_ray_origin;
    Vector3 d_ray_direction;
    const float d_ray_y_offset = 1000f;

    public OrientToTerrain(float forward_distance, LayerMask terrain_layer) {

        // Store constructor values
        this.forward_distance = forward_distance;
        this.terrain_layer = terrain_layer;

        // Allocate storage for 'downward' ray, which is fired from way up in the sky
        this.d_ray_origin = Vector3.up * d_ray_y_offset;
        this.d_ray_direction = Vector3.down;
        this.down_ray = new Ray(d_ray_origin, d_ray_direction);

        // Allocate storage for 'forward' ray, which is fired from player position to a point in front of the player
        this.f_ray_origin = Vector3.zero;
        this.f_ray_direction = Vector3.forward;
        this.forward_ray = new Ray(f_ray_origin, f_ray_direction);

    }

    public Quaternion GetPitchOrientation(Transform player) {

        // Project player forward vector onto 2d plane (xz)
        Vector3 proj_player_forward = player.forward;
        proj_player_forward.y = 0;
        proj_player_forward.Normalize();

        float check_distance = forward_distance;

        Vector3 forward_check, backward_check;
        Vector3 forward_shadow = Vector3.zero, backward_shadow = Vector3.zero;

        int max_attempts = 5;
        for (int i = 0; i < max_attempts; i++) {

            // Generate forward/backward points to check terrain height for orienting player
            forward_check = player.position + proj_player_forward * check_distance;
            backward_check = player.position - proj_player_forward * check_distance;

            // Get point in front & behind player
            forward_shadow = GetTerrainShadowPoint(forward_check);
            backward_shadow = GetTerrainShadowPoint(backward_check);

            // Check if the shadow points can 'see' each other -> If not, we're done and can generate our orientation
            bool hit_ground = CheckBlockingTerrain(backward_shadow, forward_shadow, 0.5f);
            if (!hit_ground) {
                break;
            }

            // If we get here, we hit the ground, so try the checks again, but with a shorter check distance
            check_distance = check_distance * 0.75f;
        }

        

        Debug.DrawLine(backward_shadow + Vector3.up * 5, forward_shadow + Vector3.up * 5);
        Debug.DrawLine(backward_shadow, forward_shadow);

        // Orient along vector between behind-to-forward points
        Vector3 point_to = forward_shadow - backward_shadow;
        return Quaternion.FromToRotation(proj_player_forward, point_to);
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

    bool CheckBlockingTerrain(Vector3 start_point, Vector3 end_point, float y_offset) {

        // For clarity
        Vector3 ray_direction = end_point - start_point;
        float ray_distance = ray_direction.magnitude;

        forward_ray.origin = start_point + Vector3.up * y_offset;
        forward_ray.direction = ray_direction.normalized;
        
        RaycastHit hit_info;
        bool hit_terrain = Physics.Raycast(forward_ray, out hit_info, ray_distance, terrain_layer);

        return hit_terrain;
    }
}
