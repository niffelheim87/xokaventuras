using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    public Vector2 followSpot; // Punto al que el personaje debe seguir
    public float speed; // Velocidad de movimiento del personaje
    public float perspectiveScale; // Escala de perspectiva para ajustar el tamaño del personaje en función de su posición vertical
    public float scaleRatio; // Ratio de escala para limitar el tamaño del personaje
    private NavMeshAgent agent; // Componente de navegación para el movimiento del personaje
    public Animator anim; // Componente Animator para controlar las animaciones del personaje
    private SpriteRenderer sr; // Componente SpriteRenderer para renderizar el sprite del personaje
    public Vector2 stuckDistanceCheck; // Punto de referencia para verificar si el personaje está atascado
    public bool inDialog; // Indica si el personaje está en un diálogo
    public bool cutSceneInProgress; // Indica si hay una escena en progreso
    private Verbs verb; // Referencia a la clase Verbs para gestionar los verbos de acción del personaje

    
    void Start() // Inicializa las variables antes del update del primer frame
    {
        // Obtiene el componente NavMeshAgent adjunto al objeto y lo asigna a la variable agent
        agent = GetComponent<NavMeshAgent>();

        // Obtiene el componente Animator adjunto al objeto y lo asigna a la variable anim
        anim = GetComponent<Animator>();

        // Obtiene el componente SpriteRenderer adjunto al objeto y lo asigna a la variable sr
        sr = GetComponent<SpriteRenderer>();

        // Encuentra el objeto de tipo Verbs en la escena y lo asigna a la variable verb
        verb = FindObjectOfType<Verbs>();

        // Establece el punto de seguimiento (followSpot) a la posición actual del objeto
        followSpot = transform.position;

        // Desactiva la rotación del agente de navegación
        agent.updateRotation = false;

        // Desactiva la actualización del eje vertical del agente de navegación
        agent.updateUpAxis = false;
    }
    // Update es llamado por cada frame para recoger la posición del ratón y establecer la posición de destino
    void Update()
    {
        // Verifica si no se está en diálogo
        if (!inDialog)
        {
            // Convierte la posición del mouse en pantalla a una posición en el mundo
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Si se hace clic con el botón izquierdo del mouse
            if (Input.GetMouseButtonDown(0))
            {
                // Establece el punto de seguimiento (followSpot) a la posición del mouse
                followSpot = new Vector2(mousePosition.x, mousePosition.y);
            }

            // Establece el destino del agente de navegación a la posición del punto de seguimiento
            agent.SetDestination(new Vector3(followSpot.x, followSpot.y, transform.position.z));

            // Llama al método para actualizar la animación del personaje
            UpdateAnimation();
        }

        // Llama al método para ajustar la perspectiva del personaje
        AdjustPerspective();
    }

    // Calcula la distancia entre la posición actual y el destino, actualizando la animación
    private void UpdateAnimation()
    {
        // Calcula la distancia entre la posición actual del personaje y el punto de seguimiento
        float distance = Vector2.Distance(transform.position, followSpot);

        // Verifica si el personaje está atascado (distancia entre la posición actual y el punto de referencia es cero)
        if (Vector2.Distance(stuckDistanceCheck, transform.position) == 0)
        {
            // Establece la distancia en la animación a 0 y sale del método
            anim.SetFloat("distance", 0f);
            return;
        }

        // Establece la distancia en la animación
        anim.SetFloat("distance", distance);

        // Si la distancia es mayor que 0.01 (el personaje se está moviendo)
        if (distance > 0.01)
        {
            // Calcula la dirección del movimiento
            Vector3 direction = transform.position - new Vector3(followSpot.x, followSpot.y, transform.position.z);

            // Calcula el ángulo de la dirección en grados
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            // Establece el ángulo en la animación
            anim.SetFloat("angle", angle);

            // Actualiza el punto de referencia para la comprobación de atasco
            stuckDistanceCheck = transform.position;
        }
    }

    // Ajusta la escala en función de la posición vertical y limita el tamaño
    public void AdjustPerspective()
    {
        // Obtiene la escala actual del objeto
        Vector3 scale = transform.localScale;

        // Calcula la nueva escala en el eje X en función de la posición vertical y la escala de perspectiva
        scale.x = perspectiveScale * ((scaleRatio - transform.position.y) / 2);

        // Calcula la nueva escala en el eje Y en función de la posición vertical y la escala de perspectiva
        scale.y = perspectiveScale * ((scaleRatio - transform.position.y) / 2);

        // Limita la escala en el eje Y a un máximo de 1.8
        if (scale.y > 1.8f)
            scale.y = 1.8f;

        // Limita la escala en el eje X a un máximo de 1.8
        if (scale.x > 1.8f)
            scale.x = 1.8f;

        // Limita la escala en el eje Y a un mínimo de 1.2
        if (scale.y < 1.2f)
            scale.y = 1.2f;

        // Limita la escala en el eje X a un mínimo de 1.2
        if (scale.x < 1.2f)
            scale.x = 1.2f;

        // Aplica la nueva escala al objeto
        transform.localScale = scale;
    }

    // Establece que no se está en diálogo, permitiendo el intercambio de valores de verb entre Walk y Use
    public void ExitDialog()
    {
        // Establece que no se está en diálogo
        inDialog = false;

        // Establece que no hay una escena en progreso
        cutSceneInProgress = false;

        // Establece el verbo a "Walk"
        verb.verb = Verbs.Action.Walk;

        // Actualiza el cuadro de texto del verbo
        verb.UpdateVerbTextBox(null);

        // Activa el objeto del verbo
        verb.gameObject.SetActive(true);
    }

    // Establece que se está en diálogo, desactivando el objeto del verbo
    public void EnterDialogue()
    {
        // Establece que se está en diálogo
        inDialog = true;

        // Establece que hay una escena en progreso
        cutSceneInProgress = true;

        // Establece el verbo a "Walk"
        verb.verb = Verbs.Action.Walk;

        // Actualiza el cuadro de texto del verbo
        verb.UpdateVerbTextBox(null);

        // Desactiva el objeto del verbo
        verb.gameObject.SetActive(false);
    }
}