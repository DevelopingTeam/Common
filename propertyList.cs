using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using System.Reflection.Emit;
using System.ComponentModel;
namespace Analysis
{
    /// <summary>
    /// Property操作类
    /// by zhangbc 2015-07-06 
    /// </summary>
    #region 属性表管理类
    /// <summary>
    /// 属性表管理类
    /// </summary>
    public class PropertyManageCls : CollectionBase, ICustomTypeDescriptor
    {
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="value">属性值</param>
        public void Add(Property value)
        {
            int flag = -1;
            if (value != null)
            {
                if (base.List.Count > 0)
                {
                    IList<Property> mList = new List<Property>();
                    for (int i = 0; i < base.List.Count; i++)
                    {
                        Property p = base.List[i] as Property;
                        if (value.Name == p.Name)
                        {
                            flag = i;
                        }
                        mList.Add(p);
                    }
                    if (flag == -1)
                    {
                        mList.Add(value);
                    }
                    base.List.Clear();
                    foreach (Property p in mList)
                    {
                        base.List.Add(p);
                    }
                }
                else
                {
                    base.List.Add(value);
                }
            }
        }

        /// <summary>
        /// 移除属性
        /// </summary>
        /// <param name="value">属性值</param>
        public void Remove(Property value)
        {
            if (value != null && base.List.Count > 0)
                base.List.Remove(value);
        }

        public Property this[int index]
        {
            get
            {
                return (Property)base.List[index];
            }
            set
            {
                base.List[index] = (Property)value;
            }
        }

        #region ICustomTypeDescriptor 成员
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] newProps = new PropertyDescriptor[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                Property prop = (Property)this[i];
                newProps[i] = new CustomPropertyDescriptor(ref prop, attributes);
            }
            return new PropertyDescriptorCollection(newProps);
        }
        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
    }
    #endregion

    #region 属性类
    /// <summary>
    /// 属性类
    /// </summary>
    public class Property
    {
        private string _name = string.Empty;
        private object _value = null;
        private bool _readonly = false;
        private bool _visible = true;
        private string _category = string.Empty;
        TypeConverter _converter = null;
        object _editor = null;
        private string _displayname = string.Empty;

        /// <summary>
        /// Property初始化
        /// </summary>
        /// <param name="sName">属性名</param>
        /// <param name="sValue">属性值</param>
        public Property(string sName, object sValue)
        {
            this._name = sName;
            this._value = sValue;
        }

        /// <summary>
        /// Property初始化(重载)
        /// </summary>
        /// <param name="sName">属性名</param>
        /// <param name="sValue">属性值</param>
        /// <param name="sReadonly">只读属性</param>
        /// <param name="sVisible">可见属性</param>
        public Property(string sName, object sValue, bool sReadonly, bool sVisible)
        {
            this._name = sName;
            this._value = sValue;
            this._readonly = sReadonly;
            this._visible = sVisible;
        }
        /// <summary>
        /// 获得属性名
        /// </summary>
        public string Name  
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        /// <summary>
        /// 属性显示名称
        /// </summary>
        public string DisplayName  
        {
            get
            {
                return _displayname;
            }
            set
            {
                _displayname = value;
            }
        }
        /// <summary>
        /// 类型转换器
        /// </summary>
        public TypeConverter Converter  
        {
            get
            {
                return _converter;
            }
            set
            {
                _converter = value;
            }
        }
        /// <summary>
        /// 属性所属类别
        /// </summary>
        public string Category  
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }
        /// <summary>
        /// 属性值
        /// </summary>
        public object Value  
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        /// <summary>
        /// 是否为只读属性
        /// </summary>
        public bool ReadOnly  
        {
            get
            {
                return _readonly;
            }
            set
            {
                _readonly = value;
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible  
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }
        /// <summary>
        /// 属性编辑器
        /// </summary>
        public virtual object Editor   
        {
            get
            {
                return _editor;
            }
            set
            {
                _editor = value;
            }
        }
    }
    #endregion

    #region 属性描述类
    /// <summary>
    /// 属性描述类
    /// </summary>
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        Property m_Property;
        public CustomPropertyDescriptor(ref Property myProperty, Attribute[] attrs)
            : base(myProperty.Name, attrs)
        {
            m_Property = myProperty;
        }

        #region PropertyDescriptor 重写方法
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override Type ComponentType
        {
            get
            {
                return null;
            }
        }
        public override object GetValue(object component)
        {
            return m_Property.Value;
        }
        public override string Description
        {
            get
            {
                return m_Property.Name;
            }
        }
        public override string Category
        {
            get
            {
                return m_Property.Category;
            }
        }
        public override string DisplayName
        {
            get
            {
                return m_Property.DisplayName != "" ? m_Property.DisplayName : m_Property.Name;
            }
        }
        public override bool IsReadOnly
        {
            get
            {
                return m_Property.ReadOnly;
            }
        }
        public override void ResetValue(object component)
        {
            //Have to implement
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        public override void SetValue(object component, object value)
        {
            m_Property.Value = value;
        }
        public override TypeConverter Converter
        {
            get
            {
                return m_Property.Converter;
            }
        }
        public override Type PropertyType
        {
            get { return m_Property.Value.GetType(); }
        }
        public override object GetEditor(Type editorBaseType)
        {
            return m_Property.Editor == null ? base.GetEditor(editorBaseType) : m_Property.Editor;
        }
        #endregion
    }
    #endregion
}
