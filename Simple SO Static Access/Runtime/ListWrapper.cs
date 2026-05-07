namespace QuietNoize.SimpleSOStaticAccess
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper wrapper used for Unity JsonUtility serialization of generic lists.
    /// </summary>
    [Serializable]
    public class ListWrapper<T>
    {
        public List<T> items;

        public ListWrapper() { }

        public ListWrapper(List<T> items)
        {
            this.items = items;
        }
    }
}