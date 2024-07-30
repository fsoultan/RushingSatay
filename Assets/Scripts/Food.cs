using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    float spriteHeight;
    float spriteWidth;
    private SpriteRenderer spriteRenderer;
    Camera mainCamera;

    // Speed at which the sprite will move down
    private float moveSpeed;
    // Speed at which the sprite will rotate
    public float rotationSpeed = 30.0f;

    private PolygonCollider2D polygonCollider;

    private Rigidbody2D rb;


    float screenBottom;


    public bool stopMoving = true;

    private SpriteRenderer staticSprite;  // This is the sprite that remains the same


    bool satay;

    public SpriteRenderer SpriteRenderer { get => spriteRenderer; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get the main camera
        mainCamera = Camera.main;
        polygonCollider = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        screenBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;

    }
    // Start is called before the first frame update
    void Start()
    {
        SetRandomFood();
    }

    public void SetRandomFood()
    {
        rb.isKinematic = false;
        transform.parent = null;
        moveSpeed = GameManager.instance.foodMoveSpeed;
        satay = false;
        staticSprite = Stick.instance.SpriteRenderer;   
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.angularVelocity = 0;
        polygonCollider.isTrigger = false;
        StartCoroutine(DropDelay());
        RecalculatePolygonCollider();
        SetPositionAboveScreen();
        gameObject.SetActive(true);
    }

    private IEnumerator DropDelay()
    {
        stopMoving = true;
        yield return new WaitForSeconds(Random.Range(0,3));
        stopMoving = false;
    }

    private void SetPositionAboveScreen()
    {
        // Get screen dimensions in world units
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));
        Vector3 screenTopLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, mainCamera.nearClipPlane));


        // Calculate the height of the sprite
        spriteHeight = spriteRenderer.bounds.size.y;
        spriteWidth = spriteRenderer.bounds.size.x;


        // Calculate the new position above the screen
        Vector3 newPosition = new Vector3(
        GameManager.instance.IsLeft ? Random.Range(screenTopLeft.x + spriteWidth / 2, 0 - spriteWidth / 2) : Random.Range(0 + spriteWidth / 2, screenTopRight.x - spriteWidth /2), // Alternate between left and right side of the screen
            screenTopRight.y + spriteHeight / 2,      // Above the screen in Y-axis
            transform.position.z                      // Maintain the current Z-axis position
        );

        // Place your object at the new position
        transform.position = newPosition;

        // Flip the boolean to alternate sides in the next iteration
        GameManager.instance.IsLeft = !GameManager.instance.IsLeft;
    }

    private void RecalculatePolygonCollider()
    {
        spriteRenderer.sprite = GameManager.instance.sprites[Random.Range(0, GameManager.instance.sprites.Count)];

        polygonCollider.pathCount = spriteRenderer.sprite.GetPhysicsShapeCount();

        List<Vector2> path = new List<Vector2>();

        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            path.Clear();
            spriteRenderer.sprite.GetPhysicsShape(i, path);
            polygonCollider.SetPath(i, path.ToArray());
        }
    }

    // Update is called once per frame
    void Update()
    {
        IsBottomOfDynamicSpriteEnteringTopOfStaticSprite();
    }

    void IsBottomOfDynamicSpriteEnteringTopOfStaticSprite()
    {
        // Get the bounds of both sprites
        Bounds dynamicBounds = spriteRenderer.bounds;
        Bounds staticBounds = staticSprite.bounds;

        // Calculate the center of the top bound
        Vector2 topCenter = new Vector2(staticBounds.center.x, staticBounds.max.y);

        // Calculate the center of the bottom bound
        Vector2 bottomCenter = new Vector2(dynamicBounds.center.x, dynamicBounds.min.y);

        // Calculate the distance
        float distance = Vector2.Distance(topCenter, bottomCenter);
        float tolerance = 0.5f;

        if(distance < tolerance) {
            satay = true;
        }
        else
        {
            satay = false;
        }
    }

    private void FixedUpdate()
    {
        // Check if the sprite is below the screen
        if (transform.position.y + spriteHeight / 2 < screenBottom)
        {
            // Stop the sprite's movement
            if(rb != null)
            {
                rb.velocity = Vector2.zero;
                SetRandomFood();
            }
        }
        else
        {
            if (stopMoving || rb == null) return;
            // Move the Rigidbody2D down
            rb.velocity = new Vector2(0, -moveSpeed);

            // Rotate the Rigidbody2D
            rb.rotation += rotationSpeed * Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(Interest.instance == null) return;
        if (Interest.instance.GameOver) return;
        if (collision.transform.CompareTag("Stick"))
        {
            if (satay)
            {
                BecomeSatay();
            }
        }
    }

    private void BecomeSatay()
    {
        if (Stick.instance.Foods.Count >= Stick.instance.MaxFood) return;
        stopMoving = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.angularVelocity = 0;
        polygonCollider.isTrigger = true;
        Stick.instance.AddSatay(this);
    }
}
