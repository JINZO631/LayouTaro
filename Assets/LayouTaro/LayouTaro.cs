﻿using System.Collections.Generic;
using UnityEngine;

namespace UILayouTaro
{
    public class LayouTaro
    {

        public static GameObject Layout<T>(Transform parent, Vector2 size, GameObject rootObject, ILayouter layouter) where T : LTRootElement
        {
            var originX = 0f;
            var originY = 0f;

            var rootElement = rootObject.GetComponent<T>();
            var elements = rootElement.GetLTElements();

            var rootRect = rootObject.GetComponent<RectTransform>();

            // 親の基礎サイズをセット
            rootRect.sizeDelta = size;

            // set parent.
            foreach (var element in elements)
            {
                element.gameObject.transform.SetParent(rootObject.transform);
            }

            // ここでCanvas要素にセットしちゃう(でないとTMProのinfoが取れない。)
            rootObject.transform.SetParent(parent);

            var lineContents = new List<RectTransform>();// 同じ行に入っている要素を整列させるために使用するリスト
            var currentLineMaxHeight = 0f;

            layouter.Layout(size, out originX, out originY, rootObject, rootElement, elements, ref currentLineMaxHeight, ref lineContents);

            lineContents.Clear();

            // var lastHeight = originY + elements[elements.Length - 1].GetComponent<RectTransform>().sizeDelta.y;

            return rootObject;
        }

        public static GameObject RelayoutWithUpdate<T>(Vector2 size, GameObject rootObject, Dictionary<LTElementType, object> updateValues, ILayouter layouter) where T : LTRootElement
        {
            var originX = 0f;
            var originY = 0f;

            var rootElement = rootObject.GetComponent<T>();
            var elements = rootElement.GetLTElements();
            foreach (var element in elements)
            {
                if (element is ILayoutableText)
                {
                    if (0 < element.transform.childCount)
                    {
                        for (var i = 0; i < element.transform.childCount; i++)
                        {
                            var child = element.transform.GetChild(i);
                            GameObject.Destroy(child.gameObject);
                        }
                    }
                }
            }

            layouter.UpdateValues(elements, updateValues);

            var lineContents = new List<RectTransform>();// 同じ行に入っている要素を整列させるために使用するリスト
            var currentLineMaxHeight = 0f;

            layouter.Layout(size, out originX, out originY, rootObject, rootElement, elements, ref currentLineMaxHeight, ref lineContents);

            lineContents.Clear();

            return rootObject;
        }
    }
}