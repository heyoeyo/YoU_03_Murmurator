using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCounter : MonoBehaviour {

    private void LateUpdate() {

        // Initialize counters
        SortedDictionary<string, int> flock_counters = new SortedDictionary<string, int>();

        // Find all boid listing & total up counts per flock scale
        BoidVFXControl[] all_flocks = FindObjectsOfType<BoidVFXControl>();
        foreach (BoidVFXControl flock in all_flocks) {
            string scale_name = flock.GetFlockScale().ToString();
            if (!flock_counters.ContainsKey(scale_name)) {
                flock_counters.Add(scale_name, 0);
            }
            flock_counters[scale_name] += flock.GetFlockSize();
        }

        // Get total count by adding all scale counts
        int total_flock_count = 0;
        foreach (int count in flock_counters.Values) {
            total_flock_count += count;
        }

        // Print out info
        List<string> debug_strs = new List<string>();
        debug_strs.Add(string.Format("TOTAL FLOCK COUNT: {0}", total_flock_count.ToString()));
        foreach (var item in flock_counters) {
            debug_strs.Add(string.Format("  {0}: {1}", item.Key, item.Value));
        }
        string final_str = string.Join("\n", debug_strs);
        Debug.Log(final_str);

        // Disable object after initial print out, since we don't actually care!
        this.gameObject.SetActive(false);
    }
}
