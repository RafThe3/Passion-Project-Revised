using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactText;
    [Min(0), SerializeField] private float interactDistance = 1;
    [SerializeField] private Canvas checkpointMenu;

    private void Start()
    {
        interactText.enabled = false;
        checkpointMenu.enabled = false;
    }

    private void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 playerPos = player.transform.position - transform.position;
        bool isPlayerNear = playerPos.magnitude < interactDistance;


        if (isPlayerNear)
        {
            bool hasInteracted = Input.GetButtonDown("Interact");

            interactText.enabled = Time.timeScale == 1;
            UpdateInteractText();

            if (hasInteracted)
            {
                checkpointMenu.enabled = true;
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            interactText.enabled = false;
        }
    }

    private void UpdateInteractText()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer | RuntimePlatform.WindowsEditor:
                interactText.text = "Press E to interact";
                break;

            case RuntimePlatform.XboxOne:
                interactText.text = "Press Y to interact";
                break;

            case RuntimePlatform.PS4 | RuntimePlatform.PS5:
                interactText.text = "Press Triangle to interact";
                break;

            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
