using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using Microsoft.Surface.Presentation;

namespace Blake.NUI.WPF.Surface
{
    public class SurfaceTouchDevice : TouchDevice
    {
        #region Class Members

        private static Dictionary<int, SurfaceTouchDevice> deviceDictionary = new Dictionary<int, SurfaceTouchDevice>();

        public Contact Contact { get; protected set; }

        #endregion

        #region Public Static Methods

        public static void RegisterEvents(FrameworkElement root)
        {
            Contacts.AddContactEnterHandler(root, new ContactEventHandler(ContactDown));
            Contacts.AddPreviewContactChangedHandler(root, new ContactEventHandler(ContactChanged));
            Contacts.AddContactLeaveHandler(root, new ContactEventHandler(ContactUp));
        }

        public static Contact GetContact(TouchDevice device)
        {
            try
            {
                SurfaceTouchDevice surfaceDevice = device as SurfaceTouchDevice;

                if (surfaceDevice == null)
                    return null;

                return surfaceDevice.Contact;
            }
            //Ignore InvalidOperationException due to race condition on Surface hardware
            catch (InvalidOperationException)
            { }
            return null;
        }

        #endregion

        #region Private Static Methods

        private static void ContactDown(object sender, ContactEventArgs e)
        {
            try
            {
                if (deviceDictionary.Keys.Contains(e.Contact.Id))
                {
                    return;
                }
                SurfaceTouchDevice device = new SurfaceTouchDevice(e.Contact);
                deviceDictionary.Add(e.Contact.Id, device);

                device.SetActiveSource(e.Device.ActiveSource);
                device.Activate();
                device.ReportDown();
            }
            //Ignore InvalidOperationException due to race condition on Surface hardware
            catch (InvalidOperationException)
            { }
        }

        private static void ContactChanged(object sender, ContactEventArgs e)
        {
            try
            {
                int id = e.Contact.Id;
                if (!deviceDictionary.Keys.Contains(id))
                {
                    ContactDown(sender, e);
                }

                SurfaceTouchDevice device = deviceDictionary[id];
                if (device != null &&
                    device.IsActive)
                {
                    device.Contact = e.Contact;
                    device.ReportMove();
                }
            }
            //Ignore InvalidOperationException due to race condition on Surface hardware
            catch (InvalidOperationException)
            { }
        }

        private static void ContactUp(object sender, ContactEventArgs e)
        {
            try
            {
                int id = e.Contact.Id;
                if (!deviceDictionary.Keys.Contains(id))
                {
                    ContactDown(sender, e);
                }
                SurfaceTouchDevice device = deviceDictionary[id];

                if (device != null &&
                    device.IsActive)
                {
                    device.ReportUp();
                    device.Deactivate();

                    deviceDictionary.Remove(id);
                }
            }
            //Ignore InvalidOperationException due to race condition on Surface hardware
            catch (InvalidOperationException)
            { }
        }

        #endregion

        #region Constructors

        public SurfaceTouchDevice(Contact contact) :
            base(contact.Id)
        {
            Contact = contact;
        }

        #endregion

        #region Overridden methods

        public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
        {
            TouchPointCollection collection = new TouchPointCollection();
            UIElement element = relativeTo as UIElement;

            if (element == null)
                return collection;
            try
            {
                foreach (IntermediateContact c in Contact.GetIntermediateContacts())
                {
                    Point point = c.GetPosition(null);
                    if (relativeTo != null)
                    {
                        point = this.ActiveSource.RootVisual.TransformToDescendant((Visual)relativeTo).Transform(point);
                    }
                    collection.Add(new TouchPoint(this, point, c.BoundingRect, TouchAction.Move));
                }
            }
            //Ignore InvalidOperationException due to race condition on Surface hardware
            catch (InvalidOperationException)
            { }
            return collection;
        }

        public override TouchPoint GetTouchPoint(IInputElement relativeTo)
        {
            try
            {
                Point point = this.Contact.GetPosition(null);
                if (relativeTo != null)
                {
                    point = this.ActiveSource.RootVisual.TransformToDescendant((Visual)relativeTo).Transform(point);
                }

                Rect rect = this.Contact.BoundingRect;

                return new TouchPoint(this, point, rect, TouchAction.Move);
            }
            //Ignore InvalidOperationException due to race condition on Surface hardware
            catch (InvalidOperationException)
            { }
            return null;
        }

        #endregion

    }
}
