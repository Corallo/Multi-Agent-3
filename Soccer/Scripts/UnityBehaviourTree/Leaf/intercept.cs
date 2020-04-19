using System;
using System.Collections;
using UnityEngine;

public class intercept : Leaf {
    // optional
    public intercept () { }

    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        Vector3 target = Vector3.zero;
        RaycastHit hit;

        if (Physics.Raycast (context.directions.position_ball, context.directions.velocity_ball, out hit, Mathf.Infinity, (1 << 9))) {
            target = hit.point;
        } else {
            float c = (context.self.transform.position.x - context.directions.position_ball.x) / context.directions.velocity_ball.x;
            target = context.directions.position_ball + c * context.directions.velocity_ball;

            if (target.x < 65) {
                target.x = 62;
            } else if (target.x > 235) {
                target.x = 238;
            }
        }

        context.self.m_Drone.Move_vect (context.getAcceleration (target));

        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}