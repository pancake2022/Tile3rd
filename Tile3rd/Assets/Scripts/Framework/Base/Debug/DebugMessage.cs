using UnityEngine;

namespace CSFramework
{
    public class DebugMessage : CSBehaviour
    {
        private void Log (string str)
        {
            Logger.Error(this.name + ", " + this.GetInstanceID() + " " + str);
        }
        #region Recycle
        private void Awake() 
        {
            Log("Awake");
        }
        private void Start() 
        {
            Log("Start");
        }
        #endregion

        #region Collision
        private void OnCollisionEnter2D(Collision2D other)
        {
            Log("OnCollisionEnter2D");
        }

        private void OnCollisionStay2D (Collision2D collision)
        {
            Log("OnCollisionStay2D");
        }

        private void OnCollisionExit2D (Collision2D collision)
        {
            Log("OnCollisionExit2D");
        }
        
        private void OnTriggerEnter2D (Collider2D collision)
        {
            Log("OnTriggerEnter2D");
        }

        private void OnTriggerStay2D (Collider2D collision)
        {
            Log("OnTriggerStay2D");
        }

        private void OnTriggerExit2D (Collider2D collision)
        {
            Log("OnTriggerExit2D");
        }
        #endregion
    }
}