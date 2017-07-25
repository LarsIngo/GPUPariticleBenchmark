using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUMemoryBlock
{

    #region CLASSES

    /// <summary>
    /// Handle to manage memory access to block.
    /// Handle is an interface for the developer to access memory partition.
    /// <para />
    /// ONLY ACCESS MEMORY BETWEEN [OFFSET] AND [OFFSET+COUNT].
    /// </summary>
    public class Handle
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="block">Memory block this handle maps to.</param>
        public Handle(GPUMemoryBlock block)
        {
            mBlock = block;
        }

        /// <summary>
        /// The memory block this handle maps to.
        /// </summary>
        private GPUMemoryBlock mBlock;

        /// <summary>
        /// Offset to the first element(Start index) in memory block.
        /// DO NOT ACCESS MEMORY OUTSIDE HANDLE.
        /// </summary>
        public int Offset
        {
            get
            {
                Debug.Assert(mBlock.mPartitionDictionary.ContainsKey(this), "Error: Handle maps to wrong block.");
                return mBlock.mPartitionDictionary[this].mOffset;
            }
        }

        /// <summary>
        /// Number of elemets in memory block this handle maps to.
        /// DO NOT ACCESS MEMORY OUTSIDE HANDLE.
        /// </summary>
        public int Count
        {
            get
            {
                Debug.Assert(mBlock.mPartitionDictionary.ContainsKey(this), "Error: Handle maps to wrong block.");
                return mBlock.mPartitionDictionary[this].mCount;
            }
        }
    }

    /// <summary>
    /// Private class to track data partitions.
    /// </summary>
    private class Partition
    {
        /// <summary>
        /// Offset from first element(Start index) in memory block.
        /// </summary>
        public int mOffset;

        /// <summary>
        /// Number of elemets in memory block.
        /// </summary>
        public int mCount;

        public Partition(int offset, int count)
        {
            mOffset = offset;
            mCount = count;
        }
    }

    #endregion

    /// <summary>
    /// End index in memory block. Allocates new partitions at this index.
    /// Keeps track of allocated/free memory.
    /// </summary>
    private int mEndIndex = 0;

    /// <summary>
    /// Sorted list keeping tack of allocated partitions.
    /// </summary>
    private SortedList<int, Partition> mAllocatedPartitionList = new SortedList<int, Partition>();

    /// <summary>
    /// Sorted list keeping tack of fragmented partitions.
    /// Fragmented partitions exists between allocated data.
    /// Defragment memory block to remove fragmented partitions.
    /// </summary>
    private SortedList<int, Partition> mFragmentedPartitionList = new SortedList<int, Partition>();

    /// <summary>
    /// Dictionary used to map Handle to Partition.
    /// </summary>
    private Dictionary<Handle, Partition> mPartitionDictionary = new Dictionary<Handle, Partition>();

    /// <summary>
    /// Compute buffer contaning data.
    /// </summary>
    private ComputeBuffer mComputeBuffer = null;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="capacity">Capacity(number of elemets) of memory block.</param>
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

    /// <summary>
    /// Allocate memory.
    /// Returns Handle used to access the memory.
    /// </summary>
    /// <param name="count">Number of elements to allocate.</param>
    public Handle Allocate(int count)
    {
        // Store start index.
        int offset = mEndIndex;

        // Allocate partition.
        Debug.Assert(offset + count <= Capacity, "Error: Out of memory! No allocation can be done.");
        Partition partition = new Partition(offset, count);
        mAllocatedPartitionList.Add(partition.mOffset, partition);

        //// Insert default values. // TODO
        //for (int i = startIndex; i < startIndex + size; ++i)
        //    mMemory[i] = -1;

        // Increment end index.
        mEndIndex += count;

        // Create Handle.
        Handle handle = new Handle(this);
        mPartitionDictionary[handle] = partition;
        return handle;
    }

}
