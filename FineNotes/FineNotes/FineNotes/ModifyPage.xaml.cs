using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class ModifyPage : ContentPage
    {
        Session session = Session.getInstance();
        NotesCollection notCol = NotesCollection.getInstance();
        int number;
        public ModifyPage(Note note)
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
            {
                Header.Padding = new Thickness(0, 35, 0, 0);
            }
            number = note.Number;
            Note_header.Text = note.Header;
            Note_msg.Text = note.Message;
            Email_label.Text = note.Author;
            List<string> lst = note.Allowers;
            Date_label.Text = note.Date;
        }
        private bool side_hided = true;
        private async void showSidebarClicked(object sender, EventArgs e)   //Функция показа / скрытия сайдбара
        {
            string[] users = new string[] { "111@mail.ru", "xyz12@mail.ru", "azazazaaaza@gmail.com", "321@mail.ru" };
            var template = new DataTemplate(typeof(TextCell));
            template.SetValue(TextCell.TextColorProperty, Color.FromHex("FFEEB1"));
            usersList.ItemTemplate = template;
            template.SetBinding(TextCell.TextProperty, ".");
            if (side_hided)
            {
                usersList.ItemsSource = users;
                Sidebar.IsVisible = true;
                await Sidebar.TranslateTo(0, 0, 250);
                side_hided = false;
            }
            else
            {
                await Sidebar.TranslateTo(500, 0, 250);
                Sidebar.IsVisible = false;
                side_hided = true;
            }
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private bool hided = true;
        private void ShowBtnsClicked(object sender, EventArgs e) //Функция скрывания/показа кнопок удаления и изменения
        {
            if (!hided)
            {
                buttons_layout.TranslateTo(100, 0, 250);
                ArrowFrame.RotateYTo(180, 250);
                ArrowFrame.Opacity = 0.2;
                hided = true;
            }
            else
            {
                buttons_layout.TranslateTo(0, 0, 250);
                ArrowFrame.RotateYTo(0, 250);
                ArrowFrame.Opacity = 1;
                hided = false;
            }
        }
        private void NoteChangeClicked(object sender, EventArgs e)      //Функция сохранения изменений в заметке
        {
            var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
            if (note != null)
            {
                note.Header = Note_header.Text;
                note.Message = Note_msg.Text;
                note.Date = DateTime.Now.ToString();
                notCol.Save();
                session.Modified = true;
            }
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            
        }
        private async void NoteDeleteClicked(object sender, EventArgs e)        //Функция удаления заметки
        {
            var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
            notCol.Notes.Remove(note);
            notCol.change_indexes();
            notCol.Save();
            session.Modified = true;
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            MessagingCenter.Send<Page>(this, "Show Toolbar!");
            await Navigation.PopAsync();
        }
        void editor_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (DeviceInfo.Platform.Equals(DevicePlatform.iOS))
            {
                scrollView.ScrollToAsync(scrollView.ScrollX, scrollView.ScrollY, true);
            }
        }
    }   
}

