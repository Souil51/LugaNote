using System;

namespace Assets
{
    /// <summary>
    /// Binding path for dateBinding
    /// Name is the name of the Property in the ViewModel
    /// LocalizeName the name of the value for the localization
    /// LastValue is used to know the last updated value (= current value)
    /// </summary>
    [Serializable]
    public class BindingPath
    {
        public string Name;
        public string LocalizeName;
        public string BeforeConcatenationString;
        public string AfterConcatenationString;

        public object LastValue;
    }
}
