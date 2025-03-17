using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CustomMeshGenerator : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public DepthTextureManager depthManager;  // 아래에서 생성한 DepthTextureManager 스크립트를 연결

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

        // 정점 생성: gridWidth x gridHeight 그리드로 샘플링
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int pixelX = Mathf.FloorToInt((float)x / gridWidth * texWidth);
                int pixelY = Mathf.FloorToInt((float)y / gridHeight * texHeight);

                Color depthColor = depthTexture.GetPixel(pixelX, pixelY);
                float depthValue = depthColor.r;  // 예시로 Red 채널 사용

                // depth 값으로 카메라 공간 좌표 계산
                Vector3 camSpacePos = DepthToCameraSpace(pixelX, pixelY, depthValue, texWidth, texHeight);
                // 월드 좌표로 변환
                Vector3 worldPos = Camera.main.transform.TransformPoint(camSpacePos);

                vertices[y * gridWidth + x] = worldPos;
            }
        }

        // 삼각형 인덱스 생성 (그리드 기반)
        for (int y = 0; y < gridHeight - 1; y++)
        {
            for (int x = 0; x < gridWidth - 1; x++)
            {
                int topLeft = y * gridWidth + x;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + gridWidth;
                int bottomRight = bottomLeft + 1;

                // 첫 번째 삼각형
                triangles.Add(topLeft);
                triangles.Add(bottomLeft);
                triangles.Add(topRight);

                // 두 번째 삼각형
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

    // 간단한 Depth 값 → 카메라 공간 좌표 변환 예제 (정확한 변환을 위해서는 카메라 매트릭스 필요)
    Vector3 DepthToCameraSpace(int x, int y, float depth, int textureWidth, int textureHeight)
    {
        float u = (float)x / textureWidth - 0.5f;
        float v = (float)y / textureHeight - 0.5f;
        Vector3 camSpacePoint = new Vector3(u * depth * 2f, v * depth * 2f, depth);
        return camSpacePoint;
    }
}
