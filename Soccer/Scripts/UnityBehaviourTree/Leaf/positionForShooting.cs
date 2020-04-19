using System;
using System.Collections;
using UnityEngine;

public class positionForShooting : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        Vector3 ballToGoal = context.self.other_goal.transform.position - context.directions.position_ball;
        Vector3 target = context.directions.position_ball - (ballToGoal.normalized * 4.2f) + context.directions.velocity_ball * 1.8f;

        UnityEngine.Debug.DrawLine(context.directions.position_ball, target, Color.red);

        context.self.m_Drone.Move_vect(context.getAcceleration(target));
        
        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}