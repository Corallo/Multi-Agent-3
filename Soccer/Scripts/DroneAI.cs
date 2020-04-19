using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MyGraph;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(DroneController))]
public class DroneAI : MonoBehaviour
{

    private DroneController m_Drone; // the car controller we want to use

    public GameObject my_goal_object;
    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;

    public List<Vector3> my_path = new List<Vector3>();
    public int my_path_length;
    public int skipper;
    public int lastPointInPath=0;
    public  Vector3 myVector;
    public int warning;
   // SphereCollider droneCollider;

    private void Start()
    {
        // get the drone controller
        m_Drone = GetComponent<DroneController>();
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
        Graph G;
        //droneCollider = GetComponent<SphereCollider>();
        Vector3 start_pos = transform.position;//terrain_manager.myInfo.start_pos;
        Vector3 goal_pos = my_goal_object.transform.position;//terrain_manager.myInfo.goal_pos;
        Debug.Log(start_pos);
        Debug.Log(goal_pos);
        Planner1 p=new Planner1();
        my_path=p.ComputePath(terrain_manager,start_pos,goal_pos);
        lastPointInPath = 0;
        my_path_length = my_path.Count;
        warning = 21;
        // Plan your path here
        // ...
        /*my_path.Add(start_pos);

        for (int i = 0; i < 3; i++)
        {
            Vector3 waypoint = start_pos + new Vector3(UnityEngine.Random.Range(-50.0f, 50.0f), 0, UnityEngine.Random.Range(-30.0f, 30.0f));
            my_path.Add(waypoint);
        }
        my_path.Add(goal_pos);
        */


        // Plot your path to see if it makes sense
        Vector3 old_wp = start_pos;
        foreach (var wp in my_path)
        {
            //Debug.DrawLine(old_wp, wp, Color.red, 100f);
            old_wp = wp;
        }

        
    }

