using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NavMove : MonoBehaviour {
    public Transform target;
    NavMeshAgent agent;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        agent.SetDestination(target.position);
    }
}
