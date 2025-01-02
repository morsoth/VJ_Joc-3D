using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public void Shake(float duration, float magnitude)
	{
        StartCoroutine(ShakeCamera(duration, magnitude));
    }

    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition; // Restaurar la posici�n original
    }
}
