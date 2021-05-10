using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator 
{
    ShapeSettings settings;
    NoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new NoiseFilter[settings.noiseLayers.Length];

        for (int processFilters = 0; processFilters < noiseFilters.Length; processFilters++)
        {
            noiseFilters[processFilters] = new NoiseFilter(settings.noiseLayers[processFilters].noiseSettings);
        }
    }
    
    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
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
        return pointOnUnitSphere * settings.planetRadius * (1 + elevation);
    }
}
