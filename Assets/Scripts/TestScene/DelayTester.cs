using UnityEngine;
using System;
using TMPro;

public class DelayTester : MonoBehaviour
{
    public TMP_Text delayText;
    public AudioSource metronomeSource;
    private double startDspTime;
    private double delaySum;
    private int noTouch;

    void Start()
    {

    }

    void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
            double elapsed = AudioSettings.dspTime - startDspTime;
            delaySum += elapsed - Math.Round(elapsed * 2) / 2;
            noTouch++;
            delayText.SetText($"{delaySum / noTouch * 1000:F0}ms");
        }
    }

    public void StartMetronome(){
        startDspTime = AudioSettings.dspTime + 0.2;
        metronomeSource.PlayScheduled(startDspTime);
    }
}
