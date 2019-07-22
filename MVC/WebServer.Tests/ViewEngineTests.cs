namespace WebServer.Tests
{
    using SIS.MVC.Contracts;
    using SIS.MVC.ViewEngine;
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    public class ViewEngineTests
    {
        [Theory]
        [InlineData("Empty")]
        [InlineData("ExtractDataFromModel")]
        [InlineData("IFForeach")]
        public void TestViewEngine(string fileName)
        {
            fileName += ".html";
            string path = "../../../HtmlTests/";
            string inputFolderName = "ProvidedInput/";
            string outputFolderName = "ExpectedOutput/";
            string inputFilePath = path + inputFolderName + fileName;
            string outputFilePath = path + outputFolderName + fileName;
            bool InputFileExists = File.Exists(inputFilePath);
            bool OutputFileExists = File.Exists(outputFilePath);
            Assert.True(InputFileExists && OutputFileExists);

            TestDto testModel = new TestDto()
            {
                Name = "Todor",
                Neshta = new string[] { "Qbulki", "Krushi", "Morkovi" }
            };

            string inputContent = File.ReadAllText(inputFilePath);
            IViewEngine viewEngine = new View_Engine();
            string actualResult = viewEngine.GetHtmlImbued(inputContent, testModel);
            string expectedResult = File.ReadAllText(outputFilePath);
            Assert.Equal(actualResult, expectedResult);
        }
    }

    public class TestDto
    {
        public string Name { get; set; }
        public IList<string> Neshta { get; set; }
    }
}