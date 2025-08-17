using EvolveGames;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    private Animator animator = null;
    private NavMeshAgent agent = null;
    private FieldOfView fieldOfView = null;

    [Header("General")]
    [SerializeField] private Door[] bottomFloorDoors;
    [SerializeField] private Door[] topFloorDoors;
    [SerializeField] private Transform objectChecker;
    [SerializeField] private LayerMask objectLayer;

    [Header("Patrolling")]
    [SerializeField] private WaypointCircuit bottomFloorWaypointCircuit = null;
    [SerializeField] private WaypointCircuit topFloorWaypointCircuit = null;

    [SerializeField] private float waypointThresholdDistance = 2;
    [SerializeField] private float timeToWaitBeforeChangingWaypoints = 3;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float roomInvestigationTime = 3.0f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 3.0f;
    [SerializeField] private float timeToChaseIfWithinFOVRange = 2.0f;
    [SerializeField] private float timeOutOfSightToLoseSightOfPlayer = 10;

    [Header("footsteps")]
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private float walkingFootstepRate = 0.5f;
    [SerializeField] private float runningFootstepRate = 0.3f;
    private float footstepTimer = 0.0f;

    private bool isMoving = false;
    private bool searchingRoom = false;
    private float changeWaypointTimer = 0.0f;
    private float roomInvestigationTimer = 0.0f;
    private float detectionTimer = 0.0f;

    [SerializeField] private Door doorSearching = null;
    [SerializeField] private bool hasEnteredRoomThatsSearching = false;
    [Header("Flags")]
    [SerializeField] private States currentState = States.AboutToChangeRooms;
    public bool isInBottomFloor = true;
    public Transform CurrentWaypoint = null;
    private AudioSource audioSource;
    private GameObject currentObjectLooking;
    private float outOfSightTimer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<FieldOfView>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ChangeState(States.AboutToChangeRooms);
    }

    private void Update()
    {
        RaycastForForwardObject();
        HandleStateMachine();
    }

    private void RaycastForForwardObject()
    {
        RaycastHit hit;
        var hasHit = Physics.Raycast(objectChecker.transform.position, objectChecker.transform.forward, out hit, 2, objectLayer);
        if (hasHit)
        {
            currentObjectLooking = hit.collider.gameObject;
        }
        else
        {
            currentObjectLooking = null;
        }
    }

    private void HandleStateMachine()
    {
        switch (currentState)
        {
            case States.AboutToChangeRooms:
                HandleAboutToChangeRoomsState();
                break;
            case States.Patrolling:
                HandlePatrollingState();
                HandleFootsteps();
                break;
            case States.Chasing:
                CheckForDoors();
                HandleChasingState();
                HandleFootsteps();
                break;
        }
    }

    private void HandleAboutToChangeRoomsState()
    {
        if(UIManager.instance.StaticActive == false)
        {
            ChangeState(States.Patrolling);
        }
    }

    private void HandlePatrollingState()
    {
        if (fieldOfView.canSeePlayer)
        {
            detectionTimer += Time.deltaTime;
            if(detectionTimer > timeToChaseIfWithinFOVRange)
            {
                ChangeState(States.Chasing);
                detectionTimer = 0;
            }
        }
        else
        {
            detectionTimer -= Time.deltaTime;
            if (detectionTimer < 0) detectionTimer = 0;
        }


        Transform[] waypoints = null;
        Door[] doorsInFloor = null;
        if (isInBottomFloor)
        {
            waypoints = bottomFloorWaypointCircuit.waypoints;
            doorsInFloor = bottomFloorDoors;
        }
        else
        {
            waypoints = topFloorWaypointCircuit.waypoints;
            doorsInFloor = topFloorDoors;
        }

        // Unique state for when searching a door room
        if (searchingRoom)
        {
            if (!hasEnteredRoomThatsSearching)
            {
                agent.destination = doorSearching.aiInvestigationPoint.position;
                float distanceToDoor = Vector3.Distance(transform.position, doorSearching.transform.position);
                if (distanceToDoor < 1)
                {
                    // Door is locked, AI can't access it so we return to regular patrolling
                    if (!doorSearching.AttemptToOpenDoor(false) && !doorSearching.IsOpen)
                    {
                        searchingRoom = false;
                        hasEnteredRoomThatsSearching = false;
                        doorSearching = null;
                    }
                    // AI can access this room
                    else
                    {
                        hasEnteredRoomThatsSearching = true;
                    }
                }
                return;
            }

            // This runs when the AI is investigating the room
            CheckForDoors();
            agent.destination = doorSearching.roomInvestigationPoint.position;
            float distanceToPoint = Vector3.Distance(transform.position, doorSearching.roomInvestigationPoint.position);
            if (distanceToPoint < 1)
            {
                if (agent.isStopped == false)
                {
                    animator.CrossFadeInFixedTime("Idle", 0.1f);
                    agent.isStopped = true;
                }
                roomInvestigationTimer += Time.deltaTime;
                if (roomInvestigationTimer > roomInvestigationTime)
                {
                    // Investigated
                    searchingRoom = false;
                    hasEnteredRoomThatsSearching = false;
                    doorSearching = null;
                    roomInvestigationTimer = 0;

                    agent.isStopped = false;
                    agent.speed = walkSpeed;
                    agent.destination = CurrentWaypoint.position;
                    animator.CrossFadeInFixedTime("Walk", 0.1f);
                }
            }
            return;
        }

        // Regular patrolling state
        if (isMoving)
        {
            if (CurrentWaypoint == null)
            {
                CurrentWaypoint = waypoints
                    [Random.Range(0, waypoints.Length)];
            }

            agent.destination = CurrentWaypoint.position;

            float distanceToWaypoint = Vector3.Distance(transform.position, CurrentWaypoint.position);
            if (distanceToWaypoint < waypointThresholdDistance)
            {
                animator.CrossFadeInFixedTime("Idle", 0.1f);
                isMoving = false;
                agent.isStopped = true;
            }
        }
        else
        {
            changeWaypointTimer += Time.deltaTime;
            if (changeWaypointTimer > timeToWaitBeforeChangingWaypoints)
            {
                isMoving = true;
                agent.isStopped = false;
                changeWaypointTimer = 0;
                animator.CrossFadeInFixedTime("Walk", 0.1f);

                int newWaypointIndex = Random.Range(0, waypoints.Length);
                Transform newWaypoint = waypoints[newWaypointIndex];

                while (newWaypoint == CurrentWaypoint)
                {
                    newWaypointIndex = Random.Range(0, waypoints.Length);
                    newWaypoint = waypoints[newWaypointIndex];
                }
                CurrentWaypoint = newWaypoint;

                // Random chance to search room
                float searchRoomProbability = 25f;
                if(Random.Range(0,101) < searchRoomProbability)
                {
                    searchingRoom = true;
                    doorSearching = doorsInFloor[Random.Range(0, doorsInFloor.Length)];
                    animator.CrossFadeInFixedTime("Walk", 0.1f);
                    agent.isStopped = false;
                    agent.speed = walkSpeed;
                }
            }
        }
    }

    private void HandleChasingState()
    {
        agent.destination = PlayerController.instance.transform.position;

        float distance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        if(distance < 1.6f)
        {
            GameManager.insance.ShowGameOverAnimation();
        }

        if (!fieldOfView.canSeePlayer)
        {
            outOfSightTimer += Time.deltaTime;
            if(outOfSightTimer > timeOutOfSightToLoseSightOfPlayer)
            {
                ChangeState(States.Patrolling);
                outOfSightTimer = 0;
            }
        }
        else
        {
            outOfSightTimer -= Time.deltaTime;
            if (outOfSightTimer < 0) outOfSightTimer = 0;
        }

    }
    private void CheckForDoors()
    {
        if(currentObjectLooking != null && currentObjectLooking.GetComponent<Door>() != null)
        {
            Door doorComponent = currentObjectLooking.GetComponent<Door>();
            if(doorComponent.IsOpen == false)
            {
                doorComponent.AttemptToOpenDoor(false);
            }
        }
    }

    public void ChangeState(States newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case States.AboutToChangeRooms:
                UIManager.instance.ShowStatic(2);
                break;
            case States.Patrolling:
                agent.isStopped = false;
                isMoving = true;
                animator.CrossFadeInFixedTime("Walk", 0.1f);
                changeWaypointTimer = 0;
                agent.speed = walkSpeed;
                hasEnteredRoomThatsSearching = false;
                searchingRoom = false;
                doorSearching = null;
                roomInvestigationTimer = 0;
                break;
            case States.Chasing:
                agent.speed = chaseSpeed;
                agent.isStopped = false;
                animator.CrossFadeInFixedTime("Run", 0.1f);
                
                break;
        }
    }
    private void HandleFootsteps()
    {
        if (agent.isStopped) return;

        footstepTimer += Time.deltaTime;
        float footstepRate = currentState == States.Patrolling ? walkingFootstepRate : runningFootstepRate;
        if(footstepTimer > footstepRate)
        {
            audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            footstepTimer = 0;
        }
    }
}

public enum States
{
    None,
    AboutToChangeRooms,
    Patrolling,
    Chasing
}