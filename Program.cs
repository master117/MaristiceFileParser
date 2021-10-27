using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MaristiceFileParser
{
	class Program
	{
		public struct FileInfo
		{
			public string name;
			public int size;
		}

		static void Main(string[] args)
		{
			if (args.Length == 0 || args[0] == null)
			{
				Console.WriteLine("Please drag the file to be decrypted unto the .exe");
				Console.ReadKey();
				System.Environment.Exit(1);
			}

			// Register Shift-JIS Encoding
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Encoding encoding = Encoding.GetEncoding("Shift-JIS");
			Console.OutputEncoding = encoding;

			byte[] bytes = System.IO.File.ReadAllBytes(args[0]);
			byte[] countBytes = bytes[0..4];
			int count = BitConverter.ToInt32(countBytes);

			Console.WriteLine("File contains: " + count + " ressources.");
			List<FileInfo> fileInfos = new List<FileInfo>();

			for (int i = 4; i < count * 260; i += 260)
			{
				FileInfo fi = new FileInfo();
				fi.name = encoding.GetString(bytes[i..(i + 256)]);
				fi.size = BitConverter.ToInt32(bytes[(i + 256)..(i + 260)], 0);
				fileInfos.Add(fi);
			}

			int start = count * 260 + 4;
			for(int i = 0; i < fileInfos.Count; i++)
			{
				int end = start + fileInfos[i].size;
				byte[] fileContent = bytes[start..end];
				string filename = fileInfos[i].name.Split("\0")[0];

				// Write file and path to disk
				System.IO.FileInfo file = new System.IO.FileInfo(filename);
				file.Directory.Create();
				File.WriteAllBytes(file.FullName, fileContent);
				Console.WriteLine("Wrote: " + filename);

				start = end;
			}

			Console.WriteLine("Finished decrypting");
			Console.ReadKey();


		}
	}
}
