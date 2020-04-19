using System;
using System.Collections;
using UnityEngine;

public class ballMovesTowardsGoal : Leaf {

    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        float v_x = context.directions.velocity_ball.x;

        if (context.self.friend_tag == "Blue" && v_x < 0 ||
            context.self.friend_tag == "Red" && v_x > 0) {
            return NodeStatus.SUCCESS;
        } else {
            return NodeStatus.FAILURE; 
        }
    }

    public override void OnReset () { }
}