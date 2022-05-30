// This example shows you how to:

// - Open a MPQ for reading and get the HANDLE as a IntPtr
// - Use that IntPtr to extract a file
// - Open a file inside the MPQ for reading
// - Get the size of the file
// - Read the file inside the MPQ into a byte[] buffer
// - Recreate the file from byte[] buffer using System.IO.WriteAllBytes

using System;
using System.Runtime.InteropServices;

using StormLibSharp;

namespace StormLibSharp.Examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntPtr mpqFile;
            IntPtr file;
            uint fileSizeHigh;
            uint fileRead;

            if (Storm.SFileOpenArchive("DIABDAT.MPQ", 0, 0, out mpqFile) == false)
                throw new AccessViolationException("stormlib failure");

            if (Storm.SFileExtractFile(mpqFile, "music\\dintro.wav", "dintro.wav", 0) == false)
                throw new AccessViolationException("stormlib failure, extract");
            int win32err = Marshal.GetLastWin32Error();
            if (win32err == 2)
                throw new System.IO.FileNotFoundException();

            Storm.SFileOpenFileEx(mpqFile, "music\\dintro.wav", 0, out file);
            uint fileSize = Storm.SFileGetFileSize(file, out fileSizeHigh);

            //IntPtr buf = Marshal.AllocHGlobal((int)fileSize);
            byte[] buf = new byte[fileSize];
            if (Storm.SFileReadFile(file, buf, fileSize, out fileRead, IntPtr.Zero) == false)
                Console.WriteLine("read file fail");

            byte[] bufManagedFile = new byte[fileSize];
            //Marshal.Copy(buf, bufManagedFile, 0, bufManagedFile.Length);
            int code = Marshal.GetLastWin32Error();

            System.IO.File.WriteAllBytes("data.bin", buf);

            if (Storm.SFileCloseFile(file) == false)
                throw new System.IO.IOException();
            if(Storm.SFileCloseArchive(mpqFile) == false)
                throw new System.IO.IOException();

            Console.WriteLine("Ok");
            Console.WriteLine(Marshal.GetLastWin32Error());
            Console.ReadLine();
        }
    }
}