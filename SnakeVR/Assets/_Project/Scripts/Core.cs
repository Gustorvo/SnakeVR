using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gustorvo.SnakeVR
{
    public class Core : MonoBehaviour
    {
        [SerializeField, Scene] string cameraRigScene;
        [SerializeField, Scene] string snakeScene;

        private void Start()
        {
            SceneManager.LoadSceneAsync(cameraRigScene, LoadSceneMode.Additive).completed += MergeSnakeScene;
           // SceneManager.LoadSceneAsync(snakeScene, LoadSceneMode.Additive);
        }

        private void MergeSnakeScene(AsyncOperation obj)
        {
            SceneManager.MergeScenes(SceneManager.GetSceneByName(cameraRigScene),
                SceneManager.GetSceneByName(snakeScene));
        }

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            // // check if new scene is the snake scene and make it active
            // if (arg0.name == snakeScene)
            // {
            //     SceneManager.SetActiveScene(arg0);
            // }
            // // merge camera git scene
            // if (arg0.name == cameraRigScene)
            // {
            //     SceneManager.MergeScenes(arg0, SceneManager.GetSceneByName(snakeScene));
            // }
        }
       
    }
}