﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSoccer : MonoBehaviour {

    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;

    public GameObject race_car;

    public GameObject blue_goal;
    public GameObject red_goal;

    public int blue_score;
    public int red_score;

    public GameObject ball;
    //public GameObject spawn_point_ball;

    float start_time;
    public float match_time;
    public float match_length = 100f;
    public float goal_tolerance = 10.0f;
    public bool finished = false;
    public int no_of_cars = 6;

    public List<GameObject> my_cars;
    public GameObject agent_blue_1;
    public GameObject agent_blue_2;
    public GameObject agent_blue_3;
    public GameObject agent_red_1;
    public GameObject agent_red_2;
    public GameObject agent_red_3;

    // Use this for initialization
    void Awake () {

        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager> ();

        start_time = Time.time;

        //race_car.transform.position = terrain_manager.myInfo.start_pos + 2f * Vector3.up;
        //race_car.transform.rotation = Quaternion.identity;

    }

    void Start () {
        start_time = Time.time;

        my_cars = new List<GameObject> ();
        my_cars.Add(agent_blue_1);
        my_cars.Add(agent_blue_2);
        my_cars.Add(agent_blue_3);
        my_cars.Add(agent_red_1);
        my_cars.Add(agent_red_2);
        my_cars.Add(agent_red_3);


        //race_car.transform.position = terrain_manager.myInfo.start_pos + 2f * Vector3.up;
        //race_car.transform.rotation = Quaternion.identity;

        blue_goal.transform.position = terrain_manager.myInfo.start_pos;
        red_goal.transform.position = terrain_manager.myInfo.goal_pos;

        agent_blue_1.transform.position = GetCollisionFreePosNear(CircularConfiguration (0 + 3, 6, 0.2f), 10f);
        agent_blue_2.transform.position = GetCollisionFreePosNear(CircularConfiguration (1 + 3, 6, 0.2f), 10f);
        agent_blue_3.transform.position = GetCollisionFreePosNear(CircularConfiguration (2 + 3, 6, 0.2f), 10f);
        agent_red_1.transform.position = GetCollisionFreePosNear(CircularConfiguration (3 + 3, 6, 0.2f), 10f);
        agent_red_2.transform.position = GetCollisionFreePosNear(CircularConfiguration (4 + 3, 6, 0.2f), 10f);
        agent_red_3.transform.position = GetCollisionFreePosNear(CircularConfiguration (5 + 3, 6, 0.2f), 10f);


        agent_blue_1.transform.Find("Sphere").gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        agent_blue_2.transform.Find("Sphere").gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        agent_blue_3.transform.Find("Sphere").gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        agent_red_1.transform.Find("Sphere").gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        agent_red_2.transform.Find("Sphere").gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        agent_red_3.transform.Find("Sphere").gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    // Update is called once per frame
    void Update () {
        if (!finished) {
            match_time = Time.time - start_time;
            //Debug.Log(ball.GetComponent<GoalCheck>().blue_score);
            blue_score = ball.GetComponent<GoalCheck> ().blue_score;
            red_score = ball.GetComponent<GoalCheck> ().red_score;
            if (match_time > match_length) {
                finished = true;
            }

        }

    }

    Vector3 CircularConfiguration (int i, int max_i, float factor_k) {
        float center_x = (terrain_manager.myInfo.x_high + terrain_manager.myInfo.x_low) / 2.0f;
        float center_z = (terrain_manager.myInfo.z_high + terrain_manager.myInfo.z_low) / 2.0f;
        Vector3 center = new Vector3 (center_x, 1f, center_z);

        float alpha = i * 2.0f * Mathf.PI / max_i;
        float r = (terrain_manager.myInfo.x_high - center_x) * factor_k;
        return center + new Vector3 (Mathf.Sin (alpha), 0f, Mathf.Cos (alpha)) * r;
    }

    Vector3 GetCollisionFreePosNear (Vector3 startPos, float max_dist) {

        if (terrain_manager.myInfo.is_traverable (startPos))
            return startPos;

        for (int k = 0; k <= 100; k++) {
            Vector3 delta_pos = new Vector3 (Random.Range (0f, max_dist), 0f, Random.Range (0f, max_dist));
            if (terrain_manager.myInfo.is_traverable (startPos + delta_pos))
                return startPos + delta_pos;
        }

        return Vector3.zero;
    }
}