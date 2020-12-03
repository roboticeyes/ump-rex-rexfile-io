/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using UnityEngine;
using UnityEngine.UI;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
    public class TextPrefab : RexTextObject
    {
        [SerializeField]
        private Text uiText;

        public override bool SetText (string text, Color textColor)
        {
            uiText.text = text;
            uiText.color = textColor;

            //Text has no bounds
            Bounds = new Bounds();

            return true;
        }

        public override void SetRendererEnabled (bool enabled)
        {
            uiText.enabled = enabled;
        }

        public override void SetLayer (int layer)
        {
            uiText.gameObject.layer = layer;
        }
    }
}
