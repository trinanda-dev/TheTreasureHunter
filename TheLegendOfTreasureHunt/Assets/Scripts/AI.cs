using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public enum MoveCondition {
    Patrol, Chase, Wait
}
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
//catatan dibagian komponen navmeshagent tidak perlu di operasikan, karena beberapa variable navmeshagent sudah teroperasikan di script ini
//base offset dalam komponen navmeshagent bisa di atur sesuai dengan 3D Characternya
public class AI : MonoBehaviour
{

    [Header("Steering")]
    public float angularSpeed = 120f; //kecepatan rotasi 
    public float patrolSpeed = 1.8f; //kecepatan patroli
    public float chaseSpeed = 3.6f; //kecepatan mengejar
    public float maxTimeOnWaiting = 5; //waktu untuk berhenti ditempat 
    public float maxTimeOnChasing = 10; //waktu mengejar
    public float maxDelayAttack = 3; //waktu delay setelah melakukan penyerangan
    public float maxTimeToWaiting = 15; //waktu AI akan berhenti

    [Range(1.4f, 2.2f)]
    public float stopDistance = 1.6f; //jarak berhenti antara target dengan AI
    public MoveCondition moveCondition; //kondisi bergerak

    [Header("Patrol")]
    public Transform[] patrolPoint; //titik patroli 

    [Header("FOV")]
    public Transform PFOV; //Position Field Of View atau posisi pandangan AI biasanya di tempatkan di bagian kepala atau mata (isi terlebih dahulu agar melihat GUInya)
    public float range = 5; //jarak pandangan AI

    [Range(0, 360)]
    public float angle = 135; //luas pandangan AI
    public LayerMask playerMask; //objek yang mempunyai layer tersebut yang akan dikejar oleh AI
    public LayerMask obstructionMask; //objek yang menghalangi pandangan AI

    [Header("Don't Operate this")]
    public Transform target;
    public bool isSeePlayer;
    public NavMeshAgent agent; //rekomendasi isi secara manual, agar melihat GUInya
    public AudioSource audioSource; 
    public float currentDistance;   

    //private variable
    private Vector3 newTargetFaced;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private int index;
    private float currentTimeOnWaiting, currentTimeOnChasing, currentTimeAttack, currentTimeToWaiting;

