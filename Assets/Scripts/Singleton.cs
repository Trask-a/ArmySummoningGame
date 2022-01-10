using UnityEngine;
using Unity.Netcode;

namespace Singletons
{
    public class Singleton<T> : NetworkBehaviour
        where T : Component
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    var objs = FindObjectsOfType(typeof(T)) as T[];
                    if(objs.Length > 0)
                    {
                        instance = objs[0];
                    }
                    if(objs.Length > 1)
                    {
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    }
                    if(instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = string.Format("_{0}", typeof(T).Name);
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }
}
