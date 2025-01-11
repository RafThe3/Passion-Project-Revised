using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactText;
    [Min(0), SerializeField] private float interactDistance = 1;
    [Min(0), SerializeField] private float teleportationTime = 1;
    [SerializeField] private TextMeshProUGUI countdownText;

    private float countdown;
    private bool canCountdown;

    private void Start()
    {
        interactText.enabled = false;
        countdownText.enabled = false;
        canCountdown = false;
        countdown = teleportationTime;
    }

    private void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 playerPos = player.transform.position - transform.position;
        bool isPlayerNear = playerPos.magnitude < interactDistance;
        bool hasInteracted = Input.GetButtonDown("Interact");

        if (canCountdown)
        {
            countdownText.enabled = true;
            countdownText.text = $"Teleporting in {(int)(countdown -= Time.deltaTime)}";
            if (countdown <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        if (isPlayerNear)
        {
            interactText.enabled = !canCountdown;
            UpdateInteractText();
            if (hasInteracted)
            {
                canCountdown = true;
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
