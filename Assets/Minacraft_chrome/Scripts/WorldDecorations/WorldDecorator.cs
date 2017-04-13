using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public interface IWorldDecorator
{
    void GenerateWorldDecorations(List<Chunk> chunks);
}

/// <summary>
/// Adds trees, shrubs, etc to the world.
/// </summary>
public class WorldDecorator : IWorldDecorator
{
    private readonly WorldData m_WorldData;
    private readonly IBatchProcessor<Chunk> m_BatchProcessor;
    private readonly List<IDecoration> m_Decorations;

    public WorldDecorator(WorldData worldData, IBatchProcessor<Chunk> batchProcessor, List<IDecoration> decorations)
    {
        m_WorldData = worldData;
        m_BatchProcessor = batchProcessor;
        m_Decorations = decorations;
    }


    public void GenerateWorldDecorations(List<Chunk> chunks)
    {
        m_BatchProcessor.Process(chunks, GenerateDecorationsForChunk, true); 
    }

    /// <summary>
    /// For the given chunk's topsoil blocks, ask each decorator (tree, bush, etc)
    /// to decorate here. Of course, the decorator ultimately decides if it wants to 
    /// be here or now.
    /// </summary>
    /// <param name="chunk"></param>
    private void GenerateDecorationsForChunk(Chunk chunk)
    {
        MCRandom random = new MCRandom();
        foreach (IntVect topSoilBlock in chunk.TopSoilBlocks)
        {
            foreach (IDecoration decoration in m_Decorations)
            {
                decoration.Decorate(topSoilBlock.X, topSoilBlock.Y, topSoilBlock.Z, random);
            }
        }
    }
}