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

    // ȯ�� Depth Texture�� ��ȯ�ϴ� �Լ�
    public Texture2D GetDepthTexture()
    {
        return occlusionManager.environmentDepthTexture;
    }
}
