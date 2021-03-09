using UnityEngine;

public class EndPointManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otherObjectCollider)
    {
        Debug.Log("A winner is you");
    }
}
