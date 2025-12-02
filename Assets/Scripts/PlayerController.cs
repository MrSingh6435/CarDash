using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float smoothMove = 0.15f;

    // lanes
    public const float leftLane1 = -4f;
    public const float leftLane2 = -8f;
    public const float centerLane = 0f;
    public const float rightLane1 = 4f;
    public const float rightLane2 = 8f;

    // touch input
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    // rigidbody
    private Rigidbody rb;

    // rotation settings
    private bool rotateLeft = false;
    private bool rotateRight = false;
    [SerializeField] private float smoothRotationSpeed = 0.2f;
    [SerializeField] private float rotationDegY = 15f;
    [SerializeField] private float rotationDegZ = 5f;
    private float lrSign;

    // Car move
    [SerializeField] private float moveSpeed = 25f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.centerOfMass = Vector3.zero;
            rb.interpolation = RigidbodyInterpolation.Interpolate; // smoother physics visuals
        }

        // lrSign used for flipping rotation depending on initial X.
        // If starting exactly at 0, default to -1 so rotation effect remains.
        float sign = Mathf.Sign(transform.position.x);
        if (Mathf.Approximately(sign, 0f))
            lrSign = -1f;
        else
            lrSign = -sign; // original intent was -Mathf.Sign(...)
    }

    void Update()
    {
        SwipeLeftRight();
    }

    void FixedUpdate()
    {
        // forward movement using Rigidbody (keeps physics consistent)
        if (rb != null)
        {
            rb.velocity = Vector3.forward * moveSpeed;
        }
        else
        {
            // fallback - move transform forward if no rb
            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    void SwipeLeftRight()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPos = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endTouchPos = Input.GetTouch(0).position;

            // left swipe
            if (endTouchPos.x < startTouchPos.x)
            {
                if (Mathf.Approximately(transform.position.x, centerLane))
                {
                    MoveLeft(leftLane1);
                }
                else if (Mathf.Approximately(transform.position.x, leftLane1))
                {
                    MoveLeft(leftLane2);
                }
                else if (Mathf.Approximately(transform.position.x, rightLane2))
                {
                    MoveLeft(rightLane1);
                }
                else if (Mathf.Approximately(transform.position.x, rightLane1))
                {
                    MoveLeft(centerLane);
                }
            }

            // right swipe
            if (endTouchPos.x > startTouchPos.x)
            {
                if (Mathf.Approximately(transform.position.x, centerLane))
                {
                    MoveRight(rightLane1);
                }
                else if (Mathf.Approximately(transform.position.x, rightLane1))
                {
                    MoveRight(rightLane2);
                }
                else if (Mathf.Approximately(transform.position.x, leftLane2))
                {
                    MoveRight(leftLane1);
                }
                else if (Mathf.Approximately(transform.position.x, leftLane1))
                {
                    MoveRight(centerLane);
                }
            }
        }
    }

    void MoveLeft(float lane)
    {
        // using transform DOMoveX as you originally had.
        // If you want physics-correct movement (so collisions work while lane-changing),
        // use rb.DOMove(new Vector3(lane, transform.position.y, transform.position.z), smoothMove)
        transform.DOMoveX(lane, smoothMove).OnStart(RotateLeft).OnComplete(RotateLeft);
    }

    void MoveRight(float lane)
    {
        transform.DOMoveX(lane, smoothMove).OnStart(RotateRight).OnComplete(RotateRight);
    }

    void RotateLeft()
    {
        if (rotateLeft)
        {
            transform.DORotate(new Vector3(0, 0, 0), smoothRotationSpeed);
            rotateLeft = false;
        }
        else
        {
            transform.DORotate(new Vector3(0, rotationDegY * lrSign, rotationDegZ * lrSign), smoothRotationSpeed);
            rotateLeft = true;
        }
    }

    void RotateRight()
    {
        if (rotateRight)
        {
            transform.DORotate(new Vector3(0, 0, 0), smoothRotationSpeed);
            rotateRight = false;
        }
        else
        {
            transform.DORotate(new Vector3(0, -rotationDegY * lrSign, -rotationDegZ * lrSign), smoothRotationSpeed);
            rotateRight = true;
        }
    }
}
