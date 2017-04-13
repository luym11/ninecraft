using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameObject : MonoBehaviour
{
    private WorldData m_WorldData;
    private World m_World;
    private List<IDecoration> m_WorldDecorations = new List<IDecoration>();


    // our blueprint for terrain chunks, in the Unity Hierarchy
    public Transform Chunk_Prefab;

    // All possible block textures
    public Texture2D[] World_Textures;
    // Our texture atlas
    public Texture2D WorldTextureAtlas;
    public Transform Player;
    public Transform[,,] m_ChunkGameObjects;
    public Transform Sparks;

    private Transform m_ChunksParent;
    public GUIText WorldGeneration;

    public WorldData WorldData
    {
        get { return m_WorldData; }
    }

    private void Start()
    {
        m_WorldData = new WorldData();
        m_ChunksParent = transform.FindChild("Chunks");

        m_WorldDecorations = new List<IDecoration> {new StandardTreeDecorator(WorldData)};
        // Only the dual layer terrain w/ medium valleys and standard terrain medium caves
        // currently work, I haven't updated the others to return sunlit blocks.
        m_World = new World(WorldData,
                            new TerrainGenerator(WorldData, new BatchProcessor<Chunk>(),
                                                 new DualLayerTerrainWithMediumValleys()),
                            new LightProcessor(new BatchProcessor<Chunk>(), WorldData),
                            new MeshGenerator(new BatchProcessor<Chunk>(), WorldData),
                            new WorldDecorator(WorldData, new BatchProcessor<Chunk>(), m_WorldDecorations));

        m_World.InitializeGridChunks();

        InitializeTextures();
        Player.position = new Vector3(WorldData.WidthInBlocks / 2, 120, WorldData.HeightInBlocks / 2);
        CreateWorldChunkPrefabs();
        m_World.GenerateWorld();
    }

    private void CreateWorldChunkPrefabs()
    {
        m_ChunkGameObjects = new Transform[WorldData.ChunksWide,WorldData.ChunksHigh,WorldData.ChunksDeep];
        for (int x = 0; x < WorldData.ChunksWide; x++)
        {
            for (int y = 0; y < WorldData.ChunksHigh; y++)
            {
                for (int z = 0; z < WorldData.ChunksDeep; z++)
                {
                    Chunk chunk = WorldData.Chunks[x, y, z];
                    Vector3 chunkGameObjectPosition =
                        new Vector3(chunk.X * WorldData.ChunkBlockWidth + WorldData.GlobalXOffset,
                                    0,
                                    chunk.Y * WorldData.ChunkBlockHeight + WorldData.GlobalZOffset);
                    Transform chunkGameObject =
                        Instantiate(Chunk_Prefab, chunkGameObjectPosition, Quaternion.identity) as Transform;
                    chunkGameObject.parent = m_ChunksParent;
                    chunkGameObject.name = chunk.ToString();
                    m_ChunkGameObjects[x, y, z] = chunkGameObject;
                    ChunkGameObject chunkGameObjectScript = chunkGameObject.GetComponent<ChunkGameObject>();
                    chunkGameObjectScript.Texture = WorldTextureAtlas;
                }
            }
        }
    }

    private DateTime startTime;

    private void Update()
    {
        m_World.RegenerateChunks();
        if (DateTime.Now - startTime < TimeSpan.FromSeconds(5))
        {
            return;
        }
        CreateFinishedChunk();

        ProcessPlayerInput();
        DisplayDiggings();
    }


    private void InitializeTextures()
    {
        WorldTextureAtlas = new Texture2D(2048, 2048);
        WorldData.WorldTextureAtlasUvs = WorldTextureAtlas.PackTextures(World_Textures, 0);
        //foreach (Rect worldTextureAtlasUv in m_World.WorldTextureAtlasUvs)
        //{
        //    Debug.Log(worldTextureAtlasUv);
        //}
        WorldTextureAtlas.filterMode = FilterMode.Point;
        WorldTextureAtlas.anisoLevel = 9;
        WorldTextureAtlas.Apply();

        WorldData.GenerateUVCoordinatesForAllBlocks();
    }

    private void ProcessPlayerInput()
    {
        if (!Input.anyKey)
        {
            return;
        }



        m_ProcessAllChunksAtOnce = true;



        if (Input.inputString.Contains("t"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            if (Physics.Raycast(ray, out hit, 4.0f))
            {
                WorldData.SetBlockLightWithRegeneration((int) hit.point.x, (int) hit.point.z, (int) hit.point.y, 255);
                m_World.RegenerateChunks();
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //m_World.RemoveBlockAt(blockHitPoint);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
            if (Physics.Raycast(ray, out hit, 4.0f))
            {
                Vector3 hitPoint = hit.point + (ray.direction.normalized * 0.01f);
                IntVect blockHitPoint = new IntVect((int)hitPoint.x, (int)hitPoint.z, (int)hitPoint.y);
                m_World.Dig(blockHitPoint, hitPoint);
            }
            
        }


    }


    private bool m_ProcessAllChunksAtOnce;

    private void CreateFinishedChunk()
    {
        while (WorldData.FinishedChunks.Count > 0)
        {
            if (!m_ReadyToActivatePlayer)
            {
                m_ReadyToActivatePlayer = true;
                Player.gameObject.SetActiveRecursively(true);
            }
            Chunk chunk = WorldData.GetFinishedChunk();
            if (chunk == null)
            {
                return;
            }
            ChunkGameObject chunkGameObjectScript =
                m_ChunkGameObjects[chunk.X, chunk.Y, chunk.Z].GetComponent<ChunkGameObject>();

            chunkGameObjectScript.CreateFromChunk(chunk);
            if (!m_ProcessAllChunksAtOnce)
            {
                return;
            }
        }
    }


    private bool m_ReadyToActivatePlayer;


    /// <summary>
    /// When we quit the app, be sure to shut down the threads and
    /// destory our chunks
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("Exiting!");
        for (int x = 0; x < WorldData.ChunksWide; x++)
        {
            for (int y = 0; y < WorldData.ChunksHigh; y++)
            {
                World.DestroyChunk(WorldData.Chunks[x, y, 0]);
            }
        }

        m_World = null;
    }

    private void DisplayDiggings()
    {
        if (m_World.Diggings.Count == 0)
        {
            return;
        }
        Vector3 diggingsLocation = m_World.Diggings.Dequeue();
        Debug.Log("Diggings at " + diggingsLocation);
        Instantiate(Sparks, diggingsLocation, Quaternion.identity);


    }
}