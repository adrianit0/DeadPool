using UnityEngine;
using System.Collections;

public class DpInput : MonoBehaviour {
    public float walkSpeed;
    public float jumpImpulse;

    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    private Rigidbody2D body;
    private Vector2 movement;

    private float horInput;
    private bool jumpInput;
    private bool inGround;
    private bool facingRight;

    private Animator anim;

    // ====================================
    void Start () {
        this.body = this.GetComponent<Rigidbody2D>();
        this.movement = new Vector2();
        this.inGround = false;
        this.facingRight = true;

        this.anim = this.GetComponent<Animator>();
	}

    // ====================================
    void Update () {
        this.horInput = Input.GetAxis("Horizontal");
        this.jumpInput = Input.GetKey(KeyCode.UpArrow);

        if((this.horInput < 0)&& (this.facingRight)){
            this.Flip();
            this.facingRight = false;
        }
        else if((this.horInput > 0) && (!this.facingRight))
        {
            this.Flip();
            this.facingRight = true;
        }

        this.anim.SetFloat("HorSpeed",Mathf.Abs(this.body.velocity.x));
        this.anim.SetFloat("VerSpeed", Mathf.Abs(this.body.velocity.y));

        if(Physics2D.OverlapCircle(this.groundCheckPoint.position, 0.40f, this.whatIsGround))
        {
            this.inGround = true;
        }else
        {
            this.inGround = false;

        }
	}
    // ====================================
    void FixedUpdate()
    {
        this.movement = this.body.velocity;

        this.movement.x = horInput * walkSpeed;
        if (this.jumpInput && this.inGround)
        {
            this.movement.y = jumpImpulse;
        }

        this.body.velocity = this.movement;
    }
    // ====================================
    void Flip()
    {
        Vector3 scale = this.transform.localScale;
        scale.x *= (-1);
        this.transform.localScale = scale;
    }
}
