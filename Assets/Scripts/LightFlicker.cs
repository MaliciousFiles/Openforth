using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public float minOnTime;
    public float maxOnTime;

    public int minFlickers;
    public int maxFlickers;

    public float minFlickerOnTime;
    public float maxFlickerOnTime;

    public float minFlickerOffTime;
    public float maxFlickerOffTime;

    private double changeTime = 0;
    private int flickers = 0;

    void Update()
    {
        if (Time.time > changeTime)
        {
            var light = GetComponent<Light>();

            if (changeTime > 0) light.enabled = !light.enabled;

            if (light.enabled)
            {
                if (flickers > 0)
                {
                    changeTime = Time.time + Random.Range(minFlickerOnTime, maxFlickerOnTime);
                }
                else
                {
                    changeTime = Time.time + Random.Range(minOnTime, maxOnTime);
                }
            }
            else
            {
                changeTime = Time.time + Random.Range(minFlickerOffTime, maxFlickerOffTime);

                if (flickers <= 0)
                {
                    flickers = Random.Range(minFlickers, maxFlickers);
                }

                flickers--;
            }
        }
    }
}
