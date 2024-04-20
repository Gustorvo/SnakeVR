using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.SnakeVR.Input
{
    public class GUIDirectionButtons : MonoBehaviour
    {
        private void OnGUI()
        {
            float buttonWidth = 50f;
            float buttonHeight = 50f;
            float padding = 10f;

            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;

            if (GUI.Button(new Rect(centerX - buttonWidth / 2f, centerY - buttonHeight - padding, buttonWidth, buttonHeight), "forward"))
            {
                // Handle Up button click
                SnakeMoveDirection.Direction = Vector3.forward;
            }

            if (GUI.Button(new Rect(centerX - buttonWidth / 2f, centerY + padding, buttonWidth, buttonHeight), "back")){
                // Handle Down button click
                SnakeMoveDirection.Direction = Vector3.back;
            }

            if (GUI.Button(new Rect(centerX - buttonWidth - padding, centerY - buttonHeight / 2f, buttonWidth, buttonHeight), "Left"))
            {
                // Handle Left button click
                SnakeMoveDirection.Direction = Vector3.left;
            }
            
            if (GUI.Button(new Rect(centerX + padding, centerY - buttonHeight / 2f, buttonWidth, buttonHeight), "Right")){
                // Handle Right button click
                SnakeMoveDirection.Direction = Vector3.right;
            }
            
            // if (GUI.Button(new Rect(centerX - buttonWidth / 2f, centerY - buttonHeight / 2f, buttonWidth, buttonHeight), "Forward"))
            // {
            //     // Handle Forward button click
            //     SnakeMoveDirection.Direction = Vector3.forward;
            // }
            //
            // if (GUI.Button(new Rect(centerX - buttonWidth / 2f, centerY - buttonHeight * 1.5f - padding, buttonWidth, buttonHeight), "Back"))
            // {
            //     // Handle Back button click
            //     SnakeMoveDirection.Direction = Vector3.back;
            // }
        }
    }
}
