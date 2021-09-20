using System;
using System.Windows.Forms;

namespace SettingsManager
{
    public interface ISettingsObject
    {
        string SettingsTypeName { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        Type ValueType { get; set; }
        object Value { get; set; }
        object InitialValue { get; set; }
        object Control { get; set; }
        string RegKey { get; set; }
        Keys ShortCutKeys { get; set; }

        void SetValueToControlFromMember();
        void SetValueToMemberFromControl();
        void SetValue(object value);
    }
}
