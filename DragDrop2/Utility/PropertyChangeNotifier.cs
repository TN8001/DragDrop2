//(c) Andrew Smith 2008
//https://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/

using System;
using System.Windows;
using System.Windows.Data;

namespace DragDrop2
{
    public sealed class PropertyChangeNotifier : DependencyObject, IDisposable
    {
        public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register("Value", typeof(object), typeof(PropertyChangeNotifier),
                new FrameworkPropertyMetadata(null, OnPropertyChanged));
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((PropertyChangeNotifier)d).ValueChanged?.Invoke((PropertyChangeNotifier)d, EventArgs.Empty);

        public DependencyObject PropertySource
        {
            get
            {
                try
                {
                    return _PropertySource.IsAlive ? _PropertySource.Target as DependencyObject : null;
                }
                catch { return null; }
            }
        }

        public event EventHandler ValueChanged;

        private readonly WeakReference _PropertySource;

        public PropertyChangeNotifier(DependencyObject propertySource, string path) : this(propertySource, new PropertyPath(path)) { }
        public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property) : this(propertySource, new PropertyPath(property)) { }
        public PropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if(propertySource == null) throw new ArgumentNullException("propertySource");
            if(property == null) throw new ArgumentNullException("property");

            _PropertySource = new WeakReference(propertySource);
            var binding = new Binding
            {
                Path = property,
                Mode = BindingMode.OneWay,
                Source = propertySource,
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        public void Dispose() => BindingOperations.ClearBinding(this, ValueProperty);
    }
}
