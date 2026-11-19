using System;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private bool _turnOn;
    public bool TurnOn
    {
        get => _turnOn;

        set
        {
            _turnOn = value;
            WakeUpAllFood();
        }
    }

    [Header("传送方向（局部坐标）")]
    public Vector3 direction = Vector3.forward;

    [Header("传送速度")]
    public float speed = 2f;
    
    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("CollisionExit");
        if (!_turnOn)
            return;
        if (!collision.gameObject.CompareTag("Food"))
            return;
        if (collision.gameObject.GetComponent<Rigidbody>() == null)
            return;
        collision.gameObject.GetComponent<Rigidbody>().WakeUp();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CollisionEnter");
        if (!_turnOn)
            return;
        if (!collision.gameObject.CompareTag("Food"))
            return;
        if (collision.gameObject.GetComponent<Rigidbody>() == null)
            return;
        collision.gameObject.GetComponent<Rigidbody>().Sleep();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_turnOn)
            return;
        if (!collision.gameObject.CompareTag("Food"))
            return;

        Rigidbody rb = collision.rigidbody;

        if (rb == null)
            return;

        bool onTop = false;

        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = -contact.normal;

            if (Vector3.Dot(normal, transform.up) > 0.5f)
            {
                onTop = true;
                break;
            }
        }

        if (!onTop)
            return;

        Vector3 moveDirection =
            transform.TransformDirection(direction.normalized);

        // 直接平移，不施加物理力
        rb.MovePosition(
            rb.position + moveDirection * speed * Time.fixedDeltaTime
        );
    }
    
    
    public void WakeUpAllFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");

        foreach (GameObject food in foods)
        {
            Rigidbody rb = food.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.WakeUp();
            }
        }
    }
}