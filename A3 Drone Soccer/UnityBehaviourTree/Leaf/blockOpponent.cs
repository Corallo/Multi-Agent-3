using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockOpponent : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        float ballZ = context.self.position_ball.z;
        bool aboveBall = context.self.transform.position.z > ballZ;
        List<Vector3> list_enemies_p = context.self.friend_tag == "Blue" ? context.self.positions_red : context.self.positions_blue;
        List<Vector3> list_enemies_v = context.self.friend_tag == "Blue" ? context.self.velocities_red : context.self.velocities_blue;

        float bestDistance = float.MaxValue;
        Vector3 bestTarget = Vector3.zero;
        bool targetFound = false;

        // for (int i = 0; i < 3; i++) {
        //     float d = Vector3.Distance (list_enemies_p[i], context.self.other_goal.transform.position);
        //     if (aboveBall && list_enemies_p[i].z > ballZ) {
        //         if (d < bestDistance) {
        //             bestDistance = d;
        //             bestTarget = list_enemies_p[i] + list_enemies_v[i].normalized * 10;
        //             targetFound = true;
        //         }
        //     } else if (!aboveBall && list_enemies_p[i].z < ballZ) {
        //         if (d < bestDistance) {
        //             bestDistance = d;
        //             bestTarget = list_enemies_p[i] + list_enemies_v[i].normalized * 10;
        //             targetFound = true;
        //         }
        //     }
        // }

        for (int i = 0; i < 3; i++) {
            float d = Vector3.Distance (list_enemies_p[i], context.self.other_goal.transform.position);
            if (d < bestDistance) {
                bestDistance = d;
                bestTarget = list_enemies_p[i] + list_enemies_v[i] * 5;
                targetFound = true;
            }
        }

        if (targetFound) {
            context.self.m_Drone.Move_vect (context.getAcceleration (bestTarget));
            return NodeStatus.SUCCESS;
        } else {
            return NodeStatus.FAILURE;
        }
    }

    public override void OnReset () { }
}
