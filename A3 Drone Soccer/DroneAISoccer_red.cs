using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace UnityStandardAssets.Vehicles.Car
//{
[RequireComponent (typeof (DroneController))]
public class DroneAISoccer_red : MonoBehaviour {
    public DroneController m_Drone; // the drone controller we want to use

    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;

    public GameObject[] friends;
    public string friend_tag;
    public GameObject[] enemies;
    public string enemy_tag;

    public GameObject own_goal;
    public GameObject other_goal;
    public GameObject ball;
    public int id;

    private Node behaviourTree;
    private Context behaviourState;

    public List<Vector3> velocities_blue = new List<Vector3> ();
    public List<Vector3> velocities_red = new List<Vector3> ();
    public List<Vector3> positions_blue = new List<Vector3> ();
    public List<Vector3> positions_red = new List<Vector3> ();
    public List<Vector3> positions_blue_old = new List<Vector3> ();
    public List<Vector3> positions_red_old = new List<Vector3> ();
    public Vector3 velocity_ball = Vector3.zero;
    public Vector3 position_ball;
    public Vector3 position_ball_old;

    private void Start () {
        // get the car controller
        m_Drone = GetComponent<DroneController> ();
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager> ();

        behaviourTree = CreateBehaviourTree ();
        behaviourState = new Context (this);

        // note that both arrays will have holes when objects are destroyed
        // but for initial planning they should work
        friend_tag = gameObject.tag;
        if (friend_tag == "Blue")
            enemy_tag = "Red";
        else
            enemy_tag = "Blue";

        friends = GameObject.FindGameObjectsWithTag (friend_tag);
        enemies = GameObject.FindGameObjectsWithTag (enemy_tag);
        ball = GameObject.FindGameObjectWithTag ("Ball");

        foreach (GameObject car in friends) {
            positions_red.Add (car.transform.position);
            positions_red_old.Add (car.transform.position);
            velocities_red.Add (Vector3.zero);
        }

        foreach (GameObject car in enemies) {
            positions_blue.Add (car.transform.position);
            positions_blue_old.Add (car.transform.position);
            velocities_blue.Add (Vector3.zero);
        }

        position_ball = ball.transform.position;
        position_ball_old = position_ball;
    }

    private void FixedUpdate () {
        for (int i = 0; i < 3; i++) {
            positions_red[i] = friends[i].transform.position;
            velocities_red[i] = (positions_red[i] - positions_red_old[i]) / 0.2f;
            positions_red_old[i] = positions_red[i];
        }

        for (int i = 0; i < 3; i++) {
            positions_blue[i] = enemies[i].transform.position;
            velocities_blue[i] = (positions_blue[i] - positions_blue_old[i]) / 0.2f;
            positions_blue_old[i] = positions_blue[i];
        }

        position_ball = ball.transform.position;
        velocity_ball = (position_ball - position_ball_old) / 0.2f;
        position_ball_old = position_ball;

        behaviourTree.Behave (behaviourState);
    }

    private Node CreateBehaviourTree () {

        // goalie branch
        Sequence defendAndAttack = new Sequence ("defendAndAttack",
            new ballWithinGoalDistance (),
            new behindBall (),
            new targetBallGoalie ()
        );

        Sequence defendAndIntercept = new Sequence ("defendAndIntercept",
            new ballMovesTowardsGoal (),
            new ballHasBearingTowardsGoal (),
            new Inverter (new ballWithinGoalDistance ()),
            new intercept ()
        );

        Selector decideGoalieAction = new Selector ("decideGoalieAction",
        	// defendAndIntercept,
            defendAndAttack,
            new takeDefaultPosition ()
        );

        Sequence goalieBranch = new Sequence ("goalieBranch",
            new isGoalie (),
            decideGoalieAction
        );

        // attacker branch

        Sequence shoot = new Sequence ("shoot",
            new correctShootingAngle (),
            new targetBallShoot ()
        );

        Sequence positionForShootingIfBehind = new Sequence ("positionForShootingIfBehind",
            new behindBall (),
            new positionForShooting ()
        );

        Selector takeShootingPosition = new Selector ("takeShootingPosition",
            positionForShootingIfBehind,
            new goBehindBall ()
        );

        Selector checkIfCanAttack = new Selector ("checkIfCanAttack",
            shoot,
            takeShootingPosition
        );

        Sequence attackBall = new Sequence ("attackBall",
            new isAttacker (),
            checkIfCanAttack
        );

        Selector attackerBranch = new Selector ("attackBranch",
            attackBall,
            new blockOpponent ()
        );

        Selector behave = new Selector ("behave",
            goalieBranch,
            attackerBranch
        );

        Repeater repeater = new Repeater (behave);

        return repeater;
    }
}
//}
