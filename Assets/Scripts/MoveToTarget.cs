using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoveToTarget : MonoBehaviour
    {
        [SerializeField] private float transitionDuration;
        
        public event EventHandler ReachedTarget;
        public Vector3 TargetPosition;
        private Vector3 startPosition;
        private float startTime;
        private bool isMoving = true;

        private void Start()
        {
            this.startPosition = this.transform.position;
            this.startTime = Time.time;
        }

        private void Update()
        {
            if (!isMoving) return;

            var newPosition = Vector3.Lerp(startPosition, TargetPosition, (Time.time - startTime) / transitionDuration);
            var velocity = newPosition - this.transform.position;
            if (velocity.sqrMagnitude > 0.001f)
            {
                this.transform.forward = velocity;
            }
            this.transform.position = newPosition;
            
            if (Time.time > (startTime + transitionDuration))
            {
                ReachedTarget?.Invoke(this, EventArgs.Empty);
                this.isMoving = false;
            }
        }
    }
}