using Sturfee.Unity.XR.Core.Session;
using UnityEngine;
using UnityEngine.UI;

public class TouchDataIndicator : MonoBehaviour {

    [Header("Data Set Parents")]
    public GameObject PoiData;
    public GameObject DistanceData;

    [Header("POI Set Components")]
    public Transform DataList;
    public GameObject FreeTextPanel;
    public GameObject MoreDataButton;
    public Transform DropDownArrow;

    [Header("Distance set Components")]
    public Text DistanceText;
    public Text ElevationText;

    private bool _moreDataActive;
    private Quaternion _arrowDefaultRot;

    private Vector3 _targetWorldPoint;
    private int _curVenueListLen;

    void Start() {
        FoursquareService.OnVenuesCallStarted += OnVenuesCallStarted;
        FoursquareService.OnVenuesReceived += OnVenuesReceived;
        FoursquareService.OnVenuesCallFailed += OnVenuesFailed;

        _arrowDefaultRot = Quaternion.Euler(Vector3.back * 90);
    }

    private void OnDestroy()
    {
        FoursquareService.OnVenuesCallStarted -= OnVenuesCallStarted;
        FoursquareService.OnVenuesReceived -= OnVenuesReceived;
        FoursquareService.OnVenuesCallFailed -= OnVenuesFailed;
    }

    //private void Update()
    //{
    //    // TODO: Actively update distance here when on distance mode
    //}

    // Will calculate all data, but will only show the data that matches the current touch data state.
    public void CalculateData(Vector3 worldTouchPoint)
    {
        gameObject.SetActive(true);
        _targetWorldPoint = worldTouchPoint;
        CalculatePoiData();
        CalculateDistanceData();

        Display();
    }

    private void CalculatePoiData()
    {
        var gpsPos = XRSessionManager.GetSession().LocalPositionToGps(_targetWorldPoint);
        GetComponent<FoursquareService>().GetSortedVenues(gpsPos);
    }

    private void CalculateDistanceData()
    {
        float distanceFromUser = Vector3.Magnitude(_targetWorldPoint - User.Instance.transform.position);
        DistanceText.text = "Distance: " + distanceFromUser.ToString("F2") + " m";
        ElevationText.text = "Elevation: " + _targetWorldPoint.y.ToString("F2") + " m";
    }

    public void Display()
    {
        DistanceData.SetActive((UiManager.Instance.TouchDataState == TouchDataState.Distance));
        PoiData.SetActive((UiManager.Instance.TouchDataState == TouchDataState.Poi));
    }

    public void OnMoreDataClick()
    {
        _moreDataActive = !_moreDataActive;
        DropDownArrow.Rotate(Vector3.forward * 180);

        for (int i = 1; i < _curVenueListLen; i++)
        {
            DataList.GetChild(i).gameObject.SetActive(_moreDataActive);
        }
    }

    private void ResetPoiDataVisuals()
    {
        for (int i = 0; i < DataList.childCount; i++)
        {
            DataList.GetChild(i).gameObject.SetActive(false);
        }
        _moreDataActive = false;
        MoreDataButton.SetActive(false);
        DropDownArrow.rotation = _arrowDefaultRot;
    }

    #region FoursquareServiceEvents
    private void OnVenuesCallStarted()
    {
        FreeTextPanel.SetActive(true);
        FreeTextPanel.GetComponentInChildren<Text>().text = "Searching...";

        ResetPoiDataVisuals();
    }

    private void OnVenuesReceived(FoursquareAPI.Venue[] venues)
    {
        if (venues.Length <= 0)
        {
            // No Data Found
            FreeTextPanel.transform.GetComponentInChildren<Text>().text = "No Data Found Here";
        }
        else
        {
            FreeTextPanel.SetActive(false);

            // Fill out POI data list, but only show the first item in the list
            for (int i = 0; i < venues.Length; i++)
            {
                DataList.GetChild(i).GetComponent<ListItem>().SetLocationData(venues[i].name, venues[i].stats.checkinsCount);
            }
            DataList.GetChild(0).gameObject.SetActive(true);

            if(venues.Length > 1)
            {
                MoreDataButton.SetActive(true);
            }

            _curVenueListLen = venues.Length;
        }
    }

    private void OnVenuesFailed()
    {
        FreeTextPanel.GetComponentInChildren<Text>().text = "Error";
    }
    #endregion
}
