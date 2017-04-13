using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    private Block[,,] m_Blocks;

    // Chunk coordinates, in chunks not blocks.
    // It's x,y location in the grid.
    private int m_X;
    private int m_Y;
    private int m_Z;

    private List<int> m_Indices = new List<int>();
    private List<Vector2> m_Uvs = new List<Vector2>();
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Color> m_Colors = new List<Color>();

    private Transform m_ChunkPrefab;
    /// <summary>
    /// Some decorations only consider topsoil. Let's cache these, for quicker evaluation.
    /// </summary>
    public readonly List<IntVect> TopSoilBlocks = new List<IntVect>();


    public Chunk(int x, int y, int z)
    {
        m_X = x;
        m_Y = y;
        m_Z = z;
    }

    public void InitializeBlocks(int chunkBlockWidth, int chunkBlockHeight, int chunkBlockDepth)
    {
        m_Blocks = new Block[chunkBlockWidth, chunkBlockHeight, chunkBlockDepth];
    }

    static Chunk()
    {
        WorldChunkYOffset = 0;
    }

    public int X
    {
        get { return m_X; }
        set { m_X = value; }
    }

    public int Y
    {
        get { return m_Y; }
        set { m_Y = value; }
    }

    public int Z
    {
        get { return m_Z; }
    }

    public Block[,,] Blocks
    {
        get { return m_Blocks; }
        set { m_Blocks = value; }
    }

    public List<int> Indices
    {
        get { return m_Indices; }
        set { m_Indices = value; }
    }

    public List<Vector2> Uvs
    {
        get { return m_Uvs; }
        set { m_Uvs = value; }
    }

    public List<Vector3> Vertices
    {
        get { return m_Vertices; }
        set { m_Vertices = value; }
    }

    public List<Color> Colors
    {
        get { return m_Colors; }
        set { m_Colors = value; }
    }


    public static int WorldChunkYOffset { get; set; }

    public Transform ChunkPrefab
    {
        get { return m_ChunkPrefab; }
        set { m_ChunkPrefab = value; }
    }

    public override string ToString()
    {
        return String.Format("Chunk_{0},{1},{2}", m_X, m_Y, m_Z);
    }

    public Chunk ReplacementChunk { get; set; }

    public bool NeedsRegeneration { get; set; }


}