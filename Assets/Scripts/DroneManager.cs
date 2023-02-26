using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class DroneManager : Enemy
{
    // Objects
    public GameObject playerRef;

    // Movement
    private int direction;
    [SerializeField] private float directionTime;
    [SerializeField] private float aggroSpeed;
    private Coroutine idleMoveRoutine;
    private Vector3 spawnpoint;


    // FOV
    public float radius;
    [Range(0, 360)]
    public float angle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    // Bools
    public bool canSeePlayer;
    private bool prevCanSeePlayer;

    private enum State
    {
        Idle,
        Aggro,
        Return
    };

    [SerializeField] private State state;

    private void Awake()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        spawnpoint = transform.position;
        direction = 1;
        prevCanSeePlayer = false;
        StartCoroutine(FOVRoutine());
        IsIdle();
    }

    private void Update()
    {
        if (state == State.Idle)
        {
            IdleMove();
        }
        else if (state == State.Aggro)
        {
            AggroMove();
        }
        else if (state == State.Return)
            ReturnMove();
    }

    private void IdleMove()
    {
        transform.position += new Vector3(1, 0, 0) * direction * speed;
    }

    private IEnumerator ChangeDirection(float _directionTime)
    {
        yield return new WaitForSeconds(_directionTime);
        direction *= -1;
        idleMoveRoutine = StartCoroutine(ChangeDirection(directionTime));
    }

    private void AggroMove()
    {
        Vector3 playerDirection = playerRef.transform.position- transform.position;
        if(playerDirection.x < 0.1f && playerDirection.x > -0.1f) return;
        transform.position += Mathf.Sign(playerDirection.x) * aggroSpeed * Vector3.right;
    }

    private void ReturnMove()
    {
        Vector3 spawnDirection = spawnpoint - transform.position;
        if (spawnDirection.x < 0.1f && spawnDirection.x > -0.1f)
        {
            IsIdle();
            return;
        }
        transform.position += Mathf.Sign(spawnDirection.x) * aggroSpeed * Vector3.right;
    }

    private void IsIdle()
    {
        state = State.Idle;
        idleMoveRoutine = StartCoroutine("ChangeDirection", directionTime);
    }

    private void IsReturn()
    {
        state = State.Return;
    }

    private void IsAggro()
    {
        state = State.Aggro;
        StopCoroutine(idleMoveRoutine);
    }

    // FOV
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            prevCanSeePlayer = canSeePlayer;
            yield return wait;
            FieldOfViewCheck();
            if (canSeePlayer && !prevCanSeePlayer)
                IsAggro();
            else if(!canSeePlayer && prevCanSeePlayer) 
                IsReturn();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider2D targetCol = Physics2D.OverlapCircle(transform.position, radius, targetMask);

        if (targetCol != null)
        {
            Transform target = targetCol.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.up * -1, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
