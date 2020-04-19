using System;
using System.Collections;
using UnityEngine;

public class takeDefaultPosition : Leaf {
    private Vector3 default_blue = new Vector3 (65, 0, 100);
    private Vector3 default_red = new Vector3 (235, 0, 100);
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        float offset = context.self.friend_tag == "Blue" ? -30 : 30;
        Vector3 circle = context.self.own_goal.transform.position + new Vector3 (offset, 0, 0);
        Vector3 ballToGoal = context.self.own_goal.transform.position - context.directions.position_ball;
        Vector3 ballToCircle = circle - context.directions.position_ball;
        float x_p = ballToGoal.x - offset / 6;
        float c = x_p / ballToCircle.x;

        Vector3 target = context.directions.position_ball + c * ballToCircle;

        context.self.m_Drone.Move_vect (context.getAcceleration (target));

        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}