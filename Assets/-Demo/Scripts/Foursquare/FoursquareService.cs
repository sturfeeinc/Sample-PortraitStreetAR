using System.Collections;
using System.Collections.Generic;
using Sturfee.Unity.XR.Core.Models.Location;
using UnityEngine;
using UnityEngine.Networking;

public class FoursquareService : MonoBehaviour {

    public delegate void VenuesCallStarted();
    public static event VenuesCallStarted OnVenuesCallStarted;
    public delegate void VenuesReceived(FoursquareAPI.Venue[] sortedVenues);
    public static event VenuesReceived OnVenuesReceived;
    public delegate void VenuesCallFailed();
    public static event VenuesCallFailed OnVenuesCallFailed;

    // Used in URL call
    private string _clientId = "";
    private string _clientSecret = "";
    private string _oauthToken = "";
    private int _radius = 30;

    private string _urlTail;

    private void Start()
    {
        if(!string.IsNullOrEmpty(_oauthToken))
        {
            _urlTail = "&oauth_token=" + _oauthToken + "&v=20160419&intent=browse";
        }
        else
        {
            _urlTail = "&client_id=" + _clientId + "&client_secret=" + _clientSecret + "&v=20160419&intent=browse";
        }

    }

    public void GetSortedVenues(GpsPosition gpsPos)
    {
        StartCoroutine(GetVenueData(gpsPos));
    }

    private IEnumerator GetVenueData(GpsPosition gpsPos)
    {
        OnVenuesCallStarted();

        string url = "https://api.foursquare.com/v2/venues/search?ll=" + gpsPos.Latitude.ToString() + "," + gpsPos.Longitude.ToString()
        + "&radius=" + _radius.ToString() + _urlTail;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
                OnVenuesCallFailed();
            }
            else
            {
                var d = JsonUtility.FromJson<FoursquareAPI.RootObject>(www.downloadHandler.text);
                List<FoursquareAPI.Venue> venues = d.response.venues;
                OnVenuesReceived(GetSortedArrayFromData(venues));
            }
        }
    }

    private FoursquareAPI.Venue[] GetSortedArrayFromData(List<FoursquareAPI.Venue> venues)
    {
        int maxVenues = (venues.Count >= 5) ? 5 : venues.Count;
        List<int> sortedDistOfVenues = new List<int>();

        // Make a copy of the venues, just holding the distances
        for (int i = 0; i < venues.Count; i++)
        {
            sortedDistOfVenues.Add(venues[i].location.distance);
        }
        sortedDistOfVenues.Sort();

        // Find the corresponding venue by the distance found in the sorted distance of venues array
        FoursquareAPI.Venue[] sortedVenues = new FoursquareAPI.Venue[maxVenues];
        for (int i = 0; i < maxVenues; i++)
        {
            int count = 0;
            bool foundVenue = false;
            while (!foundVenue && count < venues.Count)
            {
                if (sortedDistOfVenues[i] == venues[count].location.distance)
                {
                    sortedVenues[i] = venues[count];
                    foundVenue = true;

                    venues.RemoveAt(count);
                }
                count++;
            }
        }
        return sortedVenues;
    }

}
