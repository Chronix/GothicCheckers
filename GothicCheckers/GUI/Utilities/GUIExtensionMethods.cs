using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GothicCheckers.GUI.Utilities
{
    public static class GUIExtensionMethods
    {
        public static TranslateTransform GetTranslateTransform(this Shape s)
        {
            return s.RenderTransform as TranslateTransform;
        }
    }
}
