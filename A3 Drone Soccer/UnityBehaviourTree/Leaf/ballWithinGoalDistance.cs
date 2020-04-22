using System;
using System.Collections;
using UnityEngine;

public class ballWithinGoalDistance : Leaf {
    // optional
    public ballWithinGoalDistance () { }

    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;
        if (context.self.position_ball.z > 50 && context.self.position_ball.z < 150 &&
            (context.self.friend_tag == "Blue" && context.self.position_ball.x < 85 ||
                context.self.friend_tag == "Red" && context.self.position_ball.x > 215)) {
            return NodeStatus.SUCCESS;
        } else {
            return NodeStatus.FAILURE;
        }
    }

    public override void OnReset () { }
}
