using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Logs;
using UniversalUnity.Helpers.Tweeks.CurveAnimationHelper;
using UniversalUnity.Helpers.Utils;

namespace UniversalUnity.Helpers.AudioManager
{
    public class ManagedAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioManager.EAudioSource sourceType;
        [SerializeField] private AudioSource audioSource;

        /// <summary>
        /// For smooth clips changing. If null <see cref="PlaySmooth"/> can't be used.
        /// </summary>
        [SerializeField] [CanBeNull] private AudioSource secondAudioSource = null;

        private CancellationTokenSource _decreasingVolumeCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _stopCancellationTokenSource = new CancellationTokenSource();
        private AudioSource _activeSource;

        public AudioManager.EAudioSource SourceType => sourceType;

        private void Awake()
        {
            _activeSource = audioSource;
            audioSource.volume = PlayerPrefsManager.GetSavedVolumeForSource(SourceType);
            if (!ReferenceEquals(secondAudioSource, null))
            {
                secondAudioSource.volume = PlayerPrefsManager.GetSavedVolumeForSource(SourceType);
            }
        }

        public bool IsPlaying(AudioClip clip)
        {
            if (clip == null) Debug.LogError("AudioClip is null.");
            return _activeSource.clip != null && _activeSource.clip.Equals(clip);
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (clip == null) Debug.LogError("AudioClip is null.");
            _activeSource.PlayOneShot(clip, PlayerPrefsManager.GetSavedVolumeForSource(SourceType));
        }

        public void Play(AudioClip clip, bool loop = false)
        {
            if (clip == null) Debug.LogError("AudioClip is null.");
            _activeSource.Stop();
            _activeSource.volume = PlayerPrefsManager.GetSavedVolumeForSource(SourceType);
            _activeSource.loop = loop;
            _activeSource.clip = clip;
            _activeSource.Play();
        }

        /// <summary>
        /// Makes clip start playing smooth. like this:
        /// 
        /// old clip -            -- new clip full volume
        ///             -        --   
        ///                 --
        ///             --       -
        /// new clip --             - old clip stops
        ///
        /// </summary>
        public async UniTask PlaySmooth(AudioClip clip, bool loop = false, float changeTime = 1f)
        {
            if (ReferenceEquals(secondAudioSource, null))
            {
                LogHelper.LogError("Can't play smooth without second source!",
                    nameof(PlaySmooth));
            }

            await PlaySmoothProcess(clip, loop, changeTime);
        }

        public async UniTask StopSmooth(float stopTime = 1f)
        {
            await StopPlayingSmoothProcess(stopTime);
        }

        private async UniTask PlaySmoothProcess(AudioClip clip, bool loop, float changeTime)
        {
            if (clip == null)
            {
                Debug.LogError("AudioClip is null.");
                return;
            }

            var decreasingVolumeSource = _activeSource;
            var increasingVolumeSource = GetFreeSource();

            _activeSource = increasingVolumeSource;

            if (decreasingVolumeSource.isPlaying)
            {
                if (decreasingVolumeSource.clip != null && decreasingVolumeSource.clip.Equals(clip)) return;
                _decreasingVolumeCancellationTokenSource.Cancel();
                _decreasingVolumeCancellationTokenSource = new CancellationTokenSource();
                UniTask.Run(() =>
                    {
                        return CurveAnimationHelper.LerpFloatByCurve
                        (
                            result => decreasingVolumeSource.volume = result,
                            decreasingVolumeSource.volume,
                            0,
                            timeOrSpeed: changeTime,
                            onDone: () => { decreasingVolumeSource.Stop(); }
                        );
                    },
                    cancellationToken: _decreasingVolumeCancellationTokenSource.Token);
            }

            increasingVolumeSource.Stop();
            increasingVolumeSource.loop = loop;
            increasingVolumeSource.clip = clip;
            increasingVolumeSource.Play();

            await CurveAnimationHelper.LerpFloatByCurve
            (
                result => increasingVolumeSource.volume = result,
                0,
                PlayerPrefsManager.GetSavedVolumeForSource(SourceType),
                timeOrSpeed: changeTime,
                cancellationToken: _decreasingVolumeCancellationTokenSource.Token
            );
        }

        private async UniTask StopPlayingSmoothProcess(float stopTime)
        {
            var decreasingVolumeSource = _activeSource;
            if (decreasingVolumeSource.isPlaying)
            {
                _decreasingVolumeCancellationTokenSource.Cancel();
                _decreasingVolumeCancellationTokenSource = new CancellationTokenSource();

                if (decreasingVolumeSource.clip == null) return;
                await CurveAnimationHelper.LerpFloatByCurve
                (
                    result => decreasingVolumeSource.volume = result,
                    decreasingVolumeSource.volume,
                    0,
                    timeOrSpeed: stopTime,
                    onDone: () => { decreasingVolumeSource.Stop(); },
                    cancellationToken: _decreasingVolumeCancellationTokenSource.Token
                );
            }
        }

        private AudioSource GetFreeSource()
        {
            return ReferenceEquals(_activeSource, audioSource) ? secondAudioSource : audioSource;
        }
    }
}