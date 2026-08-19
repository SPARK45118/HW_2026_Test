using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 14f, -13f);
    public float smoothSpeed = 12f;
    public float lookHeightOffset = 1f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        Vector3 lookPoint = target.position + Vector3.up * lookHeightOffset;
        transform.LookAt(lookPoint);
    }
}