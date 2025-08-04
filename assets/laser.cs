using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LaserSight : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    public LineRenderer laserLine;
    public float maxDistance = 100f;
    public Transform laserOrigin;

    private bool isGrabbed = false;

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        laserLine.enabled = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        laserLine.enabled = false;
    }

    void Update()
    {
        if (!isGrabbed) return;

        Vector3 start = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;

        laserLine.SetPosition(0, start);

        if (Physics.Raycast(start, direction, out RaycastHit hit, maxDistance))
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, start + direction * maxDistance);
        }
    }
}