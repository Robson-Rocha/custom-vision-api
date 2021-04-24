namespace Exemplos.CustomVisionApi.Commands.OCR
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Exemplos.CustomVisionApi.Extensions;
    using McMaster.Extensions.CommandLineUtils;

    internal class OCRCommand : ICommand
    {
        public string CommandName => "ocr";

        private CommandOption _pathOption;
        private readonly string _key;
        private readonly string _endpoint;

        public OCRCommand(string key, string endpoint)
        {
            _key = key;
            _endpoint = endpoint;
        }

        public void Configure(CommandLineApplication command)
        {
            command.Description = "Tries to predict the classification (tag) of the given image.";
            command.HelpOption("-?|-h|--help");

            _pathOption = command.Option("--imagePath|-i", "Required. The path of one or more images whose classification (tag) must be predicted.", CommandOptionType.MultipleValue).IsRequired();
        }

        public int Execute()
        {
            List<string> imagePaths = _pathOption.Values;

            foreach (string imagePath in imagePaths)
                if (!File.Exists(imagePath))
                    return Util.Failure($"The path '{imagePath}' does not exist.");

            foreach (string imagePath in imagePaths)
            {
                MakeOCRRequest(imagePath).Wait();
            }

            return Util.Success();
        }

        private byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        private async Task MakeOCRRequest(string imageFilePath)
        {
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);

                string uri = $"{_endpoint}/vision/v2.1/ocr?language=pt&detectOrientation=true";

                HttpResponseMessage response;

                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);
                }

                string contentString = await response.Content.ReadAsStringAsync();

                OCRResponse ocrResponse = JsonSerializer.Deserialize<OCRResponse>(contentString);

                string numeroNota = ocrResponse.Regions.SelectMany(r => r.Lines)
                                                       .SelectMany(l => l.Words)
                                                       .Select(w => w.Text)
                                                       .FirstOrDefault(w => Regex.IsMatch(w, "^\\d{9}$") );

                Console.WriteLine($"NF: {numeroNota}");
                //Console.WriteLine("\nResponse:\n\n{0}\n", JToken.Parse(contentString).ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }        
    }

    internal class OCRResponse
    {
        internal class Region
        {
            internal class Line
            {
                internal class Word
                {
                    public string BoundingBox { get; set; }
                    public string Text { get; set; }
                }

                public string BoundingBox { get; set; }
                public Line.Word[] Words { get; set; }
            }
            public string BoundingBox { get; set; }
            public Region.Line[] Lines { get; set; }
        }

        public string Language { get; set; }
        public decimal textAngle { get; set; }
        public string Orientation { get; set; }
        public OCRResponse.Region[] Regions { get; set; }
    }
}