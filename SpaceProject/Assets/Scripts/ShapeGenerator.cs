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
        float elevation = 0;
        for (int processElevation = 0; processElevation < noiseFilters.Length; processElevation++)
        {
            if (settings.noiseLayers[processElevation].enabled)
            {
                elevation += noiseFilters[processElevation].Evaluate(pointOnUnitSphere);
            }
        }
        return pointOnUnitSphere * settings.planetRadius * (1 + elevation);
    }
}
