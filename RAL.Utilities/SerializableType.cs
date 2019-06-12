using System;
using System.Xml.Serialization;

namespace TheColonel2688.Utilities
{
    // a version of System.Type that can be serialized
    //[DataContract]
    public class SerializableType
    {

        [XmlIgnore]
        public Type Type
        {
            get
            {
                var blah = Type.GetType(TypeAsString, true);
                return blah;
            }
            set { TypeAsString = value.FullName; }

        }

        // when serializing, store as a string
        public string TypeAsString { get; set; }

        // constructors
        public SerializableType()
        {
            //Type = null;
        }
        public SerializableType(Type t)
        {
            Type = t;
        }

        // allow SerializableType to implicitly be converted to and from System.Type
        static public implicit operator Type(SerializableType stype)
        {
            return stype.Type;
        }
        static public implicit operator SerializableType(Type t)
        {
            return new SerializableType(t);
        }

        // overload the == and != operators
        public static bool operator ==(SerializableType a, SerializableType b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Type == b.Type;
        }
        public static bool operator !=(SerializableType a, SerializableType b)
        {
            return !(a == b);
        }
        // we don't need to overload operators between SerializableType and System.Type because we already enabled them to implicitly convert

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        // overload the .Equals method
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to SerializableType return false.
            SerializableType p = obj as SerializableType;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Type == p.Type);
        }
        public bool Equals(SerializableType p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Type == p.Type);
        }
    }
}
