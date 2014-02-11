using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace WpfFileManager
{
    public class Proxy : FrameworkElement
    {
        public static readonly DependencyProperty InProperty;
        public static readonly DependencyProperty OutProperty;

        public Proxy()
        {
            Visibility = Visibility.Collapsed;
        }

        static Proxy()
        {
            var inMetadata = new FrameworkPropertyMetadata(
                delegate(DependencyObject p, DependencyPropertyChangedEventArgs args)
                    {
                        var binding = BindingOperations.GetBinding(p, OutProperty);
                        if (binding != null)
                        {
                            var proxy = p as Proxy;
                            if (proxy != null)
                                proxy.Out = args.NewValue;
                        }
                    });

            inMetadata.BindsTwoWayByDefault = false;
            inMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            InProperty = DependencyProperty.Register("In", typeof(object), typeof(Proxy), inMetadata);

            var outMetadata = new FrameworkPropertyMetadata(
                delegate(DependencyObject p, DependencyPropertyChangedEventArgs args)
                    {
                        ValueSource source = DependencyPropertyHelper.GetValueSource(p, args.Property);

                        if (source.BaseValueSource != BaseValueSource.Local)
                        {
                            var proxy = p as Proxy;
                            if (proxy != null)
                            {
                                var expected = proxy.In;
                                if (!ReferenceEquals(args.NewValue, expected))
                                {
                                    Dispatcher.CurrentDispatcher.BeginInvoke(
                                        DispatcherPriority.DataBind, new Action(delegate
                                            {
                                                proxy.Out = proxy.In;
                                            }));
                                }
                            }
                        }
                    });

            outMetadata.BindsTwoWayByDefault = true;
            outMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            OutProperty = DependencyProperty.Register("Out", typeof(object), typeof(Proxy), outMetadata);
        }

        public object In
        {
            get { return GetValue(InProperty); }
            set
            {
                SetValue(InProperty, value);
            }
        }

        public object Out
        {
            get { return GetValue(OutProperty); }
            set { SetValue(OutProperty, value); }
        }
    }
}