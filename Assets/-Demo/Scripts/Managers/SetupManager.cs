using System.Collections;
using Sturfee.Unity.XR.Core.Events;
using UnityEngine;

// App starting point. Handles initial setup until first localization completes.
public class SetupManager : MonoBehaviour {     public static SetupManager Instance;      public static bool SessionReady;     public static bool Localized; 
    public GameObject ScanButton;
    public bool DistanceRelocalize;

    private void Awake()     {         Instance = this;     }      void Start()     {         SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;         SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        LocalizationManager.OnLocalizationScanStatusChanged += OnLocalizationScanStatusChanged;
        MapManager.Instance.MapboxMap.OnInitialized += OnMapboxInitialized;
    }
     void OnDestroy()     {         SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;         SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
        LocalizationManager.OnLocalizationScanStatusChanged -= OnLocalizationScanStatusChanged;
        MapManager.Instance.MapboxMap.OnInitialized -= OnMapboxInitialized;

        SessionReady = false;
        Localized = false;     } 
    public void PreLocalizedScanButtonSet(bool val)
    {
        if(!Localized)
        {
            ScanButton.SetActive(val);
        }
    }

    #region Events     void OnSessionReady()     {
        SessionReady = true;
        MapManager.Instance.InitializeMap();
    }      void OnLocalizationSuccessful()     {
        Localized = true;
        StartCoroutine(StartGame());     }

    private void OnLocalizationScanStatusChanged(ScanStatus scanStatus)
    {
        if (scanStatus == ScanStatus.Scanning || scanStatus == ScanStatus.Loading)
        {
            MapManager.Instance.MiniMapButton.interactable = false;
        }
        else if (scanStatus == ScanStatus.PreScan || scanStatus == ScanStatus.PostScan)
        {
            MapManager.Instance.MiniMapButton.interactable = true;
        }
    }

    private void OnMapboxInitialized()
    {
        StartCoroutine(FrameAfterMapboxInitialized());
    }
    #endregion

    // Waits a frame after Mapbox map initialization completes to safely perform necessary actions
    private IEnumerator FrameAfterMapboxInitialized()
    {
        yield return null;
        MapManager.Instance.AllowMapboxMiniMap();
    }

    private IEnumerator StartGame()
    {
        yield return null;

        SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;

        User.Instance.ShowVisionCone();
        UiManager.Instance.ArUiInitialize();

        if (DistanceRelocalize)
        {
            RelocalizationManager.Instance.Initialize();
        }
    } }