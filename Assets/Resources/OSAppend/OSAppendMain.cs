using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSAppendMain : MonoBehaviour
{
    const int width = 1; // TMP 8192
    const int height = 1; // TMP 4096
    const float spacing = 0.05f;

    ComputeBuffer mPositionBuffer = null;
    ComputeBuffer mArgsBuffer = null;

    ComputeShader mComputeShader = null;
    ComputeBuffer mVertexBuffer = null;

    Material mRenderMaterial = null;

	void Start ()
    {
        Camera.main.transform.position = new Vector3(width / 2.0f * spacing, height / 2.0f * spacing, -1); // TMP -150

        mPositionBuffer = new ComputeBuffer(width * height, sizeof(float) * 4, ComputeBufferType.Default);
        mArgsBuffer = new ComputeBuffer(1, sizeof(int) * 4, ComputeBufferType.IndirectArguments);

        Shader renderShader = Resources.Load<Shader>("OSAppend/OSAppendRenderShader");
        Debug.Assert(renderShader, "Failed loading render shader.");
        mRenderMaterial = new Material(renderShader);

        mComputeShader = Resources.Load<ComputeShader>("OSAppend/OSAppendComputeShader");
        Debug.Assert(mComputeShader, "Failed loading compute shader.");

        mVertexBuffer = new ComputeBuffer(width * height * 3, sizeof(float) * 4, ComputeBufferType.Append);

        Vector4[] positionArray = new Vector4[width * height];
        for (int i = 0; i < width * height; ++i)
        {
            positionArray[i] = new Vector4((i % width) * spacing, (i / width) * spacing, 0, 0);
        }
        mPositionBuffer.SetData(positionArray);

        mArgsBuffer.SetData(new int[] { 1337, 1, 0, 0 });
    }
	
	void Update ()
    {
        mVertexBuffer.SetCounterValue(0);

        mComputeShader.SetBuffer(0, "gPosition", mPositionBuffer);
        mComputeShader.SetBuffer(0, "gVertexBuffer", mVertexBuffer);
        mComputeShader.SetInt("gCount", width * height);

        mComputeShader.Dispatch(0, (int)Mathf.Ceil(width * height / 64.0f), 1, 1);
    }

    void OnRenderObject()
    {
        ComputeBuffer.CopyCount(mVertexBuffer, mArgsBuffer, 0);

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
