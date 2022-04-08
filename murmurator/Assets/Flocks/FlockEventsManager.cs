using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class FlockEventsManager
{
    public static Action<int> SizeEvent;
    public static Action<int> CollideEvent;
    public static Action AbsorbedByFlockEvent;

    public static void SubCollideEvent(Action<int> func) => CollideEvent += func;
    public static void UnsubCollideEvent(Action<int> func) => CollideEvent -= func;
    public static void TriggerCollideEvent(int num_in_other_flock) {
        if (CollideEvent != null) {
            CollideEvent(num_in_other_flock);
        }
    }


    public static void SubFlockSizeEvent(Action<int> func) => SizeEvent += func;
    public static void UnsubFlockSizeEvent(Action<int> func) => SizeEvent -= func;

    public static void TriggerFlockSizeEvent(int new_flock_size) {
        if (SizeEvent != null) {
            SizeEvent(new_flock_size);
        }
    }


    public static void SubAbsorbEvent(Action func) => AbsorbedByFlockEvent += func;
    public static void UnsubAbsorbEvent(Action func) => AbsorbedByFlockEvent -= func;

    public static void TriggerAbsorbedByFlockEvent() {
        if (AbsorbedByFlockEvent != null) {
            AbsorbedByFlockEvent();
        }
    }

}
