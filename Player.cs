using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float horizontalInput;
    [SerializeField] private float speed;

    private Rigidbody2D playerRB;

    [SerializeField] private float jumpDelayTimer; // check if right before hitting ground (allow jump)
    private float jumpDelay = 0;
    [SerializeField] private float jumpDelayTimer2; // check if grounded recently (allow jump)
    private float jumpDelay2 = 0;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float jumpVelocityCut;

    private bool wallSliding = false;
    [SerializeField] private float wallSlidingSpeed;
    private bool wallJumping;
    [SerializeField] private float xWallForce;
    [SerializeField] private float yWallForce;
    [SerializeField] private float wallJumpTime;

    [SerializeField] private LayerMask platformLayer; 
    private BoxCollider2D boxcoll2d;

    private Animator animator;

    private GameManager gmScript;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxcoll2d = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        gmScript = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        //transform.Translate(Vector3.right * Time.deltaTime * horizontalInput * speed);
        
        if(IsGrounded())
        {
            playerRB.velocity = new Vector2(horizontalInput * speed, playerRB.velocity.y);
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput * speed));

        jumpDelay -= Time.deltaTime;
        jumpDelay2 -= Time.deltaTime;

        if (Input.GetKeyDown("space"))
        {
            //Debug.Log("Hello");
            jumpDelay = jumpDelayTimer;
        }

        if (IsGrounded())
        {
            jumpDelay2 = jumpDelayTimer2;
        }

        

        if (jumpDelay > 0 && jumpDelay2 > 0)
        {
            //Debug.Log("Jump!");
            jumpDelay = 0;
            jumpDelay2 = 0;
            //playerRB.AddForce(transform.up * thrust, ForceMode2D.Impulse);
            playerRB.velocity = new Vector2(playerRB.velocity.x, jumpVelocity);
        }

        if (Input.GetKeyUp("space")) // short and long jumps
        {
            if(playerRB.velocity.y > 0)
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y * jumpVelocityCut);
            }
        }

        if (nearLeftWall() && !IsGrounded() && (Input.GetKey("left") || Input.GetKey("a")))
        {
            wallSliding = true;
            //StartCoroutine("TopRightJump");
            //playerRB.AddForce(new Vector2(10, 10), ForceMode2D.Impulse);
            //playerRB.velocity = new Vector2(20, jumpVelocity);
        }

        if (nearRightWall() && !IsGrounded() && (Input.GetKey("right") || Input.GetKey("d")))
        {
            wallSliding = true;
            //playerRB.velocity = new Vector2(-20, jumpVelocity);
            //playerRB.AddForce(transform.up * 30, ForceMode2D.Impulse);
            //playerRB.AddForce(transform.right * -30, ForceMode2D.Impulse);
        }

        if (wallSliding)
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, Mathf.Clamp(playerRB.velocity.y, -wallSlidingSpeed, float.MaxValue));

            // wall jump
            if(Input.GetKeyDown("space"))
            {
                wallJumping = true;
                Invoke("notWallJumping", wallJumpTime);
            }

        }

        if (wallJumping)
        {
            if(nearRightWall())
                playerRB.velocity = new Vector2(-xWallForce, yWallForce);
            else if(nearLeftWall())
                playerRB.velocity = new Vector2(xWallForce, yWallForce);

            //playerRB.AddForce(new Vector2(-xWallForce, yWallForce), ForceMode2D.Force);
        }

        if (!nearLeftWall() || !nearRightWall() || IsGrounded())
        {
            wallSliding = false;
        }
    }

    private void notWallJumping()
    {
        wallJumping = false;
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.2f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxcoll2d.bounds.center, boxcoll2d.bounds.size, 0f, Vector3.down, extraHeight, platformLayer);

        //Color rayColor;
        //if (raycastHit.collider != null)
        //    rayColor = Color.green;
        //else
        //    rayColor = Color.red;

        //Debug.DrawRay(boxcoll2d.bounds.center + new Vector3(boxcoll2d.bounds.extents.x, 0), Vector2.down * (boxcoll2d.bounds.extents.y + extraHeight), rayColor);
        //Debug.DrawRay(boxcoll2d.bounds.center - new Vector3(boxcoll2d.bounds.extents.x, 0), Vector2.down * (boxcoll2d.bounds.extents.y + extraHeight), rayColor);
        //Debug.DrawRay(boxcoll2d.bounds.center - new Vector3(boxcoll2d.bounds.extents.x, boxcoll2d.bounds.extents.y + extraHeight), Vector2.right * (2 * boxcoll2d.bounds.extents.x), rayColor);

        // Debug.Log(raycastHit.collider);

        return raycastHit.collider != null;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Coin")
        {
            Destroy(col.gameObject);
            gmScript.score += 1;
            //Debug.Log("Chaching!");
        }

        if (col.gameObject.tag == "Exit")
        {
            GameObject cam = GameObject.Find("Main Camera");
            cam.transform.SetParent(null);
            Debug.Log("Victory!");
            Destroy(gameObject);
        }

        if (col.gameObject.tag == "Enemy")
        {
            GameObject cam = GameObject.Find("Main Camera");
            cam.transform.SetParent(null);
            Debug.Log("Defeat!");
            //Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }


    private bool nearLeftWall()
    {
        float extraWidth = 0.1f;
        RaycastHit2D raycastHit = Physics2D.Raycast(boxcoll2d.bounds.center - new Vector3(boxcoll2d.bounds.extents.x, 0), Vector3.left, extraWidth, platformLayer);

        Color rayColor;
        if (raycastHit.collider != null)
            rayColor = Color.green;
        else
            rayColor = Color.red;

        Debug.DrawRay(boxcoll2d.bounds.center - new Vector3(boxcoll2d.bounds.extents.x, 0), new Vector3(-extraWidth, 0), rayColor);

        return raycastHit.collider != null;
    }

    private bool nearRightWall()
    {
        float extraWidth = 0.1f;
        RaycastHit2D raycastHit = Physics2D.Raycast(boxcoll2d.bounds.center + new Vector3(boxcoll2d.bounds.extents.x, 0), Vector3.right, extraWidth, platformLayer);

        Color rayColor;
        if (raycastHit.collider != null)
            rayColor = Color.green;
        else
            rayColor = Color.red;

        Debug.DrawRay(boxcoll2d.bounds.center + new Vector3(boxcoll2d.bounds.extents.x, 0), new Vector3(extraWidth, 0), rayColor);

        return raycastHit.collider != null;
    }
}
