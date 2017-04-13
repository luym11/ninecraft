using System.Collections.Generic;

public interface ITerrainGenerator
{
    void GenerateChunkTerrain(List<Chunk> chunks);
}

public class TerrainGenerator : ITerrainGenerator
{
    private readonly WorldData m_WorldData;
    private readonly IBatchProcessor<Chunk> m_BatchProcessor;
    private readonly ITerrainGenerationMethod m_TerrainGenerationMethod;


    public TerrainGenerator(WorldData worldData, IBatchProcessor<Chunk> batchProcessor, ITerrainGenerationMethod terrainGenerationMethod)
    {
        m_WorldData = worldData;
        m_BatchProcessor = batchProcessor;
        m_TerrainGenerationMethod = terrainGenerationMethod;
    }

    public void GenerateTerrain(Chunk chunk)
    {
        m_TerrainGenerationMethod.GenerateTerrain(m_WorldData, chunk, (int) m_WorldData.NoiseBlockXOffset);
    }


    public void GenerateChunkTerrain(List<Chunk> chunks)
    {
        m_BatchProcessor.Process(chunks, GenerateTerrain, true);
    }

}