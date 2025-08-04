using UnityEngine;

public class Target : MonoBehaviour
{

    public AudioSource scoreSound;
    public void OnShot()
    {
        Debug.Log("Enemy Hit!");
        Destroy(gameObject);
        ScoreManager.Instance.AddScore(1);
        if (scoreSound != null)
        {
            scoreSound.Play();
        }
    }
}

