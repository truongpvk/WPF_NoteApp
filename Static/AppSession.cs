using NoteApp_DoAn.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnNote.Models
{
    public static class AppSession
    {
        public static int UserId { get; set; } = -1;
        public static string Username { get; set; } = null;
        public static string DisplayName { get; set; } = null;
        public static bool IsOnNote { get; set; } = false;
        public static NoteView OnNote { get; set; } = null;

        public static void SetAnonymous(string alias)
        {
            UserId = -9999;
            Username = null;
            DisplayName = alias;
        }

        public static void BeginSession(int userId, string username, string displayName)
        {
            UserId = userId;
            Username = username;
            DisplayName = displayName;
        }

        public static void Clear()
        {
            UserId = -1;
            Username = null;
            DisplayName = null;
        }


    }
}
