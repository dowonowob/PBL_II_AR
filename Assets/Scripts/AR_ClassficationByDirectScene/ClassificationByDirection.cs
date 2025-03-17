using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ClassificationByDirection : MonoBehaviour
{
    // ARPlane ������Ʈ (���� ������Ʈ�� �پ� �ִٰ� ����)
    public ARPlane _arPlane;

    // ����� �ð������� ǥ���ϴ� MeshRenderer
    public MeshRenderer _meshRenderer;

    // ����� �з��� �ؽ�Ʈ�� ������ TextMesh
    public TextMesh _textMesh;

    // �ؽ�Ʈ�� �����ϴ� ������Ʈ (Label)
    public GameObject _textObj;

    GameObject _mainCam;

    void Start()
    {
        // �� ������Ʈ�� �ڵ� �Ҵ���� �ʾҴٸ�, GetComponent�� ������
        if (_arPlane == null)
            _arPlane = GetComponent<ARPlane>();

        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();

        if (_textMesh == null)
            _textMesh = GetComponentInChildren<TextMesh>();

        if (_textObj == null && _textMesh != null)
            _textObj = _textMesh.gameObject;

        // ���� ī�޶� ��������
        _mainCam = Camera.main.gameObject;

        // �ʱ� �з� ������Ʈ
        UpdateClassification();
    }

    void Update()
    {
        // �� �����Ӹ��� �ؽ�Ʈ ��ġ ����
        UpdateLabelPosition();
    }

    /// <summary>
    /// ARPlane�� alignment ���� Ȱ���Ͽ� ����� �з��ϰ�, ����� ���̺� �ؽ�Ʈ�� ������Ʈ
    /// </summary>
    void UpdateClassification()
    {
        string label = "";
        Color planeColor = Color.gray;

        // ARPlane.alignment ���� ���� ��� ������ �Ǻ�
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

        // �з� ����� �ؽ�Ʈ�� ǥ��
        _textMesh.text = label;

        // ���� ���� �߰� (��: 33% ����)
        planeColor.a = 0.33f;
        _meshRenderer.material.color = planeColor;
    }

    /// <summary>
    /// ����� �߽� ��ǥ�� ���� ��ǥ�� ��ȯ�Ͽ� �ؽ�Ʈ ���̺��� ��ġ�� ������Ʈ
    /// �׸��� ī�޶� �ٶ󺸵��� ȸ�� ó��
    /// </summary>
    void UpdateLabelPosition()
    {
        if (_textObj != null && _mainCam != null)
        {
            // ARPlane.center�� ��� �� ���� ��ǥ�̹Ƿ�, ���� ��ǥ�� ��ȯ
            Vector3 worldCenter = _arPlane.transform.TransformPoint(_arPlane.center);
            _textObj.transform.position = worldCenter;

            // �ؽ�Ʈ ������Ʈ�� ī�޶� �ٶ󺸵��� ȸ�� (LookAt ��, 180�� ȸ���Ͽ� �ùٸ� ����)
            _textObj.transform.LookAt(_mainCam.transform);
            _textObj.transform.Rotate(0, 180, 0);
        }
    }
}
