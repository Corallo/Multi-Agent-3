using System;
using System.Collections;
using UnityEngine;

public class positionForShooting : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        Vector3 ballToGoal = context.self.other_goal.transform.position - context.self.position_ball;
        Vector3 target = context.self.position_ball - (ballToGoal.normalized * 4.2f) + context.self.velocity_ball * 1.2f;

        UnityEngine.Debug.DrawLine(context.self.position_ball, target, Color.red);

        context.self.m_Drone.Move_vect(context.getAcceleration(target));
        
        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}