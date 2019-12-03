using UnityEngine;
using UnityEngine.EventSystems;

public class MapTouchController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public float MapMoveSensitivity = 0.3f;

    private int _camBoundsUp;
    private int _camBoundsDown;
    private int _camBoundsRight;
    private int _camBoundsLeft;

    // Values currently based on 300 Orthrographic camera
    private float _camBoundBaseVertOffset = 160;
    private float _camBoundBaseHorOffset = 140;

    private Vector2 _prevTouchPos;

    public void OnBeginDrag(PointerEventData data)
    {
        _prevTouchPos = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 touchDifference = (_prevTouchPos - data.position) * MapMoveSensitivity;

        Vector3 camPos = MapManager.Instance.FullMapCam.transform.position;
        camPos.x += touchDifference.x;
        camPos.z += touchDifference.y;

        // Keep the camera in the boundaries of the map
        if (camPos.x > _camBoundsRight)
        {
            camPos.x = _camBoundsRight;
        }
        else if (camPos.x < _camBoundsLeft)
        {
            camPos.x = _camBoundsLeft;
        }

        if (camPos.z > _camBoundsUp)
        {
            camPos.z = _camBoundsUp;
        }
        else if (camPos.z < _camBoundsDown)
        {
            camPos.z = _camBoundsDown;
        }

        MapManager.Instance.FullMapCam.transform.position = camPos;
        _prevTouchPos = data.position;
    }

    public void ComputeMapboxCamBounds()
    {
        float aspectRatio = MapManager.Instance.FullMapCam.GetComponent<Camera>().aspect;
        float vertOffset = (1 - aspectRatio) * _camBoundBaseVertOffset;
        float horOffset = (1 - aspectRatio) * _camBoundBaseHorOffset;
        Vector3 mapCenterCoord = MapManager.Instance.MapboxMap.transform.GetChild(0).position;
        float distFromMapCenterToEdge = Mathf.Abs((mapCenterCoord.x - MapManager.Instance.MapboxMap.transform.GetChild(1).position.x));

        _camBoundsUp = (int)(mapCenterCoord.z + distFromMapCenterToEdge - vertOffset);
        _camBoundsDown = (int)(mapCenterCoord.z - distFromMapCenterToEdge + vertOffset);
        _camBoundsRight = (int)(mapCenterCoord.x + distFromMapCenterToEdge + horOffset);
        _camBoundsLeft = (int)(mapCenterCoord.x - distFromMapCenterToEdge - horOffset);

        //print("Map center pos: " + mapCenterCoord);
        //print("Map dist: " + distFromMapCenterToEdge);

        //print("Cam Bounds Up: " + _camBoundsUp);
        //print("Cam Bounds Down: " + _camBoundsDown);
        //print("Cam Bounds Right: " + _camBoundsRight);
        //print("Cam Bounds Left: " + _camBoundsLeft);
    }

}
