using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class WorldData
{
    private int m_ChunksWide = 16;
    private int m_ChunksHigh = 16;
    private int m_ChunksDeep = 1;
    private int m_ChunkBlockWidth = 32;
    private int m_ChunkBlockHeight = 32;
    private int m_ChunkBlockDepth = 128;
    private int m_NumberOfLightShades = 10;
    private float m_NoiseBlockXOffset = UnityEngine.Random.Range(0,10000);
    private readonly Dictionary<int, BlockUVCoordinates> m_BlockUVCoordinates = new Dictionary<int, BlockUVCoordinates>();

    public WorldData()
    {
        SetShadesOfLight(m_NumberOfLightShades);
    }

    public void GenerateUVCoordinatesForAllBlocks()
    {
        // Topsoil
        SetBlockUVCoordinates(BlockType.TopSoil, 0, 5, 1);
        SetBlockUVCoordinates(BlockType.Dirt, 1,1,1);
        SetBlockUVCoordinates(BlockType.Leaves, 4,4,4);
        SetBlockUVCoordinates(BlockType.Lava, 3,3,3);
        SetBlockUVCoordinates(BlockType.Stone, 6,6,6);
    }

    private void SetBlockUVCoordinates(BlockType blockType, int topIndex, int sideIndex, int bottomIndex)
    {
        BlockUvCoordinates.Add((int)(blockType), new BlockUVCoordinates(WorldTextureAtlasUvs[topIndex], WorldTextureAtlasUvs[sideIndex], WorldTextureAtlasUvs[bottomIndex]));
    }

    public int ChunksWide
    {
        get { return m_ChunksWide; }
        set { m_ChunksWide = value; }
    }

    public int ChunksHigh
    {
        get { return m_ChunksHigh; }
        set { m_ChunksHigh = value; }
    }

    public int ChunksDeep
    {
        get { return m_ChunksDeep; }
        set { m_ChunksDeep = value; }
    }

    public int ChunkBlockWidth
    {
        get { return m_ChunkBlockWidth; }
        set { m_ChunkBlockWidth = value; }
    }

    public int ChunkBlockHeight
    {
        get { return m_ChunkBlockHeight; }
        set { m_ChunkBlockHeight = value; }
    }

    public int ChunkBlockDepth
    {
        get { return m_ChunkBlockDepth; }
        set { m_ChunkBlockDepth = value; }
    }

    public int NumberOfLightShades
    {
        get { return m_NumberOfLightShades; }
        set { m_NumberOfLightShades = value; }
    }

    public void SetShadesOfLight(int numberOfShadesOfLight)
    {
        byte[] shadesOfLight = new byte[numberOfShadesOfLight];
        m_NextLowerShadesOfLight = new byte[numberOfShadesOfLight];
        byte lightReduction = (byte)(255 / numberOfShadesOfLight);
        byte shadeOfLight = 255;
        for (int i = 0; i < numberOfShadesOfLight; i++)
        {
            shadesOfLight[i] = shadeOfLight;
            if (i > 0)
            {
                m_NextLowerShadesOfLight[i - 1] = shadeOfLight;
            }
            shadeOfLight -= lightReduction;
        }
        m_NumberOfLightShades = numberOfShadesOfLight;

        m_ShadesOfLight = shadesOfLight;
    }


    public byte[] NextLowerShadesOfLight
    {
        get { return m_NextLowerShadesOfLight; }
    }

    private byte[] m_ShadesOfLight;
    private byte[] m_NextLowerShadesOfLight;


    public byte[] ShadesOfLight
    {
        get { return m_ShadesOfLight; }
    }

    public int WidthInBlocks
    {
        get { return m_ChunksWide * m_ChunkBlockWidth; }
    }

    public int HeightInBlocks
    {
        get { return m_ChunksHigh * m_ChunkBlockHeight; }
    }

    public int DepthInBlocks
    {
        get { return m_ChunksDeep * m_ChunkBlockDepth; }
    }

    public const int BottomChunkBorderRow = 0;

    public const int LeftChunkBorderColumn = 0;

    public const int BottomVisibleChunkRow = 1;
    public const int LeftVisibleChunkColumn = 1;


    public int TopChunkBorderRow
    {
        get { return m_ChunksHigh - 1; }
    }

    public int RightChunkBorderColumn
    {
        get { return m_ChunksWide - 1; }
    }

    public int TopVisibleChunkRow
    {
        get { return m_ChunksHigh - 2; }
    }

    public int RightVisibleChunkColumn
    {
        get { return ChunksWide - 2; }
    }

    public int TotalChunks
    {
        get { return m_ChunksWide * m_ChunksHigh; }
    }

    public int CenterChunkX
    {
        get { return m_ChunksWide / 2; }
    }

    public int CenterChunkY
    {
        get { return m_ChunksHigh / 2; }
    }

    public List<Chunk> AllChunks
    {
        get
        {
            List<Chunk> chunks = new List<Chunk>();
            for (int x = LeftChunkBorderColumn; x <= RightChunkBorderColumn; x++)
            {
                for (int y = BottomChunkBorderRow; y <= TopChunkBorderRow; y++)
                {
                    chunks.Add(Chunks[x, y, 0]);
                }
            }
            return chunks;
        }
    }

    /// <summary>
    /// Returns all chunks, excluding the border chunks
    /// </summary>
    public List<Chunk> AllVisibleChunks
    {
        get
        {
            List<Chunk> chunks = new List<Chunk>();
            for (int x = LeftVisibleChunkColumn; x <= RightVisibleChunkColumn; x++)
            {
                for (int y = BottomVisibleChunkRow; y <= TopVisibleChunkRow; y++)
                {
                    chunks.Add(Chunks[x, y, 0]);
                }
            }
            return chunks;
        }
    }


    public void SetDimensions(int chunksWide, int chunksHigh, int chunksDeep, int chunkBlockWidth, int chunkBlockHeight,
                              int chunkBlockDepth)
    {
        ChunksWide = chunksWide;
        ChunksHigh = chunksHigh;
        ChunksDeep = chunksDeep;
        ChunkBlockWidth = chunkBlockWidth;
        ChunkBlockHeight = chunkBlockHeight;
        ChunkBlockDepth = chunkBlockDepth;
    }

    // Our horizontal movement offset for noise sampling

    private float m_GlobalXOffset;
    private float m_GlobalZOffset;
    public Chunk[, ,] Chunks;

    public void InitializeGridChunks()
    {
        Chunks = new Chunk[m_ChunksWide, m_ChunksHigh, m_ChunksDeep];

        // Add all world chunks to the batch for processing
        for (int x = LeftChunkBorderColumn; x <= RightChunkBorderColumn; x++)
        {
            for (int y = BottomChunkBorderRow; y <= TopChunkBorderRow; y++)
            {
                for (int z = 0; z < m_ChunksDeep; z++)
                {
                    Chunks[x, y, z] = new Chunk(x, y, z);
                    Chunks[x, y, z].InitializeBlocks(ChunkBlockWidth, ChunkBlockHeight, ChunkBlockDepth);
                }
            }
        }
    }

    public float GlobalXOffset
    {
        get { return m_GlobalXOffset; }
        set { m_GlobalXOffset = value; }
    }

    public float GlobalZOffset
    {
        get { return m_GlobalZOffset; }
        set { m_GlobalZOffset = value; }
    }

    public float NoiseBlockXOffset
    {
        get { return m_NoiseBlockXOffset; }
        set { m_NoiseBlockXOffset = value; }
    }

    public Block GetBlock(int x, int y, int z)
    {
        int chunkX = x / ChunkBlockWidth;
        int chunkY = y / ChunkBlockHeight;
        int chunkZ = z / ChunkBlockDepth;
        int blockX = x % ChunkBlockWidth;
        int blockY = y % ChunkBlockHeight;
        int blockZ = z % ChunkBlockDepth;

        return Chunks[chunkX, chunkY, chunkZ].Blocks[blockX, blockY, blockZ];
    }

    public void SetBlockTypeWithRegeneration(int x, int y, int z, BlockType blockType)
    {
        int chunkX = x / ChunkBlockWidth;
        int chunkY = y / ChunkBlockHeight;
        int chunkZ = z / ChunkBlockDepth;
        int blockX = x % ChunkBlockWidth;
        int blockY = y % ChunkBlockHeight;
        int blockZ = z % ChunkBlockDepth;

        Chunks[chunkX, chunkY, 0].Blocks[blockX, blockY, blockZ].Type = blockType;
        Chunks[chunkX, chunkY, chunkZ].NeedsRegeneration = true;
        if (blockX == ChunkBlockWidth - 1)
        {
            Chunks[chunkX + 1, chunkY, chunkZ].NeedsRegeneration = true;
        }
        else if (blockX == 0)
        {
            Chunks[chunkX - 1, chunkY, chunkZ].NeedsRegeneration = true;
        }

        if (blockY == 0)
        {
            Chunks[chunkX, chunkY - 1, chunkZ].NeedsRegeneration = true;
        }
        else if (blockY == ChunkBlockHeight - 1)
        {
            Chunks[chunkX, chunkY + 1, chunkZ].NeedsRegeneration = true;
        }
    }

    public void SetBlockType(int x, int y, int z, BlockType blockType)
    {
        int chunkX = x / ChunkBlockWidth;
        int chunkY = y / ChunkBlockHeight;
        int chunkZ = z / ChunkBlockDepth;
        int blockX = x % ChunkBlockWidth;
        int blockY = y % ChunkBlockHeight;
        int blockZ = z % ChunkBlockDepth;

        Chunks[chunkX, chunkY, 0].Blocks[blockX, blockY, blockZ].Type = blockType;
    }

    public void SetBlockLightWithRegeneration(int x, int y, int z, byte lightAmount)
    {
        int chunkX = x / ChunkBlockWidth;
        int chunkY = y / ChunkBlockHeight;
        int chunkZ = z / ChunkBlockDepth;
        int blockX = x % ChunkBlockWidth;
        int blockY = y % ChunkBlockHeight;
        int blockZ = z % ChunkBlockDepth;

        Chunks[chunkX, chunkY, chunkZ].Blocks[blockX, blockY, blockZ].Type = BlockType.Air;
        Chunks[chunkX, chunkY, chunkZ].Blocks[blockX, blockY, blockZ].LightAmount = lightAmount;
        Chunks[chunkX, chunkY, chunkZ].NeedsRegeneration = true;
    }

    public byte GetBlockLight(int x, int y, int z)
    {
        int chunkX = x / ChunkBlockWidth;
        int chunkY = y / ChunkBlockHeight;
        int chunkZ = z / ChunkBlockDepth;
        int blockX = x % ChunkBlockWidth;
        int blockY = y % ChunkBlockHeight;
        int blockZ = z % ChunkBlockDepth;

        return Chunks[chunkX, chunkY, 0].Blocks[blockX, blockY, blockZ].LightAmount;
    }

    public List<Chunk> ChunksNeedingRegeneration
    {
        get
        {
            List<Chunk> chunks = new List<Chunk>();
            // Add all world chunks to the batch for processing
            for (int x = LeftChunkBorderColumn; x <= RightChunkBorderColumn; x++)
            {
                for (int y = BottomChunkBorderRow; y <= TopChunkBorderRow; y++)
                {
                    for (int z = 0; z < m_ChunksDeep; z++)
                    {
                        if (Chunks[x, y, z].NeedsRegeneration)
                        {
                            chunks.Add(Chunks[x, y, z]);
                        }
                    }
                }
            }
            return chunks;
        }
    }

    public void AddFinishedChunk(Chunk chunk)
    {
        FinishedChunks.Enqueue(chunk);
    }

    public TQueue<Chunk> FinishedChunks = new TQueue<Chunk>();




    public Chunk GetFinishedChunk()
    {
        if (FinishedChunks.Count == 0)
        {
            return null;
        }

        return FinishedChunks.Dequeue();
    }

    public Rect[] WorldTextureAtlasUvs { get; set; }

    public Dictionary<int, BlockUVCoordinates> BlockUvCoordinates
    {
        get { return m_BlockUVCoordinates; }
    }
}

