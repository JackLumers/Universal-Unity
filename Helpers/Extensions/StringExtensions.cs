using System;
using UnityEngine;
using UnityEngine.UI;
using UniversalUnity.Helpers.Logs;

namespace UniversalUnity.Helpers.Extensions
{
    public static class StringOperationsHelper
    {
        public static string Cut(this string text, int maxTextSize)
        {
            if (text.Length <= maxTextSize) return text;
            return string.Concat(text.Substring(0, maxTextSize).Trim(' '), "...");
        }
        
        public static void FitStringInField(this string text, Text textField)
        {
            var maxTextSize = GetStingMaxSizeToFitInField(text, textField);
            textField.text = Cut(text, maxTextSize);
        }

        public static int GetStingMaxSizeToFitInField(this string text, Text textField)
        {
            textField.text = text;
            textField.font.RequestCharactersInTexture(textField.text, textField.fontSize, textField.fontStyle);
            
            var rect = textField.rectTransform.rect;
            float textFieldWidth = rect.width;
            float textFieldHeight = rect.height;
            
            int textWidth = 0;
            float textHeight = 0;
            int textSize = 0;

            // line by line
            while (textHeight < textFieldHeight && textSize < text.Length)
            {
                CharacterInfo characterInfo = default;
                // character by character
                while (textWidth < textFieldWidth && textSize < text.Length)
                {
                    if (text[textSize].Equals('\n'))
                    {
                        textHeight += textField.lineSpacing * characterInfo.glyphHeight;
                    }

                    textField.font.GetCharacterInfo(text[textSize], out characterInfo, textField.fontSize,
                        textField.fontStyle);
                    textWidth += characterInfo.advance;

                    textSize++;
                }

                textWidth = 0;
                textHeight += characterInfo.glyphHeight * 3f;
                textHeight += textField.lineSpacing * characterInfo.glyphHeight;
            }
            
            LogHelper.LogInfo($"Text height: {textHeight}", nameof(GetStingMaxSizeToFitInField));
            LogHelper.LogInfo($"TextField line spacing: {textField.lineSpacing}", nameof(GetStingMaxSizeToFitInField));
            LogHelper.LogInfo($"TextField line spacing: {textField.lineSpacing}", nameof(GetStingMaxSizeToFitInField));
            return textSize;
        }
        
        public static bool CheckLineBreaks(int index, string text, int linebreaksCount)
        {
            int breaksCount = 0;
            
            for (var i = index; i < index + Environment.NewLine.Length * linebreaksCount; i++)
            {
                if (index < text.Length 
                    && text[index] == '\n' || 
                    index + 1 < text.Length
                    && text[index] == '\n' && text[index + 1] == '\r')
                {
                    breaksCount++;
                }
            }

            return breaksCount >= linebreaksCount;
        }
    }
}
