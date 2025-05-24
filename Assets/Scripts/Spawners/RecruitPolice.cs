using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitPolice : MonoBehaviour
{
    private int _scoreMultiplierPolice;
    private int _squadMultiplierPolice;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PoliceOfficer>(out _))
        {
            
        }
    }

    public void SetBonusCount(int scoreCount, int squadCount)
    {
        _squadMultiplierPolice = squadCount;
        _scoreMultiplierPolice = scoreCount;
    }
    
    private void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if(collider == null)
            return;
        
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix; 
        Gizmos.DrawWireCube(collider.center, collider.size);
    }
}
