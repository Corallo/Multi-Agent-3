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

        if (Physics.Raycast (context.self.position_ball, context.self.velocity_ball, out hit, Mathf.Infinity, (1 << 9))) {
            target = hit.point;
        } else {
            float c = (context.self.transform.position.x - context.self.position_ball.x) / context.self.velocity_ball.x;
            target = context.self.position_ball + c * context.self.velocity_ball;

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