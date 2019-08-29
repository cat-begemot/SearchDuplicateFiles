using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchDuplicateFilesConsole
{
	public class SyncFiles
	{
		private readonly string[] foldersFullNames;
		private readonly string _initFileFullName;
		private readonly bool _isInitFileExist;
		private readonly bool _isCheckedFolderExist;
		private readonly bool _isBaseFolderExist;

		private List<FileInfoWithHash> _checkedFolderFiles = new List<FileInfoWithHash>();
		private List<FileInfoWithHash> _baseFolderFiles = new List<FileInfoWithHash>();
		private List<FileInfoWithHash> _duplicateFiles = new List<FileInfoWithHash>();

		public SyncFiles(string initFileFullName)
		{
			if (File.Exists(initFileFullName))
			{
				_initFileFullName = initFileFullName;
				_isInitFileExist = true;

				foldersFullNames = File.ReadAllLines(initFileFullName);

				if (Directory.Exists(GetCheckedFolderFullName()))
					_isCheckedFolderExist = true;

				if (Directory.Exists(GetBaseFolderFullName()))
					_isBaseFolderExist = true;

				GetAllFilesFromCheckedFolder();
				GetAllFilesFromBaseFolder();
				GetDuplicateFilesFromCheckedFolder();
			}
		}

		public List<FileInfoWithHash> GetCheckedFolderFiles() => _checkedFolderFiles;
		public List<FileInfoWithHash> GetBaseFolderFiles() => _baseFolderFiles;
		public List<FileInfoWithHash> GetDuplicateFiles() => _duplicateFiles;

		private void GetAllFilesFromCheckedFolder()
		{
			foreach (var fullName in Directory.GetFiles(GetCheckedFolderFullName(), "*.*", SearchOption.AllDirectories))
				_checkedFolderFiles.Add(new FileInfoWithHash(fullName));

			_checkedFolderFiles = _checkedFolderFiles.OrderBy(file => file.Name).ToList();
		}

		private void GetAllFilesFromBaseFolder()
		{
			foreach (var fullName in Directory.GetFiles(GetBaseFolderFullName(), "*.*", SearchOption.AllDirectories))
				_baseFolderFiles.Add(new FileInfoWithHash(fullName));

			_baseFolderFiles = _baseFolderFiles.OrderBy(file => file.Name).ToList();
		}

		private void GetDuplicateFilesFromCheckedFolder()
		{
			var commonHashes = _checkedFolderFiles
				.Intersect(_baseFolderFiles, new FileInfoWithHashComparer())
				.Select(file => file.MD5Hash)
				.ToList();

			_duplicateFiles = _checkedFolderFiles
				.Where(file =>
				{
					bool result = false;
					commonHashes.ForEach(hash =>
					{
						if (hash.SequenceEqual(file.MD5Hash))
							result = true;
					});
					return result;
				})
				.OrderBy(file => file.Name)
				.ToList();
		}

		public string GetCheckedFolderFullName() => foldersFullNames[0];
		public string GetBaseFolderFullName() => foldersFullNames[1];

		public bool CheckInitFileExistance(bool printErrorMessage)
		{
			if (!_isInitFileExist)
			{
				if (printErrorMessage)
				{
					ConsoleExtensions.WriteInColor($"The initialization file doesn't exist.\n[{_initFileFullName}]\n", ConsoleColor.Red);
					ConsoleExtensions.WriteInColor("\nApplication will be terminated.\n", ConsoleColor.Red);
				}

				return false;
			}

			return true;
		}

		public bool CheckFoldersExistance(bool printErrorMessage)
		{
			if (!_isCheckedFolderExist && printErrorMessage)
				ConsoleExtensions.WriteInColor($"The checked folder doesn't exist.\n[{GetCheckedFolderFullName()}].\n\n", ConsoleColor.Red);

			if (!_isBaseFolderExist && printErrorMessage)
				ConsoleExtensions.WriteInColor($"The base folder doesn't exist.\n[{GetBaseFolderFullName()}].\n\n", ConsoleColor.Red);

			if (!_isCheckedFolderExist || !_isBaseFolderExist)
			{
				ConsoleExtensions.WriteInColor("Application execution will be terminated.\n", ConsoleColor.Red);

				return false;
			}

			return true;
		}

		public bool GetUserAcceptanceOfFolderNamesCorrectness(bool printErrorMessage)
		{
			ConsoleExtensions.WriteInColor("Checked folder: ", ConsoleColor.Yellow);
			ConsoleExtensions.WriteInColor("(it checks whether this folder contains the same files which are in the base folder)\n", ConsoleColor.DarkGray);
			ConsoleExtensions.WriteInColor($"{GetCheckedFolderFullName()}\n\n", ConsoleColor.Gray);

			ConsoleExtensions.WriteInColor("Base folder: ", ConsoleColor.Yellow);
			ConsoleExtensions.WriteInColor("(The files in this folder won't be changed. They are used only for comparison purpose)\n", ConsoleColor.DarkGray);
			ConsoleExtensions.WriteInColor($"{GetBaseFolderFullName()}\n\n", ConsoleColor.Gray);

			while (true)
			{
				ConsoleExtensions.WriteInColor("Are these folders correct? (yes/no): ", ConsoleColor.Yellow);
				string acceptanceValue = Console.ReadLine().Trim().ToLower();

				//Console.WriteLine();

				if (acceptanceValue == "yes" || acceptanceValue == "no")
				{
					if (acceptanceValue == "no" && printErrorMessage)
					{
						ConsoleExtensions.WriteInColor("\nThe folders pathes didn't approved.\n", ConsoleColor.Red);
						ConsoleExtensions.WriteInColor("Application execution will be terminated.\n", ConsoleColor.Red);

						return false;
					}

					return true;
				}
				else
				{
					ConsoleExtensions.WriteInColor("Answer must be only 'yes' or 'no'.\n\n", ConsoleColor.Red);
				}
			}
		}
	}
}