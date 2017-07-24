using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSStuctrueMain : MonoBehaviour
{
    const int width = 1024; // TMP 8192
    const int height = 4096; // TMP 4096
    const float spacing = 0.05f;

    ComputeBuffer mPositionBuffer = null;
    ComputeBuffer mArgsBuffer = null;

    ComputeShader mComputeShader = null;
    ComputeBuffer mVertexBuffer = null;

    Material mRenderMaterial = null;

	void Start ()
    {
        Camera.main.transform.position = new Vector3(width / 2.0f * spacing, height / 2.0f * spacing, -50); // TMP -150

        mPositionBuffer = new ComputeBuffer(width * height, sizeof(float) * 4, ComputeBufferType.Default);
        mArgsBuffer = new ComputeBuffer(1, sizeof(int) * 4, ComputeBufferType.IndirectArguments);

        Shader renderShader = Resources.Load<Shader>("OSStuctrue/OSStuctrueRenderShader");
        Debug.Assert(renderShader, "Failed loading render shader.");
        mRenderMaterial = new Material(renderShader);

        mComputeShader = Resources.Load<ComputeShader>("OSStuctrue/OSStuctrueComputeShader");
        Debug.Assert(mComputeShader, "Failed loading compute shader.");

        mVertexBuffer = new ComputeBuffer(width * height * 6, sizeof(float) * 4);

        Vector4[] positionArray = new Vector4[width * height];
        for (int i = 0; i < width * height; ++i)
        {
            positionArray[i] = new Vector4((i % width) * spacing, (i / width) * spacing, 0, 0);
        }
        mPositionBuffer.SetData(positionArray);

        mArgsBuffer.SetData(new int[] { 6, width * height, 0, 0 });
    }
	
	void Update ()
    {
        mComputeShader.SetBuffer(0, "gPosition", mPositionBuffer);
        mComputeShader.SetBuffer(0, "gVertexBuffer", mVertexBuffer);
        mComputeShader.SetInt("gCount", width * height);

        mComputeShader.Dispatch(0, (int)Mathf.Ceil(width * height / 64.0f), 1, 1);
    }

    void OnRenderObject()
    {
        mRenderMaterial.SetPass(0);

        mRenderMaterial.SetBuffer("gVertexBuffer", mVertexBuffer);

        Graphics.DrawProceduralIndirect(MeshTopology.Triangles, mArgsBuffer);
    }

    void OnDestroy()
    {
        mPositionBuffer.Release();
        mArgsBuffer.Release();

        mVertexBuffer.Release();
    }

}
