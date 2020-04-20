using System;
using System.Collections;
using UnityEngine;

public class behindBall : Leaf {

    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        float x = context.self.transform.position.x;
        float z = context.self.transform.position.z;

        if (context.self.friend_tag == "Blue") {
            if (x < context.self.position_ball.x || z < 85 || z > 115) {
                return NodeStatus.SUCCESS;
            } else {
                return NodeStatus.FAILURE;
            }
        } else {
            if (x > context.self.position_ball.x || z < 85 || z > 115) {
                return NodeStatus.SUCCESS;
            } else {
                return NodeStatus.FAILURE;
            }
        }
    }

    public override void OnReset () { }
}