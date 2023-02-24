using UnityEngine;

namespace Core.Attributes
{
    public class LabelTextAttribute : PropertyAttribute
    {
        private string m_Name;
        public string Name { get { return this.m_Name; } }

        public LabelTextAttribute(string name)
        {
            this.m_Name = name;
        }
    }
}