using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReader : MonoBehaviour
{

    [SerializeField] Transform camera_ref;

    bool enable_input;
    Vector3 raw_input;
    UserInput user_input;


    // ----------------------------------------------------------------------------------------------------------------
    // Events

    private void OnEnable() => FlockEventsManager.SubAbsorbEvent(DisableInput);
    private void OnDisable() => FlockEventsManager.UnsubAbsorbEvent(DisableInput);
    void DisableInput() => this.enable_input = false;


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    private void Awake() {

        // For clarity
        this.enable_input = true;
        this.raw_input = Vector3.zero;
        this.user_input.is_active = false;
        this.user_input.direction = Vector3.forward;

    }

    private void Update() {

        // Read raw inputs
        raw_input.x = Input.GetAxisRaw("Horizontal");
        raw_input.z = Input.GetAxisRaw("Vertical");
        user_input.is_active = raw_input.sqrMagnitude > 0.1f;

        // Blank inputs when disabled
        if (!this.enable_input) {
            raw_input.x = 0;
            raw_input.z = 0;
            user_input.is_active = false;
        }

        // Update the input direction, taking into account where the camera is facing
        // -> The direction the camera is facing is considered 'forward'
        if (user_input.is_active) {
            Quaternion camera_orientation = Quaternion.Euler(0, camera_ref.localEulerAngles.y, 0);
            user_input.direction = camera_orientation * raw_input.normalized;
        }
    }


    // ----------------------------------------------------------------------------------------------------------------
    // Helpers

    public UserInput Read() {
        return this.user_input;
    }

    public struct UserInput {
        public bool is_active;
        public Vector3 direction;
    }
}
