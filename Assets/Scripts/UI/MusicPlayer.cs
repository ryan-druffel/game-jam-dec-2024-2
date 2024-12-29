using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    [SerializeField]
    EffectCardUI effectCardUI;
    [SerializeField]
    AudioSource normalMusicPlayer;
    [SerializeField]
    AudioSource fastMusicPlayer;

    float currentTimeScale = 0;
    float currentTimeStamp = 0;

    void Start() {
        Debug.Assert(effectCardUI != null, "Music Player not connected to UI");
        Debug.Assert(normalMusicPlayer != null, "Normal Music Player not connected to Audio Source");
        Debug.Assert(fastMusicPlayer != null, "Fast Music Player not connected to Audio Source");
    }

    // Update is called once per frame
    void Update()
    {
        if (normalMusicPlayer != null && fastMusicPlayer != null && effectCardUI.GridBox && effectCardUI.GridBox.GetCoordinator()) UpdateMusic();
    }

    void UpdateMusic() {
        float timeScale = effectCardUI.GridBox.GetCoordinator().TimeScale;
        if (timeScale != currentTimeScale) {
            // Get Timestamp
            if (currentTimeScale == 0) {
                // Timestamp persists
            } else if (currentTimeScale == 1) {
                currentTimeStamp = normalMusicPlayer.time / normalMusicPlayer.clip.length;
            } else if (currentTimeScale == 1.75) {
                currentTimeStamp = fastMusicPlayer.time / fastMusicPlayer.clip.length;
            }
            if (timeScale == 0) {
                normalMusicPlayer.Stop();
                fastMusicPlayer.Stop();
            } else if (timeScale == 1) {
                normalMusicPlayer.time = currentTimeStamp * normalMusicPlayer.clip.length;
                normalMusicPlayer.Play();
                fastMusicPlayer.Stop();
            } else if (timeScale == 1.75) {
                fastMusicPlayer.time = currentTimeStamp * fastMusicPlayer.clip.length;
                fastMusicPlayer.Play();
                normalMusicPlayer.Stop();
            }
            currentTimeScale = timeScale;
        }
    }
}
