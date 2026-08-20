using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 14f, -13f);
    public float smoothSpeed = 12f;
    public float lookHeightOffset = 1f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;

    private float shakeTimer = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.position += shakeOffset;
        }

        Vector3 lookPoint = target.position + Vector3.up * lookHeightOffset;
        transform.LookAt(lookPoint);
    }

    public void TriggerShake()
    {
        shakeTimer = shakeDuration;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}