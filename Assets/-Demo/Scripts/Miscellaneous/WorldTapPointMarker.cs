using Sturfee.Unity.XR.Core.Session;
using UnityEngine;

public class WorldTapPointMarker : MonoBehaviour {

    public Transform GridMarker;
    public Transform MapMarker2D;

	void Start () {
        MapMarker2D.gameObject.SetActive(true);
	}

    private void LateUpdate()
    {
        if (MapManager.MapMode == MapMode.Mini)
        {
            Vector3 forwardXZ = User.Instance.transform.forward;
            forwardXZ.y = 0;
            MapMarker2D.forward = forwardXZ;
        }
        else
        {
            MapMarker2D.rotation = Quaternion.identity;
        }
    }

    public void Set(Vector3 pos, Vector3 normal)
    {
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(normal);

        SetGrid(normal);
        MapMarker2D.rotation = Quaternion.Euler(Vector3.up);
    }

    private void SetGrid(Vector3 normal)
    {
        // Copied from GridEffect script
        Vector3 camPos = (XRSessionManager.GetSession() != null) ? XRSessionManager.GetSession().GetXRCameraPosition() : Camera.main.transform.position;
        var distance = Vector3.Distance(camPos, transform.position);
        var scaleAmount = (distance * 2) / Mathf.Pow(distance, 0.75f);
        GridMarker.localScale = Vector3.one * scaleAmount;
    }

}
