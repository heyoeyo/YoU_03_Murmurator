using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnSizer
{
    public static Dictionary<FlockScale, int> size_lut = new Dictionary<FlockScale, int> {
        { FlockScale.a_Small, 10 },
        { FlockScale.b_Small, 75 },
        { FlockScale.c_Medium, 350 },
        { FlockScale.d_Medium, 2500 },
        { FlockScale.e_Medium, 10000 },
        { FlockScale.f_Large, 40000 },
        { FlockScale.g_Large, 90000 },
    };

    public static int GetSpawnCount(FlockScale flock_scale) {
        int base_count = SpawnSizer.size_lut[flock_scale];
        return Mathf.RoundToInt(base_count * Random.Range(0.8f, 1.5f));
    }

}

public enum FlockScale {
    a_Small, b_Small, c_Medium, d_Medium, e_Medium, f_Large, g_Large
}