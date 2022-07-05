using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace FineNotes
{
    public partial class AddPage : ContentPage
    {
        NotesCollection notCol = NotesCollection.getInstance();
        public AddPage()
        {
            InitializeComponent();
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private async void AddButton_Clicked(object sender, EventArgs args)
        {   //Функция добавления заметки, если заголовок или текст не пуст
            if (!String.IsNullOrEmpty(Note_header.Text) || !String.IsNullOrEmpty(Note_msg.Text))
            {
             string note_header = Note_header.Text;
            string note_msg = Note_msg.Text;
            string note_author = "123@123.ru";
            string date = "123";
            notCol.addNote(note_header, note_msg, note_author, date);
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            await Navigation.PopAsync();
            }
        }
        public void AddTextChanged(object sender, EventArgs e)              //Функция изменения прозрачности у кнопки
        {
            if (!String.IsNullOrEmpty(Note_header.Text) || !String.IsNullOrEmpty(Note_msg.Text))
                Pen_frame.Opacity = 1;
            else
                Pen_frame.Opacity = 0.2;
        }
    }
}

