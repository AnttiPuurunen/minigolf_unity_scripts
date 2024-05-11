using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    void Update()
    {
        // Make the powerup spin in place
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }
}
