using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUMemoryManagerMainScript : MonoBehaviour {

	void Start ()
    {
        GPUMemoryManager.Instance.StartUp();

        GPUMemoryBlock positionBlock = GPUMemoryManager.Instance.CreateGPUMemoryBlock<float>("PositionBlock", 16, ComputeBufferType.Default);


        Debug.Log("EndIndex: " + positionBlock.EndIndex);

        GPUMemoryBlock.Handle emitter1 = positionBlock.Allocate(2);
        emitter1.SetData(new float[] { 1, 2 });
        Debug.Log("EndIndex: " + positionBlock.EndIndex);
        {
            float[] dataArray = emitter1.GetData<float>();
            for (int i = 0; i < dataArray.GetLength(0); ++i)
                Debug.Log(dataArray[i]);
        }

        GPUMemoryBlock.Handle emitter2 = positionBlock.Allocate(2);
        emitter2.SetData(new float[] { 3, 4 });
        Debug.Log("EndIndex: " + positionBlock.EndIndex);
        {
            float[] dataArray = emitter2.GetData<float>();
            for (int i = 0; i < dataArray.GetLength(0); ++i)
                Debug.Log(dataArray[i]);
        }


        positionBlock.Free(emitter1);
        Debug.Log("EndIndex: " + positionBlock.EndIndex);
        positionBlock.Defragment();
        Debug.Log("EndIndex: " + positionBlock.EndIndex); ;
        {
            float[] dataArray = emitter2.GetData<float>();
            for (int i = 0; i < dataArray.GetLength(0); ++i)
                Debug.Log(dataArray[i]);
        }
    }
	
	void Update ()
    {
		
	}

    void OnDestroy()
    {
        GPUMemoryManager.Instance.Shutdown();
    }
}
