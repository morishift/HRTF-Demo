using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{

    public class ScheduledAudioSource : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        public void PlayScheduled(AudioClip clip, double play_scheduled_time)
        {
            audioSource.clip = clip;
            playScheduledTime = play_scheduled_time;
            endScheduledTime = play_scheduled_time + (double)clip.samples / clip.frequency;
            audioSource.PlayScheduled(play_scheduled_time);
        }

        public void SetScheduledEndTime(double time)
        {
            endScheduledTime = time;
            audioSource.SetScheduledEndTime(time);
        }

        public void Stop()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// 再生開始時刻
        /// </summary>
        public double playScheduledTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 終了時刻
        /// </summary>
        public double endScheduledTime
        {
            get;
            private set;
        }

        public bool isPlaying
        {
            get
            {
                return audioSource.isPlaying;
            }
        }
    }
}