    /*
    private void FixedUpdate()
    {
        // Execute your path here
        // ...

        // this is how you access information about the terrain
        int i = terrain_manager.myInfo.get_i_index(transform.position.x);
        int j = terrain_manager.myInfo.get_j_index(transform.position.z);
        float grid_center_x = terrain_manager.myInfo.get_x_pos(i);
        float grid_center_z = terrain_manager.myInfo.get_z_pos(j);

        //Debug.DrawLine(transform.position, new Vector3(grid_center_x, 0f, grid_center_z), Color.white, 1f);

        
        Vector3 relVect = my_goal_object.transform.position - transform.position;

        m_Drone.Move_vect(relVect);

        

    }
    */
    private void FixedUpdate() {

        if (skipper > 0)
        {
            skipper--;
            return;
        }

        if (warning == 500)
        {
            Start();
            warning = 0;
            return;
        }

        if (m_Drone.velocity.magnitude < 1 && lastPointInPath<my_path.Count-2)
        {
            warning++;
        }

        if (warning % 100 < 20) //It is stuck since short time, make it move randomly
        {
            if (warning % 100 == 0) //first time here, choise the random position
            {
                 myVector = new Vector3(UnityEngine.Random.Range(0, 10), 1, UnityEngine.Random.Range(0, 10)).normalized;
            }

            m_Drone.Move(myVector.x,myVector.z);
        }
        int i;
        float radiusMargin =  4f;
        DroneController controller = transform.GetComponent<DroneController>();
        RaycastHit rayHit;

        /*for (; lastPointInPath < my_path.Count && !((bool)Physics.SphereCast(transform.position, radiusMargin, my_path[lastPointInPath].getPosition() - transform.position, out rayHit, Vector3.Distance(my_path[lastPointInPath].getPosition(), transform.position)))
         ; lastPointInPath++); // increase to find the farest node
        lastPointInPath--;
        */
        Vector3 target = my_path[lastPointInPath];

        int targetId = 0;
        float minDistance = 100;
        float actualDistance = 100;

        var myMask = ~LayerMask.GetMask("Drone"); // ~LayerMask.GetMask("Terrain");
        var ufo=Physics.OverlapSphere(transform.position, 3f,myMask);

        Vector3 closerFriend=Vector3.up;
        float closerDist=9999;
        float tmpDist;
        int flag = 0;
        Rigidbody friendGameObject=null;
        foreach (var d in ufo)
        {
            if (d.attachedRigidbody != null)
            {
                Vector3 friend_pos = d.attachedRigidbody.transform.position;
                if (friend_pos != transform.position)//it is not myself
                {
                    tmpDist = Vector3.Distance(friend_pos, transform.position);
                    if (tmpDist < closerDist)
                    {
                        closerDist = tmpDist;
                        flag = 1;
                        closerFriend = friend_pos;
                        friendGameObject = d.attachedRigidbody;
                    }

                    /*
                    if (d.attachedRigidbody.velocity.magnitude > m_Drone.velocity.magnitude || m_Drone.velocity.magnitude<0.5) { 
                    Debug.Log("Avoiding collision");
                    m_Drone.Move((transform.position-friend_pos).normalized.x , (transform.position - friend_pos).normalized.z);
                    return;
                    }
                    */
                }
            }
        }

        if (flag==1)
        {
            m_Drone.Move((transform.position - closerFriend).normalized.x, (transform.position - closerFriend).normalized.z);
            return;
            /*
            if (friendGameObject.velocity.magnitude > m_Drone.velocity.magnitude )
            { 
               // m_Drone.Move((transform.position - closerFriend).normalized.x,(transform.position - closerFriend).normalized.z);
                //m_Drone.Move(friendGameObject.velocity.x, friendGameObject.velocity.z);

                return;
            }*/
        }

        
        //ufo = Physics.OverlapSphere(transform.position, 10f, myMask);
        //Vector3 dir = target - transform.position;
        /*
        foreach (var d in ufo){
            if (d.attachedRigidbody != null){
                Vector3 friend_pos = d.attachedRigidbody.transform.position;
                if (friend_pos != transform.position) //it is not myself
                {
                    Vector3 v = friend_pos - transform.position;
                    float ang = Vector3.SignedAngle(target, v, Vector3.up);
                    if (ang > 10 && ang < 90)
                    {
                        Debug.Log("Giving Precedence");
                        m_Drone.Move(-controller.velocity.x * 100, -controller.velocity.z * 100);
                        return;
                    }
                }
            }
        }*/


        for (i = 0; i < my_path.Count; i++) {
            actualDistance = Vector3.Distance(my_path[i], transform.position);
            bool hitA = Physics.SphereCast(transform.position, 1, my_path[i] - transform.position, out rayHit, actualDistance);
            //bool hitB = Physics.Raycast(transform.position, my_path[i] - transform.position, actualDistance);

            if (minDistance > actualDistance && rayHit.collider==null) {
                lastPointInPath = i;
                minDistance = actualDistance;
            }
        }
        Debug.Log("I am close to node " + lastPointInPath.ToString());


        if (lastPointInPath != my_path.Count - 1) {
            target = my_path[lastPointInPath + 1];
            targetId = lastPointInPath + 1;
            /*
            for (i = lastPointInPath + 1; i < my_path.Count; i++) {
                float newDistance = Vector3.Distance(transform.position, my_path[i]);
                float distanceToTargetTemp = Vector3.Distance(transform.position, target);
                if (distanceToTargetTemp > newDistance) {
                    distanceToTargetTemp = newDistance;
                    target = my_path[i];
                    targetId = i;
                }
            }*/
            /*
            myMask = LayerMask.GetMask("Drone");
           
            if (lastPointInPath != 0 && Physics.Raycast(my_path[lastPointInPath], my_path[lastPointInPath] - transform.position, Vector3.Distance(my_path[lastPointInPath], transform.position))) {
                Debug.Log("Path is blocked by wall");
                lastPointInPath--;
            }
            */
            /* for (i=lastPointInPath+1; i < my_path.Count() ; i++)
             {
                 var highdronePosition = transform.position;
                 var lowdronePosition = transform.position;
                 var dronePosition = transform.position;
                 var pointDirection = my_path[i].getPosition();
                 highdronePosition.y += 1.1f;
                 lowdronePosition.y -= 0.9f;
                 //pointDirection.y += 1;
                 Physics.SphereCast(dronePosition, droneCollider.radius, (pointDirection - dronePosition).normalized  , out rayHit, Vector3.Distance(pointDirection,dronePosition));
                 //bool hit2=Physics.CapsuleCast(highdronePosition, lowdronePosition, droneCollider.radius*2, pointDirection - highdronePosition, Vector3.Distance(highdronePosition, pointDirection));
                 Debug.DrawRay(transform.position, (my_path[i].getPosition() - transform.position), Color.cyan, 10f);
                 //if (rayHit.collider.gameObject.name == "Cube") { 

                 if (rayHit.collider != null){
                 //if (Physics.Raycast(my_path[i].getPosition(),  my_path[i].getPosition()- transform.position, Vector3.Distance(my_path[i].getPosition(), transform.position))) { 
                     break;
                 }
             }
             i--;
             target = my_path[i].getPosition();
             targetId = i;
             */
        } else {
            target = my_path[lastPointInPath];
            targetId = lastPointInPath;
        }


        Debug.Log("Going to node n." + targetId.ToString());
        float breakingDistance = (controller.velocity.magnitude * controller.velocity.magnitude) / (controller.max_acceleration);
        float distanceToTarget = Vector3.Distance(transform.position, target);
        Debug.Log("Distance to target: " + distanceToTarget.ToString());

        //ADD offset to the right
        if (lastPointInPath > 0)
        {
            if (target.x == my_path[lastPointInPath].x)
            {
                if (target.z > my_path[lastPointInPath].z)
                {
                    target.x += 4f;
                }
                else
                {
                    target.x -= 4f;
                }
            }
            else
            {
                if (target.x > my_path[lastPointInPath].x)
                {
                    target.z -= 4f;
                }
                else
                {
                    target.z += 4f;
                }
            }
        }



        Vector3 direction = target - transform.position; //Direction to target!
        myMask = ~LayerMask.GetMask("Drone");
        bool hit = Physics.SphereCast(transform.position, radiusMargin, controller.velocity, out rayHit, breakingDistance,myMask);
        if (hit) // if my path is blocked by another drone
        {
            ufo = Physics.OverlapSphere(transform.position, 5f, myMask);

            closerFriend = Vector3.up;
            closerDist = 9999;

            flag = 0;
            friendGameObject = null;
            foreach (var d in ufo)
            {
                if (d.attachedRigidbody != null)
                {
                    Vector3 friend_pos = d.attachedRigidbody.transform.position;
                    if (friend_pos != transform.position) //it is not myself
                    {
                        tmpDist = Vector3.Distance(friend_pos, transform.position);
                        if (tmpDist < closerDist)
                        {
                            closerDist = tmpDist;
                            flag = 1;
                            closerFriend = friend_pos;
                            friendGameObject = d.attachedRigidbody;
                        }



                    }
                }
            }

            if (flag == 1)
            {
                if (friendGameObject.velocity.magnitude > m_Drone.velocity.magnitude &&
                    m_Drone.velocity.magnitude > 0.5)
                {
                    //m_Drone.Move((transform.position - closerFriend).normalized.x, (transform.position - closerFriend).normalized.z);
                    m_Drone.Move(friendGameObject.velocity.x, friendGameObject.velocity.z);

                    return;
                }
            }

        }

        direction = target - transform.position; //Direction to target!
         hit = Physics.SphereCast(transform.position, radiusMargin, controller.velocity, out rayHit, breakingDistance);
        if (rayHit.collider != null) //Slow down if we are going to hit something
        {
            Debug.Log("Breaking distance: " + breakingDistance.ToString());
            Debug.Log("OMG OMG SLOW NOW");
            //rayHit.collider.transform.position;

            m_Drone.Move((transform.position - rayHit.collider.transform.position).normalized.x, (transform.position - rayHit.collider.transform.position).normalized.z);

            // m_Drone.Move(-controller.velocity.x * 100, -controller.velocity.z * 100);
            //Debug.DrawLine(transform.position, transform.position + controller.acceleration, Color.black);
        } else {
            /*if ( breakingDistance >= distanceToTarget)
            {
                m_Drone.Move(-controller.velocity.x * 100, -controller.velocity.z * 100);
            }*/
            /*
            if (targetId == my_path.Count - 1 && breakingDistance >= distanceToTarget) {
                m_Drone.Move(-controller.velocity.x * 100, -controller.velocity.z * 100);
            } else if (controller.velocity.magnitude > 3 && targetId < my_path.Count - 2 && ((my_path[targetId].getTheta() < 130) || (my_path[targetId + 1].getTheta() < 130) || (my_path[targetId + 2].getTheta() < 130)) && breakingDistance >= distanceToTarget * 3) {
                m_Drone.Move(-controller.velocity.x * 100, -controller.velocity.z * 100);
            } else if (controller.velocity.magnitude > 3 && targetId < my_path.Count - 2 && ((my_path[targetId].getTheta() < 110) || (my_path[targetId + 1].getTheta() < 110) || (my_path[targetId + 2].getTheta() < 110)) && breakingDistance >= distanceToTarget * 2) {
                m_Drone.Move(-controller.velocity.x * 100, -controller.velocity.z * 100);
            } else if (controller.velocity.magnitude > 3 && targetId < my_path.Count - 2 && ((my_path[targetId].getTheta() < 90) || (my_path[targetId + 1].getTheta() < 90) || (my_path[targetId + 2].getTheta() < 90)) && breakingDistance >= distanceToTarget * 0.7) {
                m_Drone.Move(-controller.velocity.x * 100, -controller.velocity.z * 100);
            } else {*/
                Debug.DrawLine(transform.position, transform.position + direction, Color.yellow); //drawing the direction, and it is alwaise correct.
                                                                                                  //Add speed compensation
                Debug.Log("Angle between speed and acc " + Vector3.Angle(direction, controller.velocity).ToString());
                if (controller.velocity.magnitude > 0.5 && Vector3.Angle(direction, controller.velocity) > 1 && Vector3.Angle(direction, controller.velocity) < 50) {
                    Vector3 specularSpeed = -controller.velocity + 2 * Vector3.Dot(controller.velocity, direction.normalized) * direction.normalized;
                    Debug.DrawLine(transform.position, transform.position + specularSpeed, Color.white);
                    direction = direction.normalized + 3 * specularSpeed.normalized;
                } else if (controller.velocity.magnitude > 0.5 && Vector3.Angle(direction, controller.velocity) >= 50) {
                    Debug.Log("Wrong direction, fixing speed of magnitudo " + controller.velocity.magnitude);
                    direction = -controller.velocity * 100;
                    //m_Drone.Move(-controller.velocity.x * 100, -controller.velocity.z * 100);
                }
                //  m_Drone.Move  HERE YOU SHOULD DO ACCELLERATION IN THE DIRECTION OF THE POINT 
                // IF SPEED.DIRECTION != ACCELLERATION. DIRECTION
                // COMPENSATE THE SPEED COMPLEMENTING IT WITH SOME ACCELLERATION IN SPECULAR (NON OPPOSITE) DIRECTION.
                Vector3 high = new Vector3(0, 1, 0);
                Vector3 side1 = Vector3.Cross(direction, high).normalized;
                Vector3 side2 = -side1;
                bool hitside1 = Physics.Raycast(transform.position, side1, 4);
                bool hitside2 = Physics.Raycast(transform.position, side2, 4);
                if (hitside1 && !hitside2) {
                    direction = direction + side2;
                } else if (hitside2 && !hitside1) {
                    direction = direction + side1;
                }

                m_Drone.Move(direction.x, direction.z);
            //}
        }

        Debug.DrawLine(transform.position, transform.position + controller.velocity, Color.red);
        //Debug.DrawLine(transform.position, transform.position + controller.acceleration, Color.black);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
