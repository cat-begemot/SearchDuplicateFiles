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
//			var initFilePath = @"C:\Users\oholenko\source\repos\SearchDuplicateFiles\TestData\FolderPathesInit.txt";
			var initFilePath = @"C:\Users\cat-b\source\repos\SearchDuplicateFiles\TestData\FolderPathesInit.txt";

			var syncFiles = new SyncFiles(initFilePath);

			if (!syncFiles.CheckInitFileExistance(true) ||
			    !syncFiles.CheckFoldersExistance(true))
			{
				return;
			}

			if (!syncFiles.GetUserAcceptanceOfFolderNamesCorrectness(true))
				return;
			
			





			var firstDirectoryFiles = new List<FileInfoWithHash>();
			var secondDirectoryFiles = new List<FileInfoWithHash>();

			foreach (var fullName in Directory.GetFiles(syncFiles.GetCheckedFolderFullName(), "*.*", SearchOption.AllDirectories))
				firstDirectoryFiles.Add(new FileInfoWithHash(fullName));

			foreach (var fullName in Directory.GetFiles(syncFiles.GetBaseFolderFullName(), "*.*", SearchOption.AllDirectories))
				secondDirectoryFiles.Add(new FileInfoWithHash(fullName));
			
			var commonHashes = firstDirectoryFiles
				.Intersect(secondDirectoryFiles, new FileInfoWithHashComparer())
				.Select(file => file.MD5Hash)
				.ToList();

			var commonFiles = firstDirectoryFiles
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
				.ToList();

			// Checks the result.
			firstDirectoryFiles.OrderBy(file => file.Name).ToList().ForEach(file => Console.WriteLine(file.FullName));
			Console.WriteLine("--------");
			secondDirectoryFiles.OrderBy(file => file.Name).ToList().ForEach(file => Console.WriteLine(file.FullName));
			Console.WriteLine("--------");
			commonFiles.OrderBy(file => file.Name).ToList().ForEach(file => Console.WriteLine(file.FullName));
			Console.WriteLine();

			// Move file to Duplicates folders
			string duplicatesPath = Directory.CreateDirectory(Path.Combine(Directory.GetParent(syncFiles.GetCheckedFolderFullName()).FullName, "Duplicates")).FullName;

			commonFiles.ForEach(file =>
			{
				try
				{
					File.Copy(file.FullName, Path.Combine(duplicatesPath, file.Name));
				}
				catch(Exception ex)
				{
					ConsoleExtensions.WriteInColor($"{ex.Message}\n", ConsoleColor.Red);
				}
				finally
				{
					File.Delete(file.FullName);
				}

			});

			switch (commonFiles.Count)
			{
				case 0:
					ConsoleExtensions.WriteInColor("\nNo duplicate files.\n", ConsoleColor.Blue);
					break;
				case 1:
					ConsoleExtensions.WriteInColor($"\n{commonFiles.Count()} file was moved.\n", ConsoleColor.Blue);
					break;
				default:
					ConsoleExtensions.WriteInColor($"\n{commonFiles.Count()} files were moved.\n", ConsoleColor.Blue);
					break;
			}
        }
    }
}
