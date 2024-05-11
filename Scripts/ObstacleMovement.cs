using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [SerializeField] GameObject wall;
    public float speed;

    public void Update()
    {
        // Make the moving wall obstacles move up and down
        float y = Mathf.PingPong(Time.time * speed, 2);
        wall.transform.position = new Vector3(wall.transform.position.x, y, wall.transform.position.z);
    }
}
