using MimicSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MimicAgent : GAgent
{

    public float speed = 5f;
    NavMeshAgent navMeshAgent;
    public LayerMask lineOfSightMask;
    GameObject player;
    public GameObject markerPrefab; // Assign this in the Unity Inspector
    private GameObject currentMarker;
    private bool playerLastSeen = false;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        base.Start();

        player = GameObject.FindWithTag("Player");

        SubGoal s1 = new SubGoal("isAtPlayer", 1, true);
        goals.Add(s1, 3);

        SubGoal s2 = new SubGoal("areaChecked", 1, true);
        goals.Add(s2, 1);
    }

    private void Update()
    {
        if (currentAction != null && currentAction.target != null)
        {
            if (currentAction is GoToPlayer && CanSeePlayer(player))
            {
                navMeshAgent.SetDestination(currentAction.target.transform.position);
                playerLastSeen = true;
            }
            else if (currentAction is GoToPlayer && !CanSeePlayer(player))
            {
                if (playerLastSeen)
                {
                    PlaceMarker();
                    playerLastSeen = false;
                }
                SwitchToInvestigateArea();
            }
            else if (currentAction is InvestigateArea && currentAction.running)
            {
                // Check if the investigation is complete
                if (Vector3.Distance(transform.position, currentAction.target.transform.position) < 1f)
                {
                    StopCurrentAction();
                    ResetGoals();
                }
            }
        }
    }

    private void PlaceMarker()
    {
        if (currentMarker != null)
        {
            Destroy(currentMarker);
        }

        Vector3 markerPosition = new Vector3(player.transform.position.x, 1f, player.transform.position.z);

        currentMarker = Instantiate(markerPrefab, markerPosition, Quaternion.identity);
    }

    private void SwitchToInvestigateArea()
    {
        StopCurrentAction();
        currentAction = GetComponent<InvestigateArea>();
        if (currentAction != null)
        {
            currentAction.target = currentMarker;
            currentAction.running = true;
            navMeshAgent.SetDestination(currentAction.target.transform.position);
        }
    }

    private void StopCurrentAction()
    {
        if (currentAction != null)
        {
            currentAction.running = false;
            currentAction.PostPerform();
            currentAction = null;
            planner = null;
            navMeshAgent.ResetPath(); // Stop the NavMeshAgent from moving
        }
    }

    private void ResetGoals()
    {
        goals.Clear();
        SubGoal s1 = new SubGoal("isAtPlayer", 1, true);
        goals.Add(s1, 3);

        SubGoal s2 = new SubGoal("areaChecked", 1, true);
        goals.Add(s2, 1);
    }

    public bool CanSeePlayer(GameObject player)
    {
        Vector3 playerCenter = player.transform.position + Vector3.up * 4.5f; // tallet er til at "tilte" raycast op
        Vector3 direction = playerCenter - transform.position;
        float maxDistance = Vector3.Distance(transform.position, playerCenter);

        RaycastHit hit;
        Debug.DrawRay(transform.position, direction, Color.red);

        if (Physics.Raycast(transform.position, direction, out hit, maxDistance, lineOfSightMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }



}
