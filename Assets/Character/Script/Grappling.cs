using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    public float grapplingCd;
    private float grapplingCdTimer;

    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start() {
        pm = GetComponent<PlayerMovement>();
    }

    private void Update() {
        if (Input.GetKeyDown(grappleKey)) {
            StartGrapple();
        }

        if (grapplingCdTimer > 0) {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate() {
        lr.SetPosition(0,gunTip.position);
    }

    private void StartGrapple() {
        if (grapplingCdTimer > 0) {
            return;
        }

        grappling = true;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable)) {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        } else {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1,grapplePoint);
    }
    private void ExecuteGrapple() {
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y -1f, transform.position.z);

        float grapplePOintRealtiveYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePOintRealtiveYPos + overshootYAxis;

        if (grapplePOintRealtiveYPos < 0) {
            highestPointOnArc = overshootYAxis;
        }

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }


    public void StopGrapple() {
        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
}
