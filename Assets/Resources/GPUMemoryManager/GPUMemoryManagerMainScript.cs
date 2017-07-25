using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUMemoryManagerMainScript : MonoBehaviour {

	void Start ()
    {
        GPUMemoryManager.Instance.StartUp();

        GPUMemoryBlock positionBlock = GPUMemoryManager.Instance.CreateGPUMemoryBlock("PositionBlock", 16, sizeof(float) * 1, ComputeBufferType.Default); // TODO Template!

        Debug.Log(positionBlock.EndIndex);
        GPUMemoryBlock.Handle emitter1 = positionBlock.Allocate(2);
        Debug.Log(positionBlock.EndIndex);
        GPUMemoryBlock.Handle emitter2 = positionBlock.Allocate(2);
        Debug.Log(positionBlock.EndIndex);
        positionBlock.Free(emitter1);
        Debug.Log(positionBlock.EndIndex);
        positionBlock.Defragment();
        Debug.Log(positionBlock.EndIndex);
    }
	
	void Update ()
    {
		
	}

    void OnDestroy()
    {
        GPUMemoryManager.Instance.Shutdown();
    }
}
