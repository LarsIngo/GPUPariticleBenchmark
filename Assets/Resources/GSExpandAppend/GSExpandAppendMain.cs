using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSExpandAppendMain : MonoBehaviour
{
    const int width = 2; // TMP 8192
    const int height = 2; // TMP 4096
    const float spacing = 0.2f; // TMP 0.05f

    ComputeBuffer mPositionBuffer = null;
    ComputeBuffer mArgsBuffer = null;

    ComputeBuffer mColorBuffer = null;

    ComputeShader mComputeShader = null;

    Material mRenderMaterial = null;

	void Start ()
    {
        Camera.main.transform.position = new Vector3(width / 2.0f * spacing, height / 2.0f * spacing, -1); // TMP -150

        mPositionBuffer = new ComputeBuffer(width * height, sizeof(float) * 4, ComputeBufferType.Default);
        mArgsBuffer = new ComputeBuffer(1, sizeof(int) * 4, ComputeBufferType.IndirectArguments);

        mColorBuffer = new ComputeBuffer(width * height, sizeof(float) * 4, ComputeBufferType.Default);

        Shader renderShader = Resources.Load<Shader>("GSExpandAppend/GSExpandAppendRenderShader");
        Debug.Assert(renderShader, "Failed loading render shader.");
        mRenderMaterial = new Material(renderShader);

        mComputeShader = Resources.Load<ComputeShader>("GSExpandAppend/GSExpandAppendComputeShader");
        Debug.Assert(mComputeShader, "Failed loading compute shader.");

        mArgsBuffer.SetData(new int[] { width * height, 1, 0, 0 });
    }
	
	void Update ()
    {
        mComputeShader.SetBuffer(0, "gPosition", mPositionBuffer);

        mComputeShader.SetBuffer(0, "gColor", mColorBuffer);

        mComputeShader.SetInt("gCount", width * height);

        mComputeShader.SetFloat("gSpacing", spacing);
        mComputeShader.SetInt("gWidth", width);

        mComputeShader.Dispatch(0, (int)Mathf.Ceil(width * height / 64.0f), 1, 1);
    }

    void OnRenderObject()
    {
        mRenderMaterial.SetPass(0);

        mRenderMaterial.SetBuffer("gPosition", mPositionBuffer);

        mRenderMaterial.SetBuffer("gColor", mColorBuffer);

        Graphics.DrawProceduralIndirect(MeshTopology.Points, mArgsBuffer);
    }

    void OnDestroy()
    {
        mPositionBuffer.Release();
        mArgsBuffer.Release();
        mColorBuffer.Release();
    }

}
