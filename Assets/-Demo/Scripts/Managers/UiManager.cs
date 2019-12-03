using UnityEngine;

public enum TouchDataState
{
    Poi,
    Distance
}

// Handles swapping between various UI states (AR vs map mode, POI vs Distance Mode) as well as other UI interactions
public class UiManager : MonoBehaviour {

    public static UiManager Instance;

    public TouchDataState TouchDataState;
    public GameObject ArTouchPanel;

    [Header("Buttons")]
    public GameObject ToArModeButton;
    public GameObject RecenterCamOnUserButton;
    public GameObject TouchDataToggle;

    [Header("Touch Data Toggle Images")]
    public GameObject PoiDataIcon;
    public GameObject DistanceDataIcon;

    [Header("Scripts")]
    public TouchPointController TouchPointController;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
        TouchDataToggle.SetActive(false);
        ArTouchPanel.SetActive(false);
        ArTouchPanel.GetComponent<ArTouchController>().TouchPointController = TouchPointController;

        ToArModeButton.SetActive(false);
        RecenterCamOnUserButton.SetActive(false);
    }

    public void ArUiInitialize()
    {
        TouchDataToggle.SetActive(true);
        ArModeUi();
    }

    #region ButtonPressEvents
    public void ArModeUi()
    {
        MapManager.Instance.SetToMiniMap();  
        SetFullMapUi(false);
    }

    public void MapModeUi()
    {
        MapManager.Instance.SetToFullMap();
        SetFullMapUi(true);
    }

    public void OnRecenterCamOnUserClick()
    {
        MapManager.Instance.CenterFullMapCamOnPlayer();
    }

    public void ToggleTouchDataState()
    {
        if(TouchDataState == TouchDataState.Poi)
        {
            TouchDataState = TouchDataState.Distance;
            PoiDataIcon.SetActive(false);
            DistanceDataIcon.SetActive(true);
        }
        else
        {
            TouchDataState = TouchDataState.Poi;
            PoiDataIcon.SetActive(true);
            DistanceDataIcon.SetActive(false);
        }

        TouchPointController.TouchDataIndicator.Display();
    }
    #endregion

    private void SetFullMapUi(bool val)
    {
        if (SetupManager.Localized)
        {
            ArTouchPanel.SetActive(!val);
        }

        ToArModeButton.SetActive(val);
        RecenterCamOnUserButton.SetActive(val);

        if (SetupManager.Localized)
        {
            TouchDataToggle.SetActive(!val);
            TouchPointController.SetDataVisuals(!val);
        }
    }


}
