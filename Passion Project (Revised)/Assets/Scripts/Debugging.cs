using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debugging : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [Tooltip("How often it counts the frames."), Min(0), SerializeField] private float pollingTime = 1;

    private float time;
    private int frameCount;

    private void Update()
    {
        time += Time.deltaTime;

        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            fpsText.text = $"{frameRate} FPS";

            time -= pollingTime;
            frameCount = 0;
        }
    }
}
