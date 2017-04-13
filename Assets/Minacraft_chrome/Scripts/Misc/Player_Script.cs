using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    public Transform World_Prefab;
    // Our player is centered in the middle of the map
    //private const int MiddleXOfMap = (World.WidthInChunks/2)*Chunk.WidthInBlocks;
    //private const int MiddleYOfMap = (World.HeightInChunks / 2) * Chunk.HeightInBlocks;

    // A Rectangle 2 chunks wide originally centered around the player, but 
    // moves with the world. If the player hits it, we need to shift everything
    private Rect m_ShiftDetectionRectangle;
    private World m_World { get; set; }

    // Use this for initialization
    private void Start()
    {
        MovePlayerToCenterOfMap();

        //m_ShiftDetectionRectangle = new Rect(MiddleXOfMap - Chunk.WidthInBlocks,
        //                                     MiddleYOfMap - Chunk.HeightInBlocks, Chunk.WidthInBlocks*2,
        //                                     Chunk.HeightInBlocks*2);
    }

    // Update is called once per frame
    private void Update()
    {
        //CenterWorldAroundPlayer();
        //CheckIfWeNeedToGenerateMoreTerrain();
    }

    private void CenterWorldAroundPlayer()
    {
        // Has the player even moved? If not...we don't need
        // to shift everything else.

        //if (PlayerHasNotMoved())
        //{
        //    return;
        //}

        //MoveEverythingAroundThePlayer();

        //MovePlayerToCenterOfMap();

        //CheckIfWeNeedToGenerateMoreTerrain();
    }

    //private bool PlayerHasNotMoved()
    //{
    //    //return transform.position.x == MiddleXOfMap && transform.position.z == MiddleYOfMap;
    //}

    //private void MoveEverythingAroundThePlayer()
    //{
    //    // Everything else refers to the transform.positions of all critters and chunks
    //    Vector3 amountToShiftEverythingElse = new Vector3(MiddleXOfMap - transform.position.x, 0,
    //                                                      MiddleYOfMap - transform.position.z);

    //    // Keep track of our real location
    //    WorldGameObject.GlobalXOffset += amountToShiftEverythingElse.x;
    //    WorldGameObject.GlobalZOffset += amountToShiftEverythingElse.z;
    //    WorldGameObject.NoiseBlockXOffset += (transform.position.x - MiddleXOfMap);

    //    //MoveEverythingElseInTheOppositeDirection();
    //    MoveMapShiftDetectionRectangle(amountToShiftEverythingElse);
    //}

    //private static void MoveEverythingElseInTheOppositeDirection()
    //{
    //    for (int y = World.BottomVisibleChunkRow; y <= World.TopVisibleChunkRow; y++)
    //    {
    //        for (int x = WorldGameObject.World; x <= World.RightVisibleChunkColumn; x++)
    //        {
    //            Chunk chunk = WorldGameObject.Chunks[x, y, 0];
    //            if (chunk.ChunkPrefab != null)
    //            {
    //                chunk.ChunkPrefab.position =
    //                    new Vector3(chunk.X*Chunk.WidthInBlocks + WorldGameObject.GlobalXOffset,
    //                                0,
    //                                chunk.Y*Chunk.HeightInBlocks + WorldGameObject.GlobalZOffset);
    //            }
    //        }
    //    }
    //}

    // This rectangle tells us if we need to shift the world
    private void MoveMapShiftDetectionRectangle(Vector3 amountToMove)
    {
        m_ShiftDetectionRectangle.x += amountToMove.x;
        m_ShiftDetectionRectangle.y += amountToMove.y;
    }

    //private void CheckIfWeNeedToGenerateMoreTerrain()
    //{
    //    int xIncrement = 0;
    //    int yIncrement = 0;
    //    if (transform.position.x > m_ShiftDetectionRectangle.xMax)
    //    {
    //        Debug.Log("x" + m_ShiftDetectionRectangle.x);
    //        Debug.Log("xmin" + m_ShiftDetectionRectangle.xMin);
    //        Debug.Log("xmax" + m_ShiftDetectionRectangle.xMax);

    //        xIncrement = 1;
    //        m_ShiftDetectionRectangle.x += Chunk.WidthInBlocks;
    //    }
    //    else if (transform.position.x < m_ShiftDetectionRectangle.x)
    //    {
    //        xIncrement = -1;
    //        m_ShiftDetectionRectangle.x -= Chunk.WidthInBlocks;
    //    }

    //    //if (transform.position.y > m_ShiftDetectionRectangle.yMax)
    //    //{
    //    //    yIncrement = -1;
    //    //    m_ShiftDetectionRectangle.y -= Chunk.HeightInBlocks;
    //    //}
    //    //else if (transform.position.y < m_ShiftDetectionRectangle.y)
    //    //{
    //    //    yIncrement = 1;
    //    //    m_ShiftDetectionRectangle.x += Chunk.WidthInBlocks;
    //    //}

    //    if (xIncrement != 0 || yIncrement != 0)
    //    {
    //        ShiftAllWorldChunks(xIncrement, yIncrement);
    //    }
    //}

    



    private void MovePlayerToCenterOfMap()
    {
        //transform.position = new Vector3(MiddleXOfMap, transform.position.y, MiddleYOfMap);
    }
}