// GLib.ToggleRef.cs - GLib ToggleRef class implementation
//
// Author: Mike Kestner <mkestner@novell.com>
//
// Copyright <c> 2007 Novell, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General 
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.


namespace GLib {

	using System;
	using System.Runtime.InteropServices;

	internal class ToggleRef {

		IntPtr handle;
		object reference;
		GCHandle gch;

		public ToggleRef (GLib.Object target)
		{
			handle = target.Handle;
			gch = GCHandle.Alloc (this);
			reference = target;
			g_object_add_toggle_ref (target.Handle, ToggleNotifyCallback, (IntPtr) gch);
			g_object_unref (target.Handle);
		}

		public bool IsAlive {
			get {
				if (reference is WeakReference) {
					WeakReference weak = reference as WeakReference;
					return weak.IsAlive;
				}
				return true;
			}
		}

		public GLib.Object Target {
			get {
				if (reference is GLib.Object)
					return reference as GLib.Object;

				WeakReference weak = reference as WeakReference;
				return weak.Target as GLib.Object;
			}
		}

		public void Free ()
		{
			g_object_remove_toggle_ref (handle, ToggleNotifyCallback, (IntPtr) gch);
			gch.Free ();
		}

		void Toggle (bool is_last_ref)
		{
			if (is_last_ref && reference is GLib.Object)
				reference = new WeakReference (reference);
			else if (!is_last_ref && reference is WeakReference) {
				WeakReference weak = reference as WeakReference;
				if (weak.IsAlive)
					reference = weak.Target;
				else
					throw new Exception ("Toggling dead object wrapper");
			}
		}

		[CDeclCallback]
		delegate void ToggleNotifyHandler (IntPtr data, IntPtr handle, bool is_last_ref);

		static void RefToggled (IntPtr data, IntPtr handle, bool is_last_ref)
		{
			GCHandle gch = (GCHandle) data;
			ToggleRef tref = gch.Target as ToggleRef;
			tref.Toggle (is_last_ref);
		}

		static ToggleNotifyHandler toggle_notify_callback;
		static ToggleNotifyHandler ToggleNotifyCallback {
			get {
				if (toggle_notify_callback == null)
					toggle_notify_callback = new ToggleNotifyHandler (RefToggled);
				return toggle_notify_callback;
			}
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_object_add_toggle_ref (IntPtr raw, ToggleNotifyHandler notify_cb, IntPtr data);

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_object_remove_toggle_ref (IntPtr raw, ToggleNotifyHandler notify_cb, IntPtr data);

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_object_unref (IntPtr raw);

	}
}