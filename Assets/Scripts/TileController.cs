using UnityEngine;

public class TileController : MonoBehaviour
{
    float velocidad = 5.0f;

    private Vector3 posicionFinal;

    private Vector3 posicionInicial;

    private float tiempoTotal;

    private float tiempoTranscurrido = 0f;

    private bool mover = false;

    void Start()
    {
        posicionFinal = transform.position;

        posicionInicial =  new Vector3(posicionFinal.x, -3.0f, posicionFinal.z);

        transform.position = posicionInicial;

        float distancia = Vector3.Distance(posicionInicial, posicionFinal);

        tiempoTotal = distancia / velocidad;

        mover = true;
    }

    void Update()
    {
        if (mover)
        {
            tiempoTranscurrido += Time.deltaTime;

            float porcentaje = tiempoTranscurrido / tiempoTotal;

            transform.position = Vector3.Lerp(posicionInicial, posicionFinal, porcentaje);

            if (tiempoTranscurrido >= tiempoTotal)
            {
                transform.position = posicionFinal;
                mover = false;
            }
        }
    }
}