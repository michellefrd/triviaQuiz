using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenesApp : MonoBehaviour
{
   public void OnExitClicked()
   {
      Application.Quit();
   }

   public void OnLeaderboardClicked()
   {
      SceneManager.LoadScene("LeaderboardScene");
   }

   public void OnMenuClicked()
   {
      SceneManager.LoadScene("MenuScene");
   }

   public void OnRestartLevel()
   {
      SceneManager.LoadScene("GameScene");
   }
   
}
