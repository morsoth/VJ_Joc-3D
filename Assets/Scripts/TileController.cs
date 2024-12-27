using UnityEngine;

public class TileController : MonoBehaviour
{
    // Velocidad de movimiento en unidades por segundo
    float velocidad = 5.0f;

    // Posición final (donde se detendrá el objeto)
    private Vector3 posicionFinal;

    // Posición inicial (más abajo)
    private Vector3 posicionInicial;

    // Tiempo total necesario para completar el movimiento
    private float tiempoTotal;

    // Tiempo transcurrido
    private float tiempoTranscurrido = 0f;

    // Flag para iniciar el movimiento
    private bool mover = false;

    void Start()
    {
        // Guardar la posición final (posición actual del objeto)
        posicionFinal = transform.position;

        // Definir la posición inicial desplazándola hacia abajo en el eje Y
        posicionInicial =  new Vector3(posicionFinal.x, -3.0f, posicionFinal.z); // Puedes ajustar la distancia

        // Establecer la posición inicial del objeto
        transform.position = posicionInicial;

        // Calcular la distancia a recorrer
        float distancia = Vector3.Distance(posicionInicial, posicionFinal);

        // Calcular el tiempo total basado en la velocidad
        tiempoTotal = distancia / velocidad;

        // Iniciar el movimiento
        mover = true;
    }

    void Update()
    {
        if (mover)
        {
            // Incrementar el tiempo transcurrido
            tiempoTranscurrido += Time.deltaTime;

            // Calcular el porcentaje de progreso
            float porcentaje = tiempoTranscurrido / tiempoTotal;

            // Aplicar Lerp para mover el objeto
            transform.position = Vector3.Lerp(posicionInicial, posicionFinal, porcentaje);

            // Verificar si el movimiento ha terminado
            if (tiempoTranscurrido >= tiempoTotal)
            {
                transform.position = posicionFinal; // Asegurarse de llegar exactamente a la posición final
                mover = false; // Detener el movimiento
            }
        }
    }
}