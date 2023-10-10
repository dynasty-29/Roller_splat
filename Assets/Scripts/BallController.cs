using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;
    public bool isTravelling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;
    public int minSwipeRecognition = 200;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;
    private Color solveColor;
    public AudioSource audioSource;
    public AudioClip rollingSound;
    public AudioClip collisionSound;

    public GameObject ballTrailPrefab;
    public GameObject collisionEffectPrefab;

    private GameObject currentTrail;

    private void Start()
    {
        // Ensure Rigidbody is referenced
        rb = GetComponent<Rigidbody>();

        currentTrail = Instantiate(ballTrailPrefab, transform.position, Quaternion.identity);
        currentTrail.transform.parent = transform;
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
    }

    private void Update()
    {
        HandleKeyboardInput();
        HandleMouseSwipe();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) MoveBall(Vector3.forward);
        if (Input.GetKeyDown(KeyCode.S)) MoveBall(Vector3.back);
        if (Input.GetKeyDown(KeyCode.A)) MoveBall(Vector3.left);
        if (Input.GetKeyDown(KeyCode.D)) MoveBall(Vector3.right);
    }

    private void HandleMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipePosLastFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

            if (currentSwipe.sqrMagnitude < minSwipeRecognition) return;

            swipePosLastFrame = swipePosCurrentFrame; // Update last frame here
            currentSwipe.Normalize();

            if (Mathf.Abs(currentSwipe.x) > Mathf.Abs(currentSwipe.y))
            {
                setDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
            }
            else
            {
                setDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
            }
        }
    }

    private void MoveBall(Vector3 direction)
    {
        rb.AddForce(direction * speed, ForceMode.VelocityChange);
    }

    private void setDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }
        isTravelling = true;
    }

    private void StopBallMovement()
    {
        isTravelling = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (isTravelling)
        {
            rb.AddForce(travelDirection * speed, ForceMode.VelocityChange);
        }
        //
        if (isTravelling && !audioSource.isPlaying)
        {
            audioSource.clip = rollingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (!isTravelling)
        {
            audioSource.Stop();
        }
        

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
        }

        if (nextCollisionPosition != Vector3.zero && Vector3.Distance(transform.position, nextCollisionPosition) < 1)
        {
            StopBallMovement();
            travelDirection = Vector3.zero;
            nextCollisionPosition = Vector3.zero;
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(collisionEffectPrefab, transform.position, Quaternion.identity);

        if (collision.gameObject.CompareTag("WallPiece"))
        {
            StopBallMovement();
        }

        if (collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }
        else
        {
            Debug.LogWarning("Collision Sound is not assigned in BallController!");
        }
    }

}
