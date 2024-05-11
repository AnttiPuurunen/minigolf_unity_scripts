using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public static int strokes = 0;
    private float barMultiplier = 2f;

    private bool barCoroutineRunning = false;

    private InputActions input;

    private Rigidbody rb;
    private readonly float forceMultiplier = 10f;

    private LineRenderer lr;
    private Vector3[] positions;
    private Vector3 forward;

    [SerializeField] private Slider _slider;

    public TextMeshProUGUI score;
    public static Vector3 playerPosition;
    public TextMeshProUGUI jumpPickedUpText;
    public TextMeshProUGUI tutorialText;

    private bool hasPowerUp = false;

    private void Awake()
    {
        input = new InputActions();
    }

    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        // Set up the linerenderer to draw the line
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Color colorBlue = Color.blue;
        colorBlue.a = 0.5f;
        lr.startColor = colorBlue;
        lr.endColor = colorBlue;
        positions = new Vector3[2];

        // Register the callbacks to hit the ball, first starting the charge, then letting go of the key
        input.Player.HitBall.started += ChargeStarted;
        input.Player.HitBall.canceled += ChargeCanceled;

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(ShowTutorial());
        }
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }  

    void Update()
    {  
        if (rb.velocity.magnitude == 0)
        {
            // Get the camera's forward vector
            forward = Camera.main.transform.forward;

            // Save the player's position when on the course and standing still
            playerPosition = rb.position;

            // Draw a vector parallel to the ground
            var planeLine = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;

            var startPoint = rb.position;
            var endPoint = startPoint + planeLine * 3f;

            positions[0] = startPoint;
            positions[1] = endPoint;

            // Set the vector positions to the LineRenderer to draw line
            lr.positionCount = positions.Length;
            lr.SetPositions(positions);    
        } else
        {
            // Don't draw the line when the ball is moving
            // TODO: Find a better way of doing this?
            lr.positionCount = 0;
        }
        
        if (hasPowerUp & rb.velocity.magnitude > 0 & Input.GetKeyDown(KeyCode.LeftShift)) 
        {    
            rb.AddForce(0f, 2f, 0f, ForceMode.Impulse);
            hasPowerUp = false;
            jumpPickedUpText.gameObject.SetActive(false);
        }
    }
 
    void ChargeStarted(InputAction.CallbackContext context)
    {
        // Check that coroutine runs only once, called when the player presses down the shoot key
        if (!MenuScript.isPaused & barCoroutineRunning == false & rb.velocity.magnitude == 0)
        {
            barCoroutineRunning = true;
            StartCoroutine(PowerBarChargeRoutine());
        }
    }

    void ChargeCanceled(InputAction.CallbackContext context)
    {
        // Called when the player releases the shoot key
        if (!MenuScript.isPaused & rb.velocity.magnitude == 0)
        {
            rb.AddForce(forward * (_slider.value * forceMultiplier), ForceMode.Impulse);
            barCoroutineRunning = false;
            strokes++;
            score.text = "Strokes: " + strokes;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // When the ball goes off the course and hits the ground outside the course
        if (collision.gameObject.CompareTag("OutOfBounds"))
        {
            TeleportPlayer(rb);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            other.gameObject.SetActive(false);
            StartCoroutine(ShowTutorial());
        }
    }

    private void TeleportPlayer(Rigidbody player)
    {
        // Teleport the player back to the course
        player.position = playerPosition;
        // Set the velocity of the ball to zero, so it doesn't keep its momentum after being teleported
        player.velocity = Vector3.zero;
        player.angularVelocity = Vector3.zero;
    }

    IEnumerator PowerBarChargeRoutine()
    {
        // Increase the slider value based on time spent pressing down the shoot key
        while (barCoroutineRunning)
        {
            _slider.value += Time.deltaTime * barMultiplier;
            yield return null;
        }
        // Decrease slider value when releasing the key
        while (barCoroutineRunning == false)
        {
            _slider.value -= Time.deltaTime * barMultiplier;
            yield return null;
        }
    }

    IEnumerator ShowTutorial()
    {
        // Show and hide tutorial text
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            tutorialText.text = "Look around using mouse";
            yield return new WaitForSeconds(3);

            tutorialText.text = "Hold down space to charge powerbar and release to shoot";
            yield return new WaitForSeconds(3);

            tutorialText.text = "Press escape for pause menu";
            yield return new WaitForSeconds(3);

            tutorialText.gameObject.SetActive(false);
        }
        else if (hasPowerUp & SceneManager.GetActiveScene().buildIndex == 3) {
            // Show tutorial when picking up powerup
            jumpPickedUpText.gameObject.SetActive(true);
            tutorialText.gameObject.SetActive(true);
            tutorialText.text = "While moving, press left shift to jump";
            yield return new WaitForSeconds(3);
            tutorialText.gameObject.SetActive(false);
        }
        else
        {
            yield return null;
        }      
    }
}

