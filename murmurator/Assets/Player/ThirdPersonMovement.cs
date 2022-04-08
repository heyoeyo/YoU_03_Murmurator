using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(InputReader))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Terrain Avoidance")]
    [SerializeField] LayerMask terrain_layer;
    [SerializeField] float forward_distance = 50f;

    [Header("Movement")]
    [SerializeField] float max_speed = 10f;
    [SerializeField] float max_rotate_dps = 360f;

    InputReader input_reader;
    InputReader.UserInput user_input;

    Rigidbody rb;
    Heading2D heading;

    float move_force;

    OrientToTerrain ori;

    private void Awake() {

        this.ori = new OrientToTerrain(forward_distance, terrain_layer);

        this.input_reader = this.GetComponent<InputReader>();
        this.rb = this.GetComponent<Rigidbody>();
        this.heading = new Heading2D(Vector3.forward, max_rotate_dps);

        // Pre-calculate the force needed to achieve the desired max (terminal) speed
        CalculateMoveForce();
    }

    void CalculateMoveForce() {

        // Function which calculates the force applied (on each fixed update)
        // so that the object will reach the configured max-speed

        // For clarity
        float mass = this.rb.mass;
        float drag = this.rb.drag;
        float dt = Time.fixedDeltaTime;

        // See: https://forum.unity.com/threads/terminal-velocity.34667/
        // From MaxV = ((F / drag) - F * dt) / mass
        // V = (F/mass) * (1/drag - dt)
        // F = MaxV * mass / (1/drag - dt)
        //   = MaxV * mass * drag / (1 - dt * drag) -> more stable, no divide by zero
        this.move_force = max_speed * mass * drag / (1f - dt * drag);
    }

    private void Update() {
        Quaternion pitch_ori = this.ori.GetPitchOrientation(this.transform);

        user_input = input_reader.Read();
        if (user_input.is_active) {
            // Rotate player to point in input direction
            // (this is done in update instead of fixedupdate so that movement doesn't look jerky)
            this.heading.SmoothUpdate(user_input.direction, Time.deltaTime);
        }

        this.rb.MoveRotation(pitch_ori * this.heading.Current());

    }

    private void FixedUpdate() {

        // Move player when we have keyboard input
        if (user_input.is_active) {
            this.rb.AddForce(this.move_force * user_input.direction);
            //this.rb.AddForce(this.move_force * this.transform.forward);
        }
    }

}
