using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    private Camera _camera;

    [SerializeField]
    private float _distance = 1;

    [SerializeField]
    private float _slerpScale = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var cameraPosition = _camera.transform.position;
        var cameraDirection = _camera.transform.forward;

        var targetPosition = cameraDirection * _distance + cameraPosition;
        targetPosition.y = cameraPosition.y;
        transform.LookAt(cameraPosition);
        transform.position = Vector3.Slerp(transform.position, targetPosition, _slerpScale * Time.deltaTime);

    }
}
