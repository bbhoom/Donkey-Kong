using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;


    private new Rigidbody2D rigidbody;
    private Vector2 direction;
    public float speed = 1f;
    public float jump = 1f;
    private new Collider2D collider;
    private Collider2D[] results;
    private bool grounded;
    private bool climb;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        results = new Collider2D[4];
    }
    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f);
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void CheckCollision()
    {
        grounded = false;
        climb = false;
        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;

        // Use Physics2D.OverlapBoxAll to get the results directly
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, size, 0f);

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);

                Physics2D.IgnoreCollision(collider, hit, !grounded);
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Ladder"))
            {
                climb = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        if (climb)
        {
            direction.y = Input.GetAxis("Vertical") * speed;
        }

        else if (grounded && Input.GetButtonDown("Jump"))
        {
            direction = Vector2.up * jump;
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }
        direction.x = Input.GetAxis("Horizontal") * speed;

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }
        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero; //no update default chr
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f); //turns mario left
        }
    }
    //FixedUpdate is insync with physics and runs at an interval,reduces inconsistency in result
    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);//multiply by time to keep the movement constant
    }
    private void AnimateSprite()
    {
        if (climb)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex++;
            if (spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }
            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            enabled = false;
            FindObjectOfType<GameManager>().LevelComplete();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            enabled = false;
            FindObjectOfType<GameManager>().LevelFailed();
        }
    }
}
