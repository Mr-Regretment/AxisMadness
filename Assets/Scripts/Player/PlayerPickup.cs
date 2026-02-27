using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerPickup : PlayerHandler
    {
        private Vector3 _endSize = new Vector3(0,0,0);
        private Vector3 _startSize;
        private GameObject _nearestObj;
        private float _desiredDuration = 1f;
        private float _elapsedTime;
        private bool pickingUp = false;
        
        [SerializeField] private float percentage;
        [SerializeField] private Vector3 scale;

        private void Update()
        {
            if (!pickingUp)
            {
                _nearestObj = NearestObjectOfTagWithComponent("PickupItem");

                if (_nearestObj != null)
                {
                    float distance = Vector3.Distance(
                        transform.position,
                        _nearestObj.transform.position
                    );
                    
                    if (distance > 2.5f)
                    {
                        _nearestObj = null;
                    }
                }
            }
            
            if (_nearestObj != null && Input.GetKeyDown(KeyCode.E) && !GetComponent<PlayerCamera>().StandingOverRotatePad())
            {
                pickingUp = true;
                _elapsedTime = 0f;
                _startSize = _nearestObj.transform.localScale;
            }

            if (pickingUp && _nearestObj != null)
            {
                CompletePickup();
            }
        }

        private void CompletePickup()
        {
            PickupAnimation();

            if (percentage >= 1f)
            {
                PlayerCamera playerCamera = player.GetComponent<PlayerCamera>();

                playerCamera.tokenCount++;
                Destroy(_nearestObj);
        
                _elapsedTime = 0f;
                pickingUp = false;
                _nearestObj = null;
                percentage = 0;
            }
        }

        void PickupAnimation()
        {
            _elapsedTime += Time.deltaTime;
            percentage = _elapsedTime / _desiredDuration;
            _nearestObj.transform.localScale = 
                Vector3.Lerp(_startSize, new Vector3(0,0,0), Mathf.SmoothStep(0f, 1f, percentage));
        }
        
    }
}