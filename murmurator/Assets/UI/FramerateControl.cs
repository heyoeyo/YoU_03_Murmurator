using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateControl : MonoBehaviour
{
    [SerializeField] bool enable_framerate_limit = true;
    [SerializeField] int target_framerate = 60;

    private void Awake() {
        Application.targetFrameRate = enable_framerate_limit ? target_framerate : -1;
    }
}
