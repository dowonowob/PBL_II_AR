using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Unity.Collections;

[RequireComponent(typeof(MeshFilter))]
public class DepthMeshGenerator : MonoBehaviour
{
    public AROcclusionManager occlusionManager;
    public Camera arCamera;
    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            GenerateMeshFromDepth(image);
            image.Dispose();
        }
    }

    void GenerateMeshFromDepth(XRCpuImage image)
    {
        int width = image.width / 4; // 다운샘플링 (성능 최적화)
        int height = image.height / 4;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = TextureFormat.RFloat,
            transformation = XRCpuImage.Transformation.MirrorX
        };

        int size = image.GetConvertedDataSize(conversionParams);
        var depthBuffer = new NativeArray<byte>(size, Allocator.Temp);
        image.Convert(conversionParams, depthBuffer);

        float scale = 0.001f; // 메시 크기 조정

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * 4 * image.width + x * 4) * sizeof(float);
                if (index + 4 > depthBuffer.Length) continue;
                float depth = System.BitConverter.ToSingle(depthBuffer.ToArray(), index);
                Vector3 screenPos = new Vector3(x * 4, y * 4, depth);
                Vector3 worldPos = arCamera.ScreenToWorldPoint(screenPos);
                vertices.Add(worldPos);
            }
        }

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int i = y * width + x;
                triangles.Add(i);
                triangles.Add(i + width);
                triangles.Add(i + 1);

                triangles.Add(i + 1);
                triangles.Add(i + width);
                triangles.Add(i + width + 1);
            }
        }

        depthBuffer.Dispose();
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}