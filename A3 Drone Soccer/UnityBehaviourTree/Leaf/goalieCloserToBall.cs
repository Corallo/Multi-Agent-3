using System;
using System.Collections;
using UnityEngine;

public class goalieCloserToBall : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        float goalie_ball_distance = Vector3.Distance (context.self.transform.position, context.self.position_ball);

        if (context.self.friend_tag == "Blue") {
            foreach (Vector3 v in context.self.positions_red) {
                if (Vector3.Distance (v, context.self.position_ball) < goalie_ball_distance) {
                    return NodeStatus.FAILURE;
                }
            }
        } else {
            foreach (Vector3 v in context.self.positions_blue) {
                if (Vector3.Distance (v, context.self.position_ball) < goalie_ball_distance) {
                    return NodeStatus.FAILURE;
                }
            }
        }

        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}