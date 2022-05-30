/*
MIT License

Copyright (c) 2022 DevilutionSharp (Sergi4UA)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Runtime.InteropServices;

namespace StormLibSharp
{
    /// <summary>
    /// Low level Storm wrapper (P/Invoke)
    /// 
    /// Warning: may leak memory!
    /// </summary>
    internal class Storm
    {
        // SFileOpenArchive flags

        /// <summary>
        /// The MPQ is plain linear file. The file can have a block bitmap at the end, indicating that some file blocks may be missing. This is the default value.
        /// </summary>
        public const uint STREAM_PROVIDER_FLAT = 0x0000;
        public const uint STREAM_PROVIDER_PARTIAL = 0x0010;
        public const uint BASE_PROVIDER_FILE = 0x0000;
        public const uint BASE_PROVIDER_MAP = 0x0001;
        public const uint BASE_PROVIDER_HTTP = 0x0002;
        public const uint STREAM_FLAG_READ_ONLY = 0x00000100;
        public const uint STREAM_FLAG_WRITE_SHARE = 0x00000200;
        public const uint STREAM_FLAG_USE_BITMAP = 0x00000400;
        public const uint MPQ_OPEN_NO_LISTFILE = 0x00010000;
        public const uint MPQ_OPEN_NO_ATTRIBUTES = 0x00020000;
        public const uint MPQ_OPEN_NO_HEADER_SEARCH = 0x00040000;
        public const uint MPQ_OPEN_FORCE_MPQ_V1 = 0x00080000;
        public const uint MPQ_OPEN_CHECK_SECTOR_CRC = 0x00100000;

        /// <summary>
        /// Function SFileOpenArchive opens a MPQ archive. During the open operation, the archive is checked for corruptions, internal (listfile) and (attributes) are loaded, unless specified otherwise. The archive is open for read and write operations, unless MPQ_OPEN_READ_ONLY is specified. 
        /// Note that StormLib maintains list of all files within the MPQ, as long as the MPQ is open.At the moment of MPQ opening, when the MPQ contains an internal list file, that listfile is parsed and all files in the listfile are checked against the hash table.Every file name that exists within the MPQ is added to the internal name list.The name list can be fuhrter extended by calling SFileAddListFile.
        /// </summary>
        /// <param name="szMpqName">Archive file name</param>
        /// <param name="dwPriority">Archive priority</param>
        /// <param name="dwFlags">Open flags</param>
        /// <param name="phMPQ">Pointer to result HANDLE</param>
        /// <returns>When the function succeeds, it returns nonzero and phMPQ contains the handle of the opened archive. When the archive cannot be open, function returns false and GetLastError gives the error code.</returns>
        [DllImport("StormLib.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SFileOpenArchive([MarshalAs(UnmanagedType.LPWStr)] string szMpqName,
        uint dwPriority,
        uint dwFlags,
        out IntPtr phMPQ);

        /// <summary>
        /// Function SFileExtractFile extracts one file from an MPQ archive.
        /// </summary>
        /// <param name="hMPQ">Handle to an open MPQ archive.</param>
        /// <param name="szToExtract">Name of a file within the MPQ that is to be extracted.</param>
        /// <param name="szExtracted">Specifies the name of a local file that will be created and will contain data from the extracted MPQ file.</param>
        /// <param name="dwSearchScope">This parameter refines the definition of what to extract. See SFileOpenFileEx for more information.</param>
        /// <returns>If the MPQ file has been successfully extracted into the target file, the function returns true. On an error, the function returns false and GetLastError returns an error code.</returns>
        [DllImport("StormLib.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SFileExtractFile(IntPtr hMPQ,
            [MarshalAs(UnmanagedType.LPStr)] string szToExtract,
            [MarshalAs(UnmanagedType.LPWStr)] string szExtracted,
            uint dwSearchScope);

        /// <summary>
        /// Function SFileHasFile performs a quick check if a file exists within the MPQ archive. The function does not perform file open, not even internally. It merely checks hash table if the file is present.
        /// </summary>
        /// <param name="hMPQ">Handle to an open MPQ.</param>
        /// <param name="szToExtract">Name of the file to check.</param>
        /// <returns>When the file is present in the MPQ, function returns true.
        /// When the file is not present in the MPQ archive, the function returns false and GetLastError returns ERROR_FILE_NOT_FOUND.
        /// If an error occured, the function returns false and GetLastError returns an error code different than ERROR_FILE_NOT_FOUND.</returns>
        [DllImport("StormLib.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SFileHasFile(IntPtr hMPQ,
            [MarshalAs(UnmanagedType.LPStr)] string szFileName);

        /// <summary>
        /// Function SFileCloseArchive closes the MPQ archive. All in-memory data are freed and also any unsaved MPQ tables are saved to the archive. After this function finishes, the hMpq handle is no longer valid and may not be used in any MPQ operations.
        /// Note that this function calls SFileFlushArchive internally.
        /// </summary>
        /// <param name="hMPQ">Handle to an open MPQ.</param>
        /// <returns>When the function succeeds, it returns nonzero. On an error, the function returns false and GetLastError gives the error code.</returns>
        [DllImport("StormLib.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SFileCloseArchive(IntPtr hMPQ);

        /// <summary>
        /// Function SFileOpenFileEx opens a file from MPQ archive. The file is only open for read. The file must be closed by calling SFileCloseFile. All files must be closed before the MPQ archive is closed.
        /// </summary>
        /// <param name="hMPQ">Handle to an open archive.</param>
        /// <param name="szFileName">Name or index of the file to open.</param>
        /// <param name="dwSearchScope">Value that specifies how exactly the file should be open. It can be one of the following values:
        /// SFILE_OPEN_FROM_MPQ - (0x00000000) The file is open from the MPQ. This is the default value. hMpq must be valid if SFILE_OPEN_FROM_MPQ is specified.
        /// SFILE_OPEN_LOCAL_FILE - (0xFFFFFFFF) Opens a local file instead. The file is open using CreateFileEx with GENERIC_READ access and FILE_SHARE_READ mode.</param>
        /// <param name="phFile">Pointer to a variable of HANDLE type, that will receive HANDLE to the open file.</param>
        /// <returns>When the function succeeds, it returns nonzero and phFile contains the handle of the opened file. When the file cannot be open, function returns false and GetLastError gives the error code.</returns>
        [DllImport("StormLib.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SFileOpenFileEx(IntPtr hMPQ,
            [MarshalAs(UnmanagedType.LPStr)] string szFileName,
            uint dwSearchScope,
            out IntPtr phFile);

        /// <summary>
        /// Function SFileGetFileSize retrieves the size of an open file.
        /// </summary>
        /// <param name="hFile">Handle to an open file. The file handle must have been created by SFileOpenFileEx.</param>
        /// <param name="pdwFileSizeHigh">When the function succeeds, it returns lower 32-bit of the file size. On an error, it returns SFILE_INVALID_SIZE and GetLastError returns an error code.</param>
        /// <returns></returns>
        [DllImport("StormLib.dll", SetLastError = true)]
        public static extern UInt32 SFileGetFileSize(IntPtr hFile,
            out UInt32 pdwFileSizeHigh);

        /// <summary>
        /// Function SFileCloseFile closes an open MPQ file. All in-memory data are freed. After this function finishes, the hFile handle is no longer valid and must not be used in any file operations.
        /// </summary>
        /// <param name="hFile">Handle to an open file.</param>
        /// <returns>When the function succeeds, it returns nonzero. On an error, the function returns false and GetLastError gives the error code.</returns>
        [DllImport("StormLib.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SFileCloseFile(IntPtr hFile);

        /// <summary>
        /// Function SFileReadFile reads data from an open file.
        /// </summary>
        /// <param name="hFile">Handle to an open file. The file handle must have been created by SFileOpenFileEx.</param>
        /// <param name="lpBuffer">Pointer to buffer that will receive loaded data. The buffer size must be greater or equal to dwToRead.</param>
        /// <param name="dwToRead">Number of bytes to be read.</param>
        /// <param name="pdwRead">Pointer to DWORD that will receive number of bytes read.</param>
        /// <param name="lpOverlapped">If hFile is handle to a local disk file, lpOverlapped is passed to ReadFile. Otherwise not used.</param>
        /// <returns>When all requested bytes have been read, the function returns true.
        /// When less than requested bytes have been read, the function returns false and GetLastError returns ERROR_HANDLE_EOF.
        /// If an error occured, the function returns false and GetLastError returns an error code different from ERROR_HANDLE_EOF.</returns>
        [DllImport("StormLib.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SFileReadFile(IntPtr hFile,
            byte[] lpBuffer,
            uint dwToRead,
            out uint pdwRead,
            IntPtr lpOverlapped);
    }
}
