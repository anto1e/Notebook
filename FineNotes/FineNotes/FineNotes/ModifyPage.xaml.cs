using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class ModifyPage : ContentPage
    {
        NotesCollection notCol = NotesCollection.getInstance();
        int number;
        public ModifyPage(Note note)
        {
            InitializeComponent();
            number = note.Number;
            Note_header.Text = note.Header;
            Note_msg.Text = note.Message;
            Email_label.Text = note.Author;
            List<string> lst = note.Allowers;
            Date_label.Text = note.Date;
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
                ArrowBtn.RotateYTo(180, 250);
                ArrowFrame.Opacity = 0.2;
                hided = true;
            }
            else
            {
                buttons_layout.TranslateTo(0, 0, 250);
                ArrowBtn.RotateYTo(0, 250);
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
            }
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
        }
        private async void NoteDeleteClicked(object sender, EventArgs e)        //Функция удаления заметки
        {
            var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
            notCol.Notes.Remove(note);
            foreach(var elem in notCol.Notes)
            {
                if (elem.Number > number)
                    elem.Number -= 1;
            }
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            await Navigation.PopAsync();
        }
    }   
}

