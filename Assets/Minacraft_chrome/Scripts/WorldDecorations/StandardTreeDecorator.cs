using UnityEngine;

public class StandardTreeDecorator : IDecoration
{
    private readonly WorldData m_WorldData;

    public StandardTreeDecorator(WorldData worldData)
    {
        m_WorldData = worldData;
    }


    public void Decorate(int blockX, int blockY, int blockZ, MCRandom random)
    {
        if (IsAValidLocationforDecoration(blockX, blockY, blockZ, random))
        {
            CreateDecorationAt(blockX, blockY, blockZ, random);
        }
    }

    /// <summary>
    /// Determines if a tree decoration even wants to be at this location.
    /// </summary>
    /// <param name="blockX"></param>
    /// <param name="blockY"></param>
    /// <param name="blockZ"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    private bool IsAValidLocationforDecoration(int blockX, int blockY, int blockZ, MCRandom random)
    {
        // We don't want TOO many trees...make it a 1% chance to be drawn there.
        if (random.RandomRange(1, 100) < 99)
        {
            return false;
        }

        // Trees don't like to grow too high
        if (blockZ >= m_WorldData.DepthInBlocks - 20)
        {
            return false;
        }

        // Trees like to have a minimum amount of space to grow in.
        return SpaceAboveIsEmpty(blockX, blockY, blockZ, 8, 2, 2);
    }

    private void CreateDecorationAt(int blockX, int blockY, int blockZ, MCRandom random)
    {
        int trunkLength = random.RandomRange(6, 10);
        // Trunk
        for (int z = blockZ + 1; z <= blockZ + trunkLength; z++)
        {
            CreateTrunkAt(blockX, blockY, z);
        }

        // Leaves
        CreateSphereAt(blockX, blockY, blockZ + trunkLength, random.RandomRange(3, 4));
    }

    /// <summary>
    /// Creates the tree canopy...a ball of leaves.
    /// </summary>
    /// <param name="blockX"></param>
    /// <param name="blockY"></param>
    /// <param name="blockZ"></param>
    /// <param name="radius"></param>
    private void CreateSphereAt(int blockX, int blockY, int blockZ, int radius)
    {
        for (int x = blockX - radius; x <= blockX + radius; x++)
        {
            for (int y = blockY - radius; y <= blockY + radius; y++)
            {
                for (int z = blockZ - radius; z <= blockZ + radius; z++)
                {
                    if (Vector3.Distance(new Vector3(blockX, blockY, blockZ), new Vector3(x, y, z)) <= radius)
                    {
                        m_WorldData.SetBlockType(x, y, z, BlockType.Leaves);
                    }
                }
            }
        }
    }

    private void CreateTrunkAt(int blockX, int blockY, int z)
    {
        m_WorldData.SetBlockType(blockX, blockY, z, BlockType.Dirt);
    }

    private bool SpaceAboveIsEmpty(int blockX, int blockY, int blockZ, int depthAbove, int widthAround, int heightAround)
    {
        for (int z = blockZ + 1; z <= blockZ + depthAbove; z++)
        {
            for (int x = blockX - widthAround; x <= blockX + widthAround; x++)
            {
                for (int y = blockY - heightAround; y < blockY + heightAround; y++)
                {
                    if (m_WorldData.GetBlock(x, y, z).Type != BlockType.Air)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public override string ToString()
    {
        return "Standard Tree";
    }
}