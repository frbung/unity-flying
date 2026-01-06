using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cameraTarget;        // plane root

    public Vector3 offset = new Vector3(0, 5, -10);
    
    public float followSpeed = 5f;
    
    public float lookSpeed = 10f;

    public float minHeight = 1.5f;


    void LateUpdate()
    {
        // Desired position behind + above the plane
        var desiredPos = cameraTarget.TransformPoint(offset);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // Smooth look at the plane
        Quaternion lookRot = Quaternion.LookRotation(cameraTarget.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, lookSpeed * Time.deltaTime);

        // Keep camera above terrain
        var terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            var terrainHeight = terrain.SampleHeight(transform.position);
            var pos = transform.position;

            // Add a safety margin so the camera never clips
            var clipHeight = terrainHeight + minHeight;

            if (pos.y < clipHeight)
                pos.y = clipHeight;

            transform.position = pos;
        }
    }
}
