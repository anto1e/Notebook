using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void addBtnClicked(object sender, EventArgs args)       //Обработка нажатия на кнопку "Добавить"
        {
            header_text.Text += "4";
        }

    }
}

