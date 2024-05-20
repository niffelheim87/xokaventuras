using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    public Vector2 followSpot;
    public float speed;
    public float perspectiveScale;
    public float scaleRatio;
    private NavMeshAgent agent;
    public Animator anim;
    private SpriteRenderer sr;
    public Vector2 stuckDistanceCheck;
    public bool inDialog;
    public bool cutSceneInProgress;
    private Verbs verb;

    // Inicializa las variables antes del update del primer frame
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        verb = FindObjectOfType<Verbs>();
        followSpot = transform.position;       
        agent.updateRotation = false;
        agent.updateUpAxis = false;       
    }

    // Update es llamado por cada frame para recoge la posición del ratón para después establecer la posición de destino.
    void Update()
    {
        if(!inDialog)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                followSpot = new Vector2(mousePosition.x, mousePosition.y);

            }
            agent.SetDestination(new Vector3(followSpot.x, followSpot.y, transform.position.z));

            UpdateAnimation();
        }
        AdjustPerspective();
        //AdjustSortingLayer();
    }

    // Calcula la distancia entre la posición actual y destino actualizando la animación.
    private void UpdateAnimation()
    {
        float distance = Vector2.Distance(transform.position, followSpot);
        if(Vector2.Distance(stuckDistanceCheck,transform.position)==0)
        {
            anim.SetFloat("distance", 0f);
            return;
        }
        anim.SetFloat("distance", distance);
        if(distance>0.01)
        {
            Vector3 direction = transform.position - new Vector3(followSpot.x, followSpot.y, transform.position.z);
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            anim.SetFloat("angle", angle);
            stuckDistanceCheck = transform.position;
        }
    }

    /*private void AdjustSortingLayer()
    {
        sr.sortingOrder=(int)(transform.position.y * -100);
    }
    */

    // Ajusta la escala en función de la posición vertical y la limita para no exceder el tamaño.
    public void AdjustPerspective()
    {
        Vector3 scale = transform.localScale;
        scale.x = perspectiveScale * ((scaleRatio - transform.position.y)/2);
        scale.y = perspectiveScale * ((scaleRatio - transform.position.y)/2);
        if (scale.y > 1.8f)
            scale.y = 1.8f;
        if (scale.x > 1.8f)
            scale.x = 1.8f;
        if (scale.y < 1.2f)
            scale.y = 1.2f;  
        if (scale.x < 1.2f)
            scale.x = 1.2f;     
        transform.localScale = scale;
    }


    //Cambian los estados para mostrar y esconder los objetos relacionados al diálogo.
    public void ExitDialog()
    {
        inDialog = false;
        cutSceneInProgress = false;
        verb.verb = Verbs.Action.Walk;
        verb.UpdateVerbTextBox(null);
        verb.gameObject.SetActive(true);
    }

    public void EnterDialogue()
    {
        inDialog = true;
        cutSceneInProgress = true;
        verb.verb = Verbs.Action.Walk;
        verb.UpdateVerbTextBox(null);
        verb.gameObject.SetActive(false);
    }
}
