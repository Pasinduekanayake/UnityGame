using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateNPCmovement : MonoBehaviour
{
    // Start is called before the first frame update
    public state currentState = state.patrol;
    private GameObject player = GameObject.FindWithTag("Player");
    private GameObject enemy1 = GameObject.FindWithTag("Enemy1");
    private GameObject enemy2 = GameObject.FindWithTag("Enemy2");
    private float distance1;
    private float distance2;

    public enum state{patrol, chase, attack};

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == state.patrol)
        {

        }
    }

    private void calculateDistances()
    {
        distance1 = Vector3.Distance(player.transform.position, enemy1.transform.position);
        distance2 = Vector3.Distance(player.transform.position, enemy2.transform.position);
    }
}
