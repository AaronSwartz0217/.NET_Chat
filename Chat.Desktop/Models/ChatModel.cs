using Avalonia.Controls;
using Avalonia.Layout;
using System;

namespace Chat.Desktop.Models
{
    public class ChatModel
    {
        public string? NickName { get; set; }

        public string? Content { get; set; }

        public DateTime SendTime { get; set; }

        public HorizontalAlignment TextAlignment { get; set; } = HorizontalAlignment.Left;

        public Dock TextDock { get; set; } = Dock.Left;
    }
}