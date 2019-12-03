using UnityEngine;

// Places world markers, activates and controls touch data
public class TouchPointController : MonoBehaviour {

    public TouchDataState TouchDataState;

    public Transform PointMarker;
    public TouchDataIndicator TouchDataIndicator;

    [SerializeField]
    private ExpandingCircle _expandingCircle;

    private bool _showData;

    private Vector3 _curWorldTouchPoint;
    private Camera _userXrCam;
    private GridEffect _gridEffectScript;

    private Vector3 _inactiveMarkerPos;     // Hidden position that indicates the marker is absolutely inactive
    private bool _hasPoiData;

    void Start () {
        _userXrCam = User.Instance.GetComponent<Camera>();
        TouchDataIndicator.gameObject.SetActive(false);
        _gridEffectScript = PointMarker.GetComponentInChildren<GridEffect>();

        _inactiveMarkerPos = Vector3.down * -10000;
        PointMarker.position = _inactiveMarkerPos;
    }
	
	void LateUpdate () {
        if (_showData)
        {
            if (MapManager.MapMode == MapMode.Mini)
            {
                float angle = Vector3.Angle(_userXrCam.transform.forward, _curWorldTouchPoint - _userXrCam.transform.position);

                // Prevents seeing the screen data when looking at it from the opposite direction
                if (angle <= 90)
                {
                    TouchDataIndicator.gameObject.SetActive(true);
                    GetComponent<RectTransform>().position = _userXrCam.WorldToScreenPoint(_curWorldTouchPoint);
                }
                else
                {
                    TouchDataIndicator.gameObject.SetActive(false);
                }
            }
            else
            {
                Vector3 pos = MapManager.Instance.FullMapCam.GetComponent<Camera>().WorldToScreenPoint(_curWorldTouchPoint);
                pos.z = 0;
                GetComponent<RectTransform>().position = pos;
            }
        }
    }

    public void Set(RaycastHit hit)
    {
        _expandingCircle.Activate(hit.point, Quaternion.LookRotation(hit.normal));
        PointMarker.GetComponent<WorldTapPointMarker>().Set(hit.point, hit.normal);

        _curWorldTouchPoint = hit.point;

        bool hitBuildingLayer = (hit.collider.gameObject.layer == DemoConstants.SturfeeBuildingLayer);

        if (hitBuildingLayer)
        {
            TouchDataIndicator.CalculateData(_curWorldTouchPoint);
        }

        TouchDataIndicator.gameObject.SetActive(hitBuildingLayer);
        _showData = hitBuildingLayer;
        _hasPoiData = hitBuildingLayer;
    }

    public void ResetDataVisuals()
    {
        _showData = false;
        _hasPoiData = false;
        TouchDataIndicator.gameObject.SetActive(false);
        PointMarker.position = _inactiveMarkerPos;
    }

    // Turn data visuals on or off. Only will turn on if markerwas 'active' despite the visuals not being visible.
    public void SetDataVisuals(bool val)
    {
        if(val)
        {
            val = HasActiveMarker() && _hasPoiData;
        }

        _showData = val;
        TouchDataIndicator.gameObject.SetActive(val);
    }

    public bool HasActiveMarker()
    {
        //print("Has Active Marker: " + (PointMarker.position != _inactiveMarkerPos));
        return PointMarker.position != _inactiveMarkerPos;
    }

}
