using System;
using System.Collections;
using UnityEngine;

public class targetBallShoot : Leaf {

    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        if (!context.targeting) {
            context.targeting = true;
            context.t = Time.time;
        }

        Vector3 target = context.self.position_ball + (context.self.position_ball - context.self.transform.position).normalized * 20;
        context.self.m_Drone.Move_vect (context.getAcceleration (target));

        if (Time.time - context.t < 0.4) {
            return NodeStatus.RUNNING;
        } else {
            context.targeting = false;
            return NodeStatus.SUCCESS;
        }
    }

    public override void OnReset () { }
}