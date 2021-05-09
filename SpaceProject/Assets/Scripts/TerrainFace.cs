using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int yVertex = 0; yVertex < resolution; yVertex++)
        {
            for (int xVertex = 0; xVertex < resolution; xVertex++)
            {
                int plusLoop = xVertex + yVertex * resolution;
                Vector2 percent = new Vector2(xVertex, yVertex) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[plusLoop] = pointOnUnitSphere;

                if(xVertex != resolution - 1 && yVertex != resolution - 1)
                {
                    triangles[triIndex] = plusLoop;
                    triangles[triIndex + 1] = plusLoop + resolution + 1;
                    triangles[triIndex + 2] = plusLoop + resolution;

                    triangles[triIndex + 3] = plusLoop;
                    triangles[triIndex + 4] = plusLoop + 1;
                    triangles[triIndex + 5] = plusLoop + resolution + 1;
                    triIndex += 6;
                } 
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
