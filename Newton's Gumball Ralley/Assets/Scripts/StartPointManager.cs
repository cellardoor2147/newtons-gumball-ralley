using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPointManager : MonoBehaviour
{
    [SerializeField] private float launchAmount = 5000f;
    [SerializeField] private GameObject ballToSpawnOnStartPrefab;

    private GameObject ballToSpawnOnStart;

    private void Awake()
    {
        SpawnBall();
        LaunchBall();
    }

    private void SpawnBall()
    {
        ballToSpawnOnStart = Instantiate(ballToSpawnOnStartPrefab);
        ballToSpawnOnStart.transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            0.0f
        );
    }

    private void LaunchBall()
    {
        ballToSpawnOnStart.GetComponent<BallMovement>().AddForce(transform.right * launchAmount);
    }
}
