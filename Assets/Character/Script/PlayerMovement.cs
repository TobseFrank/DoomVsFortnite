using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject projectile;
    private bool alreadyAttacked;
    public GameObject gunTip;
    public float moveSpeed;
    public float groundDrag;
    int jumpCount = 2;
    public float jumpForce;
    public float jumpCooldown;
    public float dashForce;
    public float airMultiplier;
    bool readyToJump;
    bool activeGrapple;

    public KeyCode jumpKey = KeyCode.Space;
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    private bool enableMovementOnNextTouch;
    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    private Vector3 velocityToSet;



    private void Start() {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update() {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        if (grounded && !activeGrapple) {
            rb.drag = groundDrag;
        } else {
            rb.drag = 0;
        }
    }

    private void FixedUpdate() {
        MovePlayer();
    }

    private void MyInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Mouse0)){
            Shoot();
        }


        if(Input.GetKeyDown(jumpKey) && readyToJump && (grounded || jumpCount > 0)) {
            if (jumpCount < 1){
                readyToJump = false;
            } 
            jumpCount--;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) {
            Dash();
        }
    }

    private void MovePlayer(){
        if (activeGrapple) {
            return;
        }

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (grounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

    }

    private void SpeedControl(){
        if (activeGrapple) {
            return;
        }

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed) {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,Vector3.down *2 ,out hit, 1f)) {
            if (hit.transform.tag == "Trampo") {
                rb.velocity = new Vector3(rb.velocity.x,0f, rb.velocity.z);
                rb.AddForce(transform.up * (jumpForce * 2), ForceMode.Impulse);
                return;
            }
        }

        rb.velocity = new Vector3(rb.velocity.x,0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump(){
        readyToJump = true;
        jumpCount = 2;
    }

    private void Dash() {
        rb.AddForce(orientation.forward * dashForce + orientation.up * 1, ForceMode.Impulse);
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight) {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

        Invoke(nameof(SetVelocity),0.1f);

        Invoke(nameof(ResetRestriction),3f);
    }

    private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight) {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;

        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x,0f, endPoint.z-startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));
        
        return velocityY + velocityXZ;
    }

    
    private void SetVelocity() {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestriction(){
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (enableMovementOnNextTouch){
            enableMovementOnNextTouch = false;
            ResetRestriction();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    private void Shoot(){
        if(!alreadyAttacked) {
            Rigidbody rb = Instantiate(projectile, gunTip.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(orientation.forward *32f, ForceMode.Impulse);
            rb.AddForce(orientation.up *3f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack),0.5f);
        }
    }

    private void ResetAttack(){
        alreadyAttacked = false;
    }
}
