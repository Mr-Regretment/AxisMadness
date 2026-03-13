using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SoundEffectHandler : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public AudioClip clip;
        public float volume;
    }
    
    [SerializeField] List<SoundEffect> soundEffects = new List<SoundEffect>();
    public MonoBehaviour soundEffectController;
    public string soundEffectControllerMethod;
    private MethodInfo _cachedMethod;

    private void Update()
    {
        if(soundEffectController == null)
            return;
        
        if(string.IsNullOrEmpty(soundEffectControllerMethod))
            return;
        
        _cachedMethod =  soundEffectController.GetType().GetMethod(soundEffectControllerMethod);
        
        
        object result = _cachedMethod.Invoke(soundEffectController, null);
        if (_cachedMethod == null)
        {
            
        }
    }
}