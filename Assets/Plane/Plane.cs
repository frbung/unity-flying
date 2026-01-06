using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public float moveSpeed = 30f;

    public float yawSpeed = 90f;

    public float pitchSpeed = 60f;

    public float maxRollAngle = 45f; // how far the plane banks
    
    public float rollSpeed = 3f; // how quickly it banks

    public float autoLevelStrength = 2f; // higher = stronger auto-level

    public float boundary = 20f;

    private Rigidbody mRb;


    // Start is called before the first frame update
    void Start()
    {
        mRb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        var pitchInput = Input.GetAxis("Vertical");
        var yawInput = Input.GetAxis("Horizontal");

        // Yaw (left/right)
        var desiredYaw = transform.eulerAngles.y + yawInput * yawSpeed * Time.fixedDeltaTime;

        // Pitch (nose up/down)
        var desiredPitch = transform.eulerAngles.x - pitchInput * pitchSpeed * Time.fixedDeltaTime;
        if (desiredPitch > 90 && desiredPitch <= 180)
            desiredPitch = 90;
        else if (desiredPitch > 180 && desiredPitch <= 271)
            desiredPitch = 271;

        // Roll (banking)
        var targetRoll = -yawInput * maxRollAngle;
        var desiredRoll = Mathf.LerpAngle(transform.eulerAngles.z, targetRoll, rollSpeed * Time.fixedDeltaTime);

        if (Mathf.Abs(pitchInput) < 0.01f)
            desiredPitch = Mathf.LerpAngle(desiredPitch, 0f, autoLevelStrength * Time.fixedDeltaTime);

        var finalRot = Quaternion.Euler(desiredPitch, desiredYaw, desiredRoll);
        mRb.MoveRotation(finalRot);
        mRb.angularVelocity = Vector3.zero;

        var movement = transform.forward * moveSpeed * Time.fixedDeltaTime;
        mRb.MovePosition(mRb.position + movement);

        ClampPosition();
    }


    private void ClampPosition()
    {
        var pos = mRb.position;

        var terrain = Terrain.activeTerrain;
        var tPos = terrain.GetPosition();
        var tSize = terrain.terrainData.size;

        // Clamp X and Z to terrain boundaries
        pos.x = Mathf.Clamp(pos.x, tPos.x + boundary, tPos.x + tSize.x - boundary);
        pos.z = Mathf.Clamp(pos.z, tPos.z + boundary, tPos.z + tSize.z - boundary);

        // Optional: clamp height so it never goes underground
        float terrainHeight = terrain.SampleHeight(pos);
        pos.y = Mathf.Max(pos.y, terrainHeight + 1f); // keep 1 meter above ground

        mRb.MovePosition(pos);        
    }
}
