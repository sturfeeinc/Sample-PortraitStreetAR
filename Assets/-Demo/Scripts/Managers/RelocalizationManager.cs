using System.Collections;
using Sturfee.Unity.XR.Core.Events;
using UnityEngine;
using Sturfee.Unity.XR.Core.Session;
using System;
using Sturfee.Unity.XR.Core.Exceptions;
using Sturfee.Unity.XR.Package.Utilities;

// Handles relocalization
// Must be enabled by Initialize function, so as not to get confused with initial localization
public class RelocalizationManager : MonoBehaviour {

    public static RelocalizationManager Instance;

    public float RelocDist = 15;

    private float _minTimeBtwnReloc = 2;
    private float _distSinceLastReloc = 0;
    private Vector3 _lastTrackedPoint;          // Last point that was checked for relocalization to occur
    private float _lastRelocTime;
    private bool _relocalizing = false;

    private bool _relocalizationEnabled;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail -= OnLocalizationFail;
    }

    public void Initialize()
    {
        SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail += OnLocalizationFail;

        _relocalizationEnabled = true;
        _distSinceLastReloc = 0;
        _lastTrackedPoint = XRSessionManager.GetSession().GetXRCameraPosition();
        _lastRelocTime = Time.time;
    }

    // Checks if relocalization should be performed and does so if true
    public bool RelocalizeOnDistanceExceeded()
    {
        if(!_relocalizationEnabled)
        {
            return false;
        }

        if ((Time.time - _lastRelocTime) > _minTimeBtwnReloc && !_relocalizing)
        {
            float dist = Vector3.Distance(_lastTrackedPoint, XRSessionManager.GetSession().GetXRCameraPosition());
            _distSinceLastReloc += dist;
            _lastTrackedPoint = XRSessionManager.GetSession().GetXRCameraPosition();

            if (_distSinceLastReloc >= RelocDist)
            {
                PerformRelocalization();
            }
        }
        return _relocalizing;
    }

    public void PerformRelocalization()
    {
        if (!_relocalizationEnabled)
        {
            Debug.LogError("Relocalization not enabled");
            return;
        }

        try
        {
            _relocalizing = true;
            _lastRelocTime = Time.time;
            XRSessionManager.GetSession().PerformLocalization();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);

            if (ex is PitchRequestException)
            {
                _relocalizing = false;
            }
            else if (ex is RollRequestException)
            {
                _relocalizing = false;
            }
        }
    }

    #region SturfeeEvents
    private void OnLocalizationLoading()
    {
        _relocalizing = true;
    }

    private void OnLocalizationSuccessful()
    {
        StartCoroutine(WaitAFrameAfterLocalizationStatus(true));
    }

    void OnLocalizationFail(Sturfee.Unity.XR.Core.Localization.ErrorType errorType, string error)
    {
        StartCoroutine(WaitAFrameAfterLocalizationStatus(false));
    }
    #endregion

    //private bool RelocalizationEnabledCheck()
    //{
    //    if(!_relocalizationEnabled)
    //    {
    //        Debug.LogError("Relocalization not enabled");
    //    }
    //    return _relocalizationEnabled;
    //}

    private IEnumerator WaitAFrameAfterLocalizationStatus(bool val)
    {
        yield return null;
        _relocalizing = false;

        if (val)
        {
            _distSinceLastReloc = 0;
            _lastTrackedPoint = XRSessionManager.GetSession().GetXRCameraPosition();
            //print("Relocalize Success");
        }
        else
        {
            //print("Relocalize Failed");
            ToastManager.Instance.ShowToastTimed("Relocalization Failed", 2);
        }
    }

}
