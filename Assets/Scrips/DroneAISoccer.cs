using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (DroneController))]
public class DroneAISoccer : MonoBehaviour {
    public DroneController m_Drone; // the drone controller we want to use

    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;

    public GameObject directions_game_object;
    Directions directions;

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



    private void Start () {
        // get the car controller
        m_Drone = GetComponent<DroneController> ();
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager> ();

        friend_tag = gameObject.tag;
        if (friend_tag == "Blue") {
            enemy_tag = "Red";
        } else {
            enemy_tag = "Blue";
        }

        directions = directions_game_object.GetComponent<Directions> ();

        behaviourTree = friend_tag == "Red" ? CreateBehaviourTreeRed () : CreateBehaviourTreeBlue ();
        behaviourState = new Context (this, directions);

        friends = GameObject.FindGameObjectsWithTag (friend_tag);
        enemies = GameObject.FindGameObjectsWithTag (enemy_tag);
    }

    private void FixedUpdate () {
        behaviourTree.Behave (behaviourState);
    }

        private Node CreateBehaviourTreeRed () {

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
            //defendAndIntercept,
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

        Sequence positionForShootingIfBehind = new Sequence("positionForShootingIfBehind",
            new behindBall(),
            new positionForShooting()
        );

        Selector takeShootingPosition = new Selector("takeShootingPosition",
            positionForShootingIfBehind,
            new goBehindBall()
        );

        Selector checkIfCanAttack = new Selector("checkIfCanAttack",
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

    private Node CreateBehaviourTreeBlue () {

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
            //defendAndIntercept,
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

        Sequence attackBall = new Sequence ("attackBall",
            new isAttacker (),
            shoot,
            new positionForShooting ()
        );

        Selector attackerBranch = new Selector ("attackBranch",
            attackBall,
            new blockOpponent ()
        );

        Selector behave = new Selector ("behave",
            goalieBranch
        );

        Repeater repeater = new Repeater (behave);

        return repeater;
    }
}