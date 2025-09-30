using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NoteApp_DoAn.View
{
    public class NoteView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public SolidColorBrush Background { get; set; }

        public bool IsActive { get; set; }

        public string TitlePreview => string.IsNullOrWhiteSpace(Title) ? "(Không có tiêu đề)" : Title;
        public string PreviewContent => Content.Length > 80 ? Content.Substring(0, 80) + "..." : Content;
        public string DateString => UpdatedAt.ToString("g");
    }
}
