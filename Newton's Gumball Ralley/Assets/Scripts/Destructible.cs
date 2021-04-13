using UnityEngine;

public interface IDestructible
{
    void Split(RaycastHit2D contactPoint);
}
