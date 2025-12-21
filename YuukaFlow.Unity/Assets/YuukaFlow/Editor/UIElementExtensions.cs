

using UnityEngine;
using UnityEngine.UIElements;

namespace YuukaFlow.Unity
{
    public static class UIElementExtensions
    {

        public static VisualElement Add(this VisualElement element, params VisualElement[] children)
        {
            foreach (var child in children)
            {
                element.Add(child);
            }
            return element;
        }

        public static VisualElement AddTo(this VisualElement element, VisualElement parent)
        {
            parent.Add(element);
            return element;
        }

        public static VisualElement SetElementBorderRadius(this VisualElement element, float radius)
        {
            element.style.borderTopLeftRadius = radius;
            element.style.borderTopRightRadius = radius;
            element.style.borderBottomLeftRadius = radius;
            element.style.borderBottomRightRadius = radius;

            return element;
        }

        public static VisualElement SetElementBorderWidth(this VisualElement element, float width)
        {
            element.style.borderTopWidth = width;
            element.style.borderBottomWidth = width;
            element.style.borderLeftWidth = width;
            element.style.borderRightWidth = width;

            return element;
        }

        public static VisualElement SetElementBorderColor(this VisualElement element, Color color)
        {
            element.style.borderTopColor = color;
            element.style.borderBottomColor = color;
            element.style.borderLeftColor = color;
            element.style.borderRightColor = color;

            return element;
        }

        public static VisualElement SetElementPadding(this VisualElement element, float paddingTop, float paddingRight, float paddingBottom, float paddingLeft)
        {
            element.style.paddingTop = paddingTop;
            element.style.paddingRight = paddingRight;
            element.style.paddingBottom = paddingBottom;
            element.style.paddingLeft = paddingLeft;

            return element;
        }

        public static VisualElement SetElementMargin(this VisualElement element, float marginTop, float marginRight, float marginBottom, float marginLeft)
        {
            element.style.marginTop = marginTop;
            element.style.marginRight = marginRight;
            element.style.marginBottom = marginBottom;
            element.style.marginLeft = marginLeft;

            return element;
        }

    }
}