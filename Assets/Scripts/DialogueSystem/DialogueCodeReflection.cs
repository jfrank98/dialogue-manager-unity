using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class DialogueCodeReflection : MonoBehaviour
{
    public static object GetSingletonInstance(string singletonClassName)
    {
        // Get the type of the singleton class
        Type type = Type.GetType(singletonClassName);

        if (type != null)
        {
            // Get the property info for the 'Instance' property
            PropertyInfo instanceProperty = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

            if (instanceProperty != null)
            {
                // Get the singleton instance
                return instanceProperty.GetValue(null);
            }
        }

        // Singleton class or 'Instance' property not found
        return null;
    }

    public static object GetFieldValue(object obj, string fieldName)
    {
        // Get the type of the object
        Type type = obj.GetType();

        // Get the field information
        FieldInfo fieldInfo = type.GetField(fieldName);

        // Check if the field exists
        if (fieldInfo != null)
        {
            // Get the value of the field
            return fieldInfo.GetValue(obj);
        }

        // Field not found
        return null;
    }

    public static object GetPropertyValue(object obj, string propertyName)
    {
        // Get the type of the object
        Type type = obj.GetType();

        // Get the property information
        PropertyInfo propertyInfo = type.GetProperty(propertyName);

        // Check if the property exists
        if (propertyInfo != null)
        {
            // Get the value of the property
            return propertyInfo.GetValue(obj);
        }

        // Property not found
        return null;
    }

    public static void SetFieldValue(object obj, string fieldName, object value)
    {
        // Get the type of the object
        Type type = obj.GetType();
        // Get the field information
        FieldInfo fieldInfo = type.GetField(fieldName);
        // Check if the field exists
        if (fieldInfo != null)
        {
            // Get the type of the field
            Type fieldType = fieldInfo.FieldType;
            value = Convert.ChangeType(value, fieldType);

            // Set the value of the field
            fieldInfo.SetValue(obj, value);
        }
    }

    public static void SetPropertyValue(object obj, string propertyName, object value)
    {
        // Get the type of the object
        Type type = obj.GetType();

        // Get the property information
        PropertyInfo propertyInfo = type.GetProperty(propertyName);

        // Check if the property exists
        if (propertyInfo != null)
        {
            // Set the value of the property
            propertyInfo.SetValue(obj, value);
        }
    }

    public static object CallMethod(object obj, string methodName, object[] parameters)
    {
        // Get the type of the object
        Type type = obj.GetType();

        // Get the method information
        MethodInfo methodInfo = type.GetMethod(methodName);

        // Check if the method exists
        if (methodInfo != null)
        {
            // Call the method
            return methodInfo.Invoke(obj, parameters);
        }

        // Method not found
        return null;
    }

   public static void EmitEvent(object obj, string eventName, object parameter = null)
    {
        // Get the type of the object
        Type type = obj.GetType();

        // Try to get the event field using reflection
        FieldInfo fieldInfo = type.GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (fieldInfo == null)
        {
            Debug.LogError($"Field for event '{eventName}' not found on type '{type}'.");
            return;
        }

        // Get the UnityEvent from the field
        UnityEventBase unityEvent = fieldInfo.GetValue(obj) as UnityEventBase;
        if (unityEvent == null)
        {
            Debug.LogError($"Field '{eventName}' is not a UnityEvent.");
            return;
        }

        // Check the type of the UnityEvent to handle parameter
        Type eventType = unityEvent.GetType();
        MethodInfo invokeMethod = eventType.GetMethod("Invoke");
        ParameterInfo[] parameters = invokeMethod.GetParameters();
        if (parameters.Length == 0)
        {
            // No parameter expected
            invokeMethod.Invoke(unityEvent, new object[0]);
        }
        else if (parameters.Length == 1 && parameter != null && parameters[0].ParameterType.IsAssignableFrom(parameter.GetType()))
        {
            // Single parameter expected and provided
            invokeMethod.Invoke(unityEvent, new object[] { parameter });
        }
        else
        {
            Debug.LogError("Event parameter mismatch or not supported.");
        }
    }
}
