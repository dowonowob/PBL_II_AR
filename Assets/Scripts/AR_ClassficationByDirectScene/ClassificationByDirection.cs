using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ClassificationByDirection : MonoBehaviour
{
    // ARPlane 컴포넌트 (동일 오브젝트에 붙어 있다고 가정)
    public ARPlane _arPlane;

    // 평면을 시각적으로 표현하는 MeshRenderer
    public MeshRenderer _meshRenderer;

    // 평면의 분류를 텍스트로 보여줄 TextMesh
    public TextMesh _textMesh;

    // 텍스트를 포함하는 오브젝트 (Label)
    public GameObject _textObj;

    GameObject _mainCam;

    void Start()
    {
        // 각 컴포넌트가 자동 할당되지 않았다면, GetComponent로 가져옴
        if (_arPlane == null)
            _arPlane = GetComponent<ARPlane>();

        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();

        if (_textMesh == null)
            _textMesh = GetComponentInChildren<TextMesh>();

        if (_textObj == null && _textMesh != null)
            _textObj = _textMesh.gameObject;

        // 메인 카메라 가져오기
        _mainCam = Camera.main.gameObject;

        // 초기 분류 업데이트
        UpdateClassification();
    }

    void Update()
    {
        // 매 프레임마다 텍스트 위치 갱신
        UpdateLabelPosition();
    }

    /// <summary>
    /// ARPlane의 alignment 값을 활용하여 평면을 분류하고, 색상과 레이블 텍스트를 업데이트
    /// </summary>
    void UpdateClassification()
    {
        string label = "";
        Color planeColor = Color.gray;

        // ARPlane.alignment 값에 따라 평면 종류를 판별
        switch (_arPlane.alignment)
        {
            case PlaneAlignment.HorizontalUp:
                label = "Floor";
                planeColor = Color.green;
                break;
            case PlaneAlignment.HorizontalDown:
                label = "Ceiling";
                planeColor = Color.blue;
                break;
            case PlaneAlignment.Vertical:
                label = "Wall";
                planeColor = Color.white;
                break;
            default:
                label = "Unknown";
                planeColor = Color.gray;
                break;
        }

        // 분류 결과를 텍스트에 표시
        _textMesh.text = label;

        // 색상에 투명도 추가 (예: 33% 투명)
        planeColor.a = 0.33f;
        _meshRenderer.material.color = planeColor;
    }

    /// <summary>
    /// 평면의 중심 좌표를 월드 좌표로 변환하여 텍스트 레이블의 위치를 업데이트
    /// 그리고 카메라를 바라보도록 회전 처리
    /// </summary>
    void UpdateLabelPosition()
    {
        if (_textObj != null && _mainCam != null)
        {
            // ARPlane.center는 평면 내 로컬 좌표이므로, 월드 좌표로 변환
            Vector3 worldCenter = _arPlane.transform.TransformPoint(_arPlane.center);
            _textObj.transform.position = worldCenter;

            // 텍스트 오브젝트가 카메라를 바라보도록 회전 (LookAt 후, 180도 회전하여 올바른 방향)
            _textObj.transform.LookAt(_mainCam.transform);
            _textObj.transform.Rotate(0, 180, 0);
        }
    }
}
