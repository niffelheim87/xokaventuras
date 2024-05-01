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
    // Start is called before the first frame update
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

    // Update is called once per frame
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
        
        //AdjustSortingLayer();
    }

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
            float angel = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            anim.SetFloat("angle", angel);
            stuckDistanceCheck = transform.position;
        }
    }

    private void AdjustSortingLayer()
    {
        sr.sortingOrder=(int)(transform.position.y * -100);
    }


    //public void AdjustPerspektive()
    //{
    //    Vector3 scale = transform.localScale;
    //    scale.x = perspectiveScale * (scaleRatio - transform.position.y);
    //    scale.y = perspectiveScale * (scaleRatio - transform.position.y);
    //    transform.localScale = scale;
    //}

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
