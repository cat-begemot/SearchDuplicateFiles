using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SearchDuplicateFilesConsole
{
	public class Program
    {
	    public static void Main(string[] args)
        {
			var initFilePath = @"C:\Users\oholenko\source\repos\SearchDuplicateFiles\TestData\FolderPathesInit.txt";
//			var initFilePath = @"C:\Users\cat-b\source\repos\SearchDuplicateFiles\TestData\FolderPathesInit.txt";

			var syncFiles = new SyncFiles(initFilePath);

			if (!syncFiles.CheckInitFileExistance(true) ||
			    !syncFiles.CheckFoldersExistance(true))
			{
				return;
			}

			if (!syncFiles.GetUserAcceptanceOfFolderNamesCorrectness(true))
				return;

			ConsoleExtensions.WriteInColor($"\nChecked folder contain {syncFiles.GetCheckedFolderFiles().Count} file(s).\n",
				ConsoleColor.Blue);
			ConsoleExtensions.WriteInColor($"Base folder contain {syncFiles.GetBaseFolderFiles().Count} file(s).\n",
				ConsoleColor.Blue);
			ConsoleExtensions.WriteInColor($"It were found {syncFiles.GetDuplicateFiles().Count} duplicate file(s).\n",
				ConsoleColor.Blue);

			ConsoleExtensions.WriteInColor("\nDo move duplicate files to Duplicate folder? (yes/no): ", ConsoleColor.Yellow);
			string moveFileAnswer = Console.ReadLine().Trim().ToLower();


			//			syncFiles.GetCheckedFolderFiles().ForEach(file => Console.WriteLine(file.FullName));
			//			Console.WriteLine("--------");
			//
			//			syncFiles.GetBaseFolderFiles().ForEach(file => Console.WriteLine(file.FullName));
			//			Console.WriteLine("--------");
			//
			//			syncFiles.GetDuplicateFiles().ForEach(file => Console.WriteLine(file.FullName));
			//			Console.WriteLine();

			// Move file to Duplicates folders
			//			string duplicatesPath = Directory.CreateDirectory(Path.Combine(Directory.GetParent(syncFiles.GetCheckedFolderFullName()).FullName, "Duplicates")).FullName;
			//
			//			syncFiles.GetDuplicateFiles().ForEach(file =>
			//			{
			//				try
			//				{
			//					File.Copy(file.FullName, Path.Combine(duplicatesPath, file.Name));
			//				}
			//				catch(Exception ex)
			//				{
			//					ConsoleExtensions.WriteInColor($"{ex.Message}\n", ConsoleColor.Red);
			//				}
			//				finally
			//				{
			//					File.Delete(file.FullName);
			//				}
			//
			//			});
			//
			//			switch (syncFiles.GetDuplicateFiles().Count)
			//			{
			//				case 0:
			//					ConsoleExtensions.WriteInColor("\nNo duplicate files.\n", ConsoleColor.Blue);
			//					break;
			//				case 1:
			//					ConsoleExtensions.WriteInColor($"\n{syncFiles.GetDuplicateFiles().Count()} file was moved.\n", ConsoleColor.Blue);
			//					break;
			//				default:
			//					ConsoleExtensions.WriteInColor($"\n{syncFiles.GetDuplicateFiles().Count()} files were moved.\n", ConsoleColor.Blue);
			//					break;
			//			}
		}
    }
}
