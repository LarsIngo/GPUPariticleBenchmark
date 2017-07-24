using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    const int width = 2;
    const int height = 2;

    ComputeBuffer mPositionBuffer = null;

    Material mRenderMaterial = null;

	void Start ()
    {
        mPositionBuffer = new ComputeBuffer(width * height, sizeof(float) * 4);

        Shader renderShader = Resources.Load<Shader>("GPUParticleRenderShader");
        Debug.Assert(renderShader, "Failed loading render shader.");
        mRenderMaterial = new Material(renderShader);

        Vector4[] positionArray = new Vector4[width * height];
        for (int i = 0; i < width * height; ++i)
        {
            positionArray[i] = new Vector4(i % width, i / width, 1, 0);
        }
        mPositionBuffer.SetData(positionArray);

    }
	
	void Update ()
    {

    }

    void OnRenderObject()
    {
        mRenderMaterial.SetPass(0);

        mRenderMaterial.SetBuffer("gPosition", mPositionBuffer);

        Graphics.DrawProcedural(MeshTopology.Points, width * height);
    }

    void OnDestroy()
    {
        mPositionBuffer.Release();
    }

}
