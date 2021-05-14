using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator 
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;
    public MinMaxHeight elevationMinMax; 

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];

        for (int processFilters = 0; processFilters < noiseFilters.Length; processFilters++)
        {
            noiseFilters[processFilters] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[processFilters].noiseSettings);
        }
        elevationMinMax = new MinMaxHeight();
    }
    
    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }

        for (int processElevation = 1; processElevation < noiseFilters.Length; processElevation++)
        {
            if (settings.noiseLayers[processElevation].enabled)
            {
                float mask = (settings.noiseLayers[processElevation].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += noiseFilters[processElevation].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        elevationMinMax.AddValue(elevation);
        return elevation;
    }
    public float GetScaledElevation(float unscaledElevation)
    {
        float elevation = Mathf.Max(0, unscaledElevation);
        elevation = settings.planetRadius * (1 + elevation);
        return elevation;
    }
}
