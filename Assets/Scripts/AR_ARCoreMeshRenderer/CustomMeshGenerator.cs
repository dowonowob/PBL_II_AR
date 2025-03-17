using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CustomMeshGenerator : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public DepthTextureManager depthManager;  // �Ʒ����� ������ DepthTextureManager ��ũ��Ʈ�� ����

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Texture2D depthTexture = depthManager.GetDepthTexture();
        if (depthTexture == null)
        {
            Debug.LogWarning("Depth texture is not available.");
            return;
        }
        int texWidth = depthTexture.width;
        int texHeight = depthTexture.height;

        Vector3[] vertices = new Vector3[gridWidth * gridHeight];
        List<int> triangles = new List<int>();

        // ���� ����: gridWidth x gridHeight �׸���� ���ø�
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int pixelX = Mathf.FloorToInt((float)x / gridWidth * texWidth);
                int pixelY = Mathf.FloorToInt((float)y / gridHeight * texHeight);

                Color depthColor = depthTexture.GetPixel(pixelX, pixelY);
                float depthValue = depthColor.r;  // ���÷� Red ä�� ���

                // depth ������ ī�޶� ���� ��ǥ ���
                Vector3 camSpacePos = DepthToCameraSpace(pixelX, pixelY, depthValue, texWidth, texHeight);
                // ���� ��ǥ�� ��ȯ
                Vector3 worldPos = Camera.main.transform.TransformPoint(camSpacePos);

                vertices[y * gridWidth + x] = worldPos;
            }
        }

        // �ﰢ�� �ε��� ���� (�׸��� ���)
        for (int y = 0; y < gridHeight - 1; y++)
        {
            for (int x = 0; x < gridWidth - 1; x++)
            {
                int topLeft = y * gridWidth + x;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + gridWidth;
                int bottomRight = bottomLeft + 1;

                // ù ��° �ﰢ��
                triangles.Add(topLeft);
                triangles.Add(bottomLeft);
                triangles.Add(topRight);

                // �� ��° �ﰢ��
                triangles.Add(topRight);
                triangles.Add(bottomLeft);
                triangles.Add(bottomRight);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    // ������ Depth �� �� ī�޶� ���� ��ǥ ��ȯ ���� (��Ȯ�� ��ȯ�� ���ؼ��� ī�޶� ��Ʈ���� �ʿ�)
    Vector3 DepthToCameraSpace(int x, int y, float depth, int textureWidth, int textureHeight)
    {
        float u = (float)x / textureWidth - 0.5f;
        float v = (float)y / textureHeight - 0.5f;
        Vector3 camSpacePoint = new Vector3(u * depth * 2f, v * depth * 2f, depth);
        return camSpacePoint;
    }
}
