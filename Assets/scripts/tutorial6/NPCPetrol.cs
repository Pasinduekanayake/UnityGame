using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPetrol : MonoBehaviour
{
    //[SerializeField] private Transform position;
    public UpdateNPCmovement npcMovement;
    private GameObject playerNew = GameObject.FindWithTag("Player");
    private float distance;
    public enum state { patrol, chase, attack };

    public Vector3 position1 = new Vector3(17, 2, 9);
    public Vector3 position2 = new Vector3(17, 2, 1);
    public float pathEndThreshold = 0.1f;
    private NavMeshAgent player;
    private Vector3 currentPos;

    private void Start()
    {
        currentPos = position1;
        player.destination = position1;
    }

    private void Awake()
    {
        player = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //updateCurrentState();
       

        if (!player.pathPending && player.remainingDistance <= player.stoppingDistance)
        {
            //player.SetDestination(position2);
            positionUpdate();
            player.destination = currentPos;
        }       
    }

    private void positionUpdate()
    {
        if(currentPos == position1)
        {
            currentPos = position2;
        }
        else
        {
            currentPos = position1;
        }
    }

    /*private void updateCurrentState()
    {
        calculateDistances();

        if (distance <= 20)
        {
            npcMovement.currentState = state.chase;
        }
        else
        {
            currentState = "patrol";
        }
    }*/

    private void calculateDistances()
    {
        distance = Vector3.Distance(playerNew.transform.position, player.transform.position);
    }
}
