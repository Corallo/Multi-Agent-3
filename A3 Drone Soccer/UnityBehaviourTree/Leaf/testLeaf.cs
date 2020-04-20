using System;
using System.Collections;
using UnityEngine;

public class testLeaf : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        context.self.m_Drone.Move_vect ((context.self.position_ball - context.self.transform.position).normalized);

        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}