    private void Start () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();
        index = Random.Range(0, patrolPoint.Length); //random index titik patrol, agar AI memulai patrol tidak dititik patroli yang sama 
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine () {
        WaitForSeconds wait = new WaitForSeconds(0.3f);
        while(true) {
            yield return wait;
            FieldOfView();
        }
    }
    private void FieldOfView() {
        Collider[] atCloseRange = Physics.OverlapSphere(transform.position, agent.radius + 0.7f, playerMask); //collider ini hanya berlaku untuk objek yang memiliki playerMask

        if(atCloseRange.Length > 0){ //Jika collider di atas terkena objek yang memiliki layer = PlayerMask
            target = atCloseRange[0].transform; //ubah target menjadi objek yang terkena
            SwitchMoveCondition(MoveCondition.Chase); //moveCondition berubah menjadi mengejar (chase)
        }

        Collider[] rangeChecks = Physics.OverlapSphere(PFOV.position, range, playerMask); //collider ini hanya berlaku untuk objek yang memiliki playerMask
        if(rangeChecks.Length > 0) {
            target = rangeChecks[0].transform; //variable target di isi dengan objek yang terkena radiasi atau range bulat ini
            Vector3 directionToTarget = (target.position - PFOV.position).normalized; 
            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2) { 
                float distanceToTarget = Vector3.Distance(PFOV.position, target.position);
                if(!Physics.Raycast(PFOV.position, directionToTarget, distanceToTarget, obstructionMask)) { //ketika AI melihat ke objek yang tidak memiliki layer ObstructionMask maka dia akan mengejar objek tersebut (atau player)
                    SwitchMoveCondition(MoveCondition.Chase);
                    isSeePlayer = true;
                } else 
                    isSeePlayer = false;
            } else
                isSeePlayer = false;
        } else
            isSeePlayer = false;
    }

    private void Update () {
        switch(moveCondition) {
            case MoveCondition.Patrol :
                Patroling(); 
            break;
            case MoveCondition.Wait :
                Waiting(); 
            break;
            case MoveCondition.Chase :
                Chasing(); 
            break;
        }
        
        capsuleCollider.height = agent.height;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, (1 - agent.baseOffset), capsuleCollider.center.z);
        capsuleCollider.radius = agent.radius / 2;
        agent.autoBraking = false;
        agent.angularSpeed = 60f;

        if (agent.hasPath)
            agent.acceleration = (agent.remainingDistance < stopDistance) ? 25 : 60;

        AnimationSystem();
        Rotation();

        //arah pathfinding
        for (int i = 0; i < agent.path.corners.Length - 1; i++)
        {
            Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1] , Color.green);
        }
    }

    private void Rotation () {
        //rotation
        //rotasinya tidak menggunakan dari NavMeshAgentnya karena ada sedikit bug, di NavMeshAgent ketika target dengan AI ini berjarak dekat, AI tidak akan menghadap target
        Vector3 lookAtTarget = new Vector3(newTargetFaced.x - transform.position.x, 0, newTargetFaced.z - transform.position.z);
        if(lookAtTarget != Vector3.zero) {
            var rotation = Quaternion.LookRotation(lookAtTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, angularSpeed * Time.deltaTime);
        }
        agent.updateRotation = false; //menonaktifkan rotasi dari NavMeshAgent
    }

    private void Chasing () {
        agent.stoppingDistance = 0.2f;
        agent.speed = chaseSpeed;
        agent.destination = CalculatePositionWithDistance(transform.position, target.position, stopDistance);

        currentDistance = Vector3.Distance(transform.position, target.position);

        if(currentTimeOnChasing > maxTimeOnChasing) {
            SwitchMoveCondition(MoveCondition.Wait);
        }

        bool isAttackDistance = currentDistance < stopDistance + 0.6f; 

        if(isAttackDistance) 
            newTargetFaced = target.position;
        else 
            newTargetFaced = agent.steeringTarget;

        if(isAttackDistance && isSeePlayer && currentTimeAttack > maxDelayAttack) { 
            animator.SetTrigger("Attack");
            currentTimeAttack = 0;
        }

        //ini adalah waktu berhenti setelah melakukan serangan ke target 
        if(currentTimeAttack < maxDelayAttack)
            currentTimeAttack += Time.deltaTime;

        if(!isSeePlayer)
            currentTimeOnChasing += Time.deltaTime;
        else
            if(currentTimeOnChasing > 0) currentTimeOnChasing = 0;
    }

    private void Patroling () {
        agent.stoppingDistance = 1f;
        agent.speed = patrolSpeed;
        agent.destination = CalculatePositionWithDistance(transform.position, patrolPoint[index].position, agent.stoppingDistance / 1.5f);

        currentDistance = Vector3.Distance(transform.position, patrolPoint[index].position);

        newTargetFaced = agent.steeringTarget;

        if(currentDistance < agent.stoppingDistance) { //jika jarak AI kurang dari jarak yang telah ditentukan, maka ubah nilai moveCondition menjadi wait
            SwitchMoveCondition(MoveCondition.Wait);
        }

        if(currentTimeToWaiting > Random.Range(maxTimeToWaiting / 1.25f, maxTimeToWaiting))
            SwitchMoveCondition(MoveCondition.Wait);
        else
            currentTimeToWaiting += Time.deltaTime;
    }

    private void Waiting () {
        agent.destination = transform.position;

        if(currentTimeOnWaiting > maxTimeOnWaiting) //jika nilai currentTimeOnWaiting melebihi nilai maxTimeOnWaiting maka ubah nilai moveCondition menjadi patrol 
            SwitchMoveCondition(MoveCondition.Patrol);
        else
            currentTimeOnWaiting += Time.deltaTime;
    }

    private void SwitchMoveCondition (MoveCondition newMoveCondition) {
        if(moveCondition == newMoveCondition) return;

        moveCondition = newMoveCondition;
        Debug.Log("nilai moveCondition berubah menjadi " + newMoveCondition.ToString());

        index = Random.Range(0, patrolPoint.Length);
        currentTimeOnWaiting = currentTimeOnChasing = currentTimeToWaiting = 0;

    }

    float newMagnitude;
    private void AnimationSystem () {
        newMagnitude = Mathf.Lerp(newMagnitude, agent.velocity.magnitude, 5 * Time.deltaTime);
        animator.SetFloat("Walk", newMagnitude);
    }

    //Audio ini digunakan dalam animation Event
    public void AudioEvent (AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }

    //DamageTarget digunakan dalam animation event, clip attack
    public void DamageTarget (float damageAmount) {
        Debug.Log("Attacked Player: " + damageAmount.ToString());
    }

    //dibawah ini untuk membuat posisi dari gabungan antara stop distance, target, dan AI ini
    private Vector3 CalculatePositionWithDistance (Vector3 start, Vector3 end, float m_stopDistance) {
        Vector3 dir = (start - end).normalized;
        return end + dir * Mathf.Clamp(Vector3.Distance(start, end), 0, m_stopDistance);
    }

    
}
