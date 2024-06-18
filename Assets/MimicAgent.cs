using MimicSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class MimicAgent : GAgent
{
    public float speed = 5f;
    NavMeshAgent navMeshAgent;
    public LayerMask lineOfSightMask;
    GameObject player;
    public GameObject markerPrefab; 
    private GameObject currentMarker;
    private bool playerLastSeen = false;
    private GameObject[] patrolPoints;
    private int currentPatrolIndex = 0;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        base.Start();

        player = GameObject.FindWithTag("Player");
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");

        gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }

        SubGoal s1 = new SubGoal("isAtPlayer", 1, true);
        goals.Add(s1, 3);

        SubGoal s2 = new SubGoal("areaChecked", 1, true);
        goals.Add(s2, 1);

        
        currentMarker = Instantiate(markerPrefab, Vector3.zero, Quaternion.identity);
        currentMarker.SetActive(false);
    }

    private void Update()
    {
        // Led altid efter spiller
        if (CanSeePlayer(player))
        {
            // Ser spiller
            playerLastSeen = true;
            EvaluateGoals();
        }
        else if (currentAction is GoToPlayer)
        {
            // Mister spiller
            if (playerLastSeen)
            {
                MoveMarker(player.transform.position);
                playerLastSeen = false;
                SwitchToInvestigateArea();
            }
        }
        else if (currentAction is InvestigateArea)
        {
            // Led altid efter spiller
            if (CanSeePlayer(player))
            {
                playerLastSeen = true;
                EvaluateGoals();
            }
            else
            {
                // Check om investigation er færdig
                if (Vector3.Distance(transform.position, currentAction.target.transform.position) < 1f)
                {
                    StopCurrentAction();
                    Debug.Log("Investigation complete, transitioning to next patrol point");

                    // Marker til næste punkt
                    MoveMarkerToNextPatrolPoint();

                    EvaluateGoals();
                }
            }
        }

        
        if (currentAction == null || !currentAction.running)
        {
            EvaluateGoals();
        }
    }

    private void EvaluateGoals()
    {
        
        if (planner == null || actionQueue == null || actionQueue.Count == 0)
        {
            planner = new GPlanner();

            
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
            {
                
                if (IsGoalAchievable(sg.Key))
                {
                    Debug.Log("Evaluating goal: " + sg.Key.sgoals.Keys.First() + " with priority " + sg.Value);
                    actionQueue = planner.plan(actions, sg.Key.sgoals, null);
                    if (actionQueue != null)
                    {
                        currentGoal = sg.Key;
                        break;
                    }
                }
            }
        }

        
        if (actionQueue != null && actionQueue.Count > 0)
        {
            StopCurrentAction();
            currentAction = actionQueue.Dequeue();
            Debug.Log("Switching to action: " + currentAction.GetType().Name);
            if (currentAction.PrePerform())
            {
                currentAction.running = true;
                if (currentAction.target == null && currentAction.targetTag != "")
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }
                if (currentAction.target != null)
                {
                    navMeshAgent.SetDestination(currentAction.target.transform.position);
                    Debug.Log("Navigating to target: " + currentAction.target.name);
                }
            }
            else
            {
                actionQueue = null;
            }
        }
    }

    
    private bool IsGoalAchievable(SubGoal goal)
    {
        if (goal.sgoals.ContainsKey("isAtPlayer") && !CanSeePlayer(player))
        {
            
            return false;
        }
        return true;
    }

    private void MoveMarker(Vector3 position)
    {
        if (currentMarker != null)
        {
            currentMarker.transform.position = new Vector3(position.x, 1f, position.z);
            currentMarker.SetActive(true);
        }
    }

    private void MoveMarkerToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points set.");
            return;
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        Vector3 patrolPointPosition = patrolPoints[currentPatrolIndex].transform.position;
        MoveMarker(patrolPointPosition);
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
            navMeshAgent.ResetPath(); 
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Notify the GameManager of game over
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
            else
            {
                Debug.LogError("GameManager reference is null. Cannot trigger game over.");
            }
        }
    }

    public bool CanSeePlayer(GameObject player)
    {
        Vector3 playerCenter = player.transform.position + Vector3.up * 4.5f; 
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