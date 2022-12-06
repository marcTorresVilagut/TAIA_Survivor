using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToSurviveAgent : Agent {
    private PushBlockSettings m_pushBlockSettings;
    private Rigidbody m_AgentRb;
    private SurvivorEnvController m_GameController;
    private bool in_danger;
    private bool in_windArea;
    private Vector3 wind_direction;
    private GameObject windArea;
    private float m_SurvivedTime;

    /// <summary>
    /// Initializes the attributes of the agent controller.
    /// </summary>
    public override void Initialize() {
        m_GameController = GetComponentInParent<SurvivorEnvController>();
        m_AgentRb = GetComponent<Rigidbody>();
        m_pushBlockSettings = FindObjectOfType<PushBlockSettings>();
        in_danger = false;

        // Wind area elements
        in_windArea = false;
        windArea = null;
        m_SurvivedTime = 0f;
    }

    public override void OnEpisodeBegin() {
        m_SurvivedTime = 0f;
    }

    /// <summary>
    ///     Collects the observations from the enviroment the agent is in
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(in_danger ? 1 : 0);
        
        sensor.AddObservation(in_windArea ? 1 : 0);
        if(windArea != null) {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        } else {
            sensor.AddObservation(wind_direction.x);
            sensor.AddObservation(wind_direction.z);
        }
    }

    /// <summary>
    ///     Recieves an action for the agent to interact with the enviroment
    /// </summary>
    /// <param name="actions"> 
    ///     Action buffer containing the action that the agent has to do
    /// </param>
    public override void OnActionReceived(ActionBuffers actions) {
        // Move the agent using the action
        MoveAgent(actions.DiscreteActions);

        AddReward(+1f/m_GameController.MaxEnvironmentSteps); // Reward for lifespan length (for reward density)
    }

    /// <summary>
    ///     Moves and rotates the agent in respect to the action segment recieved
    /// </summary>
    /// <param name="act">
    ///     Action that determines what the agent should do
    /// </param>
    public void MoveAgent(ActionSegment<int> act) {
        Vector3 dirToGo = Vector3.zero;

        var action = act[0];
        switch(action) {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                dirToGo = transform.right * 1f;
                break;
            case 4:
                dirToGo = transform.right * -1f;
                break;
        }
        
        // Apply action to the agent
        m_AgentRb.AddForce(dirToGo * m_pushBlockSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    /// <summary>
    ///     Behaviour for when the agent has collided with an element of the enviroment
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision) {
        // Set reward for the agent based on the element that it collided with
        bool gameover = CheckCollision(collision.transform);
        // If agent died, start Game Over behaviour
        if (gameover) GameOver(collision.gameObject.name);
    }

    /// <summary>
    ///     Behaviour for when the agent collided with a non-leathal element of the enviroment.
    /// </summary>
    /// <param name="collider">
    ///     The element that has triggered the event
    /// </param>
    private void OnTriggerEnter(Collider collider) {
        // Set reward for the agent based on the element that it collided with
        bool gameover = CheckCollision(collider.transform);
        // If agent died, start Game Over behaviour
        if (gameover) GameOver(collider.gameObject.name);
        
        if (collider.transform.CompareTag("collision_area")) {
            in_danger = true;
        }       
        if (collider.transform.CompareTag("wind_area")) {
            in_windArea = true;
            windArea = collider.gameObject;
        } 
        
    }

    /// <summary>
    ///     Behaviour for when the agent stays in a position of danger
    /// </summary>
    /// <param name="collider">
    ///     The element that has triggered the event
    /// </param>
    private void OnTriggerStay(Collider collider) {
        if (collider.transform.CompareTag("collision_area")) {
            // Add negative reward for not moving onto a safe position
            //AddReward(-0.01f);
            in_danger = true;
        }
    }

    /// <summary>
    ///     Behaviour for when the agent has exited a danger-zone
    /// </summary>
    /// <param name="collider">
    ///     The element that has triggered the event
    /// </param>
    private void OnTriggerExit(Collider collider) {
        if (collider.transform.CompareTag("collision_area")) {
            in_danger = false;
        } else if (collider.transform.CompareTag("wind_area")) {
            in_windArea = false;
        } 
    }
    
    /// <summary>
    ///     Moves the agent in the direction of the wind when it is inside a wind area.
    ///     Updates survived time
    /// </summary>
    private void FixedUpdate() {
        if (in_windArea && windArea!=null) {
            WindController windController = windArea.GetComponent<WindController>();
            m_AgentRb.AddForce(
                m_pushBlockSettings.agentRunSpeed * windController.strength * windController.direction,
                ForceMode.VelocityChange);
        }
        m_SurvivedTime += Time.deltaTime;
    }

    /// <summary>
    ///     Gets the currently survived time of the agent in the episode
    /// </summary>
    /// <returns>The agent's survived time in the episode</returns>
    public float GetSurvivedTime() {
        return m_SurvivedTime;
    }

    /// <summary>
    ///     Adds a reward for surviving the time defined by the episode's scene
    /// </summary>
    public void AddSurvivedEpisodeReward() {;
        AddReward(1000f);
    }

    /// <summary>
    ///     Game over logic for when the agent collided with an obstacle
    /// </summary>
    /// <param name="collider_name">The name of the obstacle's collider which the agent collided with</param>
    private void GameOver(string collider_name) {
        // GameOver Behaviour
        print($"Final Score: {GetCumulativeReward()}");

        // Notify Game controller about the collision that killed the agent
        m_GameController.KilledByObstacle(this, collider_name);
    }

    /// <summary>
    ///     Adds a negative reward to the current cummulative reward of the agent
    ///     if it has collided with a hazard.
    /// </summary>
    /// <param name="col"> 
    ///     Collision object to which the agent has collided
    /// </param>
    /// <returns></returns>
    private bool CheckCollision(Transform col) {
        bool gameover = false;
        if (col.CompareTag("bounds") || 
            col.CompareTag("obstacle") ||
            col.CompareTag("cannon_ball")
            ) {
            gameover = true;
            AddReward(-1f); // Set the negative reward for dying
        }

        return gameover;
    }

    /// <summary>
    /// Defines the actions that the agent will recieve from the given input by the user when
    /// Heuristic mode is enabled for the agent. This way, the agent can be manually controled.
    /// </summary>
    /// <param name="actionsOut">Buffer for the actions that will be filled to send to the agent.</param>
    public override void Heuristic(in ActionBuffers actionsOut) {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W)) {
            discreteActionsOut[0] = 1;
        } else if (Input.GetKey(KeyCode.S)) {
            discreteActionsOut[0] = 2;
        } else if (Input.GetKey(KeyCode.D)) {
            discreteActionsOut[0] = 3;
        } else if (Input.GetKey(KeyCode.A)) {
            discreteActionsOut[0] = 4;
        }
    }
}