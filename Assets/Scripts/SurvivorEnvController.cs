using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEngine.InputSystem.XR;

public class SurvivorEnvController : MonoBehaviour {

    [System.Serializable]
    public class PlayerInfo {
        public MoveToSurviveAgent Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Collider Col;
    }

    /// <summary>
    ///  Whether to increment or not the time when successfully ending an episode
    /// </summary>
    [Header("Increment episode lenght on success")]
    public bool incrementEpisodeLenght = false;
    

    /// <summary>
    /// Time the agent has to survive to recieve a reward
    /// </summary>
    [Header("Time to survive (s)")]
    public float m_SurviveTime;

    /// <summary>
    /// The area bounds
    /// </summary>
    [HideInInspector]
    public Bounds areaBounds;

    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    public GameObject ground;

    /// <summary>
    /// We will be changing the ground material based on failue
    /// </summary>
    Renderer m_GroundRenderer;
    Material m_GroundMaterial;

    public PlayerInfo SurvivorAgent;
    public bool useRandomAgentPosition = true;
    PushBlockSettings m_PushBlockSettings;

    // Spawners objects
    CannonSpawner mCannonSpawner;
    FanSpawner mFanSpawner;

    // Start is called before the first frame update
    void Start() {
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        // Get the ground renderer so we can change the material when the agent fails
        m_GroundRenderer = ground.GetComponent<Renderer>();
        // Starting material
        m_GroundMaterial = m_GroundRenderer.material;
        m_PushBlockSettings = FindObjectOfType<PushBlockSettings>();

        // Initialize Agent
        SurvivorAgent.StartingPos = SurvivorAgent.Agent.transform.localPosition;
        SurvivorAgent.Rb = SurvivorAgent.Agent.GetComponent<Rigidbody>();
        SurvivorAgent.Col = SurvivorAgent.Agent.GetComponent<Collider>();

        // Get spawner
        mCannonSpawner = GetComponentInChildren<CannonSpawner>();
        mFanSpawner = GetComponentInChildren<FanSpawner>();
        if(mCannonSpawner != null) mCannonSpawner.gameObject.SetActive(false);
        if(mFanSpawner != null) mFanSpawner.gameObject.SetActive(false);

        m_SurviveTime = 10f;

        // Start scene
        ResetScene();
    }

    // Update is called once per frame
    void Update() {
        if(SurvivorAgent.Agent.GetSurvivedTime() >= m_SurviveTime) {
            print($"Agent survived {SurvivorAgent.Agent.GetSurvivedTime()} seconds");
            AgentSurvivedTime();    
        }
    }

    /// <summary>
    ///     Episode end's logic for when the agent has been killed by an obstacle of the enviroment
    /// </summary>
    public void KilledByObstacle(MoveToSurviveAgent agent, string obstacle_name) {
        if(mCannonSpawner != null) mCannonSpawner.gameObject.SetActive(false);
        if(mFanSpawner != null) mFanSpawner.gameObject.SetActive(false);
        agent.gameObject.SetActive(false);

        SurvivorAgent.Agent.EndEpisode();

        // Swap ground material for a bit to indicate failure
        StartCoroutine(SwapGroundMaterial(m_PushBlockSettings.failMaterial, 0.5f));
        ResetScene();
    }

    /// <summary>
    ///     Episode end's logic for when the agent has survived the `surviving time`
    /// </summary>
    public void AgentSurvivedTime() {
        SurvivorAgent.Agent.EndEpisode();
        SurvivorAgent.Agent.AddSurvivedEpisodeReward();


        // Swap ground material for a bit to indicate failure
        StartCoroutine(SwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
        if(incrementEpisodeLenght) m_SurviveTime += 5f; // Add 10s more to survive
        ResetScene(); // Restart scene
    }

    /// <summary>
    /// Swap ground material, wait time seconds, then swap back to the regular material.
    /// </summary>
    IEnumerator SwapGroundMaterial(Material mat, float time) {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time);
        m_GroundRenderer.material = m_GroundMaterial;
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos(bool spawn_agent = false) {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false) {
            var x = 0f;
            var z = 0f;

            if (spawn_agent) {
                x = areaBounds.extents.x * 0.75f;
                z = areaBounds.extents.z * 0.75f;
            } else {
                x = areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier;
                z = areaBounds.extents.z * m_PushBlockSettings.spawnAreaMarginMultiplier;
            }

            var randomPosX = Random.Range(-x, x);
            var randomPosZ = Random.Range(-z, z);

            randomSpawnPos = ground.transform.localPosition + new Vector3(randomPosX, 2f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(1.0f, 0.01f, 1.0f)) == false) {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    /// <summary>
    ///     Enables the obtacles spawner objects after the given time in seconds
    /// </summary>
    /// <param name="time">Time to wait until enabling the spawners in seconds</param>
    IEnumerator EnableObstaclesSpawners(float time) {
        yield return new WaitForSeconds(time);

        if (mCannonSpawner != null) mCannonSpawner.gameObject.SetActive(true);
        if (mFanSpawner != null) mFanSpawner.gameObject.SetActive(true);
    }

    /// <summary>
    ///     Restarts the scene enviroment
    /// </summary>
    void ResetScene() {
        // Reset counter 
        // m_ResetTimer = 0;

        // Remove all obstacles of the enviroment
        if (mCannonSpawner != null) {
            LifetimeController[] cannons = mCannonSpawner.GetComponentsInChildren<LifetimeController>();
            foreach (LifetimeController c in cannons) Destroy(c.gameObject);
        }
        if (mFanSpawner != null) {
            LifetimeController[] fans = mFanSpawner.GetComponentsInChildren<LifetimeController>();
            foreach (LifetimeController f in fans) Destroy(f.gameObject);
        }

        // Reset Agent
        var pos = useRandomAgentPosition ? GetRandomSpawnPos(true) : SurvivorAgent.StartingPos;

        SurvivorAgent.Agent.transform.localPosition = pos;
        SurvivorAgent.Rb.velocity = Vector3.zero;
        SurvivorAgent.Rb.angularVelocity = Vector3.zero;
        SurvivorAgent.Agent.gameObject.SetActive(true);

        // Wait 0.5s to enable obstacle spawners again
        StartCoroutine(EnableObstaclesSpawners(0.5f));
    }
}
