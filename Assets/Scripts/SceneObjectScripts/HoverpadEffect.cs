using System;
using UnityEngine;

public class HoverpadEffect : MonoBehaviour
{
   private bool StoodOn;
   [SerializeField] private float RaycastDistance = 1.2f;
   private string Animation;
   
   public string GetCurrentAnimation()
   {
      return Animation;
   }

   private void Update()
   {
      bool hit = Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitInfo, RaycastDistance);
    
      bool isValidObject = false;
    
      if (hit && hitInfo.transform != null)
      {
         isValidObject = hitInfo.transform.CompareTag("Player");
      }
      
      
      StoodOn = isValidObject;

      if (!StoodOn)
      {
         Animation = "Lit";
      }
      else if (StoodOn)
      {
         Animation = "Unlit";
      }
      
   }
}