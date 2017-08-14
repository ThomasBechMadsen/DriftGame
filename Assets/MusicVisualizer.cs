﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVisualizer : MonoBehaviour {

    const int numberOfBars = 32;
    const int numberOfSamples = 512;

    public GameObject barObject;
    public AudioSource musicSource;
    public float barSpacing;
    public FFTWindow fftWindow;
    public float baseBufferDecrease;
    public float bufferMultiplier;

    float[] samples;
    public Band[] bands;

    // Use this for initialization
    void Start () {
        samples = new float[numberOfSamples];
        bands = new Band[numberOfBars];
        for(int i = 0; i < bands.Length; i++)
        {
            bands[i] = new Band(0,0,0);
        }

        float boxGenStartPointX = -(barSpacing * numberOfBars) / 2;

        for (int i = 0; i < numberOfBars; i++)
        {
            GameObject g = Instantiate(barObject, transform);
            g.transform.localPosition = new Vector3(boxGenStartPointX + barSpacing * i, 0, 0);
            g.GetComponent<musicBarScript>().band = i;
        }
        musicSource.time = 80;
	}
	
	// Update is called once per frame
	void Update () {
        GetSpectrumData();
        MakeFrequencyBands32();
        BandBuffer32();
    }


    void GetSpectrumData()
    {
        musicSource.GetSpectrumData(samples, 0, fftWindow);
    }

    void MakeFrequencyBands32()
    {
        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for(int i = 0; i < 32; i++)
        {
            float average = 0;

            if (i == 8 || i == 16 || i == 20 || i == 24 || i == 28)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);
                if (power == 3)
                {
                    sampleCount -= 2;
                }
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += (samples[count] * (count + 1));
                count++;
            }

            average /= count;
            bands[i].frequency = average * 80;
        }
        
    }

    void BandBuffer32()
    {
        for (int i = 0; i < 32; ++i)
        {
            if (bands[i].frequency > bands[i].buffer)
            {
                bands[i].buffer = bands[i].frequency;
                bands[i].bufferDecrease = baseBufferDecrease;
            }
            else if (bands[i].frequency < bands[i].buffer)
            {
                bands[i].buffer -= bands[i].bufferDecrease;
                bands[i].bufferDecrease *= bufferMultiplier;
            }
        }
    }
}

[System.Serializable]
public class Band
{
    public float frequency = 0, buffer = 0, bufferDecrease = 0;

    public Band(float frequency, float buffer, float bufferDecrease)
    {
        this.frequency = frequency;
        this.buffer = buffer;
        this.bufferDecrease = bufferDecrease;
    }
}