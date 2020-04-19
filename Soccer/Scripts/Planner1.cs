using MyGraph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

public class Planner1 {

    public List<HashSet<int>> sets = new List<HashSet<int>>();
    public TerrainManager terrain_manager;
    //private Graph DroneGraph;
    public GameObject[] friends;
    public GameObject[] enemies;
    private CarController m_Car;
    //public LayerMask mask;
    private float Margin = 0;

    public List<Vector3> ComputePath(TerrainManager _terrain_manager,Vector3 start_p,Vector3 goal_p)
    {
        terrain_manager = _terrain_manager.GetComponent<TerrainManager>();
        Graph DroneGraph = new Graph();
        var watch = System.Diagnostics.Stopwatch.StartNew();
        // the code that you want to measure comes here

        DroneGraph = generateGraph(DroneGraph);
        watch.Stop();
        var elapsedMs = watch.Elapsed;
        Debug.Log("Time to create Graph:" + elapsedMs.ToString());
        friends = GameObject.FindGameObjectsWithTag("Player");
        //enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //m_Car = friends[0].GetComponent<CarController>();

        List<GraphNode> my_path = new List<GraphNode>();

        LayerMask mask = LayerMask.GetMask("Drone") & LayerMask.GetMask("Terrain");

        for (int i = 0; i < DroneGraph.getSize(); i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Collider c = cube.GetComponent<Collider>();
            c.enabled = false;
            Vector3 position = DroneGraph.getNode(i).getPosition();

            //DestroyImmediate(cube.GetComponent<Collider>());
            cube.transform.position = new Vector3(position.x, 1, position.z);
            cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
          /*  for (int k = 0; k < DroneGraph.getAdjList(i).Count; k++)
            {
                Debug.DrawLine(DroneGraph.getNode(i).getPosition(),
                    DroneGraph.getNode(DroneGraph.getAdjList(i)[k]).getPosition(), Color.yellow, 100f);
            }
            */
        }

        Vector3 start_pos = start_p;
        Vector3 goal_pos = goal_p;

        int start_idx = DroneGraph.FindClosestNode(start_pos, DroneGraph).getId();
        int goal_idx = DroneGraph.FindClosestNode(goal_pos, DroneGraph).getId();

        Debug.Log(start_idx);
        Debug.Log(goal_idx);
        ASuperStar(DroneGraph, start_idx, goal_idx);
        List<int> tmp_path = getBestPath(DroneGraph, start_idx, goal_idx);

        List<Vector3> path = new List<Vector3>();
        foreach (var element in tmp_path)
        {
            path.Add(DroneGraph.getNode(element).getPosition());
            //Debug.Log(element);
        }
        path.Add(goal_pos);
        watch.Stop();
        elapsedMs = watch.Elapsed;
        Debug.Log("Time to compute A Star" + elapsedMs.ToString());
        //my_path = pathHelp(DroneGraph, path);


       


        /*SUM COST OF A STAR */
        

        //Vector3 old_wp = DroneGraph.getNode(best_path[0][0]).getPosition();
        Vector3 old_wp = path[0];

        Color col = new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
        );
         
