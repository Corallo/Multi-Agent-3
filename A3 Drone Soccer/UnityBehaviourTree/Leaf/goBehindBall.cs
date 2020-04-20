using System;
using System.Collections;
using UnityEngine;

public class goBehindBall : Leaf {

    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        int zOffset = 7;
        if (context.self.transform.position.z < context.self.position_ball.z) {
            zOffset = -zOffset;
        }

        Vector3 target = Vector3.zero;
        if (context.self.friend_tag == "Blue") {
            target = context.self.position_ball - new Vector3 (7, 0, zOffset);
        } else {
            target = context.self.position_ball + new Vector3 (7, 0, zOffset);
        }

        context.self.m_Drone.Move_vect(context.getAcceleration(target));

        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}