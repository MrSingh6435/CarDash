using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Camera mainCam;
    private Vector3 ogPos;
    private Vector3 velocity;

    [SerializeField] private float smoothSpeed;
    void Start()
    {
        mainCam = Camera.main;
        ogPos = transform.position - player.position;
    }

    void LateUpdate()
    {
        Vector3 pos = player.position + ogPos;
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothSpeed);
    }
}
