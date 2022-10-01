using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BMFViewer.Components;

public partial class GlyphBox : UserControl
{

    public float Zoom
    {
        get { return (float)GetValue(ZoomProperty); }
        set { SetValue(ZoomProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Zoom.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ZoomProperty =
    DependencyProperty.Register("Zoom", typeof(float), typeof(GlyphBox), new PropertyMetadata(1f));

    public ImageSource Image
    {
        get { return (ImageSource)GetValue(ImageProperty); }
        set { SetValue(ImageProperty, value); }
    }

    public static readonly DependencyProperty ImageProperty =
    DependencyProperty.Register("Image", typeof(ImageSource), typeof(GlyphBox), new PropertyMetadata(null));



    public string Character
    {
        get { return (string)GetValue(CharacterProperty); }
        set
        {
            SetValue(CharacterProperty, value);
        }
    }

    public static readonly DependencyProperty CharacterProperty =
    DependencyProperty.Register("Character", typeof(string), typeof(GlyphBox), new PropertyMetadata(string.Empty));

    public string Code
    {
        get { return (string)GetValue(CodeProperty); }
        set { SetValue(CodeProperty, value); }
    }
    public static readonly DependencyProperty CodeProperty =
    DependencyProperty.Register("Code", typeof(string), typeof(GlyphBox), new PropertyMetadata(string.Empty));

    public GlyphBox()
    {
        InitializeComponent();
    }
}
