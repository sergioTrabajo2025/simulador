using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TuApp.Dxf;

namespace TuApp;

public partial class MainWindow : Window
{
    private readonly DxfButtonReader _reader = new(targetLayer: "BOTONES", attributeTag: "ID1");
    private readonly CanvasMapper _mapper = new(margin: 80);

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenDxf_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "DXF (*.dxf)|*.dxf",
            Title = "Seleccionar DXF"
        };

        if (dlg.ShowDialog() != true)
            return;

        try
        {
            LoadFromDxf(dlg.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadFromDxf(string path)
    {
        BtnCanvas.Children.Clear();

        var marks = _reader.Read(path);

        // (Opcional) si querés filtrar por IDs permitidos:
        // var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        // { "nombre1","nombre2","nombre3","nombre4","nombre5" };
        // marks = marks.Where(m => allowed.Contains(m.Id)).ToList();

        var mapped = _mapper.Map(marks, BtnCanvas.Width, BtnCanvas.Height);

        foreach (var m in mapped)
        {
            var btn = new Button
            {
                Content = m.Id,
                Tag = m.Id,
                Width = 90,
                Height = 32
            };
            btn.Click += Button_Click;

            Canvas.SetLeft(btn, m.X - btn.Width / 2);
            Canvas.SetTop(btn, m.Y - btn.Height / 2);

            BtnCanvas.Children.Add(btn);
        }

        InfoText.Text = $"Archivo: {System.IO.Path.GetFileName(path)} | Marcas: {marks.Count} | Botones creados: {mapped.Count}";
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string id)
        {
            MessageBox.Show($"Click: {id}", "Botón", MessageBoxButton.OK, MessageBoxImage.Information);
            // Acá después lo conectás a tu lógica (toggle / IO / enclavamiento)
        }
    }
}
