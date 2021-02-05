// basic preset movement

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private BoxCollider2D boxcoll2d;
    private Rigidbody2D enemyRB;

    [SerializeField] private LayerMask platformLayer;

    private bool goLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        boxcoll2d = GetComponent<BoxCollider2D>();
        StartCoroutine("Patrol");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.left * Time.deltaTime * 5);
        nearLeftWall();
        nearRightWall();
    }

    void FixedUpdate()
    {

    }

    IEnumerator Patrol()
    {
        while(true)
        {
            if(goLeft)
                transform.Translate(Vector3.left * Time.deltaTime * 2);
            else
                transform.Translate(Vector3.right * Time.deltaTime * 2);

            if (nearLeftWall())
            {
                goLeft = false;
            }  

            if(nearRightWall())
            {
                goLeft = true;
            }

            yield return null;
        }
        
    }

    private bool nearLeftWall()
    {
        float extraWidth = 0.5f;
        RaycastHit2D raycastHit = Physics2D.Raycast(boxcoll2d.bounds.center - new Vector3(boxcoll2d.bounds.extents.x, 0), Vector3.left, extraWidth, platformLayer);

        Color rayColor;
        if (raycastHit.collider != null)
            rayColor = Color.green;
        else
            rayColor = Color.red;

        //Debug.DrawRay(boxcoll2d.bounds.center - new Vector3(boxcoll2d.bounds.extents.x, 0), new Vector3(-extraWidth, 0), rayColor);

        return raycastHit.collider != null;
    }

    private bool nearRightWall()
    {
        float extraWidth = 0.5f;
        RaycastHit2D raycastHit = Physics2D.Raycast(boxcoll2d.bounds.center + new Vector3(boxcoll2d.bounds.extents.x, 0), Vector3.right, extraWidth, platformLayer);

        Color rayColor;
        if (raycastHit.collider != null)
            rayColor = Color.green;
        else
            rayColor = Color.red;

        //Debug.DrawRay(boxcoll2d.bounds.center + new Vector3(boxcoll2d.bounds.extents.x, 0), new Vector3(extraWidth, 0), rayColor);

        return raycastHit.collider != null;
    }
}