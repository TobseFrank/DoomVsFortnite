using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    public Transform orientation;
    public Rigidbody rb;
    public PlayerMovement pm;
    public LayerMask whatIsWall;

    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallfront;

    private void Update() {
        WallCheck();
        StateMachine();

        if (climbing) {
            ClimbingMovement();
        }
    }

    private void StateMachine() {
        if (wallfront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle) {
            if (!climbing && climbTimer > 0) { 
            StartClimbing();
            }

            if (climbTimer > 0) {
                climbTimer -= Time.deltaTime;
            }

            if (climbTimer < 0) {
                StopClimbing();
            }
        } else {
            if (climbing) {
                StopClimbing();
            }
        }
    }

    private void WallCheck() {
        wallfront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if (pm.grounded) {
            climbTimer = maxClimbTime;
        }
    }

    private void StartClimbing() {
        climbing = true;
    }

    private void ClimbingMovement() {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing() {
        climbing = false;
    }
}
