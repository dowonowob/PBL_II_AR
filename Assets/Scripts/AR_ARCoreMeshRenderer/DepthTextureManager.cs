using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DepthTextureManager : MonoBehaviour
{
    public AROcclusionManager occlusionManager;

    void Start()
    {
        if (occlusionManager == null)
            occlusionManager = GetComponent<AROcclusionManager>();
    }

    // 환경 Depth Texture를 반환하는 함수
    public Texture2D GetDepthTexture()
    {
        return occlusionManager.environmentDepthTexture;
    }
}
