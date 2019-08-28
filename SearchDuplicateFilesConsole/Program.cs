using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Channels;

namespace SearchDuplicateFilesConsole
{
	public class FileInfoWithHashComparer : IEqualityComparer<FileInfoWithHash>
	{
		public bool Equals(FileInfoWithHash x, FileInfoWithHash y)
		{
			return x.MD5Hash.SequenceEqual(y.MD5Hash);
		}

		public int GetHashCode(FileInfoWithHash obj)
		{
			return 0;
		}
	}

	public class FileInfoWithHash
	{
		public FileInfoWithHash(string fullName)
		{
			FullName = fullName;
			Name = GetFileName();
			MD5Hash = GetMD5Hash();
		}

		private byte[] GetMD5Hash()
		{
			using (var md5 = MD5.Create())
			using (var fileStream = File.OpenRead(FullName))
				return md5.ComputeHash(fileStream);
		}

		private string GetFileName()
		{
			int lastIndex = FullName.LastIndexOf(@"\");
			return FullName.Substring(++lastIndex);
		}

		public void PrintMD5Hash(ConsoleColor foregroundColor)
		{
			foreach (var value in MD5Hash)
				Program.WriteInColor($"{value} ", foregroundColor);
		}

		public string FullName { get; }
		public string Name { get; }
		public byte[] MD5Hash { get; }
	}

	public class Program
    {
	    public static void Main(string[] args)
        {
			var initFilePath = @"C:\Users\oholenko\source\repos\SearchDuplicateFiles\TestData\FolderPathesInit.txt";

			string[] folderPathes = File.ReadAllLines(initFilePath);

			bool isErrorOccured = false;

			foreach (var folderPath in folderPathes)
				if (!Directory.Exists(folderPath))
				{
					WriteInColor($"The folder doesn't exist: {folderPath}.\n", ConsoleColor.Red);

					isErrorOccured = true;
				}

			if (isErrorOccured)
			{
				WriteInColor("\nApplication will be terminated.\n", ConsoleColor.Red);
				return;
			}

			var firstDirectoryFiles = new List<FileInfoWithHash>();
			var secondDirectoryFiles = new List<FileInfoWithHash>();

			foreach (var fullName in Directory.GetFiles(folderPathes[0], "*.*", SearchOption.AllDirectories))
				firstDirectoryFiles.Add(new FileInfoWithHash(fullName));

			foreach (var fullName in Directory.GetFiles(folderPathes[1], "*.*", SearchOption.AllDirectories))
				secondDirectoryFiles.Add(new FileInfoWithHash(fullName));

			//			var commonFiles = firstDirectoryFiles
			//				.Intersect(secondDirectoryFiles, new FileInfoWithHashComparer())
			//				.ToList();

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

			// Move file to Duplicates folders
			string duplicatesPath = Directory.CreateDirectory(Path.Combine(Directory.GetParent(folderPathes[0]).FullName, "Duplicates")).FullName;

			commonFiles.ForEach(file =>
			{
				File.Copy(file.FullName, Path.Combine(duplicatesPath, file.Name));
				File.Delete(file.FullName);
			});

			switch (commonFiles.Count)
			{
				case 0:
					WriteInColor("\nNo duplicate files.\n", ConsoleColor.Blue);
					break;
				case 1:
					WriteInColor($"\n{commonFiles.Count()} file was moved.\n", ConsoleColor.Blue);
					break;
				default:
					WriteInColor($"\n{commonFiles.Count()} files were moved.\n", ConsoleColor.Blue);
					break;
			}
        }

		public static void WriteInColor<T>(T value, ConsoleColor foregroundColor)
		{
			var previousForegroundColor = Console.ForegroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.Write(value);
			Console.ForegroundColor = previousForegroundColor;
		}
    }
}
