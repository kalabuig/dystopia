﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    public enum State {
        Iddleing,
        Moving,
        Chasing,
        Attacking,
        KnockBack,
        Searching,
    }

    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask blockingLayerMasks; //Layers that can block the vision of the enemy
    [SerializeField] private float viewDistance = 100f; //View Distance
    [SerializeField] private float fovAngle = 60f; //Field Of View Angle
    [SerializeField] private float speed = 20f;
    [SerializeField] private float secondsSearching = 3f; //Seconds searching the player after we lost sight of it
    [SerializeField] private int amountOfSearchingsToDo = 3;

    private Transform player;
    private Vector3 lastPlayerPosition;
    private State myState; //State machine of the enemy
    private Rigidbody2D myRigidbody2D;
    private Vector3 moveDir = Vector3.zero;
    private float timePlayerHasBeenLost = 0; //Time.DeltaTime when player is lost while chasing and enemy reach the last known player position
    private int searchingCounter = 0; //Number of searchings done

    private float distanceToAttackPoint;

    private void Awake() {
        player = GameObject.Find("Player")?.transform;
        myRigidbody2D = GetComponent<Rigidbody2D>();
        distanceToAttackPoint = Vector3.Distance(attackPoint.position,transform.position);
    }

    private void FixedUpdate() {
        DoAction();
    }

    private Vector3 FindTargetPlayer() {
        if(Vector3.Distance(transform.position, player.position) < viewDistance) { //Player inside view distance
            Vector3 dirToPlayer = (player.position - transform.position).normalized; //Direction to player
            if(Vector3.Angle(myDirection(), dirToPlayer) < fovAngle / 2f) { //Player inside the view cone (field of view)
                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dirToPlayer, viewDistance, blockingLayerMasks);
                if(raycastHit2D.collider != null) { //if the raycast hits something
                    if(raycastHit2D.collider.gameObject.layer == LayerMask.NameToLayer("Player")) { //Hitting player
                        return player.position;
                    } else { //Hitting something that it is not the player
                        //Nothing
                    }
                }
            }
        }
        return Vector3.zero;
    }

    private bool CheckPlayerInView() { //if player in view, save its position as last player position and return true
        Vector3 positionPlayerInView = FindTargetPlayer();
        if(positionPlayerInView != Vector3.zero) {
            lastPlayerPosition = player.position;
            return true;
        }
        return false;
    }



    private void DoAction() {
        switch(myState) {
            case State.Iddleing:
                if(CheckPlayerInView()) {
                    myState = State.Chasing;
                }
                break;
            case State.Moving:
                if(CheckPlayerInView()) {
                    myState = State.Chasing;
                }
                break;
            case State.Chasing:
                CheckPlayerInView();
                ChasePlayer();
                break;
            case State.Attacking:
                if(CheckPlayerInView()) {
                    //Attack
                } else {
                    timePlayerHasBeenLost = Time.time;
                    myState = State.Searching;
                }
                break;
            case State.KnockBack:
                break;
            case State.Searching:
                if(CheckPlayerInView()) {
                    myState = State.Chasing;
                } else {
                    SearchPlayer();
                }
                break;
            default:
                break;
        }
    }

    private Vector3 myDirection() {
        if(attackPoint == null) return Vector3.zero;
        return (attackPoint.position - transform.position).normalized;
    }

    private void AimAtPlayer(Vector3 aimDir) {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0,0, angle);
    }

    private void ChasePlayer() {
        moveDir = (lastPlayerPosition - transform.position).normalized; //Direction to player
        AimAtPlayer(moveDir);
        myRigidbody2D?.MovePosition(transform.position + moveDir * speed * Time.deltaTime);
        if(Vector3.Distance(lastPlayerPosition, transform.position) < distanceToAttackPoint + 0.1f) { //speed * 0.1f) { //if we are very close to the target position
            myState = State.Attacking;
        }
    }

    private void SearchPlayer() {
        if(Time.time < timePlayerHasBeenLost + (secondsSearching * searchingCounter) ) {
            moveDir = myDirection();
            myRigidbody2D?.MovePosition(transform.position + moveDir * speed * Time.deltaTime);
        } else {
            float randAngle = Random.Range(0, 360); //random angle in degrees
            transform.eulerAngles = new Vector3(0,0, randAngle); //Set a random direction
            searchingCounter++;
            if(searchingCounter > amountOfSearchingsToDo) {
                searchingCounter = 0;
                myState = State.Iddleing;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")) {
            lastPlayerPosition = player.position;
            myState = State.Chasing;
        }
    }
}
