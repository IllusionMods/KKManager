using System;
using System.IO;

namespace KKManager.Util
{
    public static class PathTools
    {
        public static bool DirectoryHasWritePermission(string directoryToTest)
        {
            try
            {
                Directory.CreateDirectory(directoryToTest);

                var testFile = Path.Combine(directoryToTest, "PermissionTest.tmp");
                File.Create(testFile).Close();
                File.Delete(testFile);
            }
            catch (UnauthorizedAccessException)
            {

                return false;
            }

            return true;
        }
    }
}
