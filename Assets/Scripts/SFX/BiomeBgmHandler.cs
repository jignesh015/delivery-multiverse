using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class BiomeBgmHandler : MonoBehaviour
    {
        [Serializable] 
        public class BiomeBgm
        {
            public BiomeType biomeType;
            public AudioSource bgmSource;
        }
        
        [SerializeField] private List<BiomeBgm> biomeBgms;
        [SerializeField] private float crossFadeDuration = 1f;
        [SerializeField] private float targetBGMVolume = 0.5f;
        
        private AudioSource m_CurrentBgm;
        private AudioSource m_PreviousBgm;
        private Coroutine m_CrossfadeCoroutine;

        private void Awake()
        {
            GameStatic.OnBiomeChanged += OnBiomeChanged;
            GameStatic.OnWelcomeScreenRequested += OnWelcomeScreenRequested;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnBiomeChanged -= OnBiomeChanged;
            GameStatic.OnWelcomeScreenRequested -= OnWelcomeScreenRequested;
        }

        private void OnWelcomeScreenRequested()
        {
            OnBiomeChanged(BiomeType.Normal);
        }

        private void OnBiomeChanged(BiomeType biomeType)
        {
            var biomeBgm = biomeBgms.Find(b => b.biomeType == biomeType);
            if (biomeBgm == null || biomeBgm.bgmSource == null)
                return;

            if (m_CurrentBgm == biomeBgm.bgmSource)
                return;

            m_PreviousBgm = m_CurrentBgm;
            m_CurrentBgm = biomeBgm.bgmSource;

            if (m_CrossfadeCoroutine != null)
                StopCoroutine(m_CrossfadeCoroutine);
            m_CrossfadeCoroutine = StartCoroutine(CrossfadeBgm(m_PreviousBgm, m_CurrentBgm, crossFadeDuration));
        }

        private IEnumerator CrossfadeBgm(AudioSource from, AudioSource to, float duration)
        {
            if (to)
            {
                to.volume = 0f;
                if (!to.isPlaying)
                    to.Play();
            }
            var time = 0f;
            var fromStartVol = from ? from.volume : 0f;
            while (time < duration)
            {
                var t = time / duration;
                if (from)
                    from.volume = Mathf.Lerp(fromStartVol, 0f, t);
                if (to)
                    to.volume = Mathf.Lerp(0f, targetBGMVolume, t);
                time += Time.deltaTime;
                yield return null;
            }
            if (from)
            {
                from.volume = 0f;
                from.Stop();
            }
            if (to)
                to.volume = targetBGMVolume;
        }
    }
}