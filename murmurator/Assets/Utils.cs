using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static RaycastHit GetTerrainHit(Vector3 birds_eye_position, LayerMask terrain_layer) {

        // Initialize output
        RaycastHit hit_info;

        // Cast ray straight down at the birds eye position
        Vector3 ray_origin = birds_eye_position;
        ray_origin.y = 10000f;
        Ray down_ray = new Ray(ray_origin, Vector3.down);
        bool hit_terrain = Physics.Raycast(down_ray, out hit_info, Mathf.Infinity, terrain_layer);
        if (hit_terrain) {
            return hit_info;
        }

        // If we didn't hit the terrain, something is probably wrong...
        Debug.LogWarning("Error raycasting to terrain: No intersection found! Are we off the map?");
        return hit_info;
    }
}

public struct RandomSinusoid {

    float amplitude;
    float offset;
    float freq;
    float phase;

    public RandomSinusoid(float freq_min, float freq_max) {
        this.freq = Random.Range(freq_min, freq_max);
        this.phase = Random.Range(-1f, 1f) * Mathf.PI;
        this.amplitude = 1f;
        this.offset = 0f;
    }

    public RandomSinusoid SetRange(float y_min, float y_max) {        
        this.amplitude = Mathf.Abs(y_max - y_min);
        this.offset = (y_min + y_min) / 2f;
        return this;
    }

    public RandomSinusoid SetRange(float amplitude) {
        this.amplitude = amplitude;
        this.offset = 0f;
        return this;
    }

    public float Update(float time) {
        return this.amplitude * Mathf.Sin(this.freq * time + this.phase) + this.offset;
    }

    public float Update() {
        return this.Update(Time.time);
    }
}