        foreach (var wp in path) {
            //Debug.Log(Vector3.Distance(old_wp, DroneGraph.getNode(wp).getPosition()));
            Debug.DrawLine(old_wp, wp, col, 10000f);
            old_wp = wp;
        }
        return path;
        /*
        old_wp = DroneGraph.getNode(best_path[1][0]).getPosition();
        foreach (var wp in best_path[1]) {
            //Debug.Log(Vector3.Distance(old_wp, DroneGraph.getNode(wp).getPosition()));
            Debug.DrawLine(old_wp, DroneGraph.getNode(wp).getPosition(), Color.yellow, 10000f);
            old_wp = DroneGraph.getNode(wp).getPosition();
        }
        old_wp = DroneGraph.getNode(best_path[2][0]).getPosition();

        foreach (var wp in best_path[2]) {
            //Debug.Log(Vector3.Distance(old_wp, DroneGraph.getNode(wp).getPosition()));
            Debug.DrawLine(old_wp, DroneGraph.getNode(wp).getPosition(), Color.blue, 10000f);
            old_wp = DroneGraph.getNode(wp).getPosition();
        }

        

        
        */

    }

    float computemanhattnCost(List<int> path, Graph G) {
        float cost = 0;
        for (int i = 0; i < path.Count - 1; i++) {
            Vector3 a = G.getNode(i).getPosition();
            Vector3 b = G.getNode(i + 1).getPosition();
            cost += Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
        return cost;
    }

    public void ASuperStar(Graph G, int start_pos, int idx_goal) {
        priorityQueue Q = new priorityQueue();

        for (int i = 0; i < G.getSize(); i++) {
            G.setColorOfNode(i, 0);
            G.getNode(i).setParent(-1);
        }
        G.setColorOfNode(start_pos, 1);
        int best_GraphNode;
        float best_cost;
        //Debug.Log("RUNNING A STAR!"+ start_pos.ToString() +" " + idx_goal.ToString());

        float total_cost;
        Q.enqueue(start_pos, 0);


        while (Q.getSize() != 0) {
            best_GraphNode = Q.dequeue();
            best_cost = Q.getCost(best_GraphNode);
            //Delete GraphNode
            Q.removeGraphNode(best_GraphNode);

            if (idx_goal == best_GraphNode) {
                //Debug.Log("path found");
                return;
            }

            foreach (int child in G.getAdjList(best_GraphNode)) {
                total_cost = computeCost(G, best_GraphNode, child, idx_goal) + best_cost;

                if (Q.isInQueue(child)) {
                    if (Q.getCost(child) > total_cost) {
                        G.getNode(child).setParent(best_GraphNode);
                        Q.updateCost(child, total_cost);
                    }
                } else {
                    if (G.getNode(child).getColor() == 0) {
                        Q.enqueue(child, total_cost);
                        G.getNode(child).setParent(best_GraphNode);
                        G.getNode(child).setColor(1);
                    }
                }


            }
        }

        Debug.Log("path not found!!!!");

    }

    public float computePathCost(Graph G, List<int> _path) {
        float cost = 0;
        for (int i = 0; i < _path.Count - 1; i++) {
            cost += Vector3.Distance(G.getNode(_path[i]).getPosition(), G.getNode(_path[i + 1]).getPosition());

        }
        return cost;

    }


    public List<int> getBestPath(Graph G, int idx_start, int idx_goal) {
        List<int> path = new List<int>();
        path.Add(idx_goal);
        int idx = idx_goal;
        //Debug.Log("Following path from A star");
        while (idx != idx_start) {
            if (G.getNode(idx).getParent() == -1) {
                // Debug.Log("I am in GraphNode" + idx.ToString());
            }
            idx = G.getNode(idx).getParent();
            path.Add(idx);
            if (path.Count > 10000) {
                Debug.Log("OMG WTF?");
                break;
            }
        }
        path.Reverse();
        //Debug.Log("Path found:");
        //var strings = string.Join(", ", path);
        //Debug.Log(strings);
        return path;
    }

    public class priorityQueue {
        List<int> values;
        List<float> priority;

        public priorityQueue() {
            values = new List<int>();
            priority = new List<float>();
        }
        public void enqueue(int _value, float _p) {
            values.Add(_value);
            priority.Add(_p);
        }
        public int getSize() {
            return values.Count;
        }
        public int dequeue() {
            int best_idx = priority.IndexOf(priority.Min());
            int best_GraphNode = values[best_idx];


            return best_GraphNode;
        }
        public void removeGraphNode(int GraphNode) {
            int GraphNode_idx = values.IndexOf(GraphNode);
            priority.RemoveAt(GraphNode_idx);
            values.RemoveAt(GraphNode_idx);
        }
        public float getCost(int GraphNode) {
            int idx = values.IndexOf(GraphNode);
            return priority[idx];
        }
        public void updateCost(int GraphNode, float p) {
            int idx = values.IndexOf(GraphNode);
            priority[idx] = p;
        }
        public bool isInQueue(int GraphNode) {
            int idx = values.IndexOf(GraphNode);
            if (idx == -1) { return false; }
            return true;
        }
    }

    public float computeCost(Graph G, int parent, int child, int goal) {
        //REAL COST:
        LayerMask mask = LayerMask.GetMask("Drone") & LayerMask.GetMask("Terrain"); //LayerMask.GetMask("CubeWalls");
        float real_cost;
        float h_cost;
        float zero = 0;
        float k = 1 / zero;

        real_cost = Vector3.Distance(G.getNode(parent).getPosition(), G.getNode(child).getPosition());




        //real_cost = Math.Max(real_cost, k * (real_cost - 10));

        RaycastHit rayHit;
        bool hit = Physics.SphereCast(G.getNode(child).getPosition(), Margin, G.getNode(goal).getPosition() - G.getNode(child).getPosition(), out rayHit, Vector3.Distance(G.getNode(child).getPosition(), G.getNode(goal).getPosition()), mask);
        if (!hit) {
            h_cost = 0;
        } else {
            h_cost = Vector3.Distance(G.getNode(child).getPosition(), G.getNode(goal).getPosition());
        }


        return 3 * real_cost + h_cost; //+ (200f / G.getNode(child).getDistanceToWall());

    }

    public void computeDiStanceToWall(Graph G) {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Drone") & LayerMask.GetMask("Terrain");
        List<Vector3> radiusHelpMatrix = new List<Vector3>();

        radiusHelpMatrix.Add(new Vector3(-1f, -1f, -1f));
        radiusHelpMatrix.Add(new Vector3(1f, 1f, 1f));
        radiusHelpMatrix.Add(new Vector3(1f, 1f, -1f));
        radiusHelpMatrix.Add(new Vector3(1f, -1f, 1f));
        radiusHelpMatrix.Add(new Vector3(1f, -1f, -1f));
        radiusHelpMatrix.Add(new Vector3(-1f, 1f, 1f));
        radiusHelpMatrix.Add(new Vector3(-1f, 1f, -1f));
        radiusHelpMatrix.Add(new Vector3(-1f, -1f, 1f));

        for (int i = 0; i < G.getSize(); i++) {
            float minDistance = 5000f;
            float actualDistance;
            for (int j = 0; j < radiusHelpMatrix.Count; j++) {
                Physics.SphereCast(G.getNode(i).getPosition(), 2, radiusHelpMatrix[j], out hit, 50f, mask);
                actualDistance = hit.distance;
                if (actualDistance != 0) {
                    if (minDistance > actualDistance) {
                        minDistance = actualDistance;
                    }
                }
            }
            G.getNode(i).setDistanceToWall(minDistance);
            //Debug.Log(minDistance);
        }
    }


    List<GraphNode> pathHelp(Graph G, List<int> idList) {
        List<GraphNode> GraphNodeList = new List<GraphNode>();
        foreach (int i in idList) {
            GraphNodeList.Add(G.getNode(i));
        }
        return GraphNodeList;
    }

    Graph generateGraph(Graph G) {
        LayerMask mask = LayerMask.GetMask("Drone") & LayerMask.GetMask("Terrain");
        int col = terrain_manager.myInfo.x_N;
        int row = terrain_manager.myInfo.z_N;
        for (int i = 0; i < row; i++) //Add all the center of blocks as GraphNode
        {
            for (int j = 0; j < col; j++) {
                if (terrain_manager.myInfo.traversability[i, j] == 0f) {
                    Vector3 center = new Vector3(terrain_manager.myInfo.get_x_pos(i), 0, terrain_manager.myInfo.get_z_pos(j));
                    G.addNode(new GraphNode(center));
                }
            }
        }
        for (int i = 0; i < G.getSize(); i++) {
            Vector3 center = G.getNode(i).getPosition();
            List<int> neighbor = getNeighbor(center, G);
            foreach (int p in neighbor) {
                G.addEdge(p, i);
            }


        }
        return G;
    }

    List<int> getNeighbor(Vector3 center, Graph G) {
        List<Vector3> newPos = new List<Vector3> { center + new Vector3(15, 0, 0), center + new Vector3(-15, 0, 0), center + new Vector3(0, 0, 15), center + new Vector3(0, 0, -15) };
        List<int> idxList = new List<int>();
        GraphNode n;
        float distance;
        foreach (var p in newPos) {
            n = G.FindClosestNode(p, G);
            distance = Vector3.Distance(n.getPosition(), p);
            if (distance < 1) {
                idxList.Add(n.getId());
            }
        }
        return idxList;
    }
    List<int> getklusterNeighbor(Vector3 center, Graph G) {
        List<Vector3> newPos = new List<Vector3> { center + new Vector3(20, 0, 0), center + new Vector3(-20, 0, 0), center + new Vector3(0, 0, 20), center + new Vector3(0, 0, -20), center + new Vector3(20, 0, 20), center + new Vector3(20, 0, -20), center + new Vector3(-20, 0, 20), center + new Vector3(-20, 0, -20) };
        List<int> idxList = new List<int>();
        GraphNode n;
        float distance;
        foreach (var p in newPos) {
            n = G.FindClosestNode(p, G);
            distance = Vector3.Distance(n.getPosition(), p);
            if (distance < 1) {
                idxList.Add(n.getId());
            }
        }
        return idxList;
    }

    List<Vector3> computeTargets(TerrainManager _terrain_manager) {
        LayerMask mask = LayerMask.GetMask("Drone") & LayerMask.GetMask("Terrain");
        terrain_manager = _terrain_manager.GetComponent<TerrainManager>();

        int col = terrain_manager.myInfo.x_N;
        int row = terrain_manager.myInfo.z_N;

        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                if (terrain_manager.myInfo.traversability[i, j] == 0f) {
                    Vector3 center = new Vector3(terrain_manager.myInfo.get_x_pos(i), 0, terrain_manager.myInfo.get_z_pos(j));
                    sets.Add(new HashSet<int>());
                    for (int ii = 0; ii < row; ii++) {
                        for (int jj = 0; jj < col; jj++) {

                            if (terrain_manager.myInfo.traversability[ii, jj] == 0f) {
                                List<Vector3> targets = getCorner(ii, jj);
                                bool neverhit = true;
                                foreach (Vector3 target in targets) {
                                    bool hit = Physics.Raycast(center, target - center, Vector3.Distance(target, center) - 0.1f, mask);
                                    if (hit) { neverhit = false; }
                                }
                                if (neverhit == true) {
                                    sets[sets.Count - 1].Add(ii * col + jj);
                                }
                            }
                        }
                    }
                } else {
                    sets.Add(new HashSet<int>());
                }
            }
        }
        List<int> GraphNodes = findBestSet();
        List<Vector3> VGraphNodes = new List<Vector3>();

        foreach (int n in GraphNodes) // CONVERT TO Vector3
        {
            VGraphNodes.Add(new Vector3(terrain_manager.myInfo.get_x_pos(n / col), 0, terrain_manager.myInfo.get_z_pos(n % col)));
        }
        return VGraphNodes;
    }


    List<int> findBestSetSmart() {
        List<int> bestset = new List<int>();
        HashSet<int> missingPoints = new HashSet<int>();

        for (int i = 0; i < sets.Count; i++) {
            if (sets[i].Count != 0) {
                missingPoints.Add(i);

            }
        }

        while (missingPoints.Count != 0) {
            List<int> score = new List<int>();

            foreach (HashSet<int> set in sets) {
                score.Add(0);
                foreach (int x in set) {
                    if (missingPoints.Contains(x)) {
                        score[score.Count - 1]++;
                    }
                }
            }

            if (score.Max() == missingPoints.Count) {
                int bestidx = score.IndexOf(score.Max()); //FOND THE BEST; NOW WE ADD IT
                bestset.Add(bestidx);
                foreach (int x in sets[bestidx]) {
                    missingPoints.Remove(x);
                }
            } else {
                for (int i = 0; i < sets.Count; i++) {
                    List<int> tmp_list = new List<int>(sets[i]);
                    if (sets[i].Count == 0) { continue; }
                    for (int j = 0; j < sets.Count; j++) {
                        if (sets[j].Count == 0) { continue; }
                        tmp_list.AddRange(sets[j]);
                        int tmp_score = 0;
                        foreach (int x in tmp_list) {
                            if (missingPoints.Contains(x)) {
                                tmp_score++;
                            }
                        }

                        foreach (int x in sets[j]) { tmp_list.Remove(x); }

                    }
                }
            }


        }



        return bestset;
    }

    List<int> findBestSet() {
        List<int> bestset = new List<int>();
        HashSet<int> missingPoints = new HashSet<int>();

        for (int i = 0; i < sets.Count; i++) {
            if (sets[i].Count != 0) {
                missingPoints.Add(i);

            }
        }
        while (missingPoints.Count != 0) {
            List<int> score = new List<int>();

            foreach (HashSet<int> set in sets) {
                score.Add(0);
                foreach (int x in set) {
                    if (missingPoints.Contains(x)) {
                        score[score.Count - 1]++;
                    }
                }
            }

            int bestidx = score.IndexOf(score.Max()); //FOND THE BEST; NOW WE ADD IT
            bestset.Add(bestidx);
            foreach (int x in sets[bestidx]) {
                missingPoints.Remove(x);
            }
        }

        return bestset;
    }

    List<Vector3> getCorner(int i, int j) {
        Vector3 center = new Vector3(terrain_manager.myInfo.get_x_pos(i), 0, terrain_manager.myInfo.get_z_pos(j));
        List<Vector3> myList = new List<Vector3>();
        int x = (int)(terrain_manager.myInfo.x_high - terrain_manager.myInfo.x_low) / (2 * terrain_manager.myInfo.x_N);

        int z = (int)(terrain_manager.myInfo.z_high - terrain_manager.myInfo.z_low) / (2 * terrain_manager.myInfo.z_N);

        myList.Add(center + new Vector3(+x, 0, +z));

        myList.Add(center + new Vector3(+x, 0, -z));

        myList.Add(center + new Vector3(-x, 0, +z));

        myList.Add(center + new Vector3(-x, 0, -z));

        return myList;

    }
    List<Vector3> getCorner(Vector3 center) {
        List<Vector3> myList = new List<Vector3>(4);
        int x = (int)(terrain_manager.myInfo.x_high - terrain_manager.myInfo.x_low) / (2 * terrain_manager.myInfo.x_N);

        int z = (int)(terrain_manager.myInfo.z_high - terrain_manager.myInfo.z_low) / (2 * terrain_manager.myInfo.z_N);

        myList[0] = center + new Vector3(+x, 0, +z);

        myList[1] = center + new Vector3(+x, 0, -z);

        myList[2] = center + new Vector3(-x, 0, +z);

        myList[3] = center + new Vector3(-x, 0, -z);

        return myList;

    }

    List<List<int>> findGreedyCluster(int amountOfCars, List<int> GraphNodeList,
        Dictionary<Tuple<int, int>, float> CostMatrix, Dictionary<Tuple<int, int>, List<int>> PathMatrix, Graph G) {
        List<int> toVisit = new List<int>(GraphNodeList);
        List<List<int>> cars_clusters = new List<List<int>>();
        List<int> starts = getInitialCluster(amountOfCars, GraphNodeList, CostMatrix, PathMatrix, G);
        cars_clusters.Add(new List<int>());
        cars_clusters.Add(new List<int>());
        cars_clusters.Add(new List<int>());
        cars_clusters[0].Add(starts[0]);
        cars_clusters[1].Add(starts[1]);
        cars_clusters[2].Add(starts[2]);

        while (toVisit.Count > 0) {
            for (int i = 0; i < amountOfCars; i++) {
                int newPoint = getNewElement(G, cars_clusters[i], toVisit);
                if (newPoint != -1) {

                    cars_clusters[i].Add(newPoint);
                    toVisit.Remove(newPoint);
                }

            }
        }

        return cars_clusters;
    }

    List<int> getInitialCluster(int amountOfCars, List<int> GraphNodeList, Dictionary<Tuple<int, int>, float> CostMatrix,
        Dictionary<Tuple<int, int>, List<int>> PathMatrix, Graph G) {
        List<int> shortList = new List<int>();

        foreach (int p in GraphNodeList) {
            if (G.getNode(p).getPositionX() == 80 || G.getNode(p).getPositionX() == 420 ||
                G.getNode(p).getPositionZ() == 80 || G.getNode(p).getPositionX() == 420) {
                shortList.Add(p);
            }
        }

        List<int> bestSet = new List<int>();
        bestSet.Add(0);
        bestSet.Add(1);
        bestSet.Add(2);
        float bestScore = 0;
        List<int> thisSet = new List<int>();
        float thisScore = 0;

        // This should be done by recursion!!!! 
        for (int i = 0; i < shortList.Count; i++) {
            for (int j = 0; j < shortList.Count; j++) {
                for (int k = 0; k < shortList.Count; k++) {
                    thisScore = Vector3.Distance(G.getNode(i).getPosition(), G.getNode(j).getPosition()) +
                                Vector3.Distance(G.getNode(j).getPosition(), G.getNode(k).getPosition()) +
                                Vector3.Distance(G.getNode(k).getPosition(), G.getNode(i).getPosition());
                    if (thisScore > bestScore) {
                        bestScore = thisScore;
                        bestSet[0] = i;
                        bestSet[1] = j;
                        bestSet[2] = k;
                    }
                }
            }
        }

        return bestSet;
    }


    int getNewElement(Graph G, List<int> cluster, List<int> freePoints) {
        List<int> neighbor;
        int bestChild = -1;
        int childScore;

        int bestChildScore = 0;
        foreach (int p in freePoints) {
            neighbor = getklusterNeighbor(G.getNode(p).getPosition(), G);
            childScore = 0;
            foreach (int pp in neighbor) {
                if (cluster.Contains(pp)) {
                    childScore++;
                }
            }

            if (bestChildScore < childScore) {
                bestChild = p;
                bestChildScore = childScore;
            } else if (bestChildScore == childScore) {
                UnityEngine.Random.seed = System.DateTime.Now.Millisecond;
                int rand = UnityEngine.Random.Range(0, 10);
                if (rand > 7) {
                    bestChild = p;
                }
            }
        }

        if (bestChildScore == 0) {
            return -1;
        }

        return bestChild;

    }

    List<int> getClusterPath(Graph G, List<int> cluster, Dictionary<Tuple<int, int>, float> CostMatrix,
        Dictionary<Tuple<int, int>, List<int>> PathMatrix) {
        List<int> path = new List<int>();
        int bestStart = -1;
        int bestScore = 4;
        foreach (int p in cluster) {
            int count = getNeighbor(G.getNode(p).getPosition(), G).Count;
            if (count < bestScore) {
                bestScore = count;
                bestStart = p;
            }
        }

        int position = bestStart;
        path.Add(position);
        List<int> toVisit = new List<int>(cluster);

        while (toVisit.Count > 0) {
            float minCost = 999999999;
            float cost;
            int bestCandidate = -1;
            foreach (int p in toVisit) {
                cost = CostMatrix[new Tuple<int, int>(position, p)];
                if (minCost > cost) {
                    minCost = cost;
                    bestCandidate = p;
                }
            }
            path.Add(bestCandidate);
            position = bestCandidate;
            toVisit.Remove(bestCandidate);

        }

        path.Reverse();
        return path;

    }
}