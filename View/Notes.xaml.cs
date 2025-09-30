using DoAnNote.Models;
using DoAnNote.Static;
using NoteApp_DoAn.View;
using NoteApp_DoAn.ViewModel.NoteVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoAnNote
{
    /// <summary>
    /// Interaction logic for Notes.xaml
    /// </summary>
    public partial class Notes : Window
    {
        private List<SolidColorBrush> noteColors = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromRgb(255, 228, 196)), // Bisque
            new SolidColorBrush(Color.FromRgb(255, 250, 205)), // LemonChiffon
            new SolidColorBrush(Color.FromRgb(224, 255, 255)), // LightCyan
            new SolidColorBrush(Color.FromRgb(240, 230, 140)), // Khaki
            new SolidColorBrush(Color.FromRgb(255, 182, 193)), // LightPink
            new SolidColorBrush(Color.FromRgb(176, 224, 230)), // PowderBlue
            new SolidColorBrush(Color.FromRgb(221, 160, 221)), // Plum
            new SolidColorBrush(Color.FromRgb(144, 238, 144)), // LightGreen
            new SolidColorBrush(Color.FromRgb(255, 218, 185)), // PeachPuff
            new SolidColorBrush(Color.FromRgb(255, 239, 213))  // PapayaWhip
        };

        private Point _startPoint; // Vị trí bắt đầu khi kéo thả
        private bool _isDragging = false;

        private bool _isTrashPage = false;

        private ObservableCollection<NoteView> notes = new ObservableCollection<NoteView>();
        private ObservableCollection<NoteView> trash = new ObservableCollection<NoteView>();

        private ICollectionView notesView;
        public Notes(string userName)
        {
            InitializeComponent();


            TxtHello.Text = "Xin chào, " + (userName ?? "").Trim();

            UILoad();

        }
        public void UILoad()
        {
            if (AppSession.UserId >= 0)
                loadNote();

            ListLayer.Visibility = Visibility.Visible;
            GreetingBar.Visibility = Visibility.Visible;
            DetailLayer.Visibility = Visibility.Collapsed;


            if (_isTrashPage)
            {
                CornerZones.Visibility = Visibility.Visible;
                DropZone.Visibility = Visibility.Collapsed;
                notesView = CollectionViewSource.GetDefaultView(trash);
                NotesItems.ItemsSource = notesView;
            }
            else
            {
                CornerZones.Visibility = Visibility.Collapsed;
                DropZone.Visibility = Visibility.Visible;
                notesView = CollectionViewSource.GetDefaultView(notes);
                NotesItems.ItemsSource = notesView;
            }
        }
        private void loadNote()
        {
            using (var db = new AppDatabaseEntities())
            {
                var userNotes = db.Notes.Where(n => n.UserID == AppSession.UserId)
                                        .OrderByDescending(n => n.UpdatedAt)
                                        .ToList();
                Console.WriteLine($"Found {userNotes.Count} notes for user ID: {AppSession.UserId}");

                notes.Clear();
                trash.Clear();
                foreach (var note in userNotes)
                {
                    var view = NoteViewModel.ToView(note);
                    if (view.IsActive)
                    {
                        notes.Add(view);
                    }
                    else
                    {
                        trash.Add(view);
                    }
                }
            }
        }

        public NoteView AddNote()
        {
            var preview = new NoteView();

            if (AppSession.UserId < 0)
            {
                preview.Id = notes.Count > 0 ? notes.Max(n => n.Id) + 1 : 1;
            }
            

            preview.Title = "Tiêu đề...";
            preview.Content = "Nội dung...";
            preview.IsActive = true;

            var random = new Random();
            var brush = noteColors[random.Next(noteColors.Count)];

            preview.Background = brush;


            notes.Add(preview);

            return preview;
        }


        /// <summary>
        /// Sự kiện đăng xuất
        /// </summary>
        private void Click_Logout(object sender, RoutedEventArgs e)
        {
            AppSession.Clear();

            var win = new MainWindow();
            win.Show();
            this.Close();
        }

        /// <summary>
        /// Khi di chuột vào: phóng to icon + hiện sáng halo
        /// </summary>
        private void AnimateHover(System.Windows.Media.ScaleTransform iconScale,
                                  UIElement halo,
                                  System.Windows.Media.ScaleTransform haloScale)
        {
            // Hiện halo (opacity 0 -> 1)
            var showHalo = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            halo.BeginAnimation(UIElement.OpacityProperty, showHalo);

            // Phóng to halo nhẹ
            var growHalo = new DoubleAnimation(0.95, 1.05, TimeSpan.FromMilliseconds(200));
            haloScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, growHalo);
            haloScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, growHalo);
        }

        /// <summary>
        /// Khi rời chuột: trả về kích thước + ẩn halo
        /// </summary>
        private void AnimateLeave(System.Windows.Media.ScaleTransform iconScale,
                                  UIElement halo,
                                  System.Windows.Media.ScaleTransform haloScale)
        {
            // Ẩn halo (opacity 1 -> 0)
            var hideHalo = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            halo.BeginAnimation(UIElement.OpacityProperty, hideHalo);

            // Thu nhỏ halo về gốc
            var shrinkHalo = new DoubleAnimation(1.05, 0.95, TimeSpan.FromMilliseconds(200));
            haloScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, shrinkHalo);
            haloScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, shrinkHalo);
        }

        private void CardRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _isDragging = false;

        }

        private void CardRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
            {
                //NoteCard_Click(sender, e);
            }
        }

        private void CardRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(null);
                Vector diff = _startPoint - currentPosition;

                if (!_isDragging &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                     Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    _isDragging = true;

                    var border = sender as Border;
                    if (border != null)
                    {
                        DragDrop.DoDragDrop(border, border.DataContext, DragDropEffects.Move);
                    }
                }

            }
        }

        private void DropZoneCenter_DragEnter(object sender, DragEventArgs e)
        {
            AnimateHover(DropZoneScale, DropZoneHalo, DropHaloScale);
        }

        private void DropZoneCenter_DragLeave(object sender, DragEventArgs e)
        {
            if (!DropZoneCenter.IsMouseOver)
            {
                AnimateLeave(DropZoneScale, DropZoneHalo, DropHaloScale);
            }

        }


        private void DropZoneCenter_Drop(object sender, DragEventArgs e)
        {
            AnimateLeave(DropZoneScale, DropZoneHalo, DropHaloScale);

            // Xử lý sự kiện
            if (e.Data.GetDataPresent(typeof(NoteView)))
            {
                var droppedNote = e.Data.GetData(typeof(NoteView)) as NoteView;

                droppedNote.IsActive = false;

                notes.Remove(droppedNote);
                trash.Add(droppedNote);

                if (AppSession.UserId > 0)
                    using (var db = new AppDatabaseEntities())
                    {
                        var noteToUpdate = db.Notes.Find(droppedNote.Id);
                        if (noteToUpdate != null)
                        {
                            noteToUpdate.Status = "inactive";
                            noteToUpdate.UpdatedAt = DateTime.Now;
                        }
                        db.SaveChanges();
                    }
            }
            else
            {
                MessageBox.Show("Không nhận diện được note!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }


        private void CornerDeleteHotArea_DragEnter(object sender, DragEventArgs e)
        {
            AnimateHover(CornerDeleteScale, CornerDeleteHalo, CornerDeleteHaloScale);
        }

        private void CornerDeleteHotArea_DragLeave(object sender, DragEventArgs e)
        {
            AnimateLeave(CornerDeleteScale, CornerDeleteHalo, CornerDeleteHaloScale);

        }

        // Không dùng
        private void CornerDeleteHotArea_DragOver(object sender, DragEventArgs e)
        {

        }

        private void CornerDeleteHotArea_Drop(object sender, DragEventArgs e)
        {
            AnimateLeave(DropZoneScale, DropZoneHalo, DropHaloScale);
            if (e.Data.GetDataPresent(typeof(NoteView))) // thay bằng model note của bạn
            {
                var droppedNote = e.Data.GetData(typeof(NoteView)) as NoteView;

                trash.Remove(droppedNote);

                if (AppSession.UserId > 0)
                    using (var db = new AppDatabaseEntities())
                    {
                        var noteToDelete = db.Notes.Find(droppedNote.Id);
                        if (noteToDelete != null)
                        {
                            db.Notes.Remove(noteToDelete);
                            db.SaveChanges();
                        }
                    }
            }
            else
            {
                MessageBox.Show("Không nhận diện được note!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CornerRestoreHotArea_DragEnter(object sender, DragEventArgs e)
        {
            AnimateHover(CornerRestoreScale, CornerRestoreHalo, CornerRestoreHaloScale);
        }

        private void CornerRestoreHotArea_DragLeave(object sender, DragEventArgs e)
        {
            AnimateLeave(CornerRestoreScale, CornerRestoreHalo, CornerRestoreHaloScale);
        }

        private void CornerRestoreHotArea_Drop(object sender, DragEventArgs e)
        {
            AnimateLeave(DropZoneScale, DropZoneHalo, DropHaloScale);
            if (e.Data.GetDataPresent(typeof(NoteView))) // thay bằng model note của bạn
            {
                var droppedNote = e.Data.GetData(typeof(NoteView)) as NoteView;

                trash.Remove(droppedNote);
                droppedNote.IsActive = true;
                notes.Add(droppedNote);

                if (AppSession.UserId > 0)
                    using (var db = new AppDatabaseEntities())
                    {
                        var noteToUpdate = db.Notes.Find(droppedNote.Id);
                        if (noteToUpdate != null)
                        {
                            noteToUpdate.Status = "active";
                            noteToUpdate.UpdatedAt = DateTime.Now;
                        }
                        db.SaveChanges();
                    }
            }
            else
            {
                MessageBox.Show("Không nhận diện được note!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SelHome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListLayer.Visibility = Visibility.Visible;
            GreetingBar.Visibility = Visibility.Visible;
            DetailLayer.Visibility = Visibility.Collapsed;

            AppSession.IsOnNote = false;

            if (AppSession.UserId >= 0)
                loadNote();

            _isTrashPage = false;

            UILoad();
        }

        private void IconDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListLayer.Visibility = Visibility.Visible;
            GreetingBar.Visibility = Visibility.Visible;
            DetailLayer.Visibility = Visibility.Collapsed;

            if (AppSession.UserId >= 0 && trash.Count == 0)
                loadNote();

            NotesItems.ItemsSource = trash;

            _isTrashPage = true;
            UILoad();
        }

        private async void DetailTitleBox_DataContextChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine("Title changed");
            try
            {

                var note = AppSession.OnNote;

                if (note == null) return;
                if (!AppSession.IsOnNote) return;

                if (AppSession.UserId < 0)
                {
                    var ele = notes.FirstOrDefault(n => n.Id == note.Id);

                    if (ele == null) return;
                    var index = notes.IndexOf(ele);
                    if (index >= 0)
                    {
                        notes[index] = new NoteView
                        {
                            Id = ele.Id,
                            Title = DetailTitleBox.Text,
                            Content = ele.Content,
                            UpdatedAt = DateTime.Now,
                            Background = ele.Background
                        };
                    }
                    SaveStatusText.Text = "Đã lưu lúc " + DateTime.Now.ToString("HH:mm:ss");

                    return;
                }

                note.Title = DetailTitleBox.Text;
                await SaveNoteAsync(NoteViewModel.ToModel(note));
                SaveStatusText.Text = "Đã lưu lúc " + DateTime.Now.ToString("HH:mm:ss");

                loadNote();
                notesView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}");
            }
        }

        private async void DetailContentBox_DataContextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var note = AppSession.OnNote;

                if (note == null) return;
                if (!AppSession.IsOnNote) return;


                if (AppSession.UserId < 0)
                {
                    var ele = notes.FirstOrDefault(n => n.Id == note.Id);

                    if (ele == null) return;

                    var index = notes.IndexOf(ele);
                    if (index >= 0)
                    {
                        notes[index] = new NoteView
                        {
                            Id = ele.Id,
                            Title = ele.Title,
                            Content = DetailContentBox.Text,
                            UpdatedAt = DateTime.Now,
                            Background = ele.Background
                        };
                    }

                    SaveStatusText.Text = "Đã lưu lúc " + DateTime.Now.ToString("HH:mm:ss");
                    return;
                }

                note.Content = DetailContentBox.Text;
                await SaveNoteAsync(NoteViewModel.ToModel(note));
                SaveStatusText.Text = "Đã lưu lúc " + DateTime.Now.ToString("HH:mm:ss");

                loadNote();
                notesView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}");
            }
        }

        private async Task SaveNoteAsync(Note note)
        {
            using (var db = new AppDatabaseEntities())
            {
                var existingNote = await db.Notes.FindAsync(note.NoteID);
                if (existingNote == null) return;

                existingNote.Title = note.Title;
                existingNote.Content = note.Content;
                existingNote.UpdatedAt = DateTime.Now;

                await db.SaveChangesAsync();
            }
        }

        private void IconPlus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var newNote = AddNote();

            if (AppSession.UserId >= 0)
            {
                using (var db = new AppDatabaseEntities())
                {
                    var noteModel = NoteViewModel.ToModel(newNote);
                    noteModel.UserID = AppSession.UserId;
                    db.Notes.Add(noteModel);
                    db.SaveChanges();
                }

                loadNote();
            }
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _isDragging = false;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            UILoad();
        }

        private void ListBoxItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
            {
                if (!(sender is ListBoxItem item)) return;
                if (!(item.DataContext is NoteView note)) return;
                if (!note.IsActive) return;

                AppSession.IsOnNote = true;
                AppSession.OnNote = note;

                DetailTitleBox.Text = note.Title;
                DetailContentBox.Text = note.Content;

                ListLayer.Visibility = Visibility.Collapsed;
                GreetingBar.Visibility = Visibility.Collapsed;
                DetailLayer.Visibility = Visibility.Visible;
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = TxtSearch.Text?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                notesView.Filter = null; // bỏ lọc
            }
            else
            {
                notesView.Filter = obj =>
                {
                    if (obj is NoteView note)
                    {
                        return (note.Title?.ToLower().Contains(keyword) ?? false) ||
                               (note.Content?.ToLower().Contains(keyword) ?? false);
                    }
                    return false;
                };
            }

            notesView.Refresh();
        }
    }
}