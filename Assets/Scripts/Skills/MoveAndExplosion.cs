using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAndExplosion : MonoBehaviour
{
    public GameObject ExplosionGM;
    public Vector3 MoveToPoint;
    public float SpeedMove, ExplosionDistance;
    public bool IsMove;

    private void Update()
    {
        if (!IsMove) return;

        if(Vector2.Distance(transform.position, MoveToPoint) > ExplosionDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, MoveToPoint, SpeedMove * Time.deltaTime);
        }
        else
        {
            IsMove = false;
            SpawnExplosion();
            Destroy(this.gameObject);
        }
    }

    public void StartMove (Vector3 _endPoint)
    {
        IsMove = true;
        MoveToPoint = _endPoint;
    }


    public void SpawnExplosion ()
    {
        Instantiate(ExplosionGM, transform.position, ExplosionGM.transform.rotation);
    }
}
