using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class AutoCameraZoom : MonoBehaviour
{
    [SerializeField] float upper_flock_size = 1000000;
    [SerializeField] float min_distance = 15f;
    [SerializeField] float max_distance = 450f;
    [SerializeField] float top_height_angle_deg = 35f;
    [SerializeField] float mid_height_angle_deg = 25f;
    [SerializeField] float bot_height_angle_deg = 8f;
    [SerializeField] float seconds_to_update = 10f;

    CinemachineFreeLook camera_ref;
    float player_flock_size;
    float camera_flock_size;
    float update_rate;


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    void OnEnable() => FlockEventsManager.SubFlockSizeEvent(RecordNewPlayerFlockSize);
    void OnDisable() => FlockEventsManager.UnsubFlockSizeEvent(RecordNewPlayerFlockSize);

    void RecordNewPlayerFlockSize(int flock_size) {
        this.player_flock_size = (float)flock_size;
        this.update_rate = (this.player_flock_size - this.camera_flock_size) / seconds_to_update;
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    void Start() {
        this.camera_ref = this.GetComponent<CinemachineFreeLook>();

        UpdateCamera(0);
    }

    void Update() {

        bool need_weight_update = (Mathf.RoundToInt(this.player_flock_size) > Mathf.RoundToInt(this.camera_flock_size));
        if (need_weight_update) {
            this.camera_flock_size += this.update_rate * Time.deltaTime;
            this.camera_flock_size = Mathf.Min(this.camera_flock_size, this.player_flock_size);
            UpdateCamera(this.camera_flock_size);
        }
    }

    void UpdateCamera(float flock_size) {

        // Scale camera distance as if it is radius of a sphere, containing 'flock size' elements
        // -> V = k*r^3, so r ~ cuberoot(v)
        float norm_size = flock_size / this.upper_flock_size;
        float new_distance = min_distance + Mathf.Pow(norm_size, 1f / 3f) * (max_distance - min_distance);

        // Update camera orbits
        SetTopOrbit(new_distance);
        SetMidOrbit(new_distance);
        SetBotOrbit(new_distance);
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    void SetOrbit(int orbit_index, float distance, float height_angle_deg, float radius_scale) {

        // Calculate new height & radius for orbit
        float height = Mathf.Sin(height_angle_deg * Mathf.Deg2Rad) * distance;
        float radius = distance * radius_scale;

        // Update selected orbit
        CinemachineFreeLook.Orbit[] orbits = this.camera_ref.m_Orbits;
        orbits[orbit_index].m_Height = height;
        orbits[orbit_index].m_Radius = radius;
    }

    void SetTopOrbit(float distance) => SetOrbit(0, distance, this.top_height_angle_deg, 1f);
    void SetMidOrbit(float distance) => SetOrbit(1, distance, this.mid_height_angle_deg, 0.9f);
    void SetBotOrbit(float distance) => SetOrbit(2, distance, this.bot_height_angle_deg, 0.85f);

}
