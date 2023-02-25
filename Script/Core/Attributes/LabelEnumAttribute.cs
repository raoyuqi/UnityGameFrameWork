using UnityEngine;

public class LabelEnumAttribute : PropertyAttribute
{
    private string m_Name;
    public string Name { get { return this.m_Name; } }

    public LabelEnumAttribute(string name)
    {
        this.m_Name = name;
    }
}