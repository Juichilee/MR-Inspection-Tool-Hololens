using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public class VirtualCameraFollowHand : MonoBehaviour
{

    public GameObject RealSenseModel;
    ObjectManipulator realSenseOM;
    BoundsControl realSenseBC;
    SolverHandler solverHandler;
    public bool activateFollow = false;
    bool alreadyActive = false;
    Transform objectToFollow;
    Transform originalParent;



    public void setActivateFollow()
    {
        activateFollow = activateFollow == true ? activateFollow = false : activateFollow = true;
    }
    // Start is called before the first frame update
    private void Awake()
    {
        RealSenseModel = GameObject.Find("RealSenseModel");
        realSenseOM = RealSenseModel.GetComponent<ObjectManipulator>();
        realSenseBC = RealSenseModel.GetComponent<BoundsControl>();
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
                this.transform.parent = jointTransform;
                realSenseOM.enabled = false;
                realSenseBC.enabled = false;
                LabelManagerScript.SharedInstance.flipProjection(180); // Flip Label projection 180
            }
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 180);  
            alreadyActive = true;
        }
        else
        {
            if(alreadyActive == true)
            {
                realSenseOM.enabled = true;
                realSenseBC.enabled = true;
                LabelManagerScript.SharedInstance.flipProjection(180);
                this.transform.parent = originalParent;
                alreadyActive = false;
            }
        }
    }
}
