using System;
using System.IO;
using System.Security.Cryptography;

namespace SearchDuplicateFilesConsole
{
	public class FileInfoWithHash
	{
		public string FullName { get; }
		public string Name { get; }
		public byte[] MD5Hash { get; }

		public FileInfoWithHash(string fullName)
		{
			FullName = fullName;
			Name = GetFileName();
			MD5Hash = GetMD5Hash();
		}

		public void PrintMD5Hash(ConsoleColor foregroundColor)
		{
			foreach (var value in MD5Hash)
				ConsoleExtensions.WriteInColor($"{value} ", foregroundColor);
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
	}
}