using UnityEngine;

public class User : MonoBehaviour
{
    public static User Instance;

    public Transform MapUser;

    [HideInInspector]
    public Camera Camera;

    [SerializeField]
    private GameObject _visionCone;

    private Vector3 _rotation;

    private void Awake()
    {
        Instance = this;
        Camera = GetComponent<Camera>();

        MapUser.gameObject.SetActive(false);
        _visionCone.SetActive(false);  
    }

    void Start()
    {
        MapUser.gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        // Keep map-user upright
        _rotation = transform.rotation.eulerAngles;
        _rotation.x = 0;
        _rotation.z = 0;
        MapUser.rotation = Quaternion.Euler(_rotation);
    }

    public void ShowVisionCone()
    {
        _visionCone.SetActive(true);
    }

}
