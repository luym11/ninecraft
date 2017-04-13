using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IWorld
{
}

public class World : IWorld
{
    private readonly ITerrainGenerator m_TerrainGenerator;
    private readonly WorldData m_WorldData;
    private readonly ILightProcessor m_LightProcessor;
    private readonly IMeshGenerator m_MeshGenerator;
    private readonly IWorldDecorator m_WorldDecorator;


    public World(WorldData worldData, ITerrainGenerator terrainGenerator, ILightProcessor lightProcessor,
                 IMeshGenerator meshGenerator, IWorldDecorator worldDecorator)
    {
        m_WorldData = worldData;
        m_LightProcessor = lightProcessor;
        m_MeshGenerator = meshGenerator;
        m_WorldDecorator = worldDecorator;
        m_TerrainGenerator = terrainGenerator;
    }

    public void InitializeGridChunks()
    {
        m_WorldData.InitializeGridChunks();
    }

    public void GenerateWorld()
    {
        List<Chunk> allVisibleChunks = m_WorldData.AllVisibleChunks;
        List<Chunk> allChunks = m_WorldData.AllChunks;
        DateTime start = DateTime.Now;

        GenerateTerrain(allChunks);

        Debug.Log(DateTime.Now - start);

        GenerateWorldDecorations(allVisibleChunks);
        Debug.Log(DateTime.Now - start);

        GenerateLighting(allVisibleChunks);
        Debug.Log(DateTime.Now - start);
        allVisibleChunks.Sort(ChunksComparedByDistanceFromMapCenter);
        m_MeshGenerator.GenerateMeshes(allVisibleChunks);
        ClearRegenerationStatus(allChunks);
    }

    private void GenerateWorldDecorations(List<Chunk> chunks)
    {
        m_WorldDecorator.GenerateWorldDecorations(chunks);
    }


    private static void ClearRegenerationStatus(IEnumerable<Chunk> chunks)
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.NeedsRegeneration = false;
        }
    }

    private void GenerateLighting(List<Chunk> chunks)
    {
        m_LightProcessor.LightChunks(chunks);
    }

    private void GenerateTerrain(List<Chunk> chunks)
    {
        m_TerrainGenerator.GenerateChunkTerrain(chunks);
    }


    // this is the actual comparison method that compares them by distance
    private int ChunksComparedByDistanceFromMapCenter(Chunk firstChunk,
                                                      Chunk secondChunk)
    {
        Vector3 mapCenter = new Vector3(m_WorldData.CenterChunkX, m_WorldData.CenterChunkY, 0);
        return Vector3.Distance(
            new Vector3(firstChunk.X, firstChunk.Y, firstChunk.Z), mapCenter).
            CompareTo(
                (int) Vector3.Distance(new Vector3(secondChunk.X, secondChunk.Y, secondChunk.Z), mapCenter));
    }


    // Regenerates the target chunk first, followed by any others that need regeneration.
    public void RegenerateChunks(int chunkX, int chunkY, int chunkZ)
    {
        List<Chunk> chunksNeedingRegeneration = m_WorldData.ChunksNeedingRegeneration;
        if (chunksNeedingRegeneration.Count == 0)
        {
            return;
        }

        Chunk targetChunk = m_WorldData.Chunks[chunkX, chunkY, chunkZ];
        if (chunksNeedingRegeneration.Contains(targetChunk))
        {
            chunksNeedingRegeneration.Remove(targetChunk);
            chunksNeedingRegeneration.Insert(0, targetChunk);
        }

        RegenerateChunks(chunksNeedingRegeneration);
    }

    public void RegenerateChunks()
    {
        List<Chunk> chunksNeedingRegeneration = m_WorldData.ChunksNeedingRegeneration;
        if (chunksNeedingRegeneration.Count == 0)
        {
            return;
        }

        RegenerateChunks(chunksNeedingRegeneration);
    }

    private void RegenerateChunks(List<Chunk> chunksNeedingRegeneration)
    {
        chunksNeedingRegeneration.ForEach(chunk => Debug.Log("Regenerating " + chunk));
        m_LightProcessor.LightChunks(chunksNeedingRegeneration);
        m_MeshGenerator.GenerateMeshes(chunksNeedingRegeneration);
        ClearRegenerationStatus(chunksNeedingRegeneration);
    }

    public void RemoveBlockAt(IntVect hitPoint)
    {
        m_WorldData.SetBlockTypeWithRegeneration(hitPoint.X, hitPoint.Y, hitPoint.Z, BlockType.Air);
        m_LightProcessor.RecalculateLightingAround(hitPoint.X, hitPoint.Y, hitPoint.Z);
        RegenerateChunks(hitPoint.X / m_WorldData.ChunkBlockHeight,
                         hitPoint.Y / m_WorldData.ChunkBlockHeight,
                         hitPoint.Z / m_WorldData.ChunkBlockDepth);
    }

    public void FireNukeAt(IntVect hitPoint, Ray ray)
    {
        Debug.Log(hitPoint + " - " + ray);
        float xInc = hitPoint.X;
        float yInc = hitPoint.Y;
        float zInc = hitPoint.Z;
        for (int distance = 0; distance <= 10; distance++)
        {
            xInc += ray.direction.x;
            yInc += ray.direction.y;
            zInc += ray.direction.z;
            for (int numBlocks = 0; numBlocks < 10; numBlocks++)
            {
                int blockX = (int) (UnityEngine.Random.insideUnitSphere.x * 3 + xInc);
                int blockY = (int) (UnityEngine.Random.insideUnitSphere.y * 3 + yInc);
                int blockZ = (int) (UnityEngine.Random.insideUnitSphere.z * 3 + zInc);
                m_WorldData.SetBlockTypeWithRegeneration(blockX, blockY, blockZ, BlockType.Air);
            }
        }
    }

    private int m_DiggingAmount = 100;
    private IntVect m_DiggingLocation;
    private DateTime m_LastDigTime;
    private readonly TimeSpan m_DigDuration = TimeSpan.FromSeconds(0.25);
    public readonly Queue<Vector3> Diggings = new Queue<Vector3>();

    public void Dig(IntVect targetLocation, Vector3 hitPoint)
    {
        DateTime currentDigTime = DateTime.Now;
        if (targetLocation != m_DiggingLocation)
        {
            m_DiggingAmount = 100;
            m_DiggingLocation = targetLocation;
            m_LastDigTime = currentDigTime;
            Diggings.Enqueue(hitPoint);
        }
        else
        {
            if (currentDigTime - m_LastDigTime > m_DigDuration)
            {
                Diggings.Enqueue(hitPoint);
                m_DiggingAmount = m_DiggingAmount - 25;
                m_LastDigTime = currentDigTime;
                
                if (m_DiggingAmount <=0)
                {
                    RemoveBlockAt(targetLocation);
                    m_DiggingAmount = 100;
                }
            }
        }
    }

    public static void DestroyChunk(Chunk chunk)
    {
        if (chunk.ChunkPrefab != null)
        {
            MeshFilter meshFilter = chunk.ChunkPrefab.GetComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            Object.Destroy(meshFilter.mesh);
            meshFilter.mesh = null;
            Object.Destroy(meshFilter);
            Object.Destroy(chunk.ChunkPrefab.gameObject);
            chunk.ChunkPrefab = null;
        }
        chunk = null;
    }
}