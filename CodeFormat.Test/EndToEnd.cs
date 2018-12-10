using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace CodeFormat.Test
{
    [TestClass]
    public class EndToEnd
    {
        [TestMethod]
        public void RunSamples()
        {
            string sourcePath = @"Inputs";
            string workingPath = @"Working";
            string expectedPath = @"Expected";

            // Copy files into a working folder
            if (Directory.Exists(workingPath)) { Directory.Delete(workingPath, true); }
            DirectoryCopy(sourcePath, workingPath);

            // Run the formatters
            int exitCode = Program.Main(new string[] { workingPath });
            Assert.AreEqual(0, exitCode);

            // Compare the results to the expected results
            StringBuilder errors = new StringBuilder();
            DirectoryCompare(Path.GetFullPath(expectedPath), Path.GetFullPath(workingPath), errors);
            if(errors.Length > 0)
            {
                Assert.Fail(errors.ToString());
            }
        }

        private static void DirectoryCopy(string sourcePath, string destinationPath)
        {
            Directory.CreateDirectory(destinationPath);

            foreach(string filePath in Directory.GetFiles(sourcePath))
            {
                File.Copy(filePath, Path.Combine(destinationPath, Path.GetFileName(filePath)));
            }

            foreach (string subfolderPath in Directory.GetDirectories(sourcePath))
            {
                DirectoryCopy(subfolderPath, Path.Combine(destinationPath, Path.GetFileName(subfolderPath)));
            }
        }

        private static void DirectoryCompare(string expectedPath, string actualPath, StringBuilder errors)
        {
            foreach (string expectedFilePath in Directory.GetFiles(expectedPath))
            {
                string actualFilePath = Path.Combine(actualPath, Path.GetFileName(expectedFilePath));

                string expectedText = File.ReadAllText(expectedFilePath);
                string actualText = File.ReadAllText(actualFilePath);
                if(!expectedText.Equals(actualText))
                {
                    errors.AppendLine($"windiff {expectedFilePath} {actualFilePath}");
                }
            }

            foreach (string subfolderPath in Directory.GetDirectories(expectedPath))
            {
                DirectoryCompare(subfolderPath, Path.Combine(actualPath, Path.GetFileName(subfolderPath)), errors);
            }
        }
    }
}
