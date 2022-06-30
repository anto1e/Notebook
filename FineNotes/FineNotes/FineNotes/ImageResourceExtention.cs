﻿using System;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
namespace FineNotes
{
    [ContentProperty(nameof(Source))]
    class ImageResourceExtention : IMarkupExtension
    {
        public string Source { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            var imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtention));

            return imageSource;
        }
    }
}

