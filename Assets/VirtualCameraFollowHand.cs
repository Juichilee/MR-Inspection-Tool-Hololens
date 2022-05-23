using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class VirtualCameraFollowHand : MonoBehaviour
{

    SolverHandler solverHandler;
    public bool activateFollow = false;
    bool alreadyActive = false;
    Transform originalParent;
    //Vector3 offset;
    //Quaternion rotatedOffset;
    // Start is called before the first frame update
    private void Awake()
    {
        solverHandler = GetComponent<SolverHandler>();
        originalParent = this.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        if (activateFollow && handJointService != null)
        {
            Transform jointTransform = handJointService.RequestJointTransform(TrackedHandJoint.Wrist, Handedness.Left);
            if (!alreadyActive)
            {
                //offset = transform.position - jointTransform.transform.position;
                //rotatedOffset = transform.rotation - jointTransform.transform.rotation;
                this.transform.parent = jointTransform;
            }
            //Vector3 targetPos = jointTransform.transform.position + offset;
            //transform.position += (targetPos - transform.position);
            //transform.rotation = jointTransform.rotation;
            alreadyActive = true;
        }
        else
        {
            this.transform.parent = originalParent;
            alreadyActive = false;
        }
    }
}
