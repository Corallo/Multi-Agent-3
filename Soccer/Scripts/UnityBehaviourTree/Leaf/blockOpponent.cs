using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class blockOpponent : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        float ballZ = context.directions.position_ball.z;
        bool aboveBall = context.self.transform.position.z > ballZ;
        List<Vector3> list_enemies_p = context.self.friend_tag == "Blue" ? context.directions.positions_red : context.directions.positions_blue;
        List<Vector3> list_enemies_v = context.self.friend_tag == "Blue" ? context.directions.velocities_red : context.directions.velocities_blue;

        float bestDistance = float.MaxValue;
        Vector3 bestTarget = Vector3.zero;
        bool targetFound = false;

        for (int i = 0; i < 3; i++) {
            if (aboveBall && list_enemies_p[i].z > ballZ) {
                float d = Vector3.Distance (list_enemies_p[i], context.self.transform.position);
                if (d < bestDistance) {
                    bestDistance = d;
                    bestTarget = list_enemies_p[i];
                    targetFound = true;
                }
            } else if (!aboveBall && list_enemies_p[i].z < ballZ) {
                float d = Vector3.Distance (list_enemies_p[i], context.self.transform.position);
                if (d < bestDistance) {
                    bestDistance = d;
                    bestTarget = list_enemies_p[i] + list_enemies_v[i].normalized * 10;
                    targetFound = true;
                }
            }
        }

        if (targetFound) {
            context.self.m_Drone.Move_vect(context.getAcceleration(bestTarget));
            return NodeStatus.SUCCESS;
        } else {
            return NodeStatus.FAILURE;
        }
    }

    public override void OnReset () { }
}