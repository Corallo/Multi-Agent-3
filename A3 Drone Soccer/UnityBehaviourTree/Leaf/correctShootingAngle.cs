using System;
using System.Collections;
using UnityEngine;

public class correctShootingAngle : Leaf {
    public override NodeStatus OnBehave (BehaviourState state) {
        Context context = (Context) state;

        Vector3 selfToBall = context.self.position_ball - context.self.transform.position;
        RaycastHit hit;

        if (Physics.SphereCast (context.self.transform.position, 2.2f, new Vector3 (selfToBall.x, 0, selfToBall.z), out hit, Mathf.Infinity, (1 << 9))) {
            if (hit.collider.gameObject == context.self.other_goal && hit.point.z < 108 && hit.point.z > 92) {
                UnityEngine.Debug.DrawRay (context.self.transform.position, selfToBall * 1000, Color.blue);
                return NodeStatus.SUCCESS;
            } else {
                return NodeStatus.FAILURE;
            }

        } else {
            return NodeStatus.FAILURE;
        }
    }

    public override void OnReset () { }
}
