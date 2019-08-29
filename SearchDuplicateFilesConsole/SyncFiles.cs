using System;
using System.IO;

namespace SearchDuplicateFilesConsole
{
	public class SyncFiles
	{
		private string _initFileFullName { get; }
		
		private bool _isCheckedFolderExist { get; }
		private bool _isBaseFolderExist { get; }


		private bool _isInitFileExist { get; }

		private string[] foldersFullNames;

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
			}
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
			ConsoleExtensions.WriteInColor("(The files in this folder won't be changed. They are used only for comparison)\n", ConsoleColor.DarkGray);
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