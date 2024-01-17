using UnityEngine;

public class EnemySenses : MonoBehaviour
{
    [SerializeField] private float viewRadius = 8;
    [SerializeField] private float viewAngle = 90;
    [SerializeField] private float hearRadiusForRunning = 10;
    [SerializeField] private float hearRadiusForwalking = 6;

    [SerializeField] private LayerMask obstacleLayermask; // stop the AI from seeing the player if something is blocking them

    private Transform thisEnemyTransform;

    private int maxUpdateInterval = 0;
    private int currentUpdateInterval = 0;

    public bool SpotedPlayer { get; private set; } = false;
    public DoorController blockedByDoor;
    public Vector3 LastSeenAtPos { get; private set; } = Vector3.zero;

    private void Awake()
    {
        thisEnemyTransform = transform;
        maxUpdateInterval = Random.Range(2, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentUpdateInterval < maxUpdateInterval)
        {
            currentUpdateInterval++;
            return;
        }
        else currentUpdateInterval = 0;

        Vector3 playerTarget = Player.Instance.PlayerTransform.position - thisEnemyTransform.position;
        float distanceToTargetSquared = Vector3.SqrMagnitude(playerTarget);

        // if the player is close enough
        if (distanceToTargetSquared <= (viewRadius * viewRadius)) // this is for vision
        {
            // if player is in betwwen the angle
            // why viewAngle / 2 ? because 45 towards the lelft and 45 towards the right
            // we check if player is in between that angle
            if (Vector3.Angle(thisEnemyTransform.forward, playerTarget) < viewAngle / 2)
            {
                LastSeenAtPos = Player.Instance.PlayerTransform.position;
                if (SpotedPlayer) SpotedPlayer = false;
                if (Physics.Raycast(thisEnemyTransform.position, playerTarget, viewRadius, obstacleLayermask) == false)
                {
                    LastSeenAtPos = Player.Instance.PlayerTransform.position;
                    if (!SpotedPlayer) SpotedPlayer = true;
                    // Debug.Log("-spotted you-");
                }
                else
                {
                    LastSeenAtPos = Player.Instance.PlayerTransform.position;
                    if (SpotedPlayer) SpotedPlayer = false;
                    // Debug.Log("lost sigh of you--");
                }
            }
            else
            {
                if (SpotedPlayer) SpotedPlayer = false;
                // Debug.Log("lost sigh of you--");
            }
        }
        else
        {
            if (SpotedPlayer)
                SpotedPlayer = false;
        }

        if (Player.Instance.IsRuning() && distanceToTargetSquared < (hearRadiusForRunning * hearRadiusForRunning))
        {
            LastSeenAtPos = Player.Instance.PlayerTransform.position;
            // Debug.Log("--heard you");
        }
        else if (Player.Instance.IsWalking() && distanceToTargetSquared < (hearRadiusForwalking * hearRadiusForwalking))
        {
            LastSeenAtPos = Player.Instance.PlayerTransform.position;
            // Debug.Log("--heard you");
        }
    }

    public void Reset()
    {
        SpotedPlayer = false;
        blockedByDoor = null;
        LastSeenAtPos = Vector3.zero;
    }

    public void ResetLastSeenPosiotion()
    {
        LastSeenAtPos = Vector3.zero;
    }

    public void SetLastSeenPosiotion(Vector3 playerPos)
    {
        LastSeenAtPos = playerPos;
    }
}
