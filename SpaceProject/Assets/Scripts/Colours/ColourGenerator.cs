using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator 
{
    ColourSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColourSettings settings)
    {
        this.settings = settings;
        if (texture == null || texture.height != settings.biomeColourSettings.biomes.Length)
        {
            texture = new Texture2D(textureResolution*2, settings.biomeColourSettings.biomes.Length, TextureFormat.RGBA32, false);
        }
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColourSettings.noise);
    }

    public void UpdateElevation(MinMaxHeight elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.min, elevationMinMax.max));
    }

    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColourSettings.noiseOffset) * settings.biomeColourSettings.noiseStrength;
        float biomeIndex = 0;
        int numBiomes = settings.biomeColourSettings.biomes.Length;
        float blendRange = settings.biomeColourSettings.blendAmount / 2f + .001f; //00.1f is only because this type of code (on the for loop, more specifically weight) don't work nicely with blendAmout if this value is 0. (FUN FACT)

        for (int biomeProcess = 0; biomeProcess < numBiomes; biomeProcess++)
        {
            float distanceBiomes = heightPercent - settings.biomeColourSettings.biomes[biomeProcess].startHeight;
            float weightBiome = Mathf.InverseLerp(-blendRange, blendRange, distanceBiomes);
            biomeIndex *= (1 - weightBiome);
            biomeIndex += biomeProcess * weightBiome;
        }
        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColours()
    {
        Color[] colours = new Color[texture.width * texture.height];
        int colourIndex = 0;
        foreach (var biome in settings.biomeColourSettings.biomes)
        { 
            for (int processColours = 0; processColours < textureResolution * 2; processColours++)
            {
                Color gradientColor;
                if (processColours < textureResolution)
                {
                    gradientColor = settings.oceanColour.Evaluate(processColours / (textureResolution - 1f));
                }
                else
                {
                    gradientColor = biome.biomeGradient.Evaluate((processColours - textureResolution) / (textureResolution - 1f));
                }
                Color tintColor = biome.tint;
                colours[colourIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
                colourIndex++;
            }
        }
        
        texture.SetPixels(colours);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }

}
