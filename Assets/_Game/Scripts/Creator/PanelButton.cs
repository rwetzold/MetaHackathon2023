using Hackathon;
using Photon.Pun;
using UnityEngine;

public class PanelButton : MonoBehaviour
{
    private CreatorPanel _creatorPanel;

    [SerializeField] private UnitBehaviour _prefab;
    [SerializeField] private Vector3 _cameraOffset;

    private Camera _camera;

    private void Awake()
    {
        _creatorPanel = GetComponentInParent<CreatorPanel>();
        _camera = Camera.main;
    }

    [ContextMenu("Execute")]
    public void ButtonPressed()
    {
        if (_creatorPanel.player.TryPay((int)_prefab.Attributes.PriceValue))
        {
            var unit = Instantiate(_prefab.gameObject, _camera.transform.TransformPoint(_cameraOffset),
                Quaternion.identity);
            unit.GetComponent<UnitBehaviour>().ownerPlayer = _creatorPanel.player;
        }
    }
}