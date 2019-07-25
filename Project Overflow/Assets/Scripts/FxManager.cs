using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    public List<AudioClip> clips;
    public List<GameObject> particles;
    public GameObject clipsPool;
    public GameObject clipsLoopPool;
    public GameObject particlesPool;

    GameObject soundEmitter;

    public static FxManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            clipsPool = new GameObject("ClipsPool");
            clipsPool.transform.SetParent(transform);
            clipsLoopPool = new GameObject("ClipsLoopPool");
            clipsLoopPool.transform.SetParent(transform);
            particlesPool = new GameObject("ParticlesPool");
            particlesPool.transform.SetParent(transform);
        }
        else
        {
            Debug.Log("There is already a FxManager, the new one got destroyed");
            Destroy(gameObject);
        }
    }
    
    public static void EmitSound(AudioClip clip, bool randomizePitch = false)
    {
        foreach (Transform clipsEmiter in Instance.clipsPool.transform)
        {
            if (!clipsEmiter.GetComponent<AudioSource>().isPlaying) Instance.soundEmitter = clipsEmiter.gameObject;
        }

        if (Instance.soundEmitter == null)
        {
            Instance.soundEmitter = new GameObject("AudioEmiter", typeof(AudioSource));
            Instance.soundEmitter.transform.SetParent(Instance.clipsPool.transform);
        }

        AudioSource emitersAudioSource = Instance.soundEmitter.GetComponent<AudioSource>();

        if (randomizePitch) emitersAudioSource.pitch = Time.timeScale + Random.Range(-0.05f, 0.05f);
        else emitersAudioSource.pitch = Time.timeScale;

        if(emitersAudioSource.clip != clip) emitersAudioSource.clip = clip;
        emitersAudioSource.Play();

        Instance.soundEmitter = null;
    }
    
    public static void EmitSoundOnDestroy(string clipName, Transform followTarget = null, float minDistance = 1)
    {
        foreach (GameObject clipsEmiter in Instance.clipsPool.transform)
        {
            if (!clipsEmiter.GetComponent<AudioSource>().isPlaying) Instance.soundEmitter = clipsEmiter;
        }

        if (Instance.soundEmitter == null)
        {
            Instance.soundEmitter = new GameObject("AudioEmiter", typeof(SoundEmiter), typeof(AudioSource));
            Instance.soundEmitter.transform.SetParent(Instance.clipsPool.transform);
        } 

        AudioSource emitersAudioSource = Instance.soundEmitter.GetComponent<AudioSource>();

        Instance.soundEmitter.GetComponent<SoundEmiter>().target = followTarget;

        emitersAudioSource.playOnAwake = false;
        emitersAudioSource.spatialBlend = 1;
        emitersAudioSource.minDistance = minDistance;

        foreach (AudioClip c in Instance.clips)
        {
            if (c.name == clipName)
            {
                emitersAudioSource.clip = c;
            }
        }
    }

    public static void EmitParticleOnDestroy(string particleObjectName, Transform followTarget = null)
    {
        GameObject emiter = new GameObject("ParticleEmiter", typeof(ParticleEmiter));
        ParticleEmiter emitersParticleEmiter = emiter.GetComponent<ParticleEmiter>();

        emitersParticleEmiter.target = followTarget;

        foreach (GameObject g in Instance.particles)
        {
            if (g.name == particleObjectName)
            {
                Instantiate(g, emiter.transform);
            }
        }
    }
}
