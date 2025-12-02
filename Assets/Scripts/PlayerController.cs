using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float smoothMove;

    public const float leftLane1 = -4f;
    public const float leftLane2 = -8f;
    public const float centerLane = 0f;
    public const float rightLane1 = 4f;
    public const float rightLane2 = 8f;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    private Rigidbody rb;

    private bool rotateLeft = false;
    private bool rotateRight = false;
    [SerializeField] private float smoothRotationSpeed = 0.2f;
    [SerializeField] private float rotationDegY = 15f;
    [SerializeField] private float rotationDegZ = 5f;

    private float lrSign;

    [SerializeField] private float moveSpeed = 25f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        lrSign = 1;     // FIXED (not 0 anymore)
    }

    void Update()
    {
        SwipeLeftRight();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector3.forward * moveSpeed;   // FIXED
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

            float swipeDistX = endTouchPos.x - startTouchPos.x;

            if (Mathf.Abs(swipeDistX) < 30f) return;   // FIXED dead-zone

            if (swipeDistX < 0)
            {
                if (Mathf.Abs(transform.position.x - centerLane) < 0.1f)
                    MoveLeft(leftLane1);
                if (Mathf.Abs(transform.position.x - leftLane1) < 0.1f)
                    MoveLeft(leftLane2);

                if (Mathf.Abs(transform.position.x - rightLane2) < 0.1f)
                    MoveLeft(rightLane1);
                if (Mathf.Abs(transform.position.x - rightLane1) < 0.1f)
                    MoveLeft(centerLane);
            }

            if (swipeDistX > 0)
            {
                if (Mathf.Abs(transform.position.x - centerLane) < 0.1f)
                    MoveRight(rightLane1);
                if (Mathf.Abs(transform.position.x - rightLane1) < 0.1f)
                    MoveRight(rightLane2);

                if (Mathf.Abs(transform.position.x - leftLane2) < 0.1f)
                    MoveRight(leftLane1);
                if (Mathf.Abs(transform.position.x - leftLane1) < 0.1f)
                    MoveRight(centerLane);
            }
        }
    }

    void MoveLeft(float lane)
    {
        transform.DOMoveX(lane, smoothMove).OnStart(RotateLeft).OnComplete(RotateLeft);
    }

    void MoveRight(float lane)
    {
        transform.DOMoveX(lane, smoothMove).OnStart(RotateRight).OnComplete(RotateRight);
    }

    void RotateRight()
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

    void RotateLeft()
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
