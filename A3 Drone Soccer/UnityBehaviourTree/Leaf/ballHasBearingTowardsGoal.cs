using System;
using System.Collections;
using UnityEngine;

public class ballHasBearingTowardsGoal : Leaf {

    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        RaycastHit hit;

        if (Physics.SphereCast (context.self.position_ball, 2.2f, context.self.velocity_ball, out hit, Mathf.Infinity, (1 << 9))) {
            return NodeStatus.SUCCESS;
        } else {
            return NodeStatus.FAILURE;
        }
    }

    public override void OnReset () { }
}