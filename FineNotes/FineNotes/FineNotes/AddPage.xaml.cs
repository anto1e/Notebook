using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Text.Json;


namespace FineNotes
{
    public partial class AddPage : ContentPage
    {
        Session session = Session.getInstance();
        NotesCollection notCol = NotesCollection.getInstance();
        public AddPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
            {
                Header.Padding = new Thickness(0,35,0,0);
            }
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private async void AddButton_Clicked(object sender, EventArgs args)
        {   //Функция добавления заметки, если заголовок или текст не пуст
            if (!String.IsNullOrEmpty(Note_header.Text))
            {
             string note_header = Note_header.Text;
             string note_msg = "";
            note_msg= Note_msg.Text;
            string note_author = session.Email;
            string date = DateTime.Now.ToString();
            int type = 0;
            List<string> temp = new List<string>();     //Поменять!!!
            notCol.addNote(note_header, note_msg, note_author, date,temp,type);
            notCol.Save();
                session.Modified = true;
                MessagingCenter.Send<Page>(this, "CollectionChanged!");
                MessagingCenter.Send<Page>(this, "Show Toolbar!");
                await Navigation.PopAsync();
            }
        }
        public void AddTextChanged(object sender, EventArgs e)              //Функция изменения прозрачности у кнопки
        {
            if (!String.IsNullOrEmpty(Note_header.Text))
                Pen_frame.Opacity = 1;
            else
                Pen_frame.Opacity = 0.2;
        }
    }
}

