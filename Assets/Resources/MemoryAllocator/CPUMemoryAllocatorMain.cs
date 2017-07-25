using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUMemoryAllocatorMain : MonoBehaviour
{

    struct Chunk
    {
        public int mStartIndex;
        public int mSize;

        public Chunk(int startIndex, int size)
        {
            mStartIndex = startIndex;
            mSize = size;
        }
    }

    const int mMaxSize = 16;
    int[] mMemory = new int[mMaxSize];
    int mEndIndex = 0;

    SortedList<int, Chunk> mAllocatedList = new SortedList<int, Chunk>();
    SortedList<int, Chunk> mFragmentedFreeList = new SortedList<int, Chunk>();

    void Init()
    {
        for (int i = 0; i < mMaxSize; ++i)
            mMemory[i] = -1;
    }

    void Print()
    {
        for (int i = 0; i < mEndIndex; ++i)
            Debug.Log(i + ": " + mMemory[i]);
    }

    void PrintAll()
    {
        for (int i = 0; i < mMaxSize; ++i)
            Debug.Log(i + ": " + mMemory[i]);
    }

    // Returns start index.
    Chunk Allocate(int size)
    {
        // Store start index.
        int startIndex = mEndIndex;

        // Allocate chunk.
        Debug.Assert(startIndex + size <= mMaxSize, "Size to big, out of memory.");
        Chunk chunk = new Chunk(startIndex, size);
        mAllocatedList.Add(startIndex, chunk);

        // Insert default values.
        for (int i = startIndex; i < startIndex + size; ++i)
            mMemory[i] = mAllocatedList.Count-1;

        // Increment end index.
        mEndIndex += size;

        return chunk;
    }

    void Free(Chunk chunk)
    {
        Debug.Assert(mAllocatedList.ContainsValue(chunk), "Trying to remove chunk not allocated.");

        if (chunk.mStartIndex + chunk.mSize == mEndIndex)
        {   // Removing last chunk, move end index back.
            mEndIndex = chunk.mStartIndex;

            // Insert "null" values to flag unallocated data.
            for (int i = chunk.mStartIndex; i < chunk.mStartIndex + chunk.mSize; ++i)
                mMemory[i] = -1;
        }
        else
        {   // Not removing last chunk, add to fragmented list.
            mFragmentedFreeList.Add(chunk.mStartIndex, chunk);

            // Insert "null" values to flag unallocated data.
            for (int i = chunk.mStartIndex; i < chunk.mStartIndex + chunk.mSize; ++i)
                mMemory[i] = -1;
        }

        // Remove chunk from allocated list.
        mAllocatedList.Remove(chunk.mStartIndex);
    }

    void Defragment(uint steps = uint.MaxValue)
    {
        // Return early if dividing defragmentation over several frames.
        if (steps == 0) return;

        // If no fragmented chunks, memory is defragmented.
        if (mFragmentedFreeList.Count == 0) return;
        
        Chunk fragmentedChunk = mFragmentedFreeList.Values[0];
        if (fragmentedChunk.mStartIndex + fragmentedChunk.mSize == mEndIndex)
        {
            // Assert this is the last fragmeneted chunk. Assert should never occur.
            Debug.Assert(mFragmentedFreeList.Count == 1, "Not last fragemented chunk!");

            // Move end index and clear fragmented list.
            mEndIndex = fragmentedChunk.mStartIndex;
            mFragmentedFreeList.Clear();

            // Memory is now defragmented.
            return;
        }

        int nextChunkStartIndex = fragmentedChunk.mStartIndex + fragmentedChunk.mSize;
        if (mAllocatedList.ContainsKey(nextChunkStartIndex))
        {
            // Next chunk is allocated and should swap place with fragmented chunk.
            Chunk allocatedChunk = mAllocatedList[nextChunkStartIndex];

            // Move allocated chunk memory.
            for (int i = 0; i < allocatedChunk.mSize; ++i)
            {
                mMemory[fragmentedChunk.mStartIndex + i] = mMemory[allocatedChunk.mStartIndex + i];
            }

            // Update chunk start index.
            allocatedChunk.mStartIndex = fragmentedChunk.mStartIndex;
            fragmentedChunk.mStartIndex = allocatedChunk.mStartIndex + allocatedChunk.mSize;

            mFragmentedFreeList.RemoveAt(0);
            mFragmentedFreeList.Add(fragmentedChunk.mStartIndex, fragmentedChunk);
        }
        else if (mFragmentedFreeList.ContainsKey(nextChunkStartIndex))
        {
            // Next chunk is free and should be combined with fragmented chunk.
            Chunk nextChunk = mFragmentedFreeList[nextChunkStartIndex];

            // Combine chunks.
            fragmentedChunk.mSize += nextChunk.mSize;

            // Remove next chunk.
            mFragmentedFreeList.Remove(nextChunkStartIndex);
        }
        else
        {
            Debug.Assert(false, "Something went wrong!");
        }

        // Continue defragmentation.
        Defragment(steps - 1);
    }

	void Start ()
    {
        Init();

        Chunk a = Allocate(2);
        Chunk b = Allocate(2);
        Chunk c = Allocate(2);

        Free(b);
    }
	
	void Update ()
    {
        Debug.Log("---");

        Defragment(1);

        Print();
    }
}
