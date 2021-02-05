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


    [SerializeField] private LayerMask platformLayer; 
    private BoxCollider2D boxcoll2d;

    private GameManager gmScript;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxcoll2d = GetComponent<BoxCollider2D>();

        gmScript = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        //transform.Translate(Vector3.right * Time.deltaTime * horizontalInput * speed);
        playerRB.velocity = new Vector2(horizontalInput * speed, playerRB.velocity.y);

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
}
