using UnityEngine;
using System.Collections;
using System;

public class BlueprintLeaf : Leaf
{
    // optional
    public BlueprintLeaf()
    {
    }

    public override NodeStatus OnBehave(BehaviourState state)
    {
        Context context = (Context)state;

        if (true)
            return NodeStatus.SUCCESS;
        else if (true)
            return NodeStatus.RUNNING;
        else
            return NodeStatus.FAILURE;
    }

    public override void OnReset()
    {
    }
}
