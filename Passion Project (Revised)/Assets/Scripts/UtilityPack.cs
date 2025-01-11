using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UtilityPack : MonoBehaviour
{
    [SerializeField] private PackType packType = PackType.None;
    [SerializeField] private PickupType pickupType = PickupType.None;
    [SerializeField] private AudioClip pickupSFX;

    [Header("Ammo")]
    [Min(0), SerializeField] private int ammoToGive = 1;

    [Header("Manual Pickup Type")]
    [SerializeField] private float pickupDistance = 1;
    [SerializeField] private TextMeshProUGUI pickupText;

    private void Start()
    {
        if (pickupText)
        {
            pickupText.enabled = false;
        }
    }

    private void Update()
    {
        if (pickupType == PickupType.Manual)
        {
            ManualPickup();
        }
    }

    private void ManualPickup()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 playerPos = player.transform.position - transform.position;
        bool isPlayerNear = playerPos.magnitude < pickupDistance;

        pickupText.enabled = isPlayerNear;

        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GiveUtility();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GiveUtility(other);
        }
    }

    private void GiveUtility(Collider other)
    {
        switch (packType)
        {
            case PackType.Health:
                other.GetComponent<Player>().AddHealthPack();
                break;

            case PackType.Ammo:
                Shooting[] shootings = FindObjectsOfType<Shooting>();
                foreach (Shooting shooting in shootings)
                {
                    shooting.AddAmmo(ammoToGive);
                }
                break;

            default:
                break;
        }
        Camera.main.GetComponent<AudioSource>().PlayOneShot(pickupSFX);
        Destroy(gameObject);
    }

    private void GiveUtility()
    {
        switch (packType)
        {
            case PackType.Health:
                GameObject.FindWithTag("Player").GetComponent<Player>().AddHealthPack();
                break;

            case PackType.Ammo:
                Shooting[] shootings = FindObjectsOfType<Shooting>();
                foreach (Shooting shooting in shootings)
                {
                    shooting.AddAmmo(ammoToGive);
                }
                break;

            default:
                break;
        }
        Camera.main.GetComponent<AudioSource>().PlayOneShot(pickupSFX);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (pickupType == PickupType.Manual)
        {
            Gizmos.DrawWireSphere(transform.position, pickupDistance);
        }
    }

    private enum PackType { None, Health, Ammo }

    private enum PickupType { None, Automatic, Manual }
}
