using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUMemoryBlock
{

    /// <summary>
    /// Compute buffer contaning data.
    /// </summary>
    private ComputeBuffer mComputeBuffer = null;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="capacity">Capacity of memory block.</param>
    /// <param name="stride">Stride of memory block.</param>
    /// <param name="type">Type of memory block.</param>
    public GPUMemoryBlock(int capacity, int stride, ComputeBufferType type)
    {
        mComputeBuffer = new ComputeBuffer(capacity, stride, type);
    }

    /// <summary>
    /// Capacity(number of elements) of memory block.
    /// </summary>
    public int Capacity
    {
       get { return mComputeBuffer.count; }
    }

    /// <summary>
    /// Stride of memory block.
    /// </summary>
    public int Stride
    {
        get { return mComputeBuffer.stride; }
    }

    /// <summary>
    /// Release memory block.
    /// </summary>
    public void Release()
    {
        mComputeBuffer.Release();
    }

}
