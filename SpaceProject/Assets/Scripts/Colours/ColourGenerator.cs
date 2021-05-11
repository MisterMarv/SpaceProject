using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator 
{
    ColourSettings settings;
    Texture2D texture;
    const int textureResolution = 50;

    public void UpdateSettings(ColourSettings settings)
    {
        this.settings = settings;
        if (texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(MinMaxHeight elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.min, elevationMinMax.max));
    }

    public void UpdateColours()
    {
        Color[] colours = new Color[textureResolution];
        for (int processColours = 0; processColours < textureResolution; processColours++)
        {
            colours[processColours] = settings.gradient.Evaluate(processColours / (textureResolution - 1f)); 
        }
        texture.SetPixels(colours);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }

}
