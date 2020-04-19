using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions : MonoBehaviour {

    public GameObject agent_blue_1;
    public GameObject agent_blue_2;
    public GameObject agent_blue_3;
    public GameObject agent_red_1;
    public GameObject agent_red_2;
    public GameObject agent_red_3;
    public GameObject ball;

    public List<Vector3> velocities_blue = new List<Vector3> ();
    public List<Vector3> velocities_red = new List<Vector3> ();
    public List<Vector3> positions_blue = new List<Vector3> ();
    public List<Vector3> positions_red = new List<Vector3> ();
    public Vector3 velocity_ball = Vector3.zero;
    public Vector3 position_ball;
    public Vector3 position_ball_old;
    private Vector3 position_b1_old;
    private Vector3 position_b2_old;
    private Vector3 position_b3_old;
    private Vector3 position_r1_old;
    private Vector3 position_r2_old;
    private Vector3 position_r3_old;

    // Use this for initialization
    void Start () {
        positions_blue.Add (agent_blue_1.transform.position);
        positions_blue.Add (agent_blue_2.transform.position);
        positions_blue.Add (agent_blue_3.transform.position);
        positions_red.Add (agent_red_1.transform.position);
        positions_red.Add (agent_red_2.transform.position);
        positions_red.Add (agent_red_3.transform.position);

        position_b1_old = agent_blue_1.transform.position;
        position_b2_old = agent_blue_2.transform.position;
        position_b3_old = agent_blue_3.transform.position;
        position_r1_old = agent_red_1.transform.position;
        position_r2_old = agent_red_2.transform.position;
        position_r3_old = agent_red_3.transform.position;

        velocities_blue.Add (Vector3.zero);
        velocities_blue.Add (Vector3.zero);
        velocities_blue.Add (Vector3.zero);
        velocities_red.Add (Vector3.zero);
        velocities_red.Add (Vector3.zero);
        velocities_red.Add (Vector3.zero);

    }

    // Use this for initialization
    void Awake () {

    }

    // Update is called once per frame
    void Update () {
        positions_blue[0] = agent_blue_1.transform.position;
        positions_blue[1] = agent_blue_2.transform.position;
        positions_blue[2] = agent_blue_3.transform.position;
        positions_red[0] = agent_red_1.transform.position;
        positions_red[1] = agent_red_2.transform.position;
        positions_red[2] = agent_red_3.transform.position;
        position_ball = ball.transform.position;

        velocities_blue[0] = (positions_blue[0] - position_b1_old) / 0.2f;
        velocities_blue[1] = (positions_blue[1] - position_b2_old) / 0.2f;
        velocities_blue[2] = (positions_blue[2] - position_b3_old) / 0.2f;
        velocities_red[0] = (positions_red[0] - position_r1_old) / 0.2f;
        velocities_red[1] = (positions_red[1] - position_r2_old) / 0.2f;
        velocities_red[2] = (positions_red[2] - position_r3_old) / 0.2f;
        velocity_ball = (position_ball - position_ball_old) / 0.2f;

        position_b1_old = positions_blue[0];
        position_b2_old = positions_blue[1];
        position_b3_old = positions_blue[2];
        position_r1_old = positions_red[0];
        position_r2_old = positions_red[1];
        position_r3_old = positions_red[2];
        position_ball_old = position_ball;
    }
}