using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using dnSpy.AsmEditor.Properties;
using dnSpy.Contracts.App;
using dnSpy.Contracts.Bundles;
using dnSpy.Contracts.MVVM;
using dnSpy.Contracts.MVVM.Dialogs;
using Ookii.Dialogs.Wpf;
using WF = System.Windows.Forms;

namespace dnSpy.AsmEditor.Bundle {
	/// <summary>
	/// For saving bundle entries
	/// </summary>
	public static class SaveBundle {

		/// <summary>
		/// Gets the save path of each files in bundle
		/// </summary>
		/// <param name="infos"></param>
		/// <param name="useSubDirs"></param>
		/// <returns></returns>
		static IEnumerable<(BundleEntry data, string filename)> GetFiles(BundleEntry[] infos, bool useSubDirs) {
			if (infos.Length == 1) {
				var info = infos[0];
				var name = BundleNameCleaner.FixFileNamePart(BundleNameCleaner.GetFileName(info.FileName));
				var dlg = new WF.SaveFileDialog {
					Filter = PickFilenameConstants.AnyFilenameFilter,
					RestoreDirectory = true,
					ValidateNames = true,
					FileName = name,
				};
				var ext = Path.GetExtension(name);
				dlg.DefaultExt = string.IsNullOrEmpty(ext) ? string.Empty : ext.Substring(1);
				if (dlg.ShowDialog() != WF.DialogResult.OK)
					yield break;
				yield return (info, dlg.FileName);
			}
			else {
				var dlg = new VistaFolderBrowserDialog();
				if (dlg.ShowDialog() != true)
					yield break;
				string baseDir = dlg.SelectedPath;
				foreach (var info in infos) {
					var name = BundleNameCleaner.GetCleanedPath(info.FileName, useSubDirs);
					var pathName = Path.Combine(baseDir, name);
					yield return (info, pathName);
				}
			}
		}

		/// <summary>
		/// Saves the bundle entry nodes
		/// </summary>
		/// <param name="entries">Nodes</param>
		/// <param name="title">true to create sub directories, false to dump everything in the same folder</param>
		public static void Save(BundleEntry[] entries, string title) {
			if (entries is null)
				return;

			(BundleEntry bundleEntry, string filename)[] bundleSaveInfo;
			try {
				bundleSaveInfo = GetFiles(entries, true).ToArray();
			}
			catch (Exception ex) {
				MsgBox.Instance.Show(ex);
				return;
			}
			if (bundleSaveInfo.Length == 0)
				return;

			var data = new ProgressVM(Dispatcher.CurrentDispatcher, new BundleSaver(bundleSaveInfo));
			var win = new ProgressDlg();
			win.DataContext = data;
			win.Owner = Application.Current.MainWindow;
			win.Title = title;
			var res = win.ShowDialog();
			if (res != true)
				return;
			if (!data.WasError)
				return;
			MsgBox.Instance.Show(string.Format(dnSpy_AsmEditor_Resources.SaveBundleError, data.ErrorMessage));
		}

		sealed class BundleSaver : IProgressTask {
			public bool IsIndeterminate => false;
			public double ProgressMinimum => 0;
			public double ProgressMaximum => bundleSaveInfo.Length;

			readonly (BundleEntry bundleEntry, string filename)[] bundleSaveInfo;

			public BundleSaver((BundleEntry bundleEntry, string filename)[] bundleSaveInfo) => this.bundleSaveInfo = bundleSaveInfo;

			public void Execute(IProgress progress) {
				for (int i = 0; i < bundleSaveInfo.Length; i++) {
					progress.ThrowIfCancellationRequested();
					var saveInfo = bundleSaveInfo[i];
					progress.SetDescription(saveInfo.filename);
					progress.SetTotalProgress(i);
					Directory.CreateDirectory(Path.GetDirectoryName(saveInfo.filename)!);
					try {
						byte[]? data = saveInfo.bundleEntry.GetEntryData();
						Debug2.Assert(data != null);
						File.WriteAllBytes(saveInfo.filename, data);
					}
					catch {
						try { File.Delete(saveInfo.filename); }
						catch { }
						throw;
					}
				}
				progress.SetTotalProgress(bundleSaveInfo.Length);
			}
		}
	}
}
