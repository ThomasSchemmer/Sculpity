using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RightHand : MonoBehaviour
{
    public SteamVR_Action_Pose pose;
    public SteamVR_Action_Boolean trigger;

    private LineRenderer lr;

    private void Start() {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
    }

    void Update()
    {
        if (trigger.state) {
            Vector3 dir = Quaternion.AngleAxis(60, transform.right) * transform.forward;
            ChunkManager.CheckForIntersections(pose.localPosition, dir);
            lr.positionCount = 2;
            lr.SetPosition(0, pose.localPosition);
            lr.SetPosition(1, pose.localPosition + dir);
            lr.enabled = true;
        } else {
            lr.enabled = false;
        }
    }
}
