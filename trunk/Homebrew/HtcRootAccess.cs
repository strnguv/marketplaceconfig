using System;
using System.Net;
using System.Threading;
using System.Windows;

using System.Runtime.InteropServices;
using Homebrew.IO;

namespace Homebrew.HtcRoot
{
	public class RootException : InteropException
	{
		protected int _err;
		public RootException ()
			: this("Error - still in least privileged chamber")
		{}
		public RootException (int error)
			: this("Error - still in least privileged chamber", error)
		{ }
		public RootException (String msg)
			: this(msg, 1260)
		{}
		public RootException (String msg, int error)
			: base(msg)
		{
			_err = error;
		}

		public int Error { get { return _err; } }
	}

	[ComImport, ClassInterface(ClassInterfaceType.None), Guid("02CF6430-02A9-4256-9A50-7C28A503CAAE")]
	public class CHtcUtility
	{
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("C41A39FF-BD38-4598-974E-4EE9C4810DB8")]
	public interface IHtcUtility
	{
		void OpenHtcUtility ();

		void CloseHtcUtility ();
		
		[return : MarshalAs(UnmanagedType.Bool)]
		bool IsHtcUtilityOpen ();

		[return: MarshalAs(UnmanagedType.I4)]
		int ReadAddressInt (IntPtr piAddress);

		void WriteAddressInt (IntPtr piAddress, int iValue);

		void MakeMeRoot ();

		void RestoreToken ();

		[return : MarshalAs(UnmanagedType.I4)]
		int GetUserlandTestAddress ();

		[return : MarshalAs(UnmanagedType.I4)]
		int ReturnZeroIfRoot ();

		[return: MarshalAs(UnmanagedType.LPWStr)]
		String CheckProcessName ();
	}
	
	public class HtcRootAccess
	{
		public const int ERROR_LEAST_PRIVILEGED_CHAMBER = 1260;

		public static IHtcUtility HtcUtility;

		static HtcRootAccess ()
		{
			try
			{
				InteropHelper.RegisterDLLOrExcept("HtcRoot.dll", "02CF6430-02A9-4256-9A50-7C28A503CAAE");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error opening HtcRoot.dll:\n" + ex.ToString());
			}
			HtcUtility = new CHtcUtility() as IHtcUtility;
		}

		public static Exception TryBecomeRoot ()
		{
			try
			{
				if (!HtcUtility.IsHtcUtilityOpen())
				{
					HtcUtility.OpenHtcUtility();
				}
				HtcUtility.MakeMeRoot();

				// Check and try again if needed
				for (int i = 0;
					(i < 50) && (ERROR_LEAST_PRIVILEGED_CHAMBER == HtcUtility.ReturnZeroIfRoot());
					i++)
				{	// Re-elevate if we're still in the sandbox for some reason.
					SafeRestoreToken();
					HtcUtility.MakeMeRoot();
				}
			}
			catch (Exception ex)
			{
				if (HtcUtility.IsHtcUtilityOpen())
				{
					try
					{	// We might not *be* root, so if not just swallow it
						HtcUtility.RestoreToken();
					}
					catch (Exception) { }
					HtcUtility.CloseHtcUtility();
				}
				return ex;
			}
			int err = HtcUtility.ReturnZeroIfRoot();
			if (err != 0) return new RootException(err);
			else return null;
		}

		public static void SafeRestoreToken ()
		{
			try
			{
				if (HtcUtility.IsHtcUtilityOpen())
				{
					for (int i = 0; i < 50; i++)
					{	// Fully-unlocked ROMs will *always* return 0, so we can't keep looping until they don't
						if (HtcUtility.ReturnZeroIfRoot() == 0)
							HtcUtility.RestoreToken();
					}
					HtcUtility.CloseHtcUtility();
				}
			}
			catch (Exception)
			{ }
		}

		public static String TestBecomingRoot ()
		{
			String ret = null;
			HtcUtility.OpenHtcUtility();
			ret = "Pre-root file open: " + HtcUtility.ReturnZeroIfRoot() + "\n";
			HtcUtility.MakeMeRoot();
			ret += "With-root file open: " + HtcUtility.ReturnZeroIfRoot() + "\n";
			HtcUtility.RestoreToken();
			ret += "Post-root file open: " + HtcUtility.ReturnZeroIfRoot();
			return ret;
		}
	}
}
