using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingFood : MonoBehaviour
{
    Vector3 dir = Vector3.zero;

    public float speed = 3f;

    void Update()
    {
        if (dir != Vector3.zero)
        {
            transform.position += dir * speed * Time.deltaTime;
        }
    }

    public void SetDirection(Vector3 _dir)
    {
        dir = _dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dog") || other.CompareTag("Fox") || other.CompareTag("Horse"))
        {
            Destroy(gameObject);
        }
    }
}
