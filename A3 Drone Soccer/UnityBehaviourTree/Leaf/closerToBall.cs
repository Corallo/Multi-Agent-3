using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closerToBall : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        float distance = Vector3.Distance (context.self.transform.position, context.self.position_ball);

        List<Vector3> list = context.self.friend_tag == "Blue" ? context.self.positions_blue : context.self.positions_red;

        for (int i = 0; i < 3; i++) {
            if (Vector3.Distance (list[i], context.self.transform.position) < 1) {
                continue;
            } else {
                if (Vector3.Distance (list[i], context.self.position_ball) < distance) {
                    return NodeStatus.FAILURE;
                }
            }
        }

        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}