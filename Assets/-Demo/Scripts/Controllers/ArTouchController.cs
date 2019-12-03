using System.Collections;
using Sturfee.Unity.XR.Core.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArTouchController : MonoBehaviour, IPointerDownHandler {

    public TouchPointController TouchPointController;

    private LayerMask _interactLayerMask;

    private bool _waitingOnEventCall;     // Prevents taps on touch panel until Foursquare/Sturfee call is completed
    private Vector2 _savedTouchPos;
    private bool _savedTouch;

    void Start() {

        _interactLayerMask |= 1 << DemoConstants.SturfeeTerrainLayer;
        _interactLayerMask |= 1 << DemoConstants.SturfeeBuildingLayer;

        FoursquareService.OnVenuesCallStarted += OnVenuesCallStarted;
        FoursquareService.OnVenuesReceived += OnVenuesReceived;
        FoursquareService.OnVenuesCallFailed += OnVenuesFailed;

        if (SetupManager.Instance.DistanceRelocalize)
        {
            SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;
            SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
            SturfeeEventManager.Instance.OnLocalizationFail += OnLocalizationFail;
        }
    }

    void OnDestroy()
    {
        SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;
        SturfeeEventManager.Instance.OnLocalizationSuccessful-= OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail -= OnLocalizationFail;

        if (SetupManager.Instance.DistanceRelocalize)
        {
            FoursquareService.OnVenuesCallStarted -= OnVenuesCallStarted;
            FoursquareService.OnVenuesReceived -= OnVenuesReceived;
            FoursquareService.OnVenuesCallFailed -= OnVenuesFailed;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_waitingOnEventCall)
        {
            ScreenPosRaycast(eventData.pressPosition);
        }
    }

    private void ScreenPosRaycast(Vector2 touchPos)
    {
        Ray ray = User.Instance.Camera.ScreenPointToRay(touchPos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, _interactLayerMask))
        {
            // Relocalization is either off or requirements were not met
            if (!RelocalizationManager.Instance.RelocalizeOnDistanceExceeded())
            {
                TouchPointController.Set(hit);
            }
            else  // Relocalization has started
            {
                // save the touch position so it can be called automatically when relocalization completes
                _waitingOnEventCall = true;
                _savedTouchPos = touchPos;
                _savedTouch = true;
                TouchPointController.ResetDataVisuals();
            }
        }
    }
    #region FoursquareAPIEvents
    private void OnVenuesCallStarted()
    {
        _waitingOnEventCall = true;
    }

    private void OnVenuesReceived(FoursquareAPI.Venue[] sortedVenues)
    {
        _waitingOnEventCall = false;
    }

    private void OnVenuesFailed()
    {
        _waitingOnEventCall = false;
    }
    #endregion

    #region SturfeeRelocalizationEvents
    private void OnLocalizationLoading()
    {
        _waitingOnEventCall = true;
    }

    private void OnLocalizationSuccessful()
    {
        StartCoroutine(CallTouchedSave());
    }

    private void OnLocalizationFail(Sturfee.Unity.XR.Core.Localization.ErrorType errorType, string error)
    {
        StartCoroutine(CallTouchedSave());
    }
    #endregion

    private IEnumerator CallTouchedSave()
    {
        if (_savedTouch)
        {
            _savedTouch = false;

            yield return null;

            ScreenPosRaycast(_savedTouchPos);
        }
        _waitingOnEventCall = false;
    }

}
