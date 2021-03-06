﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context : BehaviourState {

    public DroneAISoccer_red self;
    public float t;
    public Boolean targeting;
    public Context (DroneAISoccer_red self) {
        this.self = self;
    }

    public Vector3 getAcceleration (Vector3 target) {
        if (target.x > 238) {
            target.x = 238;
        } else if (target.x < 62) {
            target.x = 62;
        }

        if (target.z > 140) {
            target.z = 140;
        } else if (target.z < 50) {
            target.z = 50;
        }

        UnityEngine.Debug.DrawLine (self.transform.position, target);

        Vector3 a = target - self.transform.position;

        if (Vector3.Magnitude (a) > 1) {
            return a.normalized;
        } else {
            return a;
        }
    }

    public Vector3 getMyVelocity () {
        Vector3 v = Vector3.zero;
        List<Vector3> list_p = self.friend_tag == "Blue" ? self.positions_blue : self.positions_red;
        List<Vector3> list_v = self.friend_tag == "Blue" ? self.velocities_blue : self.velocities_red;

        for (int i = 0; i < 3; i++) {
            if (Vector3.Distance (list_p[i], self.transform.position) < 1) {
                v = list_v[i];
            }
        }

        return v;
    }
}
