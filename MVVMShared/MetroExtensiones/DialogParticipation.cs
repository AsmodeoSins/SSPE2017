﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//namespace MVVMShared.MetroExtensiones
//{
//    public static class DialogParticipation
//    {
//        private static readonly IDictionary<object, DependencyObject> ContextRegistrationIndex = new Dictionary<object, DependencyObject>();

//        public static readonly DependencyProperty RegisterProperty = DependencyProperty.RegisterAttached(
//            "Register", typeof(object), typeof(DialogParticipation), new PropertyMetadata(default(object), RegisterPropertyChangedCallback));

//        private static void RegisterPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
//        {
//            if (dependencyPropertyChangedEventArgs.OldValue != null)
//                ContextRegistrationIndex.Remove(dependencyPropertyChangedEventArgs.OldValue);

//            if (dependencyPropertyChangedEventArgs.NewValue != null)
//                ContextRegistrationIndex[dependencyPropertyChangedEventArgs.NewValue] = dependencyObject;
//        }

//        public static void SetRegister(DependencyObject element, object context)
//        {
//            element.SetValue(RegisterProperty, context);
//        }

//        public static object GetRegister(DependencyObject element)
//        {
//            return element.GetValue(RegisterProperty);
//        }

//        internal static bool IsRegistered(object context)
//        {
//            if (context == null) throw new ArgumentNullException("context");

//            return ContextRegistrationIndex.ContainsKey(context);
//        }

//        internal static DependencyObject GetAssociation(object context)
//        {
//            if (context == null) throw new ArgumentNullException("context");

//            return ContextRegistrationIndex[context];
//        }
//    }
//}
