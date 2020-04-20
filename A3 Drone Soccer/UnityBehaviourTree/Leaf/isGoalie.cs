using System;
using System.Collections;
using UnityEngine;

public class isGoalie : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;
        
        return context.self.id == 0 ? NodeStatus.SUCCESS : NodeStatus.FAILURE;
    }

    public override void OnReset () { }
}