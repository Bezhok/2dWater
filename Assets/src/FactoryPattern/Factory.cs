using UnityEngine;

namespace src.FactoryPattern
{
    public class Factory<T> where T : MonoBehaviour, IInitializable
    {
        public static T CreateInstance(GameObject gameObject, IData data)
        {
            var obj = gameObject.AddComponent<T>();
            obj.Init(data);
            return obj;
        }
    }
}