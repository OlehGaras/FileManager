﻿using System;
using System.Windows.Controls;

namespace FileManager
{
    public interface IViewController
    {
        void SetLeftPanelContent(UserControl content);
        void SetRightPanelContent(UserControl content);
        void SetShortcutPanelContent(UserControl content);
        void ChangeStyle();
        event EventHandler StyleChanged;
    }
}
