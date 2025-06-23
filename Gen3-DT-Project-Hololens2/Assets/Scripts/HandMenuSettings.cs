using UnityEngine;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit;

public class HandMenuSettings : MonoBehaviour
{
    void Start()
    {
        EnableHandRays();
        EnableGazePinch();
    }

    private void EnableHandRays()
    {
        var handRays = PlayspaceUtilities.XROrigin.GetComponentsInChildren<MRTKRayInteractor>(true);
        foreach (var interactor in handRays)
        {
            interactor.gameObject.SetActive(true);
        }
    }

    private void EnableGazePinch()
    {
        var gazePinchInteractors = PlayspaceUtilities.XROrigin.GetComponentsInChildren<GazePinchInteractor>(true);
        foreach (var interactor in gazePinchInteractors)
        {
            interactor.gameObject.SetActive(true);
        }
    }

}
