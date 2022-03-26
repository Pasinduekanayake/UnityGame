using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class controlPlayer : MonoBehaviour
{
    [SerializeField] private Transform position;
    private NavMeshAgent player;

    private void Awake()
    {
        player = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        player.destination = position.position;
    }
}
