using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GunShooter : MonoBehaviour
{
    public Transform gunModel; // Drag your gun mesh/model here
    private Vector3 modelOriginalPosition;
    public Transform firePoint;
    public ParticleSystem muzzleFlash;
    public AudioSource gunShotSound;
    public Light muzzleLight;
    public float fireRate = 0.5f;
    public float damage = 10f;
    public float lightDuration = 0.05f;
    public float recoilDistance = 0.05f;
    public float recoilDuration = 0.05f;

    private float lastFireTime = 0f;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Vector3 originalPosition;
    private bool isRecoiling = false;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.activated.AddListener(OnTriggerPulled);
        modelOriginalPosition = gunModel.localPosition;
    }

    void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(OnTriggerPulled);
    }

    void OnTriggerPulled(ActivateEventArgs args)
    {
        if (Time.time - lastFireTime < fireRate) return;

        lastFireTime = Time.time;

        // Muzzle effects
        muzzleFlash?.Play();
        gunShotSound?.Play();
        if (muzzleLight != null) StartCoroutine(FlashMuzzleLight());
        if (!isRecoiling) StartCoroutine(DoRecoil());

        // Raycast
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log("Hit " + hit.collider.name);
            hit.collider.GetComponent<Target>()?.OnShot();
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }

    IEnumerator FlashMuzzleLight()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleLight.enabled = false;
    }

    IEnumerator DoRecoil()
    {
        isRecoiling = true;
        Vector3 recoilPos = modelOriginalPosition - gunModel.forward * recoilDistance;

        float elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            gunModel.localPosition = Vector3.Lerp(modelOriginalPosition, recoilPos, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gunModel.localPosition = recoilPos;

        elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            gunModel.localPosition = Vector3.Lerp(recoilPos, modelOriginalPosition, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gunModel.localPosition = modelOriginalPosition;
        isRecoiling = false;
    }
}

