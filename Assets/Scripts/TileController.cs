using UnityEngine;

public class TileController : MonoBehaviour
{
    float speed = 5.0f;

    Vector3 endPos;

    Vector3 initialPos;

    float totalTime;

    float elapsedTime = 0f;

    bool move = false;

    void Start()
    {
        endPos = transform.position;

        initialPos =  new Vector3(endPos.x, -3.0f, endPos.z);

        transform.position = initialPos;

        float distancia = Vector3.Distance(initialPos, endPos);

        totalTime = distancia / speed;

        move = true;
    }

    void Update()
    {
        if (move)
        {
            elapsedTime += Time.deltaTime;

            float porcentaje = elapsedTime / totalTime;

            transform.position = Vector3.Lerp(initialPos, endPos, porcentaje);

            if (elapsedTime >= totalTime)
            {
                transform.position = endPos;
                move = false;
            }
        }
    }

    public void Disapear()
	{
        initialPos = transform.position;

        endPos = new Vector3(initialPos.x, -4.0f, initialPos.z);

        elapsedTime = 0f;

        float distancia = Vector3.Distance(initialPos, endPos);

        totalTime = distancia / speed;

        move = true;
    }
}