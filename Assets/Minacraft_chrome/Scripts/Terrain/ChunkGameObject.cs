using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ChunkGameObject : MonoBehaviour
{
    public Shader Shader;

    private MeshFilter m_MeshFilter;
    private MeshCollider m_MeshCollider;
    private MeshRenderer m_MeshRenderer;

    public Texture Texture;
    /// <summary>
    /// Actually create the mesh now for the chunk.
    /// </summary>
    public void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();
        m_MeshFilter = gameObject.GetComponent<MeshFilter>();
        m_MeshCollider = gameObject.GetComponent<MeshCollider>();
        m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
        m_MeshRenderer.material.shader = Shader;
        m_MeshRenderer.material.mainTexture = Texture;
    }

    public void CreateFromChunk(Chunk chunk)
    {

        m_MeshFilter.mesh.Clear();
        m_MeshFilter.mesh.vertices = chunk.Vertices.ToArray();
        m_MeshFilter.mesh.uv = chunk.Uvs.ToArray();
        m_MeshFilter.mesh.colors = chunk.Colors.ToArray();
        m_MeshFilter.mesh.triangles = chunk.Indices.ToArray();
        m_MeshCollider.sharedMesh = null;
        m_MeshCollider.sharedMesh = m_MeshFilter.mesh;

        chunk.Vertices = new List<Vector3>();
        chunk.Uvs = new List<Vector2>();
        chunk.Colors = new List<Color>();
        chunk.Indices = new List<int>();
    }
}