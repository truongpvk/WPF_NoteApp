using DoAnNote;
using DoAnNote.Models;
using NoteApp_DoAn.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NoteApp_DoAn.ViewModel.NoteVM
{
    public static class NoteViewModel
    {
        public static NoteView ToView (Note note)
        {
            NoteView noteVM = new NoteView();
            noteVM.Id = note.NoteID;
            noteVM.Title = note.Title;
            noteVM.Content = note.Content;
            noteVM.CreatedAt = note.CreatedAt ?? DateTime.Now;
            noteVM.UpdatedAt = note.UpdatedAt ?? DateTime.Now;
            noteVM.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(note.Background));
            noteVM.IsActive = note.Status == "active";

            return noteVM;
        }

        public static Note ToModel(NoteView vm)
        {
            Note note = new Note();
            note.NoteID = vm.Id;
            note.Title = vm.Title;
            note.Content = vm.Content;
            note.CreatedAt = vm.CreatedAt;
            note.UpdatedAt = vm.UpdatedAt;
            note.Background = vm.Background.ToString();
            note.Status = vm.IsActive ? "active" : "inactive";

            note.UserID = AppSession.UserId;

            return note;
        }
    }
}
