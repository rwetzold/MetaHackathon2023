using Hackathon;
using Photon.Pun;
using UnityEngine;

public class PanelButton : MonoBehaviour
{
    private CreatorPanel _creatorPanel;

    [SerializeField] private string _prefab;
    [SerializeField] private int _price;
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
        if (_creatorPanel.player.TryPay(_price))
        {
            GameObject unit = PhotonNetwork.Instantiate(_prefab, _camera.transform.TransformPoint(_cameraOffset),
                Quaternion.identity);
            unit.GetComponent<UnitBehaviour>().ownerPlayer = _creatorPanel.player;
        }
    }
}