using System;
using System.Collections.Generic;
using UnityEngine;

public class RequiredFieldTest : MonoBehaviour
{

    public enum MyEnum
    {
        A, B, C
    }

    [Serializable]
    public class MyCustomData
    {
        [RequiredField] public string name;
        [RequiredField] public Transform target;

        [RequiredField] public MyCustomData next;
    }

    [Serializable]
    public class MyGenericType<T>
    {
        [RequiredField] public T value;
    }


    [RequiredField] private Transform m_BrakeTransform;
    public Transform BT => m_BrakeTransform;


    [FlexiableHeader("Public Fields", 24, TextAnchor.MiddleCenter, FontStyle.Bold, 1.0f, 0.0f, 0.0f, 1.0f)]
    [RequiredField] public MyGenericType<Texture2D> m_MyPublicGenericVariable;
    [RequiredField] public MyCustomData m_MyPublicCustomDataVariable;
    [RequiredField] public int m_MyPublicIntVariable;
    [RequiredField] public bool m_MyPublicBoolVariable;
    [RequiredField] public float m_MyPublicFloatVariable;
    [RequiredField] public string m_MyPublicStringVariable;
    [RequiredField] public Color m_MyPublicColorVariable;
    [RequiredField] public UnityEngine.Object m_MyPublicObjectVariable;
    [RequiredField] public LayerMask m_MyPublicLayerMaskVariable;
    [RequiredField] public MyEnum m_MyPublicEnumVariable;
    [RequiredField] public Vector2 m_MyPublicVector2Variable;
    [RequiredField] public Vector3 m_MyPublicVector3Variable;
    [RequiredField] public Vector4 m_MyPublicVector4Variable;
    [RequiredField] public Rect m_MyPublicRectVariable;
    [RequiredField] public char m_MyPublicCharVariable;
    [RequiredField] public AnimationCurve m_MyPublicAnimationCurveVariable;
    [RequiredField] public Bounds m_MyPublicBoundsVariable;
    [RequiredField] public Gradient m_MyPublicGradientVariable;
    [RequiredField] public Quaternion m_MyPublicQuaternionVariable;
    [RequiredField] public Vector2Int m_MyPublicVector2IntVariable;
    [RequiredField] public Vector3Int m_MyPublicVector3IntVariable;
    [RequiredField] public RectInt m_MyPublicRectIntVariable;
    [RequiredField] public BoundsInt m_MyPublicBoundsIntVariable;
    [RequiredField] public Hash128 m_MyPublicHash128Variable;
    [RequiredField] public Transform[] m_MyPublicArrayVariable;
    [RequiredField] public List<GameObject> m_MyPublicListVariable;


    [FlexiableHeader("Serialize Fields", 24, TextAnchor.MiddleCenter, FontStyle.Bold, 0.0f, 1.0f, 0.0f, 1.0f)]
    [SerializeField, RequiredField] private MyGenericType<int> m_MyPrivateSerializeGenericVariable;
    [SerializeField, RequiredField] private MyCustomData m_MyPrivateSerializeCustomDataVariable;
    [SerializeField, RequiredField] private int m_MyPrivateSerializeIntVariable;
    [SerializeField, RequiredField] private bool m_MyPrivateSerializeBoolVariable;
    [SerializeField, RequiredField] private float m_MyPrivateSerializeFloatVariable;
    [SerializeField, RequiredField] private string m_MyPrivateSerializeStringVariable;
    [SerializeField, RequiredField] private Color m_MyPrivateSerializeColorVariable;
    [SerializeField, RequiredField] private UnityEngine.Object m_MyPrivateSerializeObjectVariable;
    [SerializeField, RequiredField] private LayerMask m_MyPrivateSerializeLayerMaskVariable;
    [SerializeField, RequiredField] private MyEnum m_MyPrivateSerializeEnumVariable;
    [SerializeField, RequiredField] private Vector2 m_MyPrivateSerializeVector2Variable;
    [SerializeField, RequiredField] private Vector3 m_MyPrivateSerializeVector3Variable;
    [SerializeField, RequiredField] private Vector4 m_MyPrivateSerializeVector4Variable;
    [SerializeField, RequiredField] private Rect m_MyPrivateSerializeRectVariable;
    [SerializeField, RequiredField] private char m_MyPrivateSerializeCharVariable;
    [SerializeField, RequiredField] private AnimationCurve m_MyPrivateSerializeAnimationCurveVariable;
    [SerializeField, RequiredField] private Bounds m_MyPrivateSerializeBoundsVariable;
    [SerializeField, RequiredField] private Gradient m_MyPrivateSerializeGradientVariable;
    [SerializeField, RequiredField] private Quaternion m_MyPrivateSerializeQuaternionVariable;
    [SerializeField, RequiredField] private Vector2Int m_MyPrivateSerializeVector2IntVariable;
    [SerializeField, RequiredField] private Vector3Int m_MyPrivateSerializeVector3IntVariable;
    [SerializeField, RequiredField] private RectInt m_MyPrivateSerializeRectIntVariable;
    [SerializeField, RequiredField] private BoundsInt m_MyPrivateSerializeBoundsIntVariable;
    [SerializeField, RequiredField] private Hash128 m_MyPrivateSerializeHash128Variable;
    [SerializeField, RequiredField] private Transform[] m_MyPrivateSerializeArrayVariable;
    [SerializeField, RequiredField] private List<GameObject> m_MyPrivateSerializeListVariable;

}
