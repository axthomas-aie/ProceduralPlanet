﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OctaNode : MonoBehaviour {

    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private MeshRenderer meshRenderer;



	public void Build(Vector3[] corners, int divisions, float sphereRadius)
    {
        // rez is the number of vertices on one side of the mesh/triangle
        // the part in parentheses is called the "Mersenne Number"
        int rez = 2 + ((int)Mathf.Pow(2, divisions) - 1);
        // nTris is the number of tris in the mesh
        int t = rez - 2;
        int nTris = (t * (t + 1)) + (rez - 1);
        // nVerts is the number of vertices in the mesh
        // it is the formula for the "Triangle Sequence" of numbers
        int nVerts = (rez * (rez + 1)) / 2;

        Vector3[] vertices = new Vector3[nVerts];
        Vector3[] normals = new Vector3[nVerts];
        int[] indices = new int[nTris * 3];

        float dist01 = Vector3.Distance(corners[0], corners[1]);
        float dist12 = Vector3.Distance(corners[1], corners[2]);
        float dist20 = Vector3.Distance(corners[2], corners[0]);

        float lenAxis01 = dist01 / (rez - 1);
        float lenAxis12 = dist12 / (rez - 1);
        float lenAxis20 = dist20 / (rez - 1);

        Vector3 add1 = (corners[1] - corners[0]).normalized * lenAxis01;
        Vector3 add2 = (corners[2] - corners[1]).normalized * lenAxis12;

        int vIdx = 0;

        for(int i = 0; i < rez; ++i)
        {
            for(int n = 0; n <= i; ++n)
            {
                vertices[vIdx] = corners[0] + add1 * i + add2 * n;
                Vector3 normal = (vertices[vIdx]).normalized;
                vertices[vIdx] = normal * sphereRadius;

                normals[vIdx] = normal;

                ++vIdx;
            }
        }

        int indIdx = 0;
        int rowStartIdx = 1;
        int prevRowStartIdx = 0;

        for(int row = 0; row < rez - 1; ++row)
        {
            bool upright = true;
            int trisInRow = 1 + row * 2;
            int vertsInRowBottom = row + 2;

            int upTri = 0;
            int downTri = 0;

            for(int tri = 0; tri < trisInRow; ++tri)
            {
                if(upright)
                {
                    indices[indIdx  ] = rowStartIdx + upTri + 1;
                    indices[indIdx+1] = rowStartIdx + upTri;
                    indices[indIdx+2] = prevRowStartIdx + upTri;
                    ++upTri;
                }
                else
                {
                    indices[indIdx  ] = prevRowStartIdx + downTri + 1;
                    indices[indIdx+1] = rowStartIdx + downTri + 1;
                    indices[indIdx+2] = prevRowStartIdx + downTri;
                    ++downTri;
                }

                indIdx += 3;
                upright = !upright;
            }

            prevRowStartIdx = rowStartIdx;
            rowStartIdx += vertsInRowBottom;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.normals = normals;
        meshFilter.mesh = mesh;
    }

}
