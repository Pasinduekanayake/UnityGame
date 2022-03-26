using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navmeshBaker : MonoBehaviour
{
    [SerializeField]
    NavMeshSurface[] navMeshSurface;

    // Start is called before the first frame update
    void Start()
    {
        navMeshSurface[0].BuildNavMesh();
    }

}
