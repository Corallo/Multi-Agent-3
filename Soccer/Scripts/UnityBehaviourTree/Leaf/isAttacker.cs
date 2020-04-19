using System;
using System.Collections;
using UnityEngine;

public class isAttacker : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;
        
        return context.self.id == 1 ? NodeStatus.SUCCESS : NodeStatus.FAILURE;
    }

    public override void OnReset () { }
}