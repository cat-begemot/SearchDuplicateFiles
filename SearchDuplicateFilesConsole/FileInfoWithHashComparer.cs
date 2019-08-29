using System.Collections.Generic;
using System.Linq;

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
}