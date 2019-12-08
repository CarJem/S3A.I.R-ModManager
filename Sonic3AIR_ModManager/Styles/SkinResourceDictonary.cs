using System;
using System.Windows;

namespace Sonic3AIR_ModManager
{
    public class SkinResourceDictionary : ResourceDictionary
    {
        private Uri _DarkSource;
        private Uri _LightSource;

        public Uri DarkSource
        {
            get { return _DarkSource; }
            set
            {
                _DarkSource = value;
                UpdateSource();
            }
        }
        public Uri LightSource
        {
            get { return _LightSource; }
            set
            {
                _LightSource = value;
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            var val = GetSkin();
            if (val != null && base.Source != val)
                base.Source = val;
        }

        public Uri GetSkin()
        {
            if (App.Skin == Skin.Light)
            {
                return LightSource;
            }
            else if (App.Skin == Skin.Dark)
            {
                return DarkSource;

            }
            else return LightSource;
        }
    }
}
