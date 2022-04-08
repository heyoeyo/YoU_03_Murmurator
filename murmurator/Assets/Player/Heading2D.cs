using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Heading2D {

    Quaternion heading;
    Vector3 up_axis;
    float max_rotate_dps;

    public Heading2D(Vector3 initial_heading, Vector3 up_direction, float max_rotate_degrees_per_second) {
        this.heading = Quaternion.LookRotation(initial_heading, up_direction);
        this.up_axis = up_direction;
        this.max_rotate_dps = max_rotate_degrees_per_second;
    }
    public Heading2D(Vector3 initial_heading, float max_rotate_degrees_per_second) : this(initial_heading, Vector3.up, max_rotate_degrees_per_second) { }

    public void SetRotationSpeed(float max_rotate_degrees_per_second) {
        this.max_rotate_dps = max_rotate_degrees_per_second;
    }

    public Quaternion Current() {
        return this.heading;
    }

    public Quaternion Update(Vector3 target_direction) {
        this.heading = Quaternion.LookRotation(target_direction, this.up_axis);
        return this.heading;
    }

    public Quaternion SmoothUpdate(Vector3 target_direction, float delta_time) {

        Quaternion target_quaternion = Quaternion.LookRotation(target_direction, this.up_axis);
        this.heading = Quaternion.RotateTowards(this.heading, target_quaternion, max_rotate_dps * delta_time);

        return this.heading;
    }
}