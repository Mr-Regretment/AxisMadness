using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHandler : Entity
{
    
    [SerializeField] protected float moveSpeed;
    [SerializeField] public int tokenCount;
    
    
    protected GameObject NearestObjectOfTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        float nearestDistance = 100000;
        GameObject nearest = null;
        float distance;

        for (int i = 0; i < objects.Length; i++)
        {
            distance = Vector3.Distance(transform.position, objects[i].transform.position);

            if (distance < nearestDistance)
            {
                nearest = objects[i];
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    protected GameObject NearestObjectOfTagWithComponent(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (var obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }

        return nearest;
    }
}