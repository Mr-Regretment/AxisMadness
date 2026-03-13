using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class Utilities
{
    public float percentage;
    private Type _type;

    
    
    
    
    
    public static bool IsGreaterThan(Vector3 a, Vector3 b)
    {
        return a.x > b.x && a.y > b.y && a.z > b.z;
    }
    
    
    
    
    
    
    public static bool IsLessThan(Vector3 a, Vector3 b)
    {
        return a.x < b.x && a.y < b.y && a.z < b.z;
    }
    
    
    
    
    public static T SmoothLerp<T>(ref float elapsedTime, T targetValue, T currentValue, float duration)
    {
        elapsedTime += Time.deltaTime;
        Utilities utilities = new Utilities();
        
        utilities.percentage = Mathf.Clamp01(elapsedTime / duration);
        float smoothPercentage = Mathf.SmoothStep(0, 1, utilities.percentage);

        return LerpValue(currentValue, targetValue, smoothPercentage);
    }
    
    
    
    
    private static T LerpValue<T>(T from, T to, float t)
    {
        if (typeof(T) == typeof(float))
        {
            float f = (float)(object)from;
            float t2 = (float)(object)to;
            return (T)(object)Mathf.Lerp(f, t2, t);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            Vector2 f = (Vector2)(object)from;
            Vector2 t2 = (Vector2)(object)to;
            return (T)(object)Vector2.Lerp(f, t2, t);
        }
        else if (typeof(T) == typeof(Vector3))
        {
            Vector3 f = (Vector3)(object)from;
            Vector3 t2 = (Vector3)(object)to;
            return (T)(object)Vector3.Lerp(f, t2, t);
        }
        else if (typeof(T) == typeof(Vector4))
        {
            Vector4 f = (Vector4)(object)from;
            Vector4 t2 = (Vector4)(object)to;
            return (T)(object)Vector4.Lerp(f, t2, t);
        }
        else if (typeof(T) == typeof(Color))
        {
            Color f = (Color)(object)from;
            Color t2 = (Color)(object)to;
            return (T)(object)Color.Lerp(f, t2, t);
        }
        else if (typeof(T) == typeof(Quaternion))
        {
            Quaternion f = (Quaternion)(object)from;
            Quaternion t2 = (Quaternion)(object)to;
            return (T)(object)Quaternion.Lerp(f, t2, t);
        }
        
        throw new NotSupportedException($"Type {typeof(T).Name} is not supported for lerping");
    }

    
    
    
    public static bool IsLerpComplete(float percentage)
    {
        return percentage >= 1f;
    }

    
    
    
    public static void ResetLerp(ref float elapsedTime, ref float percentage)
    {
        elapsedTime = 0f;
        percentage = 0f;
    }

    public Vector3 Vector3Sign(Vector3 inputVector)
    {
        return new Vector3(SignWith0(inputVector.x),SignWith0(inputVector.y),SignWith0(inputVector.z));
    }

    public float SignWith0(float value)
    {
        if (value > 0)
            return 1;
        
        if(value < 0 )
            return -1;
        
        return value;

    }
    
    
    
    
    
    public static int BoolIntConversion(bool boolean)
    {
        return boolean switch
        {
            true => 1,
            false => 0
        };
    }
    
}