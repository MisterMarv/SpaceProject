using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    NoiseSettings.SimpleNoiseSettings settings;
    Noise noise = new Noise();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for (int processNumLayer = 0; processNumLayer < settings.numLayers; processNumLayer++)
        {
            float calculatedNoise = noise.Evaluate(point * frequency + settings.centre);
            noiseValue += (calculatedNoise + 1) * .5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }
        noiseValue = noiseValue - settings.minValue;
        return noiseValue * settings.strength;
    }
}
