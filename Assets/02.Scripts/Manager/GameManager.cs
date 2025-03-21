using System;
using UnityEngine;



namespace KKH.Manager
{
    public class GameManager : BaseBehaviour
    {
        public static GameManager Instance;
        public Action OnGameRestart;
        public Action OnGameOver;


        protected override void Initialize()
        {
            base.Initialize();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }


    }

}
