﻿using System;
using System.Collections;
using UnityEngine;

public class targetBallGoalie : Leaf {

    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        Vector3 target = context.directions.position_ball + context.directions.velocity_ball * 5;

        context.self.m_Drone.Move_vect (context.getAcceleration (target));
        return NodeStatus.SUCCESS;
    }

    public override void OnReset () { }
}