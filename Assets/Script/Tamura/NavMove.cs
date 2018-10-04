using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NavMove : MonoBehaviour {
    public Transform target;
    NavMeshAgent agent;

    public Vector3 warpPoint;

    Material mat;

    [SerializeField]
    GameObject shouko;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        
        //agent.Warp(warpPoint);

        mat = GetComponent<Renderer>().material;
        Debug.Log(mat);
    }

    void Update() {
        agent.SetDestination(target.position);

        //agent.Warp(warpPoint);

        Attack();
    }

    void Attack() {
        if (agent.remainingDistance <= 0.1) {
            mat.color = Color.red;
            Instantiate(shouko, this.transform.position, this.transform.rotation);
        }
        else {
            mat.color = Color.white;
        }
    }
}