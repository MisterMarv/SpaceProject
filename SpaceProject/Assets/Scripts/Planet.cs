using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2,256)]
    public int resolutionFace = 10;
    public bool autoUpdate = true;
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColourGenerator colourGenerator = new ColourGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int triVertex = 0; triVertex < 6; triVertex++)
        {
            if (meshFilters[triVertex] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[triVertex] = meshObj.AddComponent<MeshFilter>();
                meshFilters[triVertex].sharedMesh = new Mesh();
            }
            meshFilters[triVertex].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[triVertex] = new TerrainFace(shapeGenerator, meshFilters[triVertex].sharedMesh, resolutionFace, directions[triVertex]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == triVertex;
            meshFilters[triVertex].gameObject.SetActive(renderFace);
        }
    }
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {
        for (int processMesh = 0; processMesh < 6; processMesh++)
        {
            if (meshFilters[processMesh].gameObject.activeSelf)
            {
                terrainFaces[processMesh].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);

    }
    void GenerateColours()
    {
        colourGenerator.UpdateColours();
        for (int processMesh = 0; processMesh < 6; processMesh++)
        {
            if (meshFilters[processMesh].gameObject.activeSelf)
            {
                terrainFaces[processMesh].UpdateUVs(colourGenerator);
            }
        }
    }
}
