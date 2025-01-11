using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemSwitch : MonoBehaviour
{
    [SerializeField] private GameObject item1, item2;
    [Min(0), SerializeField] private float switchCooldown = 1;
    [SerializeField] private TextMeshProUGUI gunNameText;

    private bool hasSwitched = false;
    private float cameraDefaultFOV;

    private void Start()
    {
        item2.SetActive(false);
        item1.SetActive(true);
        gunNameText.text = item1.name;
        cameraDefaultFOV = Camera.main.fieldOfView;
    }

    private void Update()
    {
        bool switchedItem = Mathf.Abs(Input.mouseScrollDelta.y) > Mathf.Epsilon;

        if (switchedItem
            && !hasSwitched
            && Camera.main.fieldOfView == cameraDefaultFOV
            && item2
            && Time.timeScale > 0
            && !FindFirstObjectByType<Shooting>().IsReloading)
        {
            StartCoroutine(SwitchItem(switchCooldown));
        }
    }

    private IEnumerator SwitchItem(float cooldown)
    {
        hasSwitched = true;
        
        if (item1.activeSelf)
        {
            item1.SetActive(false);
            item2.SetActive(true);
            gunNameText.text = item2.name;
        }
        else if (item2.activeSelf)
        {
            item2.SetActive(false);
            item1.SetActive(true);
            gunNameText.text = item1.name;
        }

        yield return new WaitForSeconds(cooldown);

        hasSwitched = false;
    }

    /*
    public void AddItem(GameObject item)
    {
        items.Add(item);
    }
    */

    /*
    public void SwapItem(int index, GameObject item)
    {
        items.Insert(index, item);
    }
    */

    public void SwapItem(int itemNum, GameObject item)
    {
        if (itemNum is not 1 or 2)
        {
            throw new ArgumentException("Item number must be either 1 or 2.");
        }

        switch (itemNum)
        {
            case 1:
                item1 = item;
                break;

            case 2:
                item2 = item;
                break;

            default:
                break;
        }
    }
}
