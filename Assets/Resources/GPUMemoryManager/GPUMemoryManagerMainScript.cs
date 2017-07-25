using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUMemoryManagerMainScript : MonoBehaviour {

	void Start ()
    {
        GPUMemoryManager.Instance.StartUp();

        GPUMemoryBlock block = GPUMemoryManager.Instance.CreateGPUMemoryBlock("positions", 1024, sizeof(float) * 4, ComputeBufferType.Default);

    }
	
	void Update ()
    {
		
	}

    void OnDestroy()
    {
        GPUMemoryManager.Instance.Shutdown();
    }
}
