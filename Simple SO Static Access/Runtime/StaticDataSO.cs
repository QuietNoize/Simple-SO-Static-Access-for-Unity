namespace QuietNoize.SimpleSOStaticAccess
{
    using System;
    using UnityEngine;

    /// <summary>
    /// ScriptableObject that stores data used for static code generation.
    /// </summary>
    public class StaticDataSO : ScriptableObject
    {
        [SerializeField] private StaticDataCodegenSettings m_codegenSettings;
        public StaticDataCodegenSettings codegenSettings => m_codegenSettings;
    }

    [Serializable]
    public struct StaticDataCodegenSettings
    {
        [SerializeField] private bool m_isAutoSaved;
        [SerializeField] private string m_wrapperCodePath;
        [SerializeField] private string m_wrapperClassName;
        [SerializeField] private string m_wrapperCodeNamespace;

        public bool IsAutoSaved => m_isAutoSaved;
        public string WrapperCodePath => m_wrapperCodePath;
        public string WrapperClassName => m_wrapperClassName;
        public string WrapperCodeNamespace => m_wrapperCodeNamespace;
    }
}
