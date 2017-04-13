using System.Collections.Generic;

public interface ILightProcessor
{
    void LightChunks(List<Chunk> allVisibleChunks);
    void SetLightingAroundBlock(int x, int y, int z, int lightIndex);
    void RecalculateLightingAround(int x, int y, int z);
}

public class LightProcessor : ILightProcessor
{
    private readonly IBatchProcessor<Chunk> m_BatchProcessor;
    private readonly WorldData m_WorldData;

    public LightProcessor(IBatchProcessor<Chunk> batchProcessor, WorldData worldData)
    {
        m_BatchProcessor = batchProcessor;
        m_WorldData = worldData;
    }


    public void LightChunks(List<Chunk> chunks)
    {
        m_BatchProcessor.Process(chunks, LightChunk, true);
    }

    private void LightChunk(Chunk chunk)
    {
        LightSunlitBlocksInChunk(chunk);

        byte sunlight = m_WorldData.ShadesOfLight[0];
        int chunkWorldX = chunk.X * m_WorldData.ChunkBlockWidth;
        int chunkWorldY = chunk.Y * m_WorldData.ChunkBlockHeight;

        for (int x = 0; x < m_WorldData.ChunkBlockWidth; x++)
        {
            int blockX = chunkWorldX + x;
            for (int y = 0; y < m_WorldData.ChunkBlockHeight; y++)
            {
                int blockY = chunkWorldY + y;

                for (int z = m_WorldData.ChunkBlockDepth - 1; z >= 0; z--)
                {
                    Block block = chunk.Blocks[x, y, z];
                    if (block.Type == BlockType.Air && block.LightAmount == sunlight)
                    {
                        SetLightingAroundBlock(blockX, blockY, z, 1);
                    }
                }
            }
        }
    }

    private void LightSunlitBlocksInChunk(Chunk chunk)
    {
        byte sunlight = m_WorldData.ShadesOfLight[0];
        for (int x = 0; x < m_WorldData.ChunkBlockWidth; x++)
        {
            for (int y = 0; y < m_WorldData.ChunkBlockHeight; y++)
            {
                int z = m_WorldData.ChunkBlockDepth - 1;
                Block block = chunk.Blocks[x, y, z];

                while (block.Type == BlockType.Air && z >=0)
                {
                    chunk.Blocks[x, y, z].LightAmount = sunlight;
                    z--;
                    block = chunk.Blocks[x, y, z];
                }
            }
        }

        chunk.NeedsRegeneration = true;
    }

    private int LightIndexOf(byte shadeOfLight)
    {
        for (int i = 0; i < m_WorldData.ShadesOfLight.Length; i++)
        {
            if (m_WorldData.ShadesOfLight[i] == shadeOfLight)
            {
                return i;
            }
        }

        return 0;
    }


    public void RecalculateLightingAround(int x, int y, int z)
    {
        SetLightingAroundBlockRecursive(x - 1, y, z);
        SetLightingAroundBlockRecursive(x + 1, y, z);
        SetLightingAroundBlockRecursive(x, y + 1, z);
        SetLightingAroundBlockRecursive(x, y - 1, z);
        SetLightingAroundBlockRecursive(x, y, z + 1);
        SetLightingAroundBlockRecursive(x, y, z - 1);
    }

    private void SetLightingAroundBlockRecursive(int x, int y, int z)
    {
        byte currentShade = m_WorldData.GetBlockLight(x, y, z);
        if (currentShade == 0)
        {
            return;
        }

        int shadeIndex = LightIndexOf(currentShade);
        if (shadeIndex == m_WorldData.NumberOfLightShades - 1)
        {
            return;
        }

        SetLightingAroundBlock(x, y, z, shadeIndex + 1);
    }


    public void SetLightingAroundBlock(int x, int y, int z, int lightIndex)
    {
        SetLightingAroundBlockRecursive(x - 1, y, z, lightIndex);
        SetLightingAroundBlockRecursive(x + 1, y, z, lightIndex);
        SetLightingAroundBlockRecursive(x, y + 1, z, lightIndex);
        SetLightingAroundBlockRecursive(x, y - 1, z, lightIndex);
        SetLightingAroundBlockRecursive(x, y, z + 1, lightIndex);
        SetLightingAroundBlockRecursive(x, y, z - 1, lightIndex);
    }

    private void SetLightingAroundBlockRecursive(int x, int y, int z, int lightIndex)
    {
        if (x < 0 || y < 0 || x >= m_WorldData.WidthInBlocks || y >= m_WorldData.HeightInBlocks ||
            z >= m_WorldData.DepthInBlocks ||
            z < 0)
        {
            return;
        }

        int chunkX = x / m_WorldData.ChunkBlockWidth;
        int chunkY = y / m_WorldData.ChunkBlockHeight;
        int chunkZ = z / m_WorldData.ChunkBlockDepth;
        int blockX = x % m_WorldData.ChunkBlockWidth;
        int blockY = y % m_WorldData.ChunkBlockHeight;
        int blockZ = z % m_WorldData.ChunkBlockDepth;
        Chunk chunk = m_WorldData.Chunks[chunkX, chunkY, chunkZ];
        Block block = chunk.Blocks[blockX, blockY, blockZ];

        // Solid blocks don't get lit
        if (block.Type != BlockType.Air)
        {
            return;
        }

        byte lightAmount = m_WorldData.ShadesOfLight[lightIndex];

        // If it's already as bright or brighter than the shade we are working on, leave
        if (block.LightAmount >= lightAmount)
        {
            return;
        }
        chunk.Blocks[blockX, blockY, blockZ].LightAmount =
            lightAmount;
        chunk.NeedsRegeneration = true;

        int nextLightIndex = lightIndex + 1;
        if (nextLightIndex == m_WorldData.NumberOfLightShades)
        {
            return;
        }

        SetLightingAroundBlockRecursive(x - 1, y, z, nextLightIndex);
        SetLightingAroundBlockRecursive(x + 1, y, z, nextLightIndex);
        SetLightingAroundBlockRecursive(x, y + 1, z, nextLightIndex);
        SetLightingAroundBlockRecursive(x, y - 1, z, nextLightIndex);
        SetLightingAroundBlockRecursive(x, y, z + 1, nextLightIndex);
        SetLightingAroundBlockRecursive(x, y, z - 1, nextLightIndex);
        ;
    }
}