
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// namespace Test
// {
//     public class Main : MonoBehaviour
//     {
//         [SerializeField]
//         Button playButton;
//         [SerializeField]
//         AudioSource audioSource;

//         [SerializeField]
//         RectTransform circleRectTransform;
//         [SerializeField]
//         int degree = 0;
//         [SerializeField]
//         AudioClip sourceAudioClip;

//         void Start()
//         {
//             playButton.onClick.AddListener(OnClickPlay);
//         }

//         private void OnClickPlay()
//         {
//             Debug.Log($"OnClickPlay degree:{degree}");
//             degree = degree / 5 * 5;
//             circleRectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, -degree);
//             var hrtf_l = Resources.Load<AudioClip>($"Sound/elev0/L0e{degree:000}a");
//             var hrtf_r = Resources.Load<AudioClip>($"Sound/elev0/R0e{degree:000}a");
//             //var source = Resources.Load<AudioClip>("Sound/Source/drum");
//             //var source = Resources.Load<AudioClip>("Sound/Source/dog2a");
//             //var source = Resources.Load<AudioClip>("Sound/Source/loop100302");
//             //OutputAudio(hrtf_l);
//             //OutputAudio(source);
//             audioSource.clip = CreateAudioClip(hrtf_l, hrtf_r, sourceAudioClip);
//             audioSource.loop = true;
//             //OutputAudio(hrtf);
//             audioSource.Play();
//             degree += 5;
//             if (degree >= 360)
//             {
//                 degree = 0;
//             }
//         }

//         private void OutputAudio(AudioClip clip)
//         {
//             Debug.Log($"channel:{clip.channels}");
//             Debug.Log($"frequency:{clip.frequency}");
//             // float[] samples = new float[clip.samples];
//             // clip.GetData(samples, 0);
//             // for (int i = 0; i < samples.Length; ++i)
//             // {
//             //     Debug.Log($"{i}:{samples[i]:0.0000}");
//             // }
//         }

//         private AudioClip CreateAudioClip(AudioClip hrtf_l, AudioClip hrtf_r, AudioClip source)
//         {
//             float[] samples_l = CreateAudioSamples(hrtf_l, source);
//             float[] samples_r = CreateAudioSamples(hrtf_r, source);

//             int offset = 0;
//             var clip = AudioClip.Create("Sound", source.samples, 2, source.frequency, false, samples =>
//             {
//                 Debug.Log($"samples.length:{samples.Length}");
//                 for (int i = 0; i < samples.Length; i += 2)
//                 {
//                     int offset2 = (offset + i) / 2;
//                     samples[i + 0] = samples_l[offset2];
//                     samples[i + 1] = samples_r[offset2];
//                 }
//                 // source.GetData(samples, offset);
//                 offset += samples.Length;
//             });
//             return clip;
//         }

//         private float[] CreateAudioSamples(AudioClip hrtf, AudioClip source)
//         {
//             float[] samples_hrtf = new float[hrtf.samples];
//             hrtf.GetData(samples_hrtf, 0);
//             float[] samples_source = new float[source.samples];
//             source.GetData(samples_source, 0);
//             float[] samples = new float[source.samples];
//             for (int i = 0; i < samples.Length; ++i)
//             {
//                 samples[i] = 0;
//                 for (int j = 0; j <= i && j < samples_hrtf.Length; ++j)
//                 {
//                     samples[i] += samples_hrtf[j] * samples_source[i - j];
//                 }
//             }
//             return samples;
//         }
       
//     }
// }

