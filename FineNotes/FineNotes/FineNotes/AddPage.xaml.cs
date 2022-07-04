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
        private async void AddButton_Clicked(object sender, EventArgs args) {
            string note_header = Note_header.Text;
            string note_msg = Note_msg.Text;
            string note_author = "123@123.ru";
            string date = "123";
            notCol.addNote(note_header, note_msg, note_author, date);
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            await Navigation.PopAsync();
        }
    }
}

