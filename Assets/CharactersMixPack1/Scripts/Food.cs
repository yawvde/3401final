
    using UnityEngine;

    public class Food: MonoBehaviour
    {
        public void FixedUpdate()
        {
            Debug.Log("sleeping: " + gameObject.GetComponent<Rigidbody>().IsSleeping());
        }
    }
