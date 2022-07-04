using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace FineNotes
{
    public partial class ModifyPage : ContentPage
    {
        public ModifyPage(Note note)
        {
            InitializeComponent();
            Note_header.Text = note.Header;
            Note_msg.Text = note.Message;
            Email_label.Text = note.Author;
            Date_label.Text = note.Date;
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private bool hided = false;
        public void ShowBtnsClicked(object sender, EventArgs e)
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
    }
}

