using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Stick : MonoBehaviour
{
    private int maxFood = 3;
    public static Stick instance;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 offset;
    private Vector2 initialStickPosition;
    private bool isDragging = false;

    private float clampMinX;
    private float clampMaxX;

    [SerializeField] private List<Sprite> foods = new List<Sprite>();

    public SpriteRenderer SpriteRenderer { get => spriteRenderer;}
    public int MaxFood { get => maxFood; }
    public List<Sprite> Foods { get => foods; }

    private bool interactable = true;

    private List<Coroutine> coroutines = new List<Coroutine>();
    private void Awake()
    {

        instance = this;    
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Set the position of the GameObject to the bottom center of the screen
        initialStickPosition = new Vector3(0f, -Camera.main.orthographicSize + (spriteRenderer.bounds.size.y / 2f), 0f);
        transform.position = initialStickPosition;

        Camera mainCamera = Camera.main;

        // Get screen dimensions in world units
        Vector2 screenDimensions = new Vector2(mainCamera.aspect * mainCamera.orthographicSize * 2, mainCamera.orthographicSize * 2);

        // Get sprite dimensions in world units
        Vector2 spriteDimensions = spriteRenderer.bounds.size;

        // Calculate half-dimensions
        Vector2 halfScreenDimensions = screenDimensions / 2;
        Vector2 halfSpriteDimensions = spriteDimensions / 2;

        // Calculate the clamp values
        clampMinX = -halfScreenDimensions.x + halfSpriteDimensions.x;
        clampMaxX = halfScreenDimensions.x - halfSpriteDimensions.x;

        Debug.Log("Clamp Min X: " + clampMinX);
        Debug.Log("Clamp Max X: " + clampMaxX);

    }

    // Update is called once per frame
    void Update()
    {
        if (Interest.instance == null) return;
        if (!interactable) return;
        if (Interest.instance.GameOver) return;
        // Detect start of dragging (mouse or touch)
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !isDragging)
        {
            Vector2 inputPosition = GetTouchOrMouseWorldPosition();
            // Start dragging
            isDragging = true;
            offset = rb.position - inputPosition;
        }

        // Detect end of dragging (mouse or touch)
        if ((Input.GetMouseButtonUp(0) || Input.touchCount == 0) && isDragging)
        {
            isDragging = false;
        }

        // Handle dragging movement
        if (isDragging)
        {
            Vector2 inputPosition = GetTouchOrMouseWorldPosition();
            Vector2 newPosition = inputPosition + offset;

            // Clamp the new position within the specified horizontal bounds
            newPosition.x = Mathf.Clamp(newPosition.x, clampMinX, clampMaxX);


            // Update the Rigidbody2D position
            rb.MovePosition(newPosition);
        }
    }

    private Vector2 GetTouchOrMouseWorldPosition()
    {
        if (Input.touchCount > 0)
        {
            // Handle touch input
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = new Vector3(touch.position.x,transform.position.y,transform.position.z);
            touchPosition.z = Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(touchPosition);
        }
        else
        {
            // Handle mouse input
            Vector3 mousePosition = new Vector3(Input.mousePosition.x,transform.position.y,transform.position.z);
            mousePosition.z = Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    public void AddSatay(Food food)
    {
        coroutines.Add(StartCoroutine(AddingSatay(food)));
    }

    private IEnumerator AddingSatay(Food food)
    {
        foods.Add(food.SpriteRenderer.sprite);

        if(foods.Count > maxFood)
        {
            foods.RemoveAt(foods.Count - 1);
        }

        float speed = 2;

        Destroy(food.GetComponent<Rigidbody2D>());

        food.transform.SetParent(transform);


        // Get the bounds of the reference sprite
        Bounds bounds = spriteRenderer.bounds;
        // Calculate the bottom center position
        Vector3 bottomCenter = new Vector3(
            bounds.center.x,                  // X position of the center
            bounds.min.y,                     // Y position of the bottom (min y of bounds)
            bounds.center.z                   // Z position of the center (same as bounds.center.z)
        );

        // Get the size of the square
        float squareSize = food.GetComponent<SpriteRenderer>().bounds.size.y; // Adjust as needed

        Vector2 bottomCenterLocal = transform.InverseTransformPoint(bottomCenter);

        Vector2 targetPosition = bottomCenterLocal + new Vector2(0,0.5f + foods.Count - 1 * squareSize);
        SpriteRenderer foodSprite = food.GetComponent<SpriteRenderer>();
        foodSprite.sortingOrder = spriteRenderer.sortingOrder + 1 + foods.Count;

        // Calculate the distance to the target position
        while (Vector3.Distance(food.transform.localPosition, targetPosition) > 0.01f)
        {
            // Move the object a little closer to the target position each frame
            food.transform.localPosition = Vector3.MoveTowards(food.transform.localPosition, targetPosition, speed * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }

        // Ensure the object is exactly at the target position at the end
        food.transform.localPosition = targetPosition;

        coroutines.RemoveAt(coroutines.Count - 1);

        GameManager.instance.SpawnFood(1);


        if (foods.Count >= maxFood && coroutines.Count <= 0)
        {
            StartCoroutine(SubmitFood());
        }
    }

    private IEnumerator SubmitFood()
    {
        float speed = 7;

        interactable = false;

        // Calculate the x position outside the left edge of the screen
        float spriteWidth = foods[0].bounds.size.x;
        float screenLeftEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;

        // Calculate the desired x position completely outside the left edge
        float desiredX = screenLeftEdgeX - spriteWidth;

        // Set the object's position to the calculated x position
        Vector2 targetPosition = new Vector2(desiredX, transform.position.y);

        // Calculate the distance to the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // Move the object a little closer to the target position each frame
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }

        if(Interest.instance != null)
        {
            if (!Interest.instance.GameOver)
            {
                FoodInterest.instance.SubmitFood(new List<Sprite>(foods));
            }
        }

        transform.position = targetPosition;

        float screenRightEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        desiredX = screenRightEdgeX + spriteWidth;

        targetPosition = new Vector2(desiredX, transform.position.y);

        transform.position = targetPosition;

        foreach(Food food in GetComponentsInChildren<Food>())
        {
            Destroy(food.gameObject);
        }

        targetPosition = initialStickPosition;

        // Calculate the distance to the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // Move the object a little closer to the target position each frame
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }


        transform.position = targetPosition;

        interactable = true;

        foods.Clear();

    }

    public void ResetStick()
    {
        foreach (Food food in GetComponentsInChildren<Food>())
        {
            Destroy(food.gameObject);
        }

        interactable = true;

        foods.Clear();
    }
}
