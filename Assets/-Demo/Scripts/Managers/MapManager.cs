using Mapbox.Unity.Map;
using Mapbox.Utils;
using Sturfee.Unity.XR.Core.Session;
using UnityEngine;
using UnityEngine.UI;

public enum MapMode
{
    None, Mini, Full
}

public class MapManager : MonoBehaviour {

    public static MapManager Instance;
    public static MapMode MapMode;

    public AbstractMap MapboxMap;
    public GameObject FullMapCam;
    public GameObject MiniMapCam;
    public GameObject MiniMap;
    public Button MiniMapButton;
    public GameObject MapTouchPanel;

    private void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
        MiniMap.SetActive(false);
    }

    public void InitializeMap()
    {
        Vector3 mapPos = XRSessionManager.GetSession().GetXRCameraPosition();
        MapboxMap.transform.position = mapPos;

        var gpsPos = XRSessionManager.GetSession().LocalPositionToGps(mapPos);
        Vector2d mapboxCoord = new Vector2d(gpsPos.Latitude, gpsPos.Longitude);
        MapboxMap.Initialize(mapboxCoord, 16);
        MapboxMap.transform.position += Vector3.down * 0.3f;

        MiniMapCam.SetActive(true);
        MapMode = MapMode.Mini;
    }

    public void AllowMapboxMiniMap()
    {
        MapTouchPanel.GetComponent<MapTouchController>().ComputeMapboxCamBounds();
        MiniMap.SetActive(true);
    }

    #region ButtonEvents
    public void CenterFullMapCamOnPlayer()
    {
        FullMapCam.transform.position = XRSessionManager.GetSession().GetXRCameraPosition() + (Vector3.up * 100);
    }

    public void SetToMiniMap()
    {
        FullMapCam.SetActive(false);
        MapTouchPanel.SetActive(false);

        MapMode = MapMode.Mini;
        MiniMap.SetActive(true);

        SetupManager.Instance.PreLocalizedScanButtonSet(true);
    }

    public void SetToFullMap()
    {
        FullMapCam.SetActive(true);
        FullMapCam.transform.position = XRSessionManager.GetSession().GetXRCameraPosition() + (Vector3.up * 100);
        MapTouchPanel.SetActive(true);

        MapMode = MapMode.Full;
        MiniMap.SetActive(false);
        SetupManager.Instance.PreLocalizedScanButtonSet(false);
    }
    #endregion
